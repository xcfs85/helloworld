// 我的图纸列表
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/diagram_provider.dart';
import '../../router/app_router.dart';

class DiagramListView extends StatefulWidget {
  const DiagramListView({super.key});
  @override
  State<DiagramListView> createState() => _DiagramListViewState();
}

class _DiagramListViewState extends State<DiagramListView> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() => context.read<DiagramProvider>().loadDiagrams());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('我的图纸')),
      body: Consumer<DiagramProvider>(
        builder: (context, provider, _) {
          if (provider.diagrams.isEmpty) {
            return const Center(child: Text('暂无图纸'));
          }
          return GridView.builder(
            padding: const EdgeInsets.all(12),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 2,
              crossAxisSpacing: 12,
              mainAxisSpacing: 12,
              childAspectRatio: 0.85,
            ),
            itemCount: provider.diagrams.length,
            itemBuilder: (_, i) {
              final d = provider.diagrams[i];
              return InkWell(
                onTap: () => Navigator.pushNamed(context, AppRouter.diagramDetail, arguments: d['id']),
                child: Container(
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
                              : const Center(child: Icon(Icons.image, size: 40)),
                        ),
                      ),
                      Padding(
                        padding: const EdgeInsets.all(8),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(d['name'] ?? '', style: const TextStyle(fontWeight: FontWeight.bold)),
                            Text('${d['boardSize']} · ${d['beadCount']}颗', style: const TextStyle(fontSize: 12, color: Colors.grey)),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
