import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class VipPage extends StatelessWidget {
  const VipPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('会员中心'),
        backgroundColor: Colors.transparent,
        elevation: 0,
        foregroundColor: Colors.white,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28, color: Colors.white),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Column(
        children: [
          Container(
            width: double.infinity,
            padding: const EdgeInsets.fromLTRB(18, 0, 18, 24),
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                colors: [Color(0xFF1A1A2E), Color(0xFF16213E), Color(0xFF2A1F1A)],
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 20),
                Row(
                  children: [
                    const Icon(Icons.workspace_premium_rounded, color: Color(0xFFF5C45E), size: 28),
                    const SizedBox(width: 8),
                    const Text('拼豆会员', style: TextStyle(fontSize: 22, fontWeight: FontWeight.w700, color: Colors.white)),
                  ],
                ),
                const SizedBox(height: 18),
                Container(
                  padding: const EdgeInsets.all(18),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(18),
                    gradient: const LinearGradient(
                      colors: [Color(0xFFF5C45E), Color(0xFFFF8A5A)],
                    ),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('🎉 限时特惠', style: TextStyle(fontSize: 13, color: Colors.white70)),
                      const SizedBox(height: 4),
                      const Text('¥ 9.9 / 月', style: TextStyle(fontSize: 32, fontWeight: FontWeight.w700, color: Colors.white)),
                      const SizedBox(height: 4),
                      const Text('原价 ¥29.9 · 省 ¥20', style: TextStyle(fontSize: 12, color: Colors.white70)),
                      const SizedBox(height: 12),
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: () {},
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.white,
                            foregroundColor: const Color(0xFFFF7A5A),
                          ),
                          child: const Text('立即开通'),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(18),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('会员权益', style: TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
                const SizedBox(height: 14),
                _benefit('🎨', '无限生成', '不限次数 AI 生成拼豆图纸'),
                _benefit('📐', '高清导出', '最高 4K 分辨率导出'),
                _benefit('🎭', '全部风格', '解锁全部 AI 风格'),
                _benefit('📊', '色号分析', '高级色号统计与分析'),
                _benefit('☁', '云端存储', '无限云端存储空间'),
                _benefit('⭐', '专属标识', '会员专属头像框和标识'),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _benefit(String emoji, String title, String desc) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        children: [
          Container(
            width: 36,
            height: 36,
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(10),
              color: AppTheme.bg2,
            ),
            child: Center(child: Text(emoji, style: const TextStyle(fontSize: 16))),
          ),
          const SizedBox(width: 12),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
              Text(desc, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
            ],
          ),
        ],
      ),
    );
  }
}