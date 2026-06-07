import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class CreatorSubmitPage extends StatefulWidget {
  const CreatorSubmitPage({super.key});

  @override
  State<CreatorSubmitPage> createState() => _CreatorSubmitPageState();
}

class _CreatorSubmitPageState extends State<CreatorSubmitPage> {
  final _nameController = TextEditingController(text: '圣诞老人');
  final _descController = TextEditingController(text: '给孩子的圣诞礼物🎄 30 分钟能拼完简单的部分，剩 1/3 留给他自己完成 ✨');
  String _category = '节日';
  final List<String> _tags = ['亲子', '入门', '圣诞'];

  @override
  void dispose() {
    _nameController.dispose();
    _descController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('投稿模板'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
        actions: [
          TextButton(
            onPressed: () {},
            child: const Text('提交', style: TextStyle(color: AppTheme.primaryInk, fontWeight: FontWeight.w600)),
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('上传封面', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            AspectRatio(
              aspectRatio: 1,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  gradient: const LinearGradient(
                    colors: [Color(0xFFFFB088), Color(0xFFFF7A5A)],
                  ),
                ),
                child: const Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.add_photo_alternate_rounded, color: Colors.white, size: 40),
                      SizedBox(height: 8),
                      Text('点击上传封面图', style: TextStyle(color: Colors.white70, fontSize: 12)),
                    ],
                  ),
                ),
              ),
            ),
            const SizedBox(height: 18),
            const Text('上传图纸', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(14),
                border: Border.all(color: AppTheme.line, style: BorderStyle.solid),
                color: AppTheme.surface,
              ),
              child: const Column(
                children: [
                  Icon(Icons.upload_file_rounded, size: 32, color: AppTheme.ink3),
                  SizedBox(height: 8),
                  Text('上传图纸文件', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                  Text('支持 PDF / PNG · 最大 50MB', style: TextStyle(fontSize: 10, color: AppTheme.ink3)),
                ],
              ),
            ),
            const SizedBox(height: 18),
            const Text('名称', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            TextField(
              controller: _nameController,
              decoration: InputDecoration(
                hintText: '给模板起个名字',
                hintStyle: const TextStyle(fontSize: 14, color: AppTheme.ink3),
                filled: true,
                fillColor: AppTheme.surface,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: AppTheme.line),
                ),
              ),
            ),
            const SizedBox(height: 18),
            const Text('分类', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: ['节日', '卡通', '二次元', '宠物', '风景', '像素游戏', '国风', '表情包', '文字', '其他'].map((c) {
                final active = _category == c;
                return GestureDetector(
                  onTap: () => setState(() => _category = c),
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(10),
                      color: active ? AppTheme.ink : AppTheme.surface,
                      border: Border.all(color: active ? AppTheme.ink : AppTheme.line),
                    ),
                    child: Text(
                      c,
                      style: TextStyle(
                        fontSize: 13,
                        color: active ? Colors.white : AppTheme.ink2,
                      ),
                    ),
                  ),
                );
              }).toList(),
            ),
            const SizedBox(height: 18),
            const Text('标签', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            Row(
              children: [
                ..._tags.map((t) => Container(
                  margin: const EdgeInsets.only(right: 6),
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(999),
                    color: AppTheme.ink,
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(t, style: const TextStyle(color: Colors.white, fontSize: 12)),
                      const SizedBox(width: 4),
                      const Icon(Icons.close, size: 12, color: Colors.white70),
                    ],
                  ),
                )),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(999),
                    border: Border.all(color: AppTheme.line),
                  ),
                  child: const Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.add, size: 12, color: AppTheme.ink3),
                      SizedBox(width: 4),
                      Text('添加', style: TextStyle(fontSize: 12, color: AppTheme.ink3)),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 18),
            const Text('说明', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            TextField(
              controller: _descController,
              maxLines: 4,
              decoration: InputDecoration(
                hintText: '介绍一下这个模板…',
                hintStyle: const TextStyle(fontSize: 14, color: AppTheme.ink3),
                filled: true,
                fillColor: AppTheme.surface,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide(color: AppTheme.line),
                ),
              ),
            ),
            const SizedBox(height: 32),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () {},
                child: const Text('提交审核'),
              ),
            ),
            const SizedBox(height: 10),
            const Center(
              child: Text(
                '提交后 1-3 个工作日内审核',
                style: TextStyle(fontSize: 11, color: AppTheme.ink3),
              ),
            ),
          ],
        ),
      ),
    );
  }
}