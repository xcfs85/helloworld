import 'package:flutter/material.dart';
import '../config/app_theme.dart';
import '../models/template.dart';

class TemplateCard extends StatelessWidget {
  final Template template;
  final VoidCallback? onTap;

  const TemplateCard({
    super.key,
    required this.template,
    this.onTap,
  });

  Color get _picColor {
    final colors = [
      [AppTheme.primary, AppTheme.accent],
      [AppTheme.sky, AppTheme.mint],
      [AppTheme.rose, AppTheme.violet],
      [AppTheme.accent, AppTheme.primary],
      [AppTheme.mint, AppTheme.sky],
      [AppTheme.violet, AppTheme.rose],
      [AppTheme.primary, AppTheme.brown],
    ];
    final hash = template.name.hashCode.abs();
    return Color.lerp(
      Color(colors[hash % colors.length][0].value),
      Color(colors[hash % colors.length][1].value),
      (hash % 10) / 10,
    )!;
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(
          color: AppTheme.surface,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: AppTheme.line),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            AspectRatio(
              aspectRatio: 1,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: const BorderRadius.vertical(top: Radius.circular(15)),
                  gradient: LinearGradient(
                    colors: [AppTheme.primary, AppTheme.accent],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                ),
                child: Stack(
                  children: [
                    Positioned(
                      left: 8,
                      top: 8,
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                        decoration: BoxDecoration(
                          color: Colors.black.withOpacity(0.5),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Text(
                          template.category,
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 10,
                          ),
                        ),
                      ),
                    ),
                    Positioned(
                      right: 8,
                      top: 8,
                      child: Icon(
                        Icons.favorite_border,
                        color: Colors.white.withOpacity(0.8),
                        size: 16,
                      ),
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
                  Text(
                    template.name,
                    style: const TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        '${template.beadParams.colorCount} 色 / ${template.beadParams.boardSize}',
                        style: const TextStyle(
                          fontSize: 10,
                          color: AppTheme.ink3,
                        ),
                      ),
                      Row(
                        children: [
                          const Icon(Icons.favorite, size: 10, color: AppTheme.rose),
                          const SizedBox(width: 2),
                          Text(
                            '${template.likeCount}',
                            style: const TextStyle(
                              fontSize: 10,
                              color: AppTheme.ink3,
                            ),
                          ),
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
  }
}