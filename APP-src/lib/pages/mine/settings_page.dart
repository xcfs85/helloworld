import 'package:flutter/material.dart';
import '../../config/app_theme.dart';

class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bg,
      appBar: AppBar(
        title: const Text('设置'),
        backgroundColor: AppTheme.surface,
        elevation: 0.5,
        leading: IconButton(
          icon: const Icon(Icons.chevron_left, size: 28),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Column(
        children: [
          const SizedBox(height: 10),
          Container(
            color: AppTheme.surface,
            child: Column(
              children: [
                _settingRow('编辑资料', '', () {}),
                _settingRow('账号安全', '', () {}),
                _settingRow('通知设置', '评论、点赞、关注', () {}),
                _settingRow('隐私设置', '公开', () {}),
              ],
            ),
          ),
          const SizedBox(height: 10),
          Container(
            color: AppTheme.surface,
            child: Column(
              children: [
                _settingRow('深色模式', '', () {}, trailing: Switch(
                  value: false,
                  onChanged: (v) {},
                  activeColor: AppTheme.primary,
                )),
                _settingRow('消息提醒', '', () {}, trailing: Switch(
                  value: true,
                  onChanged: (v) {},
                  activeColor: AppTheme.primary,
                )),
                _settingRow('清除缓存', '128 MB', () {}),
              ],
            ),
          ),
          const SizedBox(height: 10),
          Container(
            color: AppTheme.surface,
            child: Column(
              children: [
                _settingRow('用户协议', '', () {}),
                _settingRow('隐私政策', '', () {}),
                _settingRow('关于', 'v0.1.0', () {}),
              ],
            ),
          ),
          const Spacer(),
          Padding(
            padding: const EdgeInsets.all(18),
            child: SizedBox(
              width: double.infinity,
              child: OutlinedButton(
                onPressed: () {},
                style: OutlinedButton.styleFrom(
                  foregroundColor: const Color(0xFFFF3B30),
                  side: const BorderSide(color: Color(0xFFFF3B30)),
                ),
                child: const Text('退出登录'),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _settingRow(String title, String subtitle, VoidCallback onTap, {Widget? trailing}) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
        decoration: BoxDecoration(
          border: Border(bottom: BorderSide(color: AppTheme.line)),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w500)),
                  if (subtitle.isNotEmpty)
                    Text(subtitle, style: const TextStyle(fontSize: 12, color: AppTheme.ink3)),
                ],
              ),
            ),
            trailing ?? const Icon(Icons.chevron_right, size: 16, color: AppTheme.ink3),
          ],
        ),
      ),
    );
  }
}