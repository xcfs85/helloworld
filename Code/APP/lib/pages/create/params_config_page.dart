import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/app_constants.dart';
import '../../config/routes.dart';

class ParamsConfigPage extends StatefulWidget {
  const ParamsConfigPage({super.key});

  @override
  State<ParamsConfigPage> createState() => _ParamsConfigPageState();
}

class _ParamsConfigPageState extends State<ParamsConfigPage> {
  int _beadCount = 5000;
  String _boardSize = '50×50';
  String _difficulty = '进阶';
  String _style = '写实';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('参数配置'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pushNamed(context, AppRoutes.generating),
            child: const Text('跳过', style: TextStyle(color: AppTheme.primaryInk, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
      body: Column(
        children: [
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildRecommend(),
                  const SizedBox(height: 10),
                  _buildPreview(),
                  const SizedBox(height: 16),
                  _buildSection('总颗数', '颗数越多越精细'),
                  _buildBeadCountOptions(),
                  const SizedBox(height: 16),
                  _buildSection('底板规格', '29 / 50 / 58'),
                  _buildBoardSizeOptions(),
                  const SizedBox(height: 16),
                  _buildSection('难度', '影响 AI 处理的色数'),
                  _buildDifficultyOptions(),
                  const SizedBox(height: 16),
                  _buildSection('风格', 'MVP 仅 2 种'),
                  _buildStyleOptions(),
                ],
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(16),
            child: SizedBox(
              width: double.infinity,
              height: 48,
              child: ElevatedButton(
                onPressed: () => Navigator.pushNamed(context, AppRoutes.generating),
                child: const Text('开始 AI 生成'),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildRecommend() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: const Color(0xFFFFF1E2),
        borderRadius: BorderRadius.circular(999),
      ),
      child: const Text(
        '✨ 智能推荐 · 这是一张人物肖像',
        style: TextStyle(fontSize: 11, fontWeight: FontWeight.w600, color: AppTheme.primaryInk),
      ),
    );
  }

  Widget _buildPreview() {
    return Row(
      children: [
        Container(
          width: 90,
          height: 90,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(14),
            gradient: const LinearGradient(colors: [Color(0xFFFFD2B0), Color(0xFFF5C45E)]),
          ),
        ),
        const SizedBox(width: 10),
        const Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('建议：50×50 板 / 5000 颗 / 进阶', style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
              SizedBox(height: 2),
              Text('预计 8-10 小时完成 · 适合熟手', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildSection(String title, String hint) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(title, style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
          Text(hint, style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
        ],
      ),
    );
  }

  Widget _buildBeadCountOptions() {
    final options = [
      (500, '钥匙扣'),
      (1000, '小卡'),
      (2000, null),
      (3000, null),
      (5000, '推荐'),
      (8000, null),
      (10000, '大作'),
    ];
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: options.map((o) {
        final isActive = _beadCount == o.$1;
        return GestureDetector(
          onTap: () => setState(() => _beadCount = o.$1),
          child: Container(
            width: (MediaQuery.of(context).size.width - 48) / 4,
            padding: const EdgeInsets.symmetric(vertical: 10),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              color: isActive ? const Color(0xFFFFF1E8) : AppTheme.surface,
              border: Border.all(
                color: isActive ? AppTheme.primary : AppTheme.line,
                width: 1.5,
              ),
            ),
            child: Column(
              children: [
                Text(
                  '${o.$1}',
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: isActive ? AppTheme.primaryInk : AppTheme.ink2,
                  ),
                ),
                if (o.$2 != null)
                  Text(
                    o.$2!,
                    style: TextStyle(
                      fontSize: 10,
                      color: isActive ? AppTheme.primaryInk.withOpacity(0.7) : AppTheme.ink3,
                    ),
                  ),
              ],
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildBoardSizeOptions() {
    final options = [
      ('29×29', '7.5cm'),
      ('50×50', '13cm'),
      ('58×58', '15cm'),
    ];
    return Row(
      children: options.map((o) {
        final isActive = _boardSize == o.$1;
        return Expanded(
          child: GestureDetector(
            onTap: () => setState(() => _boardSize = o.$1),
            child: Container(
              margin: const EdgeInsets.symmetric(horizontal: 4),
              padding: const EdgeInsets.symmetric(vertical: 10),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                color: isActive ? const Color(0xFFFFF1E8) : AppTheme.surface,
                border: Border.all(
                  color: isActive ? AppTheme.primary : AppTheme.line,
                  width: 1.5,
                ),
              ),
              child: Column(
                children: [
                  Text(
                    o.$1,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: isActive ? AppTheme.primaryInk : AppTheme.ink2,
                    ),
                  ),
                  Text(
                    o.$2,
                    style: TextStyle(
                      fontSize: 10,
                      color: isActive ? AppTheme.primaryInk.withOpacity(0.7) : AppTheme.ink3,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildDifficultyOptions() {
    final options = [
      ('简单', '8-15 色'),
      ('进阶', '16-30 色'),
      ('写实', '30-80 色'),
    ];
    return Row(
      children: options.map((o) {
        final isActive = _difficulty == o.$1;
        return Expanded(
          child: GestureDetector(
            onTap: () => setState(() => _difficulty = o.$1),
            child: Container(
              margin: const EdgeInsets.symmetric(horizontal: 4),
              padding: const EdgeInsets.symmetric(vertical: 10),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                color: isActive ? const Color(0xFFFFF1E8) : AppTheme.surface,
                border: Border.all(
                  color: isActive ? AppTheme.primary : AppTheme.line,
                  width: 1.5,
                ),
              ),
              child: Column(
                children: [
                  Text(
                    o.$1,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: isActive ? AppTheme.primaryInk : AppTheme.ink2,
                    ),
                  ),
                  Text(
                    o.$2,
                    style: TextStyle(
                      fontSize: 10,
                      color: isActive ? AppTheme.primaryInk.withOpacity(0.7) : AppTheme.ink3,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildStyleOptions() {
    final options = [
      ('写实', '还原度高'),
      ('卡通', '色块鲜明'),
    ];
    return Row(
      children: options.map((o) {
        final isActive = _style == o.$1;
        return Expanded(
          child: GestureDetector(
            onTap: () => setState(() => _style = o.$1),
            child: Container(
              margin: const EdgeInsets.symmetric(horizontal: 4),
              padding: const EdgeInsets.symmetric(vertical: 10),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(12),
                color: isActive ? const Color(0xFFFFF1E8) : AppTheme.surface,
                border: Border.all(
                  color: isActive ? AppTheme.primary : AppTheme.line,
                  width: 1.5,
                ),
              ),
              child: Column(
                children: [
                  Text(
                    o.$1,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: isActive ? AppTheme.primaryInk : AppTheme.ink2,
                    ),
                  ),
                  Text(
                    o.$2,
                    style: TextStyle(
                      fontSize: 10,
                      color: isActive ? AppTheme.primaryInk.withOpacity(0.7) : AppTheme.ink3,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }
}