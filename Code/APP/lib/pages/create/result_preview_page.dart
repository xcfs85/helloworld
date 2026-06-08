import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class ResultPreviewPage extends StatefulWidget {
  const ResultPreviewPage({super.key});

  @override
  State<ResultPreviewPage> createState() => _ResultPreviewPageState();
}

class _ResultPreviewPageState extends State<ResultPreviewPage> {
  bool _showGrid = true;
  bool _showColorLabels = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF1A1A1A),
      appBar: AppBar(
        backgroundColor: Colors.black.withOpacity(0.4),
        foregroundColor: Colors.white,
        title: const Text('小咪 · 5,000 颗'),
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28, color: Colors.white),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.more_horiz, color: Colors.white),
            onPressed: () {},
          ),
        ],
      ),
      body: Column(
        children: [
          Expanded(
            child: _buildPreview(),
          ),
          _buildToolbar(),
          _buildBottomBar(),
        ],
      ),
    );
  }

  Widget _buildPreview() {
    final colors = [
      AppTheme.primary, AppTheme.accent, AppTheme.mint,
      AppTheme.sky, AppTheme.violet, AppTheme.brown, AppTheme.rose,
    ];
    return Stack(
      children: [
        Positioned.fill(
          child: CustomPaint(
            painter: _BeadGridPainter(colors: colors, showGrid: _showGrid),
          ),
        ),
        Positioned(
          left: 14,
          top: 14,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.5),
              borderRadius: BorderRadius.circular(999),
            ),
            child: const Text('50 × 50 · 进阶', style: TextStyle(color: Colors.white, fontSize: 11)),
          ),
        ),
        Positioned(
          right: 14,
          top: 14,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.5),
              borderRadius: BorderRadius.circular(999),
            ),
            child: const Text('28 色', style: TextStyle(color: Colors.white, fontSize: 11)),
          ),
        ),
      ],
    );
  }

  Widget _buildToolbar() {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 14),
      color: Colors.black.withOpacity(0.4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _toolItem(Icons.grid_view_rounded, '格线', _showGrid, () => setState(() => _showGrid = !_showGrid)),
          _toolItem(Icons.palette_rounded, '色号', _showColorLabels, () => setState(() => _showColorLabels = !_showColorLabels)),
          _toolItem(Icons.compare_rounded, '对比', false, () {}),
          _toolItem(Icons.zoom_in_rounded, '缩放', false, () {}),
        ],
      ),
    );
  }

  Widget _toolItem(IconData icon, String label, bool active, VoidCallback onTap) {
    return GestureDetector(
      onTap: onTap,
      child: Column(
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              color: active ? AppTheme.primary : Colors.white.withOpacity(0.1),
            ),
            child: Icon(icon, color: Colors.white, size: 20),
          ),
          const SizedBox(height: 4),
          Text(label, style: TextStyle(fontSize: 10, color: active ? Colors.white : Colors.white70)),
        ],
      ),
    );
  }

  Widget _buildBottomBar() {
    return Container(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 30),
      color: Colors.black.withOpacity(0.6),
      child: SafeArea(
        top: false,
        child: Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () {},
                icon: const Icon(Icons.refresh_rounded, size: 16),
                label: const Text('重新生成'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: Colors.white,
                  side: BorderSide(color: Colors.white.withOpacity(0.2)),
                  backgroundColor: Colors.white.withOpacity(0.1),
                ),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: ElevatedButton(
                onPressed: () => Navigator.pushNamed(context, AppRoutes.colorsTable),
                child: const Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text('查看色号表'),
                    SizedBox(width: 6),
                    Icon(Icons.chevron_right, size: 16),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _BeadGridPainter extends CustomPainter {
  final List<Color> colors;
  final bool showGrid;

  _BeadGridPainter({required this.colors, required this.showGrid});

  @override
  void paint(Canvas canvas, Size size) {
    final cellSize = size.width / 24;
    for (int y = 0; y < 24; y++) {
      for (int x = 0; x < 24; x++) {
        final color = colors[(x + y * 7) % colors.length];
        final rect = Rect.fromLTWH(x * cellSize, y * cellSize, cellSize, cellSize);
        canvas.drawRect(rect, Paint()..color = color);
        if (showGrid) {
          canvas.drawRect(rect, Paint()..color = Colors.black.withOpacity(0.4)..style = PaintingStyle.stroke..strokeWidth = 0.5);
        }
      }
    }
  }

  @override
  bool shouldRepaint(covariant _BeadGridPainter oldDelegate) => oldDelegate.showGrid != showGrid;
}