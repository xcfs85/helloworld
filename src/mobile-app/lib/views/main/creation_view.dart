// 创作中心
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/diagram_provider.dart';
import '../../router/app_router.dart';

class CreationView extends StatelessWidget {
  const CreationView({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('创作中心'), centerTitle: true),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // 创作入口
          Row(
            children: [
              Expanded(
                child: _actionCard(
                  context, 'AI生图', '上传图片,AI智能转换',
                  Icons.image, Colors.pink,
                  () => Navigator.pushNamed(context, AppRouter.diagramCreate),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _actionCard(
                  context, '我的图纸', '查看历史创作',
                  Icons.history, Colors.blue,
                  () => Navigator.pushNamed(context, AppRouter.diagramList),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          // 创作引导
          _guideCard(),
          const SizedBox(height: 24),
          // 我的创作
          const Text('我的创作', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
          const SizedBox(height: 12),
          Consumer<DiagramProvider>(
            builder: (context, provider, _) {
              if (provider.diagrams.isEmpty) {
                return Container(
                  height: 200,
                  decoration: BoxDecoration(
                    color: Colors.grey[100],
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Center(child: Text('暂无作品,开始创作吧')),
                );
              }
              return GridView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 2,
                  crossAxisSpacing: 12,
                  mainAxisSpacing: 12,
                  childAspectRatio: 0.85,
                ),
                itemCount: provider.diagrams.length,
                itemBuilder: (_, i) {
                  final d = provider.diagrams[i];
                  return _diagramCard(d);
                },
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _actionCard(BuildContext context, String title, String subtitle, IconData icon, Color color, VoidCallback onTap) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: color.withOpacity(0.1),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color, size: 32),
            const SizedBox(height: 8),
            Text(title, style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
            const SizedBox(height: 4),
            Text(subtitle, style: const TextStyle(fontSize: 12, color: Colors.grey)),
          ],
        ),
      ),
    );
  }

  Widget _guideCard() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.amber[50],
        borderRadius: BorderRadius.circular(12),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.tips_and_updates, color: Colors.amber),
              SizedBox(width: 8),
              Text('创作小贴士', style: TextStyle(fontWeight: FontWeight.bold)),
            ],
          ),
          SizedBox(height: 8),
          Text('1. 选择清晰的图片,效果更佳', style: TextStyle(fontSize: 13)),
          Text('2. 简单图案使用简单难度,复杂图案使用困难', style: TextStyle(fontSize: 13)),
          Text('3. 同步生成只支持5000颗以内', style: TextStyle(fontSize: 13)),
        ],
      ),
    );
  }

  Widget _diagramCard(Map<String, dynamic> d) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        boxShadow: [BoxShadow(color: Colors.grey.withOpacity(0.1), blurRadius: 4)],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          AspectRatio(
            aspectRatio: 1,
            child: Container(
              decoration: BoxDecoration(
                color: Colors.grey[200],
                borderRadius: const BorderRadius.vertical(top: Radius.circular(8)),
              ),
              child: d['previewUrl'] != null
                  ? Image.network(d['previewUrl'], fit: BoxFit.cover)
                  : const Center(child: Icon(Icons.image)),
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(d['name'] ?? '图纸', style: const TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 4),
                Text('${d['boardSize']} · ${d['beadCount']}颗', style: const TextStyle(fontSize: 12, color: Colors.grey)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
