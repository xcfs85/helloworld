import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class PostDetailPage extends StatelessWidget {
  const PostDetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Column(
            children: [
              Stack(
                children: [
                  Container(
                    height: 280,
                    decoration: const BoxDecoration(
                      gradient: LinearGradient(
                        colors: [Color(0xFFFFD2B0), Color(0xFFF5C45E), Color(0xFFFF7A5A)],
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                      ),
                    ),
                  ),
                  Positioned(
                    left: 0, right: 0, bottom: 0,
                    child: Container(
                      height: 100,
                      decoration: const BoxDecoration(
                        gradient: LinearGradient(
                          begin: Alignment.topCenter,
                          end: Alignment.bottomCenter,
                          colors: [Colors.transparent, Colors.white],
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 18, bottom: 36,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('🎁 给闺蜜的礼物', style: TextStyle(color: Colors.white, fontSize: 11)),
                        const SizedBox(height: 4),
                        const Text('50×50 板\n28 色 / 拼了 8 小时', style: TextStyle(color: Colors.white, fontSize: 22, fontWeight: FontWeight.w700, height: 1.3)),
                      ],
                    ),
                  ),
                ],
              ),
              Expanded(
                child: SingleChildScrollView(
                  padding: const EdgeInsets.fromLTRB(18, 0, 18, 100),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 40),
                      Row(
                        children: [
                          const CircleAvatar(
                            radius: 21,
                            backgroundColor: Color(0xFFFF7A5A),
                            child: Icon(Icons.person, color: Colors.white),
                          ),
                          const SizedBox(width: 10),
                          const Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text('小美 ✨', style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                                Text('拼豆玩家 · 作品 23 · 粉丝 89', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
                              ],
                            ),
                          ),
                          ElevatedButton(
                            onPressed: () {},
                            style: ElevatedButton.styleFrom(
                              minimumSize: Size.zero,
                              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                              textStyle: const TextStyle(fontSize: 13),
                            ),
                            child: const Text('+ 关注'),
                          ),
                        ],
                      ),
                      const SizedBox(height: 14),
                      const Text(
                        '闺蜜下个月生日🎂 她超喜欢我们家小咪，于是决定把小咪拼成拼豆送给她！虽然第 5 行拼错了一次又拆了重来，但最后还是完成啦～ 看到成品的那一刻眼泪都流出来了 😭',
                        style: TextStyle(fontSize: 14, color: AppTheme.ink, height: 1.7),
                      ),
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          Expanded(child: _imageBox(const Color(0xFF9DC8E5), const Color(0xFF6BC7A1))),
                          const SizedBox(width: 6),
                          Expanded(child: _imageBox(const Color(0xFFFFD2B0), const Color(0xFFF5C45E))),
                          const SizedBox(width: 6),
                          Expanded(child: _imageBox(const Color(0xFFB49DD8), const Color(0xFFF2A6A6))),
                        ],
                      ),
                      const SizedBox(height: 14),
                      Wrap(
                        spacing: 6,
                        runSpacing: 6,
                        children: ['#生日礼物', '#闺蜜', '#猫咪', '#拼豆日常'].map((t) => Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppTheme.bg2,
                            borderRadius: BorderRadius.circular(999),
                          ),
                          child: Text(t, style: const TextStyle(fontSize: 10, color: AppTheme.ink2)),
                        )).toList(),
                      ),
                      const SizedBox(height: 18),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text('热门评论 89', style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600)),
                          const Text('按热度 ▾', style: TextStyle(fontSize: 11, color: AppTheme.ink3)),
                        ],
                      ),
                      const SizedBox(height: 12),
                      _commentItem(
                        const Color(0xFF9DC8E5), const Color(0xFF6BC7A1),
                        'Lily 老师', '认证创作者',
                        '太厉害啦！第 5 行能看出是米色拼错了色，色号是 H08 哦～',
                        '2 小时前', '23',
                      ),
                      _commentItem(
                        const Color(0xFFB49DD8), const Color(0xFFF2A6A6),
                        '求图达人', '',
                        '求这个图纸！可以分享吗？',
                        '1 小时前', '5',
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
          Positioned(
            top: 0, left: 0, right: 0,
            child: SafeArea(
              child: Row(
                children: [
                  IconButton(
                    icon: const Icon(Icons.chevron_left, color: Colors.white, size: 28),
                    onPressed: () => Navigator.pop(context),
                    style: IconButton.styleFrom(
                      backgroundColor: Colors.black.withOpacity(0.3),
                    ),
                  ),
                  const Spacer(),
                  IconButton(
                    icon: const Icon(Icons.more_horiz, color: Colors.white, size: 22),
                    onPressed: () {},
                    style: IconButton.styleFrom(
                      backgroundColor: Colors.black.withOpacity(0.3),
                    ),
                  ),
                ],
              ),
            ),
          ),
          Positioned(
            left: 0, right: 0, bottom: 0,
            child: Container(
              padding: const EdgeInsets.fromLTRB(14, 10, 14, 24),
              decoration: BoxDecoration(
                color: Colors.white,
                border: Border(top: BorderSide(color: AppTheme.line)),
              ),
              child: SafeArea(
                top: false,
                child: Row(
                  children: [
                    Expanded(
                      child: Container(
                        height: 42,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        decoration: BoxDecoration(
                          color: AppTheme.bg2,
                          borderRadius: BorderRadius.circular(21),
                        ),
                        child: const Align(
                          alignment: Alignment.centerLeft,
                          child: Text('说点什么…', style: TextStyle(color: AppTheme.ink3, fontSize: 13)),
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    ...['❤', '💬', '⭐'].map((e) => Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 6),
                      child: Text(e, style: const TextStyle(fontSize: 20)),
                    )),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _imageBox(Color c1, Color c2) {
    return AspectRatio(
      aspectRatio: 1,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          gradient: LinearGradient(colors: [c1, c2]),
        ),
      ),
    );
  }

  Widget _commentItem(Color c1, Color c2, String name, String badge, String content, String time, String likes) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 14),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          CircleAvatar(
            radius: 16,
            backgroundColor: c1,
          ),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(name, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
                    if (badge.isNotEmpty) ...[
                      const SizedBox(width: 6),
                      Text(badge, style: const TextStyle(fontSize: 10, color: AppTheme.ink3)),
                    ],
                  ],
                ),
                const SizedBox(height: 2),
                Text(content, style: const TextStyle(fontSize: 13, color: AppTheme.ink2, height: 1.6)),
                const SizedBox(height: 4),
                Text('$time · ♡ $likes', style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}