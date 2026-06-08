import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class SelectImagePage extends StatelessWidget {
  const SelectImagePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('选择照片'),
        backgroundColor: Colors.transparent,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Column(
        children: [
          Expanded(
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _sectionTitle('从相册选择', '全部 ›'),
                  _buildPhotoGrid(),
                  _sectionTitle('推荐示例图', '更多 ›'),
                  _buildExampleGrid(),
                ],
              ),
            ),
          ),
          _buildBottomBar(context),
        ],
      ),
    );
  }

  Widget _sectionTitle(String title, String action) {
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

  Widget _buildPhotoGrid() {
    final colors = [
      [const Color(0xFFFFD2B0), AppTheme.primary],
      [const Color(0xFF9DC8E5), AppTheme.mint],
      [const Color(0xFFF2A6A6), const Color(0xFFB49DD8)],
      [const Color(0xFFF5C45E), const Color(0xFFFF8A5A)],
      [const Color(0xFF6BC7A1), const Color(0xFF9DC8E5)],
      [const Color(0xFFB49DD8), const Color(0xFFF2A6A6)],
    ];
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: GridView.builder(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 3,
          crossAxisSpacing: 8,
          mainAxisSpacing: 8,
        ),
        itemCount: 9,
        itemBuilder: (context, index) {
          final c = colors[index % colors.length];
          return GestureDetector(
            onTap: () => Navigator.pushNamed(context, AppRoutes.editImage),
            child: Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(14),
                gradient: LinearGradient(colors: c),
              ),
              child: index == 0
                  ? Stack(
                      children: [
                        Positioned(
                          left: 8,
                          bottom: 6,
                          child: Text('小咪', style: const TextStyle(color: Colors.white, fontSize: 10, fontWeight: FontWeight.w600)),
                        ),
                        Positioned(
                          top: 6,
                          right: 6,
                          child: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: Colors.black.withOpacity(0.4),
                              borderRadius: BorderRadius.circular(6),
                            ),
                            child: const Text('最近', style: TextStyle(color: Colors.white, fontSize: 9)),
                          ),
                        ),
                      ],
                    )
                  : null,
            ),
          );
        },
      ),
    );
  }

  Widget _buildExampleGrid() {
    final examples = [
      ('宠物合集\n8 张', const Color(0xFFFFE6D4)),
      ('卡通角色\n12 张', const Color(0xFFDDE9FF)),
      ('风景\n6 张', const Color(0xFFDFF5E9)),
      ('情侣\n8 张', const Color(0xFFFFE2D3)),
      ('节日\n10 张', const Color(0xFFFFF1E2)),
      ('头像\n14 张', const Color(0xFFEFE6FF)),
    ];
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: GridView.builder(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 3,
          crossAxisSpacing: 8,
          mainAxisSpacing: 8,
        ),
        itemCount: 6,
        itemBuilder: (context, index) {
          return Container(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(14),
              color: examples[index].$2,
            ),
            child: Center(
              child: Text(
                examples[index].$1,
                textAlign: TextAlign.center,
                style: const TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.w600,
                  color: AppTheme.primaryInk,
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildBottomBar(BuildContext context) {
    return Container(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.95),
        border: Border(top: BorderSide(color: AppTheme.line)),
      ),
      child: SafeArea(
        top: false,
        child: Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () {},
                icon: const Icon(Icons.camera_alt_rounded, size: 18),
                label: const Text('拍照'),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: ElevatedButton.icon(
                onPressed: () => Navigator.pushNamed(context, AppRoutes.editImage),
                icon: const Icon(Icons.photo_library_rounded, size: 18),
                label: const Text('从相册选择'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}