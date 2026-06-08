import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class MessagesPage extends StatelessWidget {
  const MessagesPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('消息'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.tune_rounded, size: 22),
            onPressed: () {},
          ),
        ],
      ),
      body: Container(
        color: AppTheme.surface,
        child: ListView(
          children: [
            _msgRow('💬', const [Color(0xFFFF7A5A), Color(0xFFF5C45E)], '评论', 'Lily 老师：太厉害啦！第 5 行能看出…', 12),
            _msgRow('❤', const [Color(0xFFF2A6A6), Color(0xFFFF7A5A)], '收到的赞', '你的作品"给闺蜜的生日礼物🎁"收获了 234 个赞', 234),
            _msgRow('➕', const [Color(0xFF6BC7A1), Color(0xFF9DC8E5)], '新增关注', '求图达人 等 5 人关注了你', 5),
            _msgRow('🔔', const [Color(0xFF9DC8E5), Color(0xFFB49DD8)], '@我的', '求图达人 @你 求这个图纸！', 2),
            _msgRow('📢', const [Color(0xFFF5C45E), Color(0xFFFF8A5A)], '系统通知', '【活动】新用户首单免费生成', 0),
          ],
        ),
      ),
    );
  }

  Widget _msgRow(String emoji, List<Color> gradient, String title, String subtitle, int badgeCount) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
      decoration: BoxDecoration(
        border: Border(bottom: BorderSide(color: AppTheme.line)),
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              gradient: LinearGradient(colors: gradient),
            ),
            child: Center(
              child: Text(emoji, style: const TextStyle(fontSize: 18)),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                const SizedBox(height: 2),
                Text(
                  subtitle,
                  style: const TextStyle(fontSize: 11, color: AppTheme.ink3),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
          if (badgeCount > 0)
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                color: const Color(0xFFFF3B30),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Text(
                '$badgeCount',
                style: const TextStyle(color: Colors.white, fontSize: 9, fontWeight: FontWeight.w600),
              ),
            ),
          const SizedBox(width: 8),
          const Icon(Icons.chevron_right, color: AppTheme.ink3, size: 18),
        ],
      ),
    );
  }
}