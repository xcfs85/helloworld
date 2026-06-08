import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class MyDiagramsPage extends StatefulWidget {
  const MyDiagramsPage({super.key});

  @override
  State<MyDiagramsPage> createState() => _MyDiagramsPageState();
}

class _MyDiagramsPageState extends State<MyDiagramsPage> {
  String _activeTab = '全部';

  final List<Map<String, dynamic>> _diagrams = [
    {'name': '小咪肖像', 'board': '50×50', 'colors': '28 色', 'time': '今天 14:23', 'status': '已完成'},
    {'name': '海边日落', 'board': '58×58', 'colors': '45 色', 'time': '昨天', 'status': '导出'},
    {'name': '星空熊', 'board': '29×29', 'colors': '20 色', 'time': '2 天前', 'status': '已完成'},
    {'name': '樱花树', 'board': '50×50', 'colors': '32 色', 'time': '3 天前', 'status': '已完成'},
    {'name': '雪山小屋', 'board': '58×58', 'colors': '56 色', 'time': '1 周前', 'status': '已完成'},
    {'name': '薰衣草田', 'board': '50×50', 'colors': '42 色', 'time': '1 周前', 'status': '已完成'},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('我的图纸'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          IconButton(icon: const Icon(Icons.grid_view_rounded, size: 22), onPressed: () {}),
        ],
      ),
      body: Container(
        color: AppTheme.surface,
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  children: ['全部', '已完成', '生成中', '导出'].map((t) {
                    final active = _activeTab == t;
                    return GestureDetector(
                      onTap: () => setState(() => _activeTab = t),
                      child: Container(
                        margin: const EdgeInsets.only(right: 8),
                        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 6),
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(999),
                          color: active ? AppTheme.ink : AppTheme.bg2,
                        ),
                        child: Text(
                          t,
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
              child: ListView.separated(
                padding: const EdgeInsets.all(16),
                itemCount: _diagrams.length,
                separatorBuilder: (_, __) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  final d = _diagrams[index];
                  final gradients = [
                    [const Color(0xFFFFD2B0), const Color(0xFFFF7A5A)],
                    [const Color(0xFF9DC8E5), const Color(0xFF6BC7A1)],
                    [const Color(0xFFB49DD8), const Color(0xFFF2A6A6)],
                    [const Color(0xFFF5C45E), const Color(0xFFFFB088)],
                    [const Color(0xFF6BC7A1), const Color(0xFF9DC8E5)],
                    [const Color(0xFFF2A6A6), const Color(0xFFB49DD8)],
                  ];
                  return GestureDetector(
                    onTap: () => Navigator.pushNamed(context, AppRoutes.resultPreview),
                    child: Container(
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        border: Border.all(color: AppTheme.line),
                      ),
                      child: Row(
                        children: [
                          Container(
                            width: 72,
                            height: 72,
                            margin: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(12),
                              gradient: LinearGradient(colors: gradients[index]),
                            ),
                          ),
                          Expanded(
                            child: Padding(
                              padding: const EdgeInsets.symmetric(vertical: 10),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(d['name'], style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                                  const SizedBox(height: 2),
                                  Text(
                                    '${d['board']} 板 · ${d['colors']}',
                                    style: const TextStyle(fontSize: 11, color: AppTheme.ink3),
                                  ),
                                  const SizedBox(height: 2),
                                  Text(d['time'], style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
                                ],
                              ),
                            ),
                          ),
                          Padding(
                            padding: const EdgeInsets.only(right: 12),
                            child: Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(999),
                                color: d['status'] == '已完成' ? AppTheme.mint.withOpacity(0.2) : AppTheme.accent.withOpacity(0.2),
                              ),
                              child: Text(
                                d['status'],
                                style: TextStyle(
                                  fontSize: 10,
                                  color: d['status'] == '已完成' ? AppTheme.mint : AppTheme.accent,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
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
      ),
    );
  }
}