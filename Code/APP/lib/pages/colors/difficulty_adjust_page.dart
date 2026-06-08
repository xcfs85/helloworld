import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class DifficultyAdjustPage extends StatefulWidget {
  const DifficultyAdjustPage({super.key});

  @override
  State<DifficultyAdjustPage> createState() => _DifficultyAdjustPageState();
}

class _DifficultyAdjustPageState extends State<DifficultyAdjustPage> {
  int _currentDifficulty = 2; // 0=极简, 1=简单, 2=标准, 3=精细, 4=极致
  final List<Map<String, String>> _levels = [
    {'name': '极简', 'range': '8-12'},
    {'name': '简单', 'range': '13-20'},
    {'name': '标准', 'range': '21-35'},
    {'name': '精细', 'range': '36-60'},
    {'name': '极致', 'range': '60-100'},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('难度调整'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('取消', style: TextStyle(color: AppTheme.primaryInk, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
      body: Column(
        children: [
          Container(
            padding: const EdgeInsets.all(18),
            color: AppTheme.surface,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('当前色数', style: TextStyle(fontSize: 13, color: AppTheme.ink3)),
                const SizedBox(height: 4),
                const Row(
                  crossAxisAlignment: CrossAxisAlignment.baseline,
                  textBaseline: TextBaseline.alphabetic,
                  children: [
                    Text('28', style: TextStyle(fontSize: 32, fontWeight: FontWeight.w700)),
                    SizedBox(width: 8),
                    Text('色 · 5,000 颗', style: TextStyle(fontSize: 14, color: AppTheme.ink3)),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 10),
          Container(
            padding: const EdgeInsets.all(18),
            color: AppTheme.surface,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('目标难度', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                const SizedBox(height: 14),
                ClipRRect(
                  borderRadius: BorderRadius.circular(4),
                  child: SizedBox(
                    height: 8,
                    child: Row(
                      children: List.generate(5, (i) {
                        return Expanded(
                          child: Container(
                            color: i <= _currentDifficulty ? AppTheme.primary : AppTheme.bg2,
                          ),
                        );
                      }),
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                Row(
                  children: List.generate(5, (i) {
                    final active = i == _currentDifficulty;
                    return Expanded(
                      child: GestureDetector(
                        onTap: () => setState(() => _currentDifficulty = i),
                        child: Column(
                          children: [
                            Text(
                              _levels[i]['name']!,
                              style: TextStyle(
                                fontSize: 10,
                                fontWeight: active ? FontWeight.w600 : FontWeight.w400,
                                color: active ? AppTheme.primaryInk : AppTheme.ink,
                              ),
                            ),
                            Text(
                              _levels[i]['range']!,
                              style: const TextStyle(fontSize: 10, color: AppTheme.ink3),
                            ),
                          ],
                        ),
                      ),
                    );
                  }),
                ),
                const SizedBox(height: 18),
                Container(
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: AppTheme.bg2,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('预计调整', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                      const SizedBox(height: 4),
                      RichText(
                        text: const TextSpan(
                          style: TextStyle(fontSize: 12, color: AppTheme.ink2, height: 1.6),
                          children: [
                            TextSpan(text: '当前 28 色 → '),
                            TextSpan(text: '18 色', style: TextStyle(fontWeight: FontWeight.w700)),
                            TextSpan(text: '\n减少 10 色 · 智能合并相近色\n预计用时从 10h → '),
                            TextSpan(text: '7h', style: TextStyle(fontWeight: FontWeight.w700)),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 18),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 18),
            child: SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () {},
                child: const Text('应用调整'),
              ),
            ),
          ),
          const SizedBox(height: 10),
          const Center(
            child: Text(
              '调整不会损失原图，可在历史版本回退',
              style: TextStyle(fontSize: 11, color: AppTheme.ink3),
            ),
          ),
        ],
      ),
    );
  }
}