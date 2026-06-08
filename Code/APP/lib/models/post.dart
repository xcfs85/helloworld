class Post {
  final String id;
  final String userId;
  final String type;
  final String title;
  final String content;
  final List<PostMedia> media;
  final List<String> topicIds;
  final Map<String, dynamic>? beadParams;
  final String? diagramId;
  final int likeCount;
  final int commentCount;
  final int favoriteCount;
  final int shareCount;
  final String status;
  final String createdAt;
  final String publishedAt;
  final PostUser? user;

  Post({
    required this.id,
    required this.userId,
    required this.type,
    required this.title,
    required this.content,
    required this.media,
    required this.topicIds,
    this.beadParams,
    this.diagramId,
    this.likeCount = 0,
    this.commentCount = 0,
    this.favoriteCount = 0,
    this.shareCount = 0,
    required this.status,
    required this.createdAt,
    required this.publishedAt,
    this.user,
  });

  factory Post.fromJson(Map<String, dynamic> json) {
    return Post(
      id: json['id'] ?? '',
      userId: json['user_id'] ?? '',
      type: json['type'] ?? 'work',
      title: json['title'] ?? '',
      content: json['content'] ?? '',
      media: (json['media'] as List<dynamic>?)
          ?.map((m) => PostMedia.fromJson(m))
          .toList() ?? [],
      topicIds: (json['topic_ids'] as List<dynamic>?)
          ?.map((t) => t.toString())
          .toList() ?? [],
      beadParams: json['bead_params'],
      diagramId: json['diagram_id'],
      likeCount: json['like_count'] ?? 0,
      commentCount: json['comment_count'] ?? 0,
      favoriteCount: json['favorite_count'] ?? 0,
      shareCount: json['share_count'] ?? 0,
      status: json['status'] ?? 'published',
      createdAt: json['created_at'] ?? '',
      publishedAt: json['published_at'] ?? '',
      user: json['user'] != null ? PostUser.fromJson(json['user']) : null,
    );
  }
}

class PostMedia {
  final String url;
  final String type;
  final int width;
  final int height;

  PostMedia({
    required this.url,
    required this.type,
    required this.width,
    required this.height,
  });

  factory PostMedia.fromJson(Map<String, dynamic> json) {
    return PostMedia(
      url: json['url'] ?? '',
      type: json['type'] ?? 'image',
      width: json['width'] ?? 0,
      height: json['height'] ?? 0,
    );
  }
}

class PostUser {
  final String userId;
  final String nickname;
  final String avatar;
  final bool isFollowed;

  PostUser({
    required this.userId,
    required this.nickname,
    required this.avatar,
    this.isFollowed = false,
  });

  factory PostUser.fromJson(Map<String, dynamic> json) {
    return PostUser(
      userId: json['user_id'] ?? '',
      nickname: json['nickname'] ?? '',
      avatar: json['avatar'] ?? '',
      isFollowed: json['is_followed'] ?? false,
    );
  }
}

class Comment {
  final String id;
  final String postId;
  final String userId;
  final String content;
  final String? parentId;
  final String? replyToUserId;
  final int likeCount;
  final String createdAt;
  final CommentUser? user;

  Comment({
    required this.id,
    required this.postId,
    required this.userId,
    required this.content,
    this.parentId,
    this.replyToUserId,
    this.likeCount = 0,
    required this.createdAt,
    this.user,
  });

  factory Comment.fromJson(Map<String, dynamic> json) {
    return Comment(
      id: json['id'] ?? '',
      postId: json['post_id'] ?? '',
      userId: json['user_id'] ?? '',
      content: json['content'] ?? '',
      parentId: json['parent_id'],
      replyToUserId: json['reply_to_user_id'],
      likeCount: json['like_count'] ?? 0,
      createdAt: json['created_at'] ?? '',
      user: json['user'] != null ? CommentUser.fromJson(json['user']) : null,
    );
  }
}

class CommentUser {
  final String userId;
  final String nickname;
  final String avatar;

  CommentUser({
    required this.userId,
    required this.nickname,
    required this.avatar,
  });

  factory CommentUser.fromJson(Map<String, dynamic> json) {
    return CommentUser(
      userId: json['user_id'] ?? '',
      nickname: json['nickname'] ?? '',
      avatar: json['avatar'] ?? '',
    );
  }
}