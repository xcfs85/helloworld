import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';
import '../../widgets/tab_bar_widget.dart';

class ForumPage extends StatefulWidget {
  const ForumPage({super.key});

  @override
  State<ForumPage> createState() => _ForumPageState();
}

class _ForumPageState extends State<ForumPage> {
  String _activeTab = '推荐';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('社区'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        actions: [
          IconButton(
            icon: const Icon(Icons.search_rounded, size: 22),
            onPressed: () {},
          ),
          IconButton(
            icon: const Icon(Icons.add_rounded, size: 22),
            onPressed: () => Navigator.pushNamed(context, AppRoutes.postCreate),
          ],
        ],
      ),
      body: Column(
        children: [
          Container(
            color: AppTheme.surface,
            child: Row(
              children: ['推荐', '关注', '话题'].map((tab) {
                final active = _activeTab == tab;
                return GestureDetector(
                  onTap: () => setState(() => _activeTab = tab),
                  child: Container(
                    padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 18),
                    decoration: BoxDecoration(
                      border: Border(
                        bottom: BorderSide(
                          color: active ? AppTheme.primary : Colors.transparent,
                          width: 3,
                        ),
                      ),
                    ),
                    child: Text(
                      tab,
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                        color: active ? AppTheme.ink : AppTheme.ink3,
                      ),
                    ),
                  ),
                );
              }).toList(),
            ),
          ),
          Expanded(
            child: ListView(
              children: [
                _buildPostCard(
                  avatarGradient: const [Color(0xFFFFB088), Color(0xFFFF7A5A)],
                  name: '小美',
                  time: '2 小时前',
                  isFollowed: false,
                  title: '给闺蜜的生日礼物🎁',
                  content: '50×50 板 / 28 色 / 拼了 8 小时终于完成！闺蜜看到哭了一小时 😭',
                  type: '作品',
                  typeColor: AppTheme.primary,
                  hasImages: true,
                  likes: '1.2k',
                  comments: '89',
                  favorites: '234',
                  tags: ['#生日礼物', '#闺蜜', '#拼豆日常'],
                ),
                _buildPostCard(
                  avatarGradient: const [Color(0xFF9DC8E5), Color(0xFF6BC7A1)],
                  name: 'Lily 老师',
                  time: '5 小时前 · 认证创作者',
                  isFollowed: true,
                  title: '【教程】0 基础必看！拼豆入门 8 问 8 答',
                  content: '从工具选购到第一颗拼豆，手把手带你避坑。视频教程在第 3 楼 ⬇️',
                  type: '教程',
                  typeColor: AppTheme.mint,
                  hasVideo: true,
                  likes: '567',
                  comments: '23',
                  favorites: '178',
                  tags: ['#教程', '#新手入门'],
                ),
                _buildPostCard(
                  avatarGradient: const [Color(0xFFB49DD8), Color(0xFFF2A6A6)],
                  name: '求图达人',
                  time: '昨天',
                  isFollowed: false,
                  title: '求一张宫崎骏《千与千寻》千寻的拼豆图纸 🙏',
                  content: '想拼给女朋友当周年礼物，最好 50×50 进阶难度，能有大大分享吗？',
                  type: '求图',
                  typeColor: AppTheme.rose,
                  hasRefImage: true,
                  likes: '12',
                  comments: '8',
                  favorites: '0',
                  tags: ['#求图', '#千与千寻'],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPostCard({
    required List<Color> avatarGradient,
    required String name,
    required String time,
    required bool isFollowed,
    required String title,
    required String content,
    required String type,
    required Color typeColor,
    bool hasImages = false,
    bool hasVideo = false,
    bool hasRefImage = false,
    required String likes,
    required String comments,
    required String favorites,
    required List<String> tags,
  }) {
    return GestureDetector(
      onTap: () => Navigator.pushNamed(context, AppRoutes.postDetail),
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
        decoration: BoxDecoration(
          color: AppTheme.surface,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: AppTheme.line),
        ),
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  CircleAvatar(
                    radius: 18,
                    backgroundColor: avatarGradient[0],
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(name, style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                        Text(time, style: const TextStyle(fontSize: 11, color: AppTheme.ink3)),
                      ],
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: typeColor.withOpacity(0.15),
                      borderRadius: BorderRadius.circular(999),
                    ),
                    child: Text(type, style: TextStyle(fontSize: 10, color: typeColor, fontWeight: FontWeight.w600)),
                  ),
                ],
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 14),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600, height: 1.5)),
                  const SizedBox(height: 6),
                  Text(content, style: const TextStyle(fontSize: 12, color: AppTheme.ink2, height: 1.6), maxLines: 2, overflow: TextOverflow.ellipsis),
                ],
              ),
            ),
            if (hasImages || hasVideo || hasRefImage) ...[
              const SizedBox(height: 10),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 12),
                child: hasVideo
                    ? AspectRatio(
                        aspectRatio: 1.6,
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(10),
                            gradient: const LinearGradient(colors: [Color(0xFFFFE2D3), Color(0xFFF5C45E)]),
                          ),
                          child: const Center(
                            child: Icon(Icons.play_circle_fill_rounded, color: Colors.white, size: 48),
                          ),
                        ),
                      )
                    : hasRefImage
                        ? AspectRatio(
                            aspectRatio: 1.4,
                            child: Container(
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(10),
                                gradient: const LinearGradient(colors: [Color(0xFF9DC8E5), Color(0xFFB49DD8)]),
                              ),
                            ),
                          )
                        : Row(
                            children: List.generate(3, (i) {
                              final gradColors = [
                                [const Color(0xFFFFD2B0), const Color(0xFFF5C45E)],
                                [const Color(0xFF9DC8E5), const Color(0xFF6BC7A1)],
                                [const Color(0xFFF2A6A6), const Color(0xFFB49DD8)],
                              ];
                              return Expanded(
                                child: Padding(
                                  padding: EdgeInsets.only(right: i < 2 ? 4 : 0),
                                  child: AspectRatio(
                                    aspectRatio: 1,
                                    child: Container(
                                      decoration: BoxDecoration(
                                        borderRadius: BorderRadius.circular(8),
                                        gradient: LinearGradient(colors: gradColors[i]),
                                      ),
                                    ),
                                  ),
                                ),
                              );
                            }),
                          ),
              ),
            ],
            if (tags.isNotEmpty) ...[
              const SizedBox(height: 8),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 14),
                child: Wrap(
                  spacing: 6,
                  children: tags.map((t) => Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: AppTheme.bg2,
                      borderRadius: BorderRadius.circular(999),
                    ),
                    child: Text(t, style: const TextStyle(fontSize: 10, color: AppTheme.ink2)),
                  )).toList(),
                ),
              ),
            ],
            Container(
              margin: const EdgeInsets.only(top: 10),
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
              decoration: BoxDecoration(border: Border(top: BorderSide(color: AppTheme.line))),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  _act(Icons.favorite_border_rounded, likes),
                  _act(Icons.chat_bubble_outline_rounded, comments),
                  _act(Icons.star_border_rounded, favorites),
                  _act(Icons.share_rounded, '分享'),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _act(IconData icon, String label) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 16, color: AppTheme.ink3),
        const SizedBox(width: 4),
        Text(label, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
      ],
    );
  }
}