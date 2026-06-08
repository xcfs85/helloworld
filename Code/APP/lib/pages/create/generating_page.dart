import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class GeneratingPage extends StatefulWidget {
  const GeneratingPage({super.key});

  @override
  State<GeneratingPage> createState() => _GeneratingPageState();
}

class _GeneratingPageState extends State<GeneratingPage> {
  int _progress = 62;
  final List<Map<String, dynamic>> _stages = [
    {'name': '正在分析图像', 'done': true},
    {'name': '正在智能裁剪', 'done': true},
    {'name': '正在映射色号', 'done': false, 'active': true, 'percent': 62},
    {'name': '正在生成图纸', 'done': false},
    {'name': '即将完成', 'done': false},
  ];

  @override
  void initState() {
    super.initState();
    Future.delayed(const Duration(seconds: 3), () {
      if (mounted) {
        Navigator.pushReplacementNamed(context, AppRoutes.resultPreview);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: const BoxDecoration(
          gradient: RadialGradient(
            center: Alignment(0, -0.3),
            colors: [Color(0xFFFFF1E2), Color(0xFFFBF7F2)],
          ),
        ),
        child: SafeArea(
          child: Column(
            children: [
              const Spacer(flex: 2),
              _buildArt(),
              const SizedBox(height: 28),
              const Text(
                '正在映射色号',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700),
              ),
              const SizedBox(height: 6),
              const Text(
                '智能识别每一颗拼豆的色号\n约还需要 12 秒',
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 13, color: AppTheme.ink3, height: 1.6),
              ),
              const SizedBox(height: 18),
              _buildProgress(),
              const SizedBox(height: 24),
              _buildStages(),
              const Spacer(),
              Padding(
                padding: const EdgeInsets.all(24),
                child: SizedBox(
                  width: double.infinity,
                  child: OutlinedButton(
                    onPressed: () {},
                    style: OutlinedButton.styleFrom(
                      backgroundColor: Colors.white.withOpacity(0.7),
                      side: BorderSide.none,
                    ),
                    child: const Text('后台运行，完成后通知我'),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildArt() {
    return Container(
      width: 240,
      height: 240,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(32),
        gradient: const LinearGradient(
          colors: [Color(0xFFFF8A5A), Color(0xFFF5C45E)],
        ),
        boxShadow: [
          BoxShadow(
            color: const Color(0xFFFF8A5A).withOpacity(0.3),
            blurRadius: 60,
            offset: const Offset(0, 20),
          ),
        ],
      ),
      child: Stack(
        children: [
          Positioned.fill(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(32),
              child: CustomPaint(
                painter: _GridPainter(),
              ),
            ),
          ),
          const Center(
            child: Icon(Icons.auto_awesome_rounded, color: Colors.white, size: 48),
          ),
          Positioned(
            bottom: 18,
            left: 0,
            right: 0,
            child: Text(
              '$_progress%',
              textAlign: TextAlign.center,
              style: const TextStyle(color: Colors.white, fontSize: 14, fontWeight: FontWeight.w600),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildProgress() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 48),
      child: Column(
        children: [
          ClipRRect(
            borderRadius: BorderRadius.circular(3),
            child: LinearProgressIndicator(
              value: _progress / 100,
              backgroundColor: AppTheme.bg2,
              valueColor: const AlwaysStoppedAnimation<Color>(AppTheme.primary),
              minHeight: 6,
            ),
          ),
          const SizedBox(height: 6),
          const Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('已完成 3 / 5', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
              Text('剩余 12s', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildStages() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 48),
      child: Column(
        children: _stages.map((s) {
          final done = s['done'] == true;
          final active = s['active'] == true;
          return Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Row(
              children: [
                Container(
                  width: 16,
                  height: 16,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: done
                        ? AppTheme.mint
                        : active
                            ? AppTheme.primary
                            : AppTheme.bg2,
                  ),
                  child: done
                      ? const Icon(Icons.check, size: 10, color: Colors.white)
                      : null,
                ),
                const SizedBox(width: 10),
                Text(
                  s['name'],
                  style: TextStyle(
                    fontSize: 12,
                    color: done ? AppTheme.ink : active ? AppTheme.primaryInk : AppTheme.ink3,
                    fontWeight: active ? FontWeight.w600 : FontWeight.w400,
                  ),
                ),
                if (active) ...[
                  const SizedBox(width: 4),
                  Text(
                    '(${s['percent']}%)',
                    style: const TextStyle(fontSize: 12, color: AppTheme.primaryInk, fontWeight: FontWeight.w600),
                  ),
                ],
              ],
            ),
          );
        }).toList(),
      ),
    );
  }
}

class _GridPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white.withOpacity(0.15)
      ..strokeWidth = 1;
    for (double y = 0; y < size.height; y += 16) {
      canvas.drawLine(Offset(0, y), Offset(size.width, y), paint);
    }
    for (double x = 0; x < size.width; x += 16) {
      canvas.drawLine(Offset(x, 0), Offset(x, size.height), paint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}