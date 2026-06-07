// 创建图纸
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';
import '../../providers/diagram_provider.dart';

class DiagramCreateView extends StatefulWidget {
  const DiagramCreateView({super.key});
  @override
  State<DiagramCreateView> createState() => _DiagramCreateViewState();
}

class _DiagramCreateViewState extends State<DiagramCreateView> {
  File? _image;
  String _boardSize = '29x29';
  String _difficulty = 'easy';
  String _style = 'pixel';
  bool _isSync = false;
  bool _generating = false;
  int _progress = 0;

  final _picker = ImagePicker();

  Future<void> _pickImage() async {
    final picked = await _picker.pickImage(source: ImageSource.gallery);
    if (picked != null) {
      setState(() => _image = File(picked.path));
    }
  }

  Future<void> _startGenerate() async {
    if (_image == null) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('请先选择图片')));
      return;
    }
    setState(() {
      _generating = true;
      _progress = 0;
    });

    // 模拟上传和生成
    for (var i = 0; i <= 100; i += 10) {
      await Future.delayed(const Duration(milliseconds: 200));
      if (!mounted) return;
      setState(() => _progress = i);
    }

    setState(() => _generating = false);
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('生成完成')));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('AI生图')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 图片选择
            InkWell(
              onTap: _pickImage,
              child: Container(
                height: 200,
                decoration: BoxDecoration(
                  color: Colors.grey[100],
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey[300]!),
                ),
                child: _image != null
                    ? Image.file(_image!, fit: BoxFit.contain)
                    : const Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.add_photo_alternate, size: 48, color: Colors.grey),
                            SizedBox(height: 8),
                            Text('点击选择图片'),
                          ],
                        ),
                      ),
              ),
            ),
            const SizedBox(height: 20),

            // 参数设置
            const Text('底板规格', style: TextStyle(fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              children: ['29x29', '58x58', '87x87', '116x116'].map((s) =>
                ChoiceChip(
                  label: Text(s),
                  selected: _boardSize == s,
                  onSelected: (_) => setState(() => _boardSize = s),
                )
              ).toList(),
            ),
            const SizedBox(height: 16),

            const Text('难度', style: TextStyle(fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              children: {
                'easy': '简单', 'medium': '中等', 'hard': '困难', 'expert': '专家'
              }.entries.map((e) =>
                ChoiceChip(
                  label: Text(e.value),
                  selected: _difficulty == e.key,
                  onSelected: (_) => setState(() => _difficulty = e.key),
                )
              ).toList(),
            ),
            const SizedBox(height: 16),

            const Text('风格', style: TextStyle(fontWeight: FontWeight.bold)),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              children: {
                'pixel': '像素', 'cartoon': '卡通', 'realistic': '写实', 'chibi': 'Q版'
              }.entries.map((e) =>
                ChoiceChip(
                  label: Text(e.value),
                  selected: _style == e.key,
                  onSelected: (_) => setState(() => _style = e.key),
                )
              ).toList(),
            ),
            const SizedBox(height: 16),

            // 同步生成开关
            SwitchListTile(
              title: const Text('同步生成'),
              subtitle: const Text('只支持5000颗以内'),
              value: _isSync,
              onChanged: (v) => setState(() => _isSync = v),
            ),

            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _generating ? null : _startGenerate,
              style: ElevatedButton.styleFrom(
                minimumSize: const Size(double.infinity, 50),
                backgroundColor: const Color(0xFFFF6B6B),
              ),
              child: const Text('开始生成', style: TextStyle(fontSize: 16, color: Colors.white)),
            ),

            if (_generating) ...[
              const SizedBox(height: 24),
              const Text('生成中...', style: TextStyle(fontWeight: FontWeight.bold)),
              const SizedBox(height: 8),
              LinearProgressIndicator(
                value: _progress / 100,
                backgroundColor: Colors.grey[200],
              ),
              const SizedBox(height: 8),
              Center(child: Text('$_progress%', style: const TextStyle(color: Colors.grey))),
            ],
          ],
        ),
      ),
    );
  }
}
