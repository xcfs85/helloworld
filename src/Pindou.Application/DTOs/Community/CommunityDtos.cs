using Pindou.Application.Common;

namespace Pindou.Application.DTOs.Community;

public class CreatePostRequest
{
    public string Type { get; set; } = "work";
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<MediaItem>? Media { get; set; }
    public List<string>? TopicIds { get; set; }
    public string? DiagramId { get; set; }
    public string? BeadParams { get; set; }
}

public class MediaItem
{
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = "image";
    public int? Width { get; set; }
    public int? Height { get; set; }
}

public class FeedQuery : PageRequest
{
    /// <summary>类型 work/tutorial/discussion/recommend</summary>
    public string? Type { get; set; }
    public string? UserId { get; set; }
    public string? TopicId { get; set; }
}

public class PostDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<MediaItem> Media { get; set; } = new();
    public string? DiagramId { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int FavoriteCount { get; set; }
    public int ViewCount { get; set; }
    public string ReviewStatus { get; set; } = "approved";
    public DateTime PublishTime { get; set; }
    public AuthorBrief Author { get; set; } = new();
    public bool IsLiked { get; set; }
    public bool IsFavorited { get; set; }
}

public class PostDetailDto : PostDto
{
    public List<string> TopicIds { get; set; } = new();
    public string? BeadParams { get; set; }
}

public class AuthorBrief
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsMember { get; set; }
}

public class CreateCommentRequest
{
    public string PostId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ReplyToUserId { get; set; }
}

public class CommentDto
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ReplyToUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public DateTime CreateTime { get; set; }
    public AuthorBrief Author { get; set; } = new();
    public bool IsLiked { get; set; }
}

public class TopicDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public int PostCount { get; set; }
    public bool IsHot { get; set; }
}

public class CreateTopicRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
}
