import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class ColorsTablePage extends StatefulWidget {
  const ColorsTablePage({super.key});

  @override
  State<ColorsTablePage> createState() => _ColorsTablePageState();
}

class _ColorsTablePageState extends State<ColorsTablePage> {
  String _filter = '全部';
  final List<Map<String, dynamic>> _colors = [
    {'code': 'M01', 'name': '珊瑚红', 'color': Color(0xFFFF7A5A), 'beads': 234, 'pct': 4.7},
    {'code': 'M15', 'name': '深棕黑', 'color': Color(0xFF2A1F1A), 'beads': 189, 'pct': 3.8},
    {'code': 'H08', 'name': '奶油黄', 'color': Color(0xFFF5C45E), 'beads': 156, 'pct': 3.1},
    {'code': 'G05', 'name': '薄荷绿', 'color': Color(0xFF6BC7A1), 'beads': 128, 'pct': 2.6},
    {'code': 'B12', 'name': '天空蓝', 'color': Color(0xFF9DC8E5), 'beads': 112, 'pct': 2.2},
    {'code': 'V03', 'name': '淡紫', 'color': Color(0xFFB49DD8), 'beads': 98, 'pct': 2.0},
    {'code': 'N07', 'name': '巧克力', 'color': Color(0xFF8B5A3C), 'beads': 76, 'pct': 1.5},
    {'code': 'P04', 'name': '樱花粉', 'color': Color(0xFFF2A6A6), 'beads': 62, 'pct': 1.2},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('色号表'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () {},
            child: const Text('导出', style: TextStyle(color: AppTheme.primaryInk, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
      body: Column(
        children: [
          _buildSummary(),
          _buildFilter(),
          Expanded(child: _buildTable()),
          _buildBottomBar(),
        ],
      ),
    );
  }

  Widget _buildSummary() {
    return Container(
      padding: const EdgeInsets.all(18),
      color: AppTheme.surface,
      child: Column(
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  RichText(
                    text: const TextSpan(
                      style: TextStyle(fontSize: 28, fontWeight: FontWeight.w700, color: AppTheme.ink),
                      children: [
                        TextSpan(text: '共 28 '),
                        TextSpan(text: '色', style: TextStyle(fontSize: 14, fontWeight: FontWeight.w500, color: AppTheme.ink3)),
                      ],
                    ),
                  ),
                  const Text('5,000 颗 · 50×50 板', style: TextStyle(fontSize: 13, color: AppTheme.ink3)),
                ],
              ),
              const Spacer(),
              const Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Text('完成时间', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
                  SizedBox(height: 2),
                  Text('约 8-10h', style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                ],
              ),
            ],
          ),
          const SizedBox(height: 12),
          ClipRRect(
            borderRadius: BorderRadius.circular(3),
            child: SizedBox(
              height: 6,
              child: Row(
                children: [
                  _barSegment(const Color(0xFFFF7A5A), 4.7),
                  _barSegment(const Color(0xFF2A1F1A), 3.8),
                  _barSegment(const Color(0xFFF5C45E), 3.2),
                  _barSegment(const Color(0xFF6BC7A1), 2.5),
                  _barSegment(const Color(0xFF9DC8E5), 2.1),
                  _barSegment(const Color(0xFFB49DD8), 1.8),
                  _barSegment(const Color(0xFF8B5A3C), 1.5),
                  _barSegment(const Color(0xFFF2A6A6), 1.2),
                  Expanded(child: _barSegment(AppTheme.bg2, 79.2)),
                ],
              ),
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              _statBox('5', '主色 (>5%)'),
              _statBox('12', '辅色 (1-5%)'),
              _statBox('11', '配色 (<1%)'),
            ],
          ),
        ],
      ),
    );
  }

  Widget _barSegment(Color color, double flex) {
    return Container(color: color);
  }

  Widget _statBox(String num, String label) {
    return Expanded(
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 4),
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: AppTheme.bg2,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Column(
          children: [
            Text(num, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
            Text(label, style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
          ],
        ),
      ),
    );
  }

  Widget _buildFilter() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 12),
      color: AppTheme.surface,
      child: Row(
        children: [
          Expanded(
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: ['全部 28', '主色', '辅色', '配色'].map((f) {
                  final active = _filter == f;
                  return GestureDetector(
                    onTap: () => setState(() => _filter = f),
                    child: Container(
                      margin: const EdgeInsets.only(right: 6),
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(999),
                        color: active ? AppTheme.ink : AppTheme.surface,
                        border: Border.all(color: active ? AppTheme.ink : AppTheme.line),
                      ),
                      child: Text(
                        f,
                        style: TextStyle(
                          fontSize: 12,
                          color: active ? Colors.white : AppTheme.ink2,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
          GestureDetector(
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(999),
                border: Border.all(color: AppTheme.line),
              ),
              child: const Row(
                children: [
                  Icon(Icons.format_list_numbered_rounded, size: 12, color: AppTheme.ink2),
                  SizedBox(width: 4),
                  Text('颗数↓', style: TextStyle(fontSize: 12, color: AppTheme.ink2)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTable() {
    return Container(
      color: AppTheme.surface,
      child: ListView.separated(
        itemCount: _colors.length,
        separatorBuilder: (_, __) => Divider(height: 1, color: AppTheme.line),
        itemBuilder: (context, index) {
          final c = _colors[index];
          return Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
            child: Row(
              children: [
                SizedBox(
                  width: 24,
                  child: Text(
                    '${(index + 1).toString().padLeft(2, '0')}',
                    style: const TextStyle(fontSize: 12, color: AppTheme.ink3),
                  ),
                ),
                const SizedBox(width: 8),
                Text(c['code'], style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
                const SizedBox(width: 8),
                Container(
                  width: 24,
                  height: 24,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(6),
                    color: c['color'],
                    border: Border.all(color: AppTheme.line),
                  ),
                ),
                const SizedBox(width: 6),
                Text(c['name'], style: const TextStyle(fontSize: 12, color: AppTheme.ink2)),
                const Spacer(),
                Text('${c['beads']}', style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
                const SizedBox(width: 24),
                Text('${c['pct']}%', style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
                const SizedBox(width: 16),
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: List.generate(5, (i) {
                    return Container(
                      width: 6,
                      height: 6,
                      margin: const EdgeInsets.only(right: 2),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(1),
                        color: i < (c['pct'] as double / 2).round() ? AppTheme.ink2 : AppTheme.ink3.withOpacity(0.3),
                      ),
                    );
                  }),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildBottomBar() {
    return Container(
      padding: const EdgeInsets.fromLTRB(16, 10, 16, 24),
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
                icon: const Icon(Icons.download_rounded, size: 16),
                label: const Text('导出'),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: ElevatedButton(
                onPressed: () {},
                child: const Text('耗材推荐 · MARD 168 色 ¥89', style: TextStyle(fontSize: 12)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}