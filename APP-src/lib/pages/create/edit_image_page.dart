import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class EditImagePage extends StatefulWidget {
  const EditImagePage({super.key});

  @override
  State<EditImagePage> createState() => _EditImagePageState();
}

class _EditImagePageState extends State<EditImagePage> {
  String _activeTool = '裁剪';
  double _brightness = 10;
  double _contrast = 5;
  double _saturation = 15;

  final List<Map<String, dynamic>> _tools = [
    {'name': '裁剪', 'icon': Icons.crop_rounded},
    {'name': '旋转', 'icon': Icons.rotate_right_rounded},
    {'name': '水平', 'icon': Icons.flip_rounded},
    {'name': '垂直', 'icon': Icons.flip_to_back_rounded},
    {'name': '亮度', 'icon': Icons.brightness_6_rounded},
    {'name': '对比', 'icon': Icons.contrast_rounded},
    {'name': '饱和', 'icon': Icons.colorize_rounded},
    {'name': '比例', 'icon': Icons.aspect_ratio_rounded},
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('编辑'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pushNamed(context, AppRoutes.paramsConfig),
            child: const Text('下一步', style: TextStyle(color: AppTheme.primaryInk, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
      body: Column(
        children: [
          _buildCanvas(),
          _buildTools(),
          _buildSliders(),
          _buildBottomActions(context),
        ],
      ),
    );
  }

  Widget _buildCanvas() {
    return Container(
      margin: const EdgeInsets.all(16),
      height: 280,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        gradient: const LinearGradient(
          colors: [Color(0xFFFFD2B0), Color(0xFFFF7A5A), Color(0xFF8B5A3C)],
        ),
      ),
      child: Stack(
        children: [
          Positioned(
            top: 24,
            left: 24,
            right: 24,
            bottom: 24,
            child: Container(
              decoration: BoxDecoration(
                border: Border.all(color: Colors.white, width: 2, strokeAlign: BorderSide.strokeAlignInside),
                borderRadius: BorderRadius.circular(8),
                boxShadow: const [
                  BoxShadow(color: Colors.black38, blurRadius: 0, spreadRadius: 9999),
                ],
              ),
            ),
          ),
          ...['tl', 'tr', 'bl', 'br'].map((pos) {
            double? top, left, right, bottom;
            if (pos.contains('t')) top = 16;
            if (pos.contains('b')) bottom = 16;
            if (pos.contains('l')) left = 16;
            if (pos.contains('r')) right = 16;
            return Positioned(
              top: top,
              left: left,
              right: right,
              bottom: bottom,
              child: Container(
                width: 18,
                height: 18,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: Colors.white,
                  boxShadow: [BoxShadow(color: Colors.black.withOpacity(0.3), blurRadius: 6, offset: const Offset(0, 2))],
                ),
              ),
            );
          }),
        ],
      ),
    );
  }

  Widget _buildTools() {
    return SizedBox(
      height: 70,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemCount: _tools.length,
        separatorBuilder: (_, __) => const SizedBox(width: 6),
        itemBuilder: (context, index) {
          final tool = _tools[index];
          final isActive = _activeTool == tool['name'];
          return GestureDetector(
            onTap: () => setState(() => _activeTool = tool['name']),
            child: SizedBox(
              width: 56,
              child: Column(
                children: [
                  Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(12),
                      color: isActive ? AppTheme.ink : AppTheme.surface,
                      border: Border.all(color: isActive ? AppTheme.ink : AppTheme.line),
                    ),
                    child: Icon(
                      tool['icon'],
                      size: 18,
                      color: isActive ? Colors.white : AppTheme.ink2,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    tool['name'],
                    style: TextStyle(
                      fontSize: 10,
                      color: isActive ? AppTheme.ink : AppTheme.ink3,
                      fontWeight: isActive ? FontWeight.w600 : FontWeight.w400,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildSliders() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      child: Column(
        children: [
          _sliderRow('亮度', _brightness),
          const SizedBox(height: 10),
          _sliderRow('对比度', _contrast),
          const SizedBox(height: 10),
          _sliderRow('饱和度', _saturation),
        ],
      ),
    );
  }

  Widget _sliderRow(String label, double value) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(label, style: const TextStyle(fontSize: 13, color: AppTheme.ink2, fontWeight: FontWeight.w500)),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
              decoration: BoxDecoration(
                color: AppTheme.bg2,
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                '${value > 0 ? "+" : ""}${value.toInt()}',
                style: const TextStyle(fontSize: 13, color: AppTheme.ink, fontWeight: FontWeight.w600),
              ),
            ),
          ],
        ),
        SliderTheme(
          data: SliderThemeData(
            trackHeight: 6,
            thumbShape: const RoundSliderThumbShape(enabledThumbRadius: 12),
            activeTrackColor: AppTheme.primary,
            inactiveTrackColor: AppTheme.bg2,
            thumbColor: Colors.white,
            overlayColor: AppTheme.primary.withOpacity(0.2),
          ),
          child: Slider(
            value: value,
            min: -100,
            max: 100,
            onChanged: (v) => setState(() {
              if (label == '亮度') _brightness = v;
              if (label == '对比度') _contrast = v;
              if (label == '饱和度') _saturation = v;
            }),
          ),
        ),
      ],
    );
  }

  Widget _buildBottomActions(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(18),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          OutlinedButton.icon(
            onPressed: () {},
            icon: const Icon(Icons.undo_rounded, size: 14),
            label: const Text('撤销'),
            style: OutlinedButton.styleFrom(
              minimumSize: Size.zero,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
              textStyle: const TextStyle(fontSize: 13),
            ),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pushNamed(context, AppRoutes.paramsConfig),
            style: ElevatedButton.styleFrom(
              minimumSize: Size.zero,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
              textStyle: const TextStyle(fontSize: 13),
            ),
            child: const Text('下一步'),
          ),
        ],
      ),
    );
  }
}