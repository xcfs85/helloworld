// 图纸详情
import 'package:flutter/material.dart';

class DiagramDetailView extends StatelessWidget {
  final String diagramId;
  const DiagramDetailView({super.key, required this.diagramId});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('图纸详情')),
      body: SingleChildScrollView(
        child: Column(
          children: [
            AspectRatio(
              aspectRatio: 1,
              child: Container(
                color: Colors.grey[200],
                child: const Center(child: Text('图纸预览')),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('规格: 29x29', style: TextStyle(fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  const Text('颗数: 841'),
                  const Text('难度: 简单'),
                  const Text('风格: 像素'),
                  const Text('色号: 12色'),
                  const SizedBox(height: 24),
                  Row(
                    children: [
                      Expanded(child: ElevatedButton.icon(onPressed: () {}, icon: const Icon(Icons.file_download), label: const Text('导出'))),
                      const SizedBox(width: 12),
                      Expanded(child: OutlinedButton.icon(onPressed: () {}, icon: const Icon(Icons.share), label: const Text('分享'))),
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
