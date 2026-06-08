import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class PostCreatePage extends StatefulWidget {
  const PostCreatePage({super.key});

  @override
  State<PostCreatePage> createState() => _PostCreatePageState();
}

class _PostCreatePageState extends State<PostCreatePage> {
  String _activeType = '作品';
  final _titleController = TextEditingController(text: '给闺蜜的生日礼物🎁');
  final _contentController = TextEditingController(text: '50×50 板 / 28 色 / 拼了 8 小时终于完成！闺蜜看到哭了一小时 😭');

  @override
  void dispose() {
    _titleController.dispose();
    _contentController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('发布'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.close, size: 22),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () {},
            child: const Text('草稿', style: TextStyle(color: AppTheme.ink3, fontSize: 13)),
          ),
          ElevatedButton(
            onPressed: () {},
            style: ElevatedButton.styleFrom(
              minimumSize: Size.zero,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              textStyle: const TextStyle(fontSize: 13),
            ),
            child: const Text('发布'),
          ),
          const SizedBox(width: 8),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 12),
              color: AppTheme.surface,
              child: Row(
                children: ['作品', '求图', '教程', '讨论'].map((t) {
                  final active = _activeType == t;
                  return GestureDetector(
                    onTap: () => setState(() => _activeType = t),
                    child: Container(
                      margin: const EdgeInsets.only(right: 8),
                      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(10),
                        color: active ? AppTheme.ink : AppTheme.bg2,
                      ),
                      child: Text(
                        t,
                        style: TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.w500,
                          color: active ? Colors.white : AppTheme.ink2,
                        ),
                      ),
                    ),
                  );
                }).toList(),
              ),
            ),
            Container(
              color: AppTheme.surface,
              padding: const EdgeInsets.all(18),
              child: Column(
                children: [
                  Row(
                    children: [
                      ...List.generate(5, (i) {
                        return Expanded(
                          child: Padding(
                            padding: const EdgeInsets.only(right: 6),
                            child: AspectRatio(
                              aspectRatio: 1,
                              child: Container(
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(10),
                                  gradient: LinearGradient(
                                    colors: [
                                      [const Color(0xFFFFD2B0), const Color(0xFFFF7A5A)],
                                      [const Color(0xFF9DC8E5), const Color(0xFF6BC7A1)],
                                      [const Color(0xFFB49DD8), const Color(0xFFF2A6A6)],
                                      [const Color(0xFFF5C45E), const Color(0xFFFF8A5A)],
                                      [const Color(0xFF6BC7A1), const Color(0xFF9DC8E5)],
                                    ][i],
                                  ),
                                ),
                              ),
                            ),
                          ),
                        );
                      }),
                      Expanded(
                        child: AspectRatio(
                          aspectRatio: 1,
                          child: Container(
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(10),
                              color: AppTheme.bg2,
                              border: Border.all(color: AppTheme.line, style: BorderStyle.solid),
                            ),
                            child: const Center(
                              child: Icon(Icons.add, size: 28, color: AppTheme.ink3),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 14),
                  TextField(
                    controller: _titleController,
                    style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                    decoration: const InputDecoration(
                      hintText: '给作品起个标题…',
                      hintStyle: TextStyle(fontSize: 16, fontWeight: FontWeight.w400, color: AppTheme.ink3),
                      border: InputBorder.none,
                      fillColor: AppTheme.bg2,
                      filled: true,
                      contentPadding: EdgeInsets.all(12),
                    ),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _contentController,
                    maxLines: 4,
                    decoration: const InputDecoration(
                      hintText: '说点什么… 支持 @用户 和 #话题',
                      hintStyle: TextStyle(fontSize: 14, color: AppTheme.ink3),
                      border: InputBorder.none,
                      fillColor: AppTheme.bg2,
                      filled: true,
                      contentPadding: EdgeInsets.all(12),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),
            Container(
              margin: const EdgeInsets.symmetric(horizontal: 16),
              decoration: BoxDecoration(
                color: AppTheme.surface,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppTheme.line),
              ),
              child: Column(
                children: [
                  _paramRow('📐 拼豆参数', '50×50 板 / 28 色 / 8h'),
                  _paramRow('🔗 关联图纸', '小咪 50×50'),
                  _paramRow('#️⃣ 话题', '#生日礼物 #闺蜜'),
                  _paramRow('📍 位置', '不显示'),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _paramRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      child: Row(
        children: [
          Text(label, style: const TextStyle(fontSize: 13, color: AppTheme.ink)),
          const Spacer(),
          Text(value, style: const TextStyle(fontSize: 13, color: AppTheme.ink3)),
          const SizedBox(width: 4),
          const Icon(Icons.chevron_right, size: 14, color: AppTheme.ink3),
        ],
      ),
    );
  }
}