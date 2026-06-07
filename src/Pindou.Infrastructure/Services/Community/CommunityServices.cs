using System.Text.Json;
using Pindou.Application.Common;
using Pindou.Application.DTOs.Community;
using Pindou.Application.Interfaces.Community;
using Pindou.Application.Interfaces.System;
using Pindou.Domain.Entities.Community;
using Pindou.Domain.Entities.User;
using Pindou.Infrastructure.Repositories;
using SqlSugar;

namespace Pindou.Infrastructure.Services.Community;

public class PostService : IPostService
{
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<Comment> _commentRepo;
    private readonly IRepository<Like> _likeRepo;
    private readonly IRepository<Favorite> _favoriteRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IContentReviewService _contentReview;

    public PostService(
        IRepository<Post> postRepo,
        IRepository<Comment> commentRepo,
        IRepository<Like> likeRepo,
        IRepository<Favorite> favoriteRepo,
        IRepository<User> userRepo,
        IContentReviewService contentReview)
    {
        _postRepo = postRepo;
        _commentRepo = commentRepo;
        _likeRepo = likeRepo;
        _favoriteRepo = favoriteRepo;
        _userRepo = userRepo;
        _contentReview = contentReview;
    }

    public async Task<string> CreatePostAsync(string userId, CreatePostRequest request)
    {
        // 内容审核
        var (passed, reason, replacedContent) = await _contentReview.CheckAsync(request.Content);
        if (!passed) throw new BizException(reason, 3002);

        var post = new Post
        {
            UserId = userId,
            Type = request.Type,
            Title = request.Title,
            Content = replacedContent ?? request.Content,
            Media = request.Media != null ? JsonSerializer.Serialize(request.Media) : "[]",
            TopicIds = request.TopicIds != null ? JsonSerializer.Serialize(request.TopicIds) : null,
            DiagramId = request.DiagramId,
            BeadParams = request.BeadParams,
            Status = "active",
            ReviewStatus = "pending",
            PublishTime = DateTime.Now
        };
        await _postRepo.InsertAsync(post);
        return post.Id;
    }

    public async Task<PagedResult<PostDto>> GetFeedAsync(string userId, FeedQuery query)
    {
        var exp = Expressionable.Create<Post>().And(p => p.Status == "active" && p.ReviewStatus == "approved");

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            if (query.Type == "recommend")
            {
                // 推荐：按热度排序
            }
            else
            {
                exp.And(p => p.Type == query.Type);
            }
        }
        if (!string.IsNullOrWhiteSpace(query.UserId))
            exp.And(p => p.UserId == query.UserId);
        if (!string.IsNullOrWhiteSpace(query.TopicId))
            exp.And(p => p.TopicIds != null && p.TopicIds.Contains(query.TopicId));

        var orderBy = query.Type == "recommend"
            ? (Expression<Func<Post, object>>)(p => p.LikeCount + p.CommentCount + p.FavoriteCount)
            : (Expression<Func<Post, object>>)(p => p.PublishTime);

        var (list, total) = await _postRepo.GetPagedAsync(
            exp.ToExpression(),
            query.Page,
            query.Size,
            orderBy,
            true);

        var result = new PagedResult<PostDto>
        {
            Page = query.Page,
            Size = query.Size,
            Total = total,
            List = new List<PostDto>()
        };

        foreach (var post in list)
        {
            var author = await _userRepo.GetByIdAsync(post.UserId);
            var isLiked = await _likeRepo.AnyAsync(l => l.UserId == userId && l.TargetId == post.Id && l.TargetType == "post");
            var isFavorited = await _favoriteRepo.AnyAsync(f => f.UserId == userId && f.TargetId == post.Id && f.TargetType == "post");

            result.List.Add(new PostDto
            {
                Id = post.Id,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                Media = DeserializeMedia(post.Media),
                DiagramId = post.DiagramId,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                FavoriteCount = post.FavoriteCount,
                ViewCount = post.ViewCount,
                ReviewStatus = post.ReviewStatus,
                PublishTime = post.PublishTime,
                Author = new AuthorBrief
                {
                    Id = author?.Id ?? string.Empty,
                    Nickname = author?.Nickname ?? string.Empty,
                    Avatar = author?.Avatar,
                    IsMember = author?.IsMember ?? false
                },
                IsLiked = isLiked,
                IsFavorited = isFavorited
            });
        }

        return result;
    }

    public async Task<PagedResult<PostDto>> GetUserPostsAsync(string userId, PageRequest request)
    {
        var exp = Expressionable.Create<Post>().And(p => p.UserId == userId && p.Status == "active");

        var (list, total) = await _postRepo.GetPagedAsync(
            exp.ToExpression(),
            request.Page,
            request.Size,
            p => p.PublishTime,
            true);

        var author = await _userRepo.GetByIdAsync(userId);
        var result = new PagedResult<PostDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<PostDto>()
        };

        foreach (var post in list)
        {
            var isLiked = await _likeRepo.AnyAsync(l => l.UserId == userId && l.TargetId == post.Id && l.TargetType == "post");
            var isFavorited = await _favoriteRepo.AnyAsync(f => f.UserId == userId && f.TargetId == post.Id && f.TargetType == "post");

            result.List.Add(new PostDto
            {
                Id = post.Id,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                Media = DeserializeMedia(post.Media),
                DiagramId = post.DiagramId,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                FavoriteCount = post.FavoriteCount,
                ViewCount = post.ViewCount,
                ReviewStatus = post.ReviewStatus,
                PublishTime = post.PublishTime,
                Author = new AuthorBrief
                {
                    Id = author?.Id ?? string.Empty,
                    Nickname = author?.Nickname ?? string.Empty,
                    Avatar = author?.Avatar,
                    IsMember = author?.IsMember ?? false
                },
                IsLiked = isLiked,
                IsFavorited = isFavorited
            });
        }

        return result;
    }

    public async Task<PostDetailDto> GetPostDetailAsync(string userId, string postId)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post == null || post.Status == "deleted") throw new BizException("帖子不存在", ErrorCodes.NotFound);

        // 增加浏览次数
        post.ViewCount++;
        await _postRepo.UpdateAsync(post);

        var author = await _userRepo.GetByIdAsync(post.UserId);
        var isLiked = await _likeRepo.AnyAsync(l => l.UserId == userId && l.TargetId == post.Id && l.TargetType == "post");
        var isFavorited = await _favoriteRepo.AnyAsync(f => f.UserId == userId && f.TargetId == post.Id && f.TargetType == "post");

        var topicIds = new List<string>();
        if (!string.IsNullOrWhiteSpace(post.TopicIds))
        {
            try { topicIds = JsonSerializer.Deserialize<List<string>>(post.TopicIds) ?? new(); }
            catch { }
        }

        return new PostDetailDto
        {
            Id = post.Id,
            Type = post.Type,
            Title = post.Title,
            Content = post.Content,
            Media = DeserializeMedia(post.Media),
            DiagramId = post.DiagramId,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            FavoriteCount = post.FavoriteCount,
            ViewCount = post.ViewCount,
            ReviewStatus = post.ReviewStatus,
            PublishTime = post.PublishTime,
            Author = new AuthorBrief
            {
                Id = author?.Id ?? string.Empty,
                Nickname = author?.Nickname ?? string.Empty,
                Avatar = author?.Avatar,
                IsMember = author?.IsMember ?? false
            },
            IsLiked = isLiked,
            IsFavorited = isFavorited,
            TopicIds = topicIds,
            BeadParams = post.BeadParams
        };
    }

    public async Task<bool> DeletePostAsync(string userId, string postId)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post == null) throw new BizException("帖子不存在", ErrorCodes.NotFound);
        if (post.UserId != userId) throw new BizException("无权删除他人帖子", ErrorCodes.NoPermission);

        post.Status = "deleted";
        post.UpdateTime = DateTime.Now;
        return await _postRepo.UpdateAsync(post);
    }

    public async Task<bool> LikeAsync(string userId, string targetId, string targetType)
    {
        var exists = await _likeRepo.AnyAsync(l => l.UserId == userId && l.TargetId == targetId && l.TargetType == targetType);
        if (exists) return true;

        await _likeRepo.InsertAsync(new Like
        {
            UserId = userId,
            TargetId = targetId,
            TargetType = targetType
        });

        // 更新计数
        if (targetType == "post")
        {
            var post = await _postRepo.GetByIdAsync(targetId);
            if (post != null) { post.LikeCount++; await _postRepo.UpdateAsync(post); }
        }
        else if (targetType == "comment")
        {
            var comment = await _commentRepo.GetByIdAsync(targetId);
            if (comment != null) { comment.LikeCount++; await _commentRepo.UpdateAsync(comment); }
        }

        return true;
    }

    public async Task<bool> UnlikeAsync(string userId, string targetId, string targetType)
    {
        var like = await _likeRepo.FirstOrDefaultAsync(l => l.UserId == userId && l.TargetId == targetId && l.TargetType == targetType);
        if (like == null) return true;

        await _likeRepo.DeleteAsync(like.Id);

        if (targetType == "post")
        {
            var post = await _postRepo.GetByIdAsync(targetId);
            if (post != null && post.LikeCount > 0) { post.LikeCount--; await _postRepo.UpdateAsync(post); }
        }
        else if (targetType == "comment")
        {
            var comment = await _commentRepo.GetByIdAsync(targetId);
            if (comment != null && comment.LikeCount > 0) { comment.LikeCount--; await _commentRepo.UpdateAsync(comment); }
        }

        return true;
    }

    public async Task<bool> FavoriteAsync(string userId, string targetId, string targetType)
    {
        var exists = await _favoriteRepo.AnyAsync(f => f.UserId == userId && f.TargetId == targetId && f.TargetType == targetType);
        if (exists) return true;

        await _favoriteRepo.InsertAsync(new Favorite
        {
            UserId = userId,
            TargetId = targetId,
            TargetType = targetType
        });

        if (targetType == "post")
        {
            var post = await _postRepo.GetByIdAsync(targetId);
            if (post != null) { post.FavoriteCount++; await _postRepo.UpdateAsync(post); }
        }

        return true;
    }

    public async Task<bool> UnfavoriteAsync(string userId, string targetId, string targetType)
    {
        var fav = await _favoriteRepo.FirstOrDefaultAsync(f => f.UserId == userId && f.TargetId == targetId && f.TargetType == targetType);
        if (fav == null) return true;

        await _favoriteRepo.DeleteAsync(fav.Id);

        if (targetType == "post")
        {
            var post = await _postRepo.GetByIdAsync(targetId);
            if (post != null && post.FavoriteCount > 0) { post.FavoriteCount--; await _postRepo.UpdateAsync(post); }
        }

        return true;
    }

    public async Task<string> CreateCommentAsync(string userId, CreateCommentRequest request)
    {
        // 检查帖子
        var post = await _postRepo.GetByIdAsync(request.PostId);
        if (post == null || post.Status == "deleted") throw new BizException("帖子不存在", ErrorCodes.NotFound);

        // 内容审核
        var (passed, reason, replacedContent) = await _contentReview.CheckAsync(request.Content);
        if (!passed) throw new BizException(reason, 3002);

        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = userId,
            ParentId = request.ParentId,
            ReplyToUserId = request.ReplyToUserId,
            Content = replacedContent ?? request.Content,
            Status = "active"
        };
        await _commentRepo.InsertAsync(comment);

        // 更新帖子评论数
        post.CommentCount++;
        await _postRepo.UpdateAsync(post);

        return comment.Id;
    }

    public async Task<PagedResult<CommentDto>> GetCommentsAsync(string postId, PageRequest request)
    {
        var (list, total) = await _commentRepo.GetPagedAsync(
            c => c.PostId == postId && c.Status == "active",
            request.Page,
            request.Size,
            c => c.CreateTime,
            false);

        var result = new PagedResult<CommentDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<CommentDto>()
        };

        foreach (var comment in list)
        {
            var author = await _userRepo.GetByIdAsync(comment.UserId);
            var isLiked = await _likeRepo.AnyAsync(l => l.UserId == comment.UserId && l.TargetId == comment.Id && l.TargetType == "comment");

            result.List.Add(new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                ParentId = comment.ParentId,
                ReplyToUserId = comment.ReplyToUserId,
                Content = comment.Content,
                LikeCount = comment.LikeCount,
                CreateTime = comment.CreateTime,
                Author = new AuthorBrief
                {
                    Id = author?.Id ?? string.Empty,
                    Nickname = author?.Nickname ?? string.Empty,
                    Avatar = author?.Avatar,
                    IsMember = author?.IsMember ?? false
                },
                IsLiked = isLiked
            });
        }

        return result;
    }

    public async Task<bool> DeleteCommentAsync(string userId, string commentId)
    {
        var comment = await _commentRepo.GetByIdAsync(commentId);
        if (comment == null) throw new BizException("评论不存在", ErrorCodes.NotFound);
        if (comment.UserId != userId) throw new BizException("无权删除他人评论", ErrorCodes.NoPermission);

        comment.Status = "deleted";
        comment.UpdateTime = DateTime.Now;
        await _commentRepo.UpdateAsync(comment);

        // 更新帖子评论数
        var post = await _postRepo.GetByIdAsync(comment.PostId);
        if (post != null && post.CommentCount > 0)
        {
            post.CommentCount--;
            await _postRepo.UpdateAsync(post);
        }

        return true;
    }

    private static List<MediaItem> DeserializeMedia(string media)
    {
        try { return JsonSerializer.Deserialize<List<MediaItem>>(media) ?? new(); }
        catch { return new(); }
    }
}

public class FollowService : IFollowService
{
    private readonly IRepository<Follow> _followRepo;
    private readonly IRepository<User> _userRepo;

    public FollowService(IRepository<Follow> followRepo, IRepository<User> userRepo)
    {
        _followRepo = followRepo;
        _userRepo = userRepo;
    }

    public async Task<bool> FollowAsync(string userId, string followUserId)
    {
        if (userId == followUserId) throw new BizException("不能关注自己", ErrorCodes.ParamError);

        var exists = await _followRepo.AnyAsync(f => f.UserId == userId && f.FollowUserId == followUserId);
        if (exists) return true;

        await _followRepo.InsertAsync(new Follow
        {
            UserId = userId,
            FollowUserId = followUserId
        });

        return true;
    }

    public async Task<bool> UnfollowAsync(string userId, string followUserId)
    {
        var follow = await _followRepo.FirstOrDefaultAsync(f => f.UserId == userId && f.FollowUserId == followUserId);
        if (follow == null) return true;

        await _followRepo.DeleteAsync(follow.Id);
        return true;
    }

    public async Task<PagedResult<DTOs.User.UserListDto>> GetFollowListAsync(string userId, PageRequest request)
    {
        var (follows, total) = await _followRepo.GetPagedAsync(
            f => f.UserId == userId,
            request.Page,
            request.Size,
            f => f.CreateTime,
            true);

        var result = new PagedResult<DTOs.User.UserListDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<DTOs.User.UserListDto>()
        };

        foreach (var follow in follows)
        {
            var user = await _userRepo.GetByIdAsync(follow.FollowUserId);
            if (user != null)
            {
                result.List.Add(new DTOs.User.UserListDto
                {
                    Id = user.Id,
                    Nickname = user.Nickname,
                    Avatar = user.Avatar,
                    Gender = user.Gender,
                    IsMember = user.IsMember,
                    MemberExpireTime = user.MemberExpireTime,
                    Status = user.Status,
                    CreateTime = user.CreateTime
                });
            }
        }

        return result;
    }

    public async Task<PagedResult<DTOs.User.UserListDto>> GetFansListAsync(string userId, PageRequest request)
    {
        var (follows, total) = await _followRepo.GetPagedAsync(
            f => f.FollowUserId == userId,
            request.Page,
            request.Size,
            f => f.CreateTime,
            true);

        var result = new PagedResult<DTOs.User.UserListDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<DTOs.User.UserListDto>()
        };

        foreach (var follow in follows)
        {
            var user = await _userRepo.GetByIdAsync(follow.UserId);
            if (user != null)
            {
                result.List.Add(new DTOs.User.UserListDto
                {
                    Id = user.Id,
                    Nickname = user.Nickname,
                    Avatar = user.Avatar,
                    Gender = user.Gender,
                    IsMember = user.IsMember,
                    MemberExpireTime = user.MemberExpireTime,
                    Status = user.Status,
                    CreateTime = user.CreateTime
                });
            }
        }

        return result;
    }
}

public class TopicService : ITopicService
{
    private readonly IRepository<Topic> _topicRepo;
    private readonly IRepository<Post> _postRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Like> _likeRepo;
    private readonly IRepository<Favorite> _favoriteRepo;

    public TopicService(
        IRepository<Topic> topicRepo,
        IRepository<Post> postRepo,
        IRepository<User> userRepo,
        IRepository<Like> likeRepo,
        IRepository<Favorite> favoriteRepo)
    {
        _topicRepo = topicRepo;
        _postRepo = postRepo;
        _userRepo = userRepo;
        _likeRepo = likeRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<PagedResult<TopicDto>> GetHotTopicsAsync(PageRequest request)
    {
        var (list, total) = await _topicRepo.GetPagedAsync(
            t => t.Status == "active",
            request.Page,
            request.Size,
            t => t.PostCount,
            true);

        var result = new PagedResult<TopicDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<TopicDto>()
        };

        foreach (var topic in list)
        {
            result.List.Add(new TopicDto
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                CoverUrl = topic.CoverUrl,
                PostCount = topic.PostCount,
                IsHot = topic.IsHot
            });
        }

        return result;
    }

    public async Task<TopicDto> GetTopicAsync(string topicId)
    {
        var topic = await _topicRepo.GetByIdAsync(topicId);
        if (topic == null) throw new BizException("话题不存在", ErrorCodes.NotFound);

        return new TopicDto
        {
            Id = topic.Id,
            Name = topic.Name,
            Description = topic.Description,
            CoverUrl = topic.CoverUrl,
            PostCount = topic.PostCount,
            IsHot = topic.IsHot
        };
    }

    public async Task<PagedResult<PostDto>> GetTopicPostsAsync(string topicId, PageRequest request)
    {
        var (list, total) = await _postRepo.GetPagedAsync(
            p => p.Status == "active" && p.ReviewStatus == "approved" && p.TopicIds != null && p.TopicIds.Contains(topicId),
            request.Page,
            request.Size,
            p => p.PublishTime,
            true);

        var result = new PagedResult<PostDto>
        {
            Page = request.Page,
            Size = request.Size,
            Total = total,
            List = new List<PostDto>()
        };

        foreach (var post in list)
        {
            var author = await _userRepo.GetByIdAsync(post.UserId);
            result.List.Add(new PostDto
            {
                Id = post.Id,
                Type = post.Type,
                Title = post.Title,
                Content = post.Content,
                Media = new List<MediaItem>(),
                DiagramId = post.DiagramId,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                FavoriteCount = post.FavoriteCount,
                ViewCount = post.ViewCount,
                ReviewStatus = post.ReviewStatus,
                PublishTime = post.PublishTime,
                Author = new AuthorBrief
                {
                    Id = author?.Id ?? string.Empty,
                    Nickname = author?.Nickname ?? string.Empty,
                    Avatar = author?.Avatar,
                    IsMember = author?.IsMember ?? false
                },
                IsLiked = false,
                IsFavorited = false
            });
        }

        return result;
    }
}