class Message {
  final String id;
  final String type;
  final String title;
  final String content;
  final String? fromUserId;
  final String? fromUserName;
  final String? fromUserAvatar;
  final String? postId;
  final String? commentId;
  final bool isRead;
  final String createdAt;

  Message({
    required this.id,
    required this.type,
    required this.title,
    required this.content,
    this.fromUserId,
    this.fromUserName,
    this.fromUserAvatar,
    this.postId,
    this.commentId,
    this.isRead = false,
    required this.createdAt,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      id: json['id'] ?? '',
      type: json['type'] ?? 'system',
      title: json['title'] ?? '',
      content: json['content'] ?? '',
      fromUserId: json['from_user_id'],
      fromUserName: json['from_user_name'],
      fromUserAvatar: json['from_user_avatar'],
      postId: json['post_id'],
      commentId: json['comment_id'],
      isRead: json['is_read'] ?? false,
      createdAt: json['created_at'] ?? '',
    );
  }
}

class UnreadCount {
  final int comment;
  final int like;
  final int follow;
  final int at;
  final int system;
  final int total;

  UnreadCount({
    this.comment = 0,
    this.like = 0,
    this.follow = 0,
    this.at = 0,
    this.system = 0,
    this.total = 0,
  });

  factory UnreadCount.fromJson(Map<String, dynamic> json) {
    return UnreadCount(
      comment: json['comment'] ?? 0,
      like: json['like'] ?? 0,
      follow: json['follow'] ?? 0,
      at: json['at'] ?? 0,
      system: json['system'] ?? 0,
      total: json['total'] ?? 0,
    );
  }
}

class MessageSettings {
  final bool commentEnabled;
  final bool likeEnabled;
  final bool followEnabled;
  final bool atEnabled;
  final bool systemEnabled;

  MessageSettings({
    this.commentEnabled = true,
    this.likeEnabled = true,
    this.followEnabled = true,
    this.atEnabled = true,
    this.systemEnabled = true,
  });

  factory MessageSettings.fromJson(Map<String, dynamic> json) {
    return MessageSettings(
      commentEnabled: json['comment_enabled'] ?? true,
      likeEnabled: json['like_enabled'] ?? true,
      followEnabled: json['follow_enabled'] ?? true,
      atEnabled: json['at_enabled'] ?? true,
      systemEnabled: json['system_enabled'] ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'comment_enabled': commentEnabled,
      'like_enabled': likeEnabled,
      'follow_enabled': followEnabled,
      'at_enabled': atEnabled,
      'system_enabled': systemEnabled,
    };
  }
}