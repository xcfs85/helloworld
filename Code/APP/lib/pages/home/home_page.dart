import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';
import '../../widgets/tab_bar_widget.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _currentTab = 0;

  void _onTabChanged(int index) {
    setState(() => _currentTab = index);
    switch (index) {
      case 1:
        Navigator.of(context).pushNamed(AppRoutes.templates);
        break;
      case 2:
        Navigator.of(context).pushNamed(AppRoutes.selectImage);
        break;
      case 3:
        Navigator.of(context).pushNamed(AppRoutes.forum);
        break;
      case 4:
        Navigator.of(context).pushNamed(AppRoutes.mine);
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          _buildAppBar(),
          Expanded(
            child: SingleChildScrollView(
              child: Column(
                children: [
                  _buildHero(),
                  _buildGrid(),
                  _buildBanner(),
                  _buildSectionTitle('社区精选', '查看更多 ›'),
                  _buildFeedList(),
                ],
              ),
            ),
          ),
          PindouTabBar(currentIndex: _currentTab, onTap: _onTabChanged),
        ],
      ),
    );
  }

  Widget _buildAppBar() {
    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 10),
        child: Row(
          children: [
            Container(
              width: 30,
              height: 30,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(9),
                gradient: const LinearGradient(
                  colors: [Color(0xFFFFE2D3), Color(0xFFFFD2B0)],
                ),
              ),
              child: const Icon(Icons.grid_view_rounded, size: 18, color: AppTheme.primary),
            ),
            const SizedBox(width: 6),
            const Text(
              '拼豆',
              style: TextStyle(fontSize: 15, fontWeight: FontWeight.w700),
            ),
            const Spacer(),
            IconButton(
              icon: const Icon(Icons.search_rounded, size: 22),
              onPressed: () {},
              color: AppTheme.ink2,
              style: IconButton.styleFrom(
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
            ),
            IconButton(
              icon: const Icon(Icons.notifications_outlined, size: 22),
              onPressed: () => Navigator.pushNamed(context, AppRoutes.messages),
              color: AppTheme.ink2,
              style: IconButton.styleFrom(
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHero() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(24),
        gradient: const LinearGradient(
          colors: [Color(0xFFFFB088), Color(0xFFFF7A5A), Color(0xFFE25D3E)],
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            '把照片变成拼豆',
            style: TextStyle(
              fontSize: 22,
              fontWeight: FontWeight.w700,
              color: Colors.white,
            ),
          ),
          const SizedBox(height: 6),
          const Text(
            'AI 一键生成 · 1 分钟得到可拼图纸',
            style: TextStyle(fontSize: 13, color: Colors.white70),
          ),
          const SizedBox(height: 16),
          ElevatedButton(
            onPressed: () => Navigator.pushNamed(context, AppRoutes.selectImage),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.white,
              foregroundColor: AppTheme.primaryInk,
              minimumSize: Size.zero,
              padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 10),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18)),
              elevation: 0,
            ),
            child: const Text('开始创作', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
    );
  }

  Widget _buildGrid() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Row(
        children: [
          _gridTile(Icons.auto_awesome_rounded, const Color(0xFFFFE2D3), AppTheme.primary, 'AI 生成'),
          _gridTile(Icons.forum_rounded, const Color(0xFFFFE0DC), AppTheme.rose, '社区'),
          _gridTile(Icons.grid_view_rounded, const Color(0xFFDDE9FF), const Color(0xFF5A8AFF), '模板库'),
          _gridTile(Icons.star_rounded, const Color(0xFFDFF5E9), AppTheme.mint, '色号推荐'),
        ],
      ),
    );
  }

  Widget _gridTile(IconData icon, Color bg, Color color, String label) {
    return Expanded(
      child: GestureDetector(
        onTap: () {
          if (label == 'AI 生成') Navigator.pushNamed(context, AppRoutes.selectImage);
          if (label == '社区') Navigator.pushNamed(context, AppRoutes.forum);
          if (label == '模板库') Navigator.pushNamed(context, AppRoutes.templates);
        },
        child: Container(
          margin: const EdgeInsets.symmetric(horizontal: 4, vertical: 12),
          padding: const EdgeInsets.symmetric(vertical: 14),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: AppTheme.line),
          ),
          child: Column(
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: bg,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(icon, size: 20, color: color),
              ),
              const SizedBox(height: 6),
              Text(label, style: const TextStyle(fontSize: 11, color: AppTheme.ink2, fontWeight: FontWeight.w500)),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildBanner() {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      height: 96,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(18),
        gradient: const LinearGradient(
          colors: [Color(0xFF9DC8E5), Color(0xFF6BC7A1)],
        ),
      ),
      child: Row(
        children: [
          const Padding(
            padding: EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text(
                  '新手福利',
                  style: TextStyle(fontSize: 15, fontWeight: FontWeight.w700, color: Colors.white),
                ),
                SizedBox(height: 2),
                Text(
                  '首次生成免费 · 赠 5 张色卡',
                  style: TextStyle(fontSize: 11, color: Colors.white70),
                ),
              ],
            ),
          ),
          const Spacer(),
          Container(
            width: 32,
            height: 32,
            margin: const EdgeInsets.only(right: 16),
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: Colors.white.withOpacity(0.25),
            ),
            child: const Icon(Icons.chevron_right, color: Colors.white, size: 18),
          ),
        ],
      ),
    );
  }

  Widget _buildSectionTitle(String title, String action) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(18, 18, 18, 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(title, style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
          Text(action, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
        ],
      ),
    );
  }

  Widget _buildFeedList() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          _feedItem(
            gradient: const [Color(0xFFFFD2B0), Color(0xFFF5C45E)],
            type: '作品',
            title: '给小咪的拼豆肖像 🐱',
            info: '@小美 · 50×50 板 · 28 色 · 拼了 8h',
            likes: '1.2k',
            comments: '89',
            favorites: '234',
          ),
          _feedItem(
            gradient: const [Color(0xFF9DC8E5), Color(0xFF6BC7A1)],
            type: '教程',
            title: '新手必看 · 0 基础入门拼豆',
            info: '@Lily 老师 · 8 分钟看完',
            likes: '567',
            comments: '23',
            favorites: '178',
          ),
        ],
      ),
    );
  }

  Widget _feedItem({
    required List<Color> gradient,
    required String type,
    required String title,
    required String info,
    required String likes,
    required String comments,
    required String favorites,
  }) {
    return GestureDetector(
      onTap: () => Navigator.pushNamed(context, AppRoutes.postDetail),
      child: Container(
        margin: const EdgeInsets.only(bottom: 14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: AppTheme.line),
        ),
        child: Column(
          children: [
            AspectRatio(
              aspectRatio: 1.6,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: const BorderRadius.vertical(top: Radius.circular(17)),
                  gradient: LinearGradient(colors: gradient),
                ),
                child: Stack(
                  children: [
                    Positioned(
                      left: 12,
                      bottom: 10,
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: Colors.black.withOpacity(0.4),
                          borderRadius: BorderRadius.circular(999),
                        ),
                        child: Text(type, style: const TextStyle(color: Colors.white, fontSize: 11)),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                  const SizedBox(height: 4),
                  Text(info, style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      _actIcon(Icons.favorite_border_rounded, likes),
                      const SizedBox(width: 14),
                      _actIcon(Icons.chat_bubble_outline_rounded, comments),
                      const SizedBox(width: 14),
                      _actIcon(Icons.star_border_rounded, favorites),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _actIcon(IconData icon, String label) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppTheme.ink3),
        const SizedBox(width: 4),
        Text(label, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
      ],
    );
  }
}