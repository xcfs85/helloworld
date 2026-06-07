// 登录页
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';
import '../router/app_router.dart';

class LoginView extends StatefulWidget {
  const LoginView({super.key});
  @override
  State<LoginView> createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {
  final _phoneCtrl = TextEditingController();
  final _codeCtrl = TextEditingController();
  bool _loading = false;

  Future<void> _phoneLogin() async {
    if (_phoneCtrl.text.isEmpty || _codeCtrl.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('请输入手机号和验证码')),
      );
      return;
    }
    setState(() => _loading = true);
    final auth = context.read<AuthProvider>();
    final ok = await auth.phoneLogin(_phoneCtrl.text, _codeCtrl.text);
    setState(() => _loading = false);
    if (ok && mounted) {
      Navigator.pushReplacementNamed(context, AppRouter.home);
    } else if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('登录失败，请重试')),
      );
    }
  }

  Future<void> _guestLogin() async {
    setState(() => _loading = true);
    final auth = context.read<AuthProvider>();
    final ok = await auth.guestLogin();
    setState(() => _loading = false);
    if (ok && mounted) {
      Navigator.pushReplacementNamed(context, AppRouter.home);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 60),
              const Center(
                child: Icon(Icons.palette, size: 80, color: Color(0xFFFF6B6B)),
              ),
              const SizedBox(height: 16),
              const Center(
                child: Text('拼豆', style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold)),
              ),
              const SizedBox(height: 60),
              TextField(
                controller: _phoneCtrl,
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: '手机号',
                  prefixIcon: Icon(Icons.phone),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _codeCtrl,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(
                        labelText: '验证码',
                        prefixIcon: Icon(Icons.message),
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  ElevatedButton(onPressed: () {}, child: const Text('发送')),
                ],
              ),
              const SizedBox(height: 24),
              ElevatedButton(
                onPressed: _loading ? null : _phoneLogin,
                child: _loading ? const Text('登录中...') : const Text('登录'),
              ),
              const SizedBox(height: 12),
              OutlinedButton(
                onPressed: _loading ? null : _guestLogin,
                child: const Text('游客登录'),
              ),
              const Spacer(),
              const Center(child: Text('登录即代表同意《用户协议》和《隐私政策》', style: TextStyle(color: Colors.grey, fontSize: 12))),
            ],
          ),
        ),
      ),
    );
  }
}
