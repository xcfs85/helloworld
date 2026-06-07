import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  String _activeTab = '作品';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      body: Column(
        children: [
          Stack(
            children: [
              Container(
                height: 160,
                decoration: const BoxDecoration(
                  gradient: LinearGradient(
                    colors: [Color(0xFFFFB088), Color(0xFFFF7A5A), Color(0xFFE25D3E)],
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
                        icon: const Icon(Icons.more_horiz, color: Colors.white, size: 22),
                        onPressed: () {},
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
          Container(
            color: AppTheme.surface,
            padding: const EdgeInsets.fromLTRB(18, 0, 18, 16),
            margin: const EdgeInsets.only(top: 0),
            child: Column(
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Container(
                      margin: const EdgeInsets.only(top: 0),
                      width: 72,
                      height: 72,
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        border: Border.all(color: Colors.white, width: 4),
                        gradient: const LinearGradient(
                          colors: [Color(0xFFFFB088), Color(0xFFFF7A5A)],
                        ),
                      ),
                      child: const Center(
                        child: Text('小', style: TextStyle(fontSize: 28, fontWeight: FontWeight.w700, color: Colors.white)),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Padding(
                        padding: const EdgeInsets.only(bottom: 6),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                const Text('小美 ✨', style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
                                const SizedBox(width: 6),
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                  decoration: BoxDecoration(
                                    color: const Color(0xFFFFE8D2),
                                    borderRadius: BorderRadius.circular(999),
                                  ),
                                  child: const Text('认证创作者', style: TextStyle(fontSize: 9, color: Color(0xFFB65A18))),
                                ),
                              ],
                            ),
                            const Text('ID: pd_87231 · 北京', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                          ],
                        ),
                      ),
                    ),
                    ElevatedButton(
                      onPressed: () {},
                      style: ElevatedButton.styleFrom(
                        minimumSize: Size.zero,
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                        textStyle: const TextStyle(fontSize: 13),
                      ),
                      child: const Text('+ 关注'),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                const Text(
                  '拼豆玩家 / 喜欢小动物 / 作品以肖像为主 🐱🐶',
                  style: TextStyle(fontSize: 13, color: AppTheme.ink2, height: 1.6),
                ),
                const SizedBox(height: 14),
                Container(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  decoration: BoxDecoration(
                    color: AppTheme.bg2,
                    borderRadius: BorderRadius.circular(14),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      _stat('23', '作品'),
                      _stat('89', '粉丝'),
                      _stat('156', '关注'),
                      _stat('1.2k', '获赞'),
                    ],
                  ),
                ),
              ],
            ),
          ),
          Container(
            color: AppTheme.surface,
            child: Row(
              children: ['作品 23', '教程 5', '收藏 87'].map((t) {
                final active = _activeTab == t.split(' ')[0];
                return GestureDetector(
                  onTap: () => setState(() => _activeTab = t.split(' ')[0]),
                  child: Container(
                    padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 18),
                    decoration: BoxDecoration(
                      border: Border(
                        bottom: BorderSide(
                          color: active ? AppTheme.primary : Colors.transparent,
                          width: 3,
                        ),
                      ),
                    ),
                    child: Text(
                      t,
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                        color: active ? AppTheme.ink : AppTheme.ink3,
                      ),
                    ),
                  ),
                );
              }).toList(),
            ),
          ),
          Expanded(
            child: Container(
              color: AppTheme.surface,
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 8,
                mainAxisSpacing: 8,
                padding: const EdgeInsets.all(12),
                children: List.generate(6, (i) {
                  final items = [
                    ('小咪肖像', '28 色 / 50×50', '♡ 234', const Color(0xFFFFD2B0)),
                    ('海边日落', '45 色 / 58×58', '♡ 567', const Color(0xFF9DC8E5)),
                    ('樱花树', '32 色 / 50×50', '♡ 189', const Color(0xFFB49DD8)),
                    ('星空熊', '20 色 / 29×29', '♡ 1024', const Color(0xFFF5C45E)),
                    ('雪山小屋', '56 色 / 58×58', '♡ 432', const Color(0xFF6BC7A1)),
                    ('薰衣草田', '42 色 / 50×50', '♡ 678', const Color(0xFFF2A6A6)),
                  ];
                  return Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: AppTheme.line),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Expanded(
                          child: Container(
                            decoration: BoxDecoration(
                              borderRadius: const BorderRadius.vertical(top: Radius.circular(15)),
                              gradient: LinearGradient(
                                colors: [items[i].$4, items[i].$4.withOpacity(0.5)],
                              ),
                            ),
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.all(10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(items[i].$1, style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                              Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(items[i].$2, style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
                                  Text(items[i].$3, style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  );
                }),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _stat(String num, String label) {
    return Column(
      children: [
        Text(num, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
        Text(label, style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
      ],
    );
  }
}