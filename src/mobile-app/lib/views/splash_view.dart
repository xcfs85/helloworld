// 启动页
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../router/app_router.dart';

class SplashView extends StatefulWidget {
  const SplashView({super.key});
  @override
  State<SplashView> createState() => _SplashViewState();
}

class _SplashViewState extends State<SplashView> {
  @override
  void initState() {
    super.initState();
    _navigate();
  }

  Future<void> _navigate() async {
    await Future.delayed(const Duration(milliseconds: 1500));
    if (!mounted) return;
    final auth = context.read<AuthProvider>();
    await auth.init();
    if (!mounted) return;
    Navigator.pushReplacementNamed(
      context,
      auth.isLoggedIn ? AppRouter.home : AppRouter.login,
    );
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.palette, size: 100, color: Color(0xFFFF6B6B)),
            SizedBox(height: 20),
            Text('拼豆', style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold)),
            SizedBox(height: 12),
            Text('AI照片转拼豆图纸', style: TextStyle(color: Colors.grey)),
          ],
        ),
      ),
    );
  }
}
