// 我的
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/auth_provider.dart';

class ProfileView extends StatelessWidget {
  const ProfileView({super.key});
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('我的'), centerTitle: true),
      body: Consumer<AuthProvider>(
        builder: (context, auth, _) {
          return ListView(
            children: [
              // 用户信息
              Container(
                padding: const EdgeInsets.all(20),
                child: Row(
                  children: [
                    CircleAvatar(
                      radius: 30,
                      backgroundImage: auth.avatar != null ? NetworkImage(auth.avatar!) : null,
                      child: auth.avatar == null ? const Icon(Icons.person) : null,
                    ),
                    const SizedBox(width: 12),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(auth.nickname ?? '游客', style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                        if (auth.isMember) const Chip(label: Text('会员')),
                      ],
                    ),
                  ],
                ),
              ),
              // 功能列表
              _item(context, '我的作品', Icons.image),
              _item(context, '我的收藏', Icons.bookmark),
              _item(context, '消息中心', Icons.message),
              _item(context, '会员中心', Icons.card_membership),
              _item(context, '设置', Icons.settings),
              _item(context, '意见反馈', Icons.feedback),
              _item(context, '关于', Icons.info),
              const Divider(),
              ListTile(
                leading: const Icon(Icons.logout, color: Colors.red),
                title: const Text('退出登录', style: TextStyle(color: Colors.red)),
                onTap: () async {
                  await auth.logout();
                  if (context.mounted) {
                    Navigator.pushReplacementNamed(context, '/login');
                  }
                },
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _item(BuildContext context, String title, IconData icon) {
    return ListTile(
      leading: Icon(icon),
      title: Text(title),
      trailing: const Icon(Icons.chevron_right),
      onTap: () {},
    );
  }
}
