import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class TemplatesPage extends StatefulWidget {
  const TemplatesPage({super.key});

  @override
  State<TemplatesPage> createState() => _TemplatesPageState();
}

class _TemplatesPageState extends State<TemplatesPage> {
  String _activeCat = '推荐';

  final List<Map<String, dynamic>> _templates = [
    {'name': '圣诞老人', 'cat': '节日', 'colors': '28 色', 'board': '50×50', 'likes': '1.2k'},
    {'name': '小柯基', 'cat': '宠物', 'colors': '22 色', 'board': '29×29', 'likes': '892'},
    {'name': 'Hello Kitty', 'cat': '卡通', 'colors': '15 色', 'board': '29×29', 'likes': '2.1k'},
    {'name': '中秋玉兔', 'cat': '节日', 'colors': '34 色', 'board': '50×50', 'likes': '543'},
    {'name': '初音未来', 'cat': '二次元', 'colors': '42 色', 'board': '50×50', 'likes': '3.4k'},
    {'name': '海边日落', 'cat': '风景', 'colors': '38 色', 'board': '50×50', 'likes': '678'},
    {'name': '青绿山水', 'cat': '国风', 'colors': '52 色', 'board': '58×58', 'likes': '432'},
    {'name': '熊猫头', 'cat': '表情', 'colors': '8 色', 'board': '29×29', 'likes': '5.6k'},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('模板'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          IconButton(icon: const Icon(Icons.search_rounded, size: 22), onPressed: () {}),
          IconButton(icon: const Icon(Icons.filter_list_rounded, size: 22), onPressed: () {}),
        ],
      ),
      body: Column(
        children: [
          Container(
            color: AppTheme.surface,
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 12),
            child: TextField(
              decoration: InputDecoration(
                hintText: '搜索模板 / 创作者 / 标签',
                hintStyle: const TextStyle(fontSize: 13, color: AppTheme.ink3),
                prefixIcon: const Icon(Icons.search_rounded, size: 16, color: AppTheme.ink3),
                suffixIcon: const Icon(Icons.format_list_bulleted_rounded, size: 16, color: AppTheme.ink3),
                filled: true,
                fillColor: AppTheme.bg,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide.none,
                ),
                contentPadding: EdgeInsets.zero,
              ),
            ),
          ),
          Container(
            color: AppTheme.surface,
            padding: const EdgeInsets.only(bottom: 12),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Row(
                children: [
                  '推荐', '节日', '卡通', '二次元', '宠物', '风景', '像素游戏', '国风', '表情包', '文字',
                ].map((cat) {
                  final active = _activeCat == cat;
                  return GestureDetector(
                    onTap: () => setState(() => _activeCat = cat),
                    child: Container(
                      margin: const EdgeInsets.only(right: 8),
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(999),
                        color: active ? AppTheme.ink : AppTheme.bg,
                      ),
                      child: Text(
                        cat,
                        style: TextStyle(
                          fontSize: 12,
                          color: active ? Colors.white : AppTheme.ink2,
                        ),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
          Expanded(
            child: GridView.builder(
              padding: const EdgeInsets.all(12),
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2,
                crossAxisSpacing: 8,
                mainAxisSpacing: 8,
                childAspectRatio: 0.78,
              ),
              itemCount: _templates.length,
              itemBuilder: (context, index) {
                final t = _templates[index];
                final gradients = [
                  [const Color(0xFFFFD2B0), const Color(0xFFFF7A5A)],
                  [const Color(0xFF9DC8E5), const Color(0xFF6BC7A1)],
                  [const Color(0xFFF2A6A6), const Color(0xFFB49DD8)],
                  [const Color(0xFFF5C45E), const Color(0xFFFFB088)],
                  [const Color(0xFF6BC7A1), const Color(0xFF9DC8E5)],
                  [const Color(0xFFB49DD8), const Color(0xFFF2A6A6)],
                  [const Color(0xFFFF7A5A), const Color(0xFF8B5A3C)],
                  [const Color(0xFF9DC8E5), const Color(0xFFB49DD8)],
                ];
                return GestureDetector(
                  onTap: () => Navigator.pushNamed(context, AppRoutes.templateDetail),
                  child: Container(
                    decoration: BoxDecoration(
                      color: AppTheme.surface,
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
                              gradient: LinearGradient(colors: gradients[index]),
                            ),
                            child: Stack(
                              children: [
                                Positioned(
                                  left: 8, top: 8,
                                  child: Container(
                                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                                    decoration: BoxDecoration(
                                      color: Colors.black.withOpacity(0.5),
                                      borderRadius: BorderRadius.circular(8),
                                    ),
                                    child: Text(t['cat'], style: const TextStyle(color: Colors.white, fontSize: 10)),
                                  ),
                                ),
                                Positioned(
                                  right: 8, top: 8,
                                  child: Icon(Icons.favorite_border, color: Colors.white.withOpacity(0.8), size: 16),
                                ),
                              ],
                            ),
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.all(10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(t['name'], style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                              const SizedBox(height: 4),
                              Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  Text('${t['colors']} / ${t['board']}', style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
                                  Row(
                                    children: [
                                      const Icon(Icons.favorite, size: 10, color: AppTheme.rose),
                                      const SizedBox(width: 2),
                                      Text(t['likes'], style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
                                    ],
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}