import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class TemplateDetailPage extends StatelessWidget {
  const TemplateDetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      body: Stack(
        children: [
          SingleChildScrollView(
            child: Column(
              children: [
                Stack(
                  children: [
                    Container(
                      height: 380,
                      decoration: const BoxDecoration(
                        gradient: LinearGradient(
                          colors: [Color(0xFFFFB088), Color(0xFFFF7A5A), Color(0xFF8B5A3C)],
                          begin: Alignment.topLeft,
                        ),
                      ),
                    ),
                    Positioned(
                      top: 0, left: 0, right: 0,
                      child: SafeArea(
                        child: Row(
                          children: [
                            IconButton(
                              icon: const Icon(Icons.chevron_left, color: Colors.white, size: 28),
                              onPressed: () => Navigator.pop(context),
                            ),
                            const Spacer(),
                            IconButton(
                              icon: const Icon(Icons.more_horiz, color: Colors.white),
                              onPressed: () {},
                            ),
                          ],
                        ),
                      ),
                    ),
                    Positioned(
                      left: 18, bottom: 24,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                            decoration: BoxDecoration(
                              color: Colors.black.withOpacity(0.4),
                              borderRadius: BorderRadius.circular(999),
                            ),
                            child: const Text('节日 · 亲子 · 入门', style: TextStyle(color: Colors.white, fontSize: 11)),
                          ),
                          const SizedBox(height: 8),
                          const Text('👀 12,432 · ♡ 1.2k · 已用 234', style: TextStyle(color: Colors.white70, fontSize: 11)),
                        ],
                      ),
                    ),
                  ],
                ),
                Container(
                  color: AppTheme.surface,
                  padding: const EdgeInsets.fromLTRB(18, 18, 18, 100),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('圣诞老人 🎅', style: TextStyle(fontSize: 22, fontWeight: FontWeight.w700)),
                      const SizedBox(height: 6),
                      const Wrap(
                        spacing: 14,
                        runSpacing: 4,
                        children: [
                          Text('📊 28 色 / 50×50 板', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                          Text('⏱ 8-10h', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                          Text('⭐ 4.9', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                          Text('📌 入门', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                        ],
                      ),
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          const CircleAvatar(
                            radius: 19,
                            backgroundColor: Color(0xFFFFB088),
                          ),
                          const SizedBox(width: 10),
                          const Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text('@拼豆达人 Lily', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                                Text('小红书：拼豆达人 Lily · 5,432 粉丝', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
                              ],
                            ),
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(999),
                              border: Border.all(color: AppTheme.line),
                            ),
                            child: const Text('+ 关注', style: TextStyle(fontSize: 12, color: AppTheme.ink2)),
                          ),
                        ],
                      ),
                      const SizedBox(height: 14),
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: AppTheme.bg2,
                          borderRadius: BorderRadius.circular(14),
                        ),
                        child: const Text(
                          '"给孩子的圣诞礼物🎄 30 分钟能拼完简单的部分，剩 1/3 留给他自己完成 ✨"',
                          style: TextStyle(fontSize: 13, color: AppTheme.ink2, height: 1.6),
                        ),
                      ),
                      const SizedBox(height: 14),
                      const Text('色号预览', style: TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          ...[
                            const Color(0xFFFF7A5A), const Color(0xFF2A1F1A), const Color(0xFFF5C45E),
                            const Color(0xFF6BC7A1), const Color(0xFF9DC8E5), const Color(0xFFB49DD8),
                            const Color(0xFF8B5A3C), const Color(0xFFF2A6A6),
                          ].map((c) => Container(
                            width: 24, height: 24,
                            margin: const EdgeInsets.only(right: 4),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(6),
                              color: c,
                              border: Border.all(color: AppTheme.line),
                            ),
                          )),
                          Container(
                            width: 24, height: 24,
                            margin: const EdgeInsets.only(right: 4),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(6),
                              color: Colors.white,
                              border: Border.all(color: AppTheme.line),
                            ),
                            child: const Center(
                              child: Text('+20', style: TextStyle(fontSize: 10, color: AppTheme.ink3)),
                            ),
                          ),
                          const Spacer(),
                          const Text('查看全部 ›', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
                        ],
                      ),
                      const SizedBox(height: 18),
                      const Text('同款作品', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                      const SizedBox(height: 10),
                      Row(
                        children: List.generate(3, (i) {
                          final colors = [
                            [const Color(0xFFFFD2B0), const Color(0xFFF5C45E)],
                            [const Color(0xFF9DC8E5), const Color(0xFF6BC7A1)],
                            [const Color(0xFFF2A6A6), const Color(0xFFB49DD8)],
                          ];
                          return Expanded(
                            child: Padding(
                              padding: EdgeInsets.only(right: i < 2 ? 6 : 0),
                              child: AspectRatio(
                                aspectRatio: 1,
                                child: Container(
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(10),
                                    gradient: LinearGradient(colors: colors[i]),
                                  ),
                                ),
                              ),
                            ),
                          );
                        }),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          Positioned(
            left: 0, right: 0, bottom: 0,
            child: Container(
              padding: const EdgeInsets.fromLTRB(16, 10, 16, 24),
              decoration: BoxDecoration(
                color: Colors.white,
                border: Border(top: BorderSide(color: AppTheme.line)),
              ),
              child: SafeArea(
                top: false,
                child: Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(14),
                        border: Border.all(color: AppTheme.line),
                      ),
                      child: const Icon(Icons.star_border_rounded, size: 20, color: AppTheme.ink2),
                    ),
                    const SizedBox(width: 10),
                    Expanded(
                      child: ElevatedButton(
                        onPressed: () {},
                        child: const Text('使用这个模板'),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}