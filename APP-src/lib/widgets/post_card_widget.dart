import 'package:flutter/material.dart';
import '../config/app_theme.dart';
import '../models/post.dart';

class PostCard extends StatelessWidget {
  final Post post;
  final VoidCallback? onTap;
  final VoidCallback? onLike;
  final VoidCallback? onComment;
  final VoidCallback? onFavorite;
  final VoidCallback? onShare;
  final VoidCallback? onUserTap;

  const PostCard({
    super.key,
    required this.post,
    this.onTap,
    this.onLike,
    this.onComment,
    this.onFavorite,
    this.onShare,
    this.onUserTap,
  });

  String get _typeLabel {
    switch (post.type) {
      case 'work': return '作品';
      case 'request': return '求图';
      case 'tutorial': return '教程';
      case 'discussion': return '讨论';
      default: return '作品';
    }
  }

  Color get _typeColor {
    switch (post.type) {
      case 'work': return AppTheme.primary;
      case 'request': return AppTheme.rose;
      case 'tutorial': return AppTheme.mint;
      case 'discussion': return AppTheme.sky;
      default: return AppTheme.primary;
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
        decoration: BoxDecoration(
          color: AppTheme.surface,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: AppTheme.line),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildHeader(),
            _buildBody(),
            if (post.media.isNotEmpty) _buildImages(),
            if (post.topicIds.isNotEmpty) _buildTags(),
            _buildActions(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Padding(
      padding: const EdgeInsets.all(12),
      child: Row(
        children: [
          GestureDetector(
            onTap: onUserTap,
            child: CircleAvatar(
              radius: 18,
              backgroundColor: AppTheme.primary,
              backgroundImage: post.user?.avatar.isNotEmpty == true
                  ? NetworkImage(post.user!.avatar)
                  : null,
              child: post.user?.avatar.isEmpty ?? true
                  ? Text(post.user?.nickname.substring(0, 1) ?? '?',
                      style: const TextStyle(color: Colors.white))
                  : null,
            ),
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  post.user?.nickname ?? '用户',
                  style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
                ),
                Text(
                  post.publishedAt,
                  style: const TextStyle(fontSize: 11, color: AppTheme.ink3),
                ),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
            decoration: BoxDecoration(
              color: _typeColor.withOpacity(0.15),
              borderRadius: BorderRadius.circular(999),
            ),
            child: Text(
              _typeLabel,
              style: TextStyle(fontSize: 10, color: _typeColor, fontWeight: FontWeight.w600),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBody() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (post.title.isNotEmpty)
            Text(
              post.title,
              style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600, height: 1.5),
            ),
          if (post.content.isNotEmpty) ...[
            const SizedBox(height: 6),
            Text(
              post.content,
              style: const TextStyle(fontSize: 12, color: AppTheme.ink2, height: 1.6),
              maxLines: 3,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildImages() {
    final count = post.media.length.clamp(1, 3);
    return Padding(
      padding: const EdgeInsets.all(12),
      child: Row(
        children: List.generate(count, (i) {
          return Expanded(
            child: Padding(
              padding: EdgeInsets.only(right: i < count - 1 ? 4 : 0),
              child: AspectRatio(
                aspectRatio: 1,
                child: Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(8),
                    gradient: LinearGradient(
                      colors: [AppTheme.primary, AppTheme.accent],
                    ),
                  ),
                ),
              ),
            ),
          );
        }),
      ),
    );
  }

  Widget _buildTags() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 14),
      child: Wrap(
        spacing: 6,
        runSpacing: 4,
        children: post.topicIds.map((tag) {
          return Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
            decoration: BoxDecoration(
              color: AppTheme.bg2,
              borderRadius: BorderRadius.circular(999),
            ),
            child: Text(
              '#$tag',
              style: const TextStyle(fontSize: 10, color: AppTheme.ink2),
            ),
          );
        }).toList(),
      ),
    );
  }

  Widget _buildActions() {
    return Container(
      margin: const EdgeInsets.only(top: 10),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      decoration: BoxDecoration(
        border: Border(top: BorderSide(color: AppTheme.line)),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _actionButton(Icons.favorite_border_rounded, '${post.likeCount}', onLike),
          _actionButton(Icons.chat_bubble_outline_rounded, '${post.commentCount}', onComment),
          _actionButton(Icons.star_border_rounded, '${post.favoriteCount}', onFavorite),
          _actionButton(Icons.share_rounded, '分享', onShare),
        ],
      ),
    );
  }

  Widget _actionButton(IconData icon, String label, VoidCallback? onTap) {
    return GestureDetector(
      onTap: onTap,
      behavior: HitTestBehavior.opaque,
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 16, color: AppTheme.ink3),
          const SizedBox(width: 4),
          Text(label, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
        ],
      ),
    );
  }
}