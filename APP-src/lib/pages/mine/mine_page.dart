import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class MinePage extends StatelessWidget {
  const MinePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('我的'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        actions: [
          IconButton(
            icon: const Icon(Icons.tune_rounded, size: 22),
            onPressed: () => Navigator.pushNamed(context, AppRoutes.settings),
          ),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            Container(
              color: AppTheme.surface,
              padding: const EdgeInsets.all(18),
              child: Row(
                children: [
                  const CircleAvatar(
                    radius: 30,
                    backgroundColor: Color(0xFFFFB088),
                    child: Text('小', style: TextStyle(fontSize: 24, fontWeight: FontWeight.w700, color: Colors.white)),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('小美', style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
                        const SizedBox(height: 4),
                        Row(
                          children: [
                            _stat('23', '作品'),
                            const SizedBox(width: 24),
                            _stat('89', '粉丝'),
                            const SizedBox(width: 24),
                            _stat('156', '关注'),
                          ],
                        ),
                      ],
                    ),
                  ),
                  const Icon(Icons.chevron_right, color: AppTheme.ink3),
                ],
              ),
            ),
            const SizedBox(height: 10),
            Container(
              color: AppTheme.surface,
              padding: const EdgeInsets.symmetric(vertical: 16),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  _menuIcon(Icons.grid_view_rounded, '我的图纸', '12', () => Navigator.pushNamed(context, AppRoutes.myDiagrams)),
                  _menuIcon(Icons.star_rounded, '我的收藏', '87', () {}),
                  _menuIcon(Icons.history_rounded, '历史', '34', () {}),
                  _menuIcon(Icons.download_rounded, '下载', '8', () {}),
                ],
              ),
            ),
            const SizedBox(height: 10),
            Container(
              color: AppTheme.surface,
              child: Column(
                children: [
                  _menuItem(Icons.workspace_premium_rounded, '会员中心', '免费体验', () => Navigator.pushNamed(context, AppRoutes.vip)),
                  _menuItem(Icons.auto_awesome_rounded, '创作者中心', '投稿模板', () => Navigator.pushNamed(context, AppRoutes.creatorSubmit)),
                  _menuItem(Icons.card_giftcard_rounded, '邀请好友', '得免费生成次数', () {}),
                  _menuItem(Icons.shopping_bag_rounded, '商城', '拼豆工具 & 材料', () {}),
                ],
              ),
            ),
            const SizedBox(height: 10),
            Container(
              color: AppTheme.surface,
              child: Column(
                children: [
                  _menuItem(Icons.help_outline_rounded, '帮助与反馈', '', () {}),
                  _menuItem(Icons.info_outline_rounded, '关于拼豆', 'v0.1.0', () {}),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _stat(String num, String label) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(num, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w700)),
        const SizedBox(width: 2),
        Text(label, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
      ],
    );
  }

  Widget _menuIcon(IconData icon, String label, String count, VoidCallback onTap) {
    return GestureDetector(
      onTap: onTap,
      child: Column(
        children: [
          Icon(icon, size: 22, color: AppTheme.ink2),
          const SizedBox(height: 4),
          Text(count, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w700)),
          Text(label, style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
        ],
      ),
    );
  }

  Widget _menuItem(IconData icon, String title, String subtitle, VoidCallback onTap) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
        decoration: BoxDecoration(
          border: Border(bottom: BorderSide(color: AppTheme.line)),
        ),
        child: Row(
          children: [
            Container(
              width: 32,
              height: 32,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(8),
                color: AppTheme.bg2,
              ),
              child: Icon(icon, size: 16, color: AppTheme.ink2),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500)),
            ),
            if (subtitle.isNotEmpty)
              Text(subtitle, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
            const SizedBox(width: 4),
            const Icon(Icons.chevron_right, size: 16, color: AppTheme.ink3),
          ],
        ),
      ),
    );
  }
}