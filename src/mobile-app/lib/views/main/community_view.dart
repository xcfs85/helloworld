// 社区
import 'package:flutter/material.dart';

class CommunityView extends StatelessWidget {
  const CommunityView({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('社区'),
        centerTitle: true,
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(40),
          child: Row(
            children: ['推荐', '关注', '话题'].map((t) => Padding(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              child: Text(t, style: const TextStyle(fontSize: 14)),
            )).toList(),
          ),
        ),
      ),
      body: ListView.builder(
        itemCount: 10,
        itemBuilder: (_, i) => _postCard(i),
      ),
    );
  }

  Widget _postCard(int i) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        boxShadow: [BoxShadow(color: Colors.grey.withOpacity(0.1), blurRadius: 4)],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const CircleAvatar(radius: 16, child: Icon(Icons.person, size: 16)),
              const SizedBox(width: 8),
              const Text('用户名', style: TextStyle(fontWeight: FontWeight.bold)),
              const Spacer(),
              IconButton(icon: const Icon(Icons.more_horiz), onPressed: () {}),
            ],
          ),
          const SizedBox(height: 8),
          const Text('拼豆作品分享 - 第${i + 1}个作品'),
          const SizedBox(height: 8),
          AspectRatio(
            aspectRatio: 1,
            child: Container(
              decoration: BoxDecoration(
                color: Colors.grey[200],
                borderRadius: BorderRadius.circular(8),
              ),
            ),
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              IconButton(icon: const Icon(Icons.favorite_border), onPressed: () {}),
              const Text('0'),
              const SizedBox(width: 16),
              IconButton(icon: const Icon(Icons.chat_bubble_outline), onPressed: () {}),
              const Text('0'),
              const Spacer(),
              IconButton(icon: const Icon(Icons.bookmark_border), onPressed: () {}),
            ],
          ),
        ],
      ),
    );
  }
}
