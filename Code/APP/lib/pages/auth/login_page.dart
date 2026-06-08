import 'package:flutter/material.dart';
import '../../config/app_theme.dart';
import '../../config/routes.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _phoneController = TextEditingController(text: '138 8888 8888');
  final _codeController = TextEditingController();
  bool _agreed = true;
  bool _codeSent = false;
  int _countdown = 59;

  @override
  void dispose() {
    _phoneController.dispose();
    _codeController.dispose();
    super.dispose();
  }

  void _sendCode() {
    setState(() {
      _codeSent = true;
      _countdown = 59;
    });
    _startCountdown();
  }

  void _startCountdown() {
    Future.doWhile(() async {
      await Future.delayed(const Duration(seconds: 1));
      if (!mounted) return false;
      setState(() {
        _countdown--;
      });
      return _countdown > 0;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        width: double.infinity,
        height: double.infinity,
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.center,
            colors: [Color(0xFFFFF4EC), Color(0xFFFBF7F2)],
          ),
        ),
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 40),
                Row(
                  children: [
                    Container(
                      width: 44,
                      height: 44,
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(14),
                        gradient: const LinearGradient(
                          colors: [Color(0xFFFF8A5A), Color(0xFFF5C45E)],
                        ),
                        boxShadow: [
                          BoxShadow(
                            color: const Color(0xFFFF8A5A).withOpacity(0.3),
                            blurRadius: 20,
                            offset: const Offset(0, 8),
                          ),
                        ],
                      ),
                      child: const Icon(Icons.grid_view_rounded, color: Colors.white, size: 24),
                    ),
                    const SizedBox(width: 10),
                    const Text(
                      '拼豆',
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 1,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 42),
                const Text(
                  '欢迎来到拼豆',
                  style: TextStyle(fontSize: 28, fontWeight: FontWeight.w700, height: 1.3),
                ),
                const SizedBox(height: 8),
                const Text(
                  '用 AI 把喜欢的照片变成可拼的拼豆图纸\n新手也能完成第一个作品',
                  style: TextStyle(fontSize: 14, color: AppTheme.ink3, height: 1.6),
                ),
                const SizedBox(height: 32),
                Container(
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(18),
                    border: Border.all(color: AppTheme.line),
                  ),
                  padding: const EdgeInsets.all(18),
                  child: Column(
                    children: [
                      TextField(
                        controller: _phoneController,
                        keyboardType: TextInputType.phone,
                        style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600, letterSpacing: 1),
                        decoration: InputDecoration(
                          hintText: '请输入手机号',
                          hintStyle: const TextStyle(fontSize: 18, fontWeight: FontWeight.w400, color: AppTheme.ink3),
                          border: InputBorder.none,
                          contentPadding: EdgeInsets.zero,
                          prefixIcon: const Icon(Icons.phone_android_rounded, color: AppTheme.ink3, size: 18),
                          prefixIconConstraints: const BoxConstraints(minWidth: 32),
                        ),
                      ),
                      const SizedBox(height: 14),
                      Row(
                        children: [
                          const Icon(Icons.lock_outline_rounded, color: AppTheme.ink3, size: 18),
                          const SizedBox(width: 8),
                          Expanded(
                            child: TextField(
                              controller: _codeController,
                              keyboardType: TextInputType.number,
                              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600, letterSpacing: 1),
                              decoration: const InputDecoration(
                                hintText: '6 位验证码',
                                hintStyle: TextStyle(fontSize: 18, fontWeight: FontWeight.w400, color: AppTheme.ink3),
                                border: InputBorder.none,
                                contentPadding: EdgeInsets.zero,
                              ),
                            ),
                          ),
                          GestureDetector(
                            onTap: _countdown == 0 ? _sendCode : null,
                            child: Text(
                              _codeSent ? '${_countdown}s 后重发' : '获取验证码',
                              style: TextStyle(
                                fontSize: 13,
                                fontWeight: FontWeight.w600,
                                color: _countdown == 0 ? AppTheme.primaryInk : AppTheme.ink3,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 18),
                SizedBox(
                  width: double.infinity,
                  height: 48,
                  child: ElevatedButton(
                    onPressed: () {
                      Navigator.of(context).pushReplacementNamed(AppRoutes.home);
                    },
                    child: const Text('手机号快捷登录'),
                  ),
                ),
                const SizedBox(height: 8),
                const Center(
                  child: Text(
                    '其他方式登录',
                    style: TextStyle(fontSize: 12, color: AppTheme.ink3),
                  ),
                ),
                const SizedBox(height: 16),
                Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    _providerButton(Icons.wechat_rounded, const Color(0xFF1AAD19), '微信'),
                    const SizedBox(width: 14),
                    _providerButton(Icons.apple_rounded, Colors.black, 'Apple'),
                    const SizedBox(width: 14),
                    _providerButton(Icons.person_outline_rounded, AppTheme.ink2, '游客'),
                  ],
                ),
                const SizedBox(height: 18),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    GestureDetector(
                      onTap: () => setState(() => _agreed = !_agreed),
                      child: Container(
                        width: 16,
                        height: 16,
                        margin: const EdgeInsets.only(top: 1, right: 8),
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(4),
                          color: _agreed ? AppTheme.primary : Colors.transparent,
                          border: Border.all(
                            color: _agreed ? AppTheme.primary : AppTheme.ink3,
                            width: 1.5,
                          ),
                        ),
                        child: _agreed
                            ? const Icon(Icons.check, size: 11, color: Colors.white)
                            : null,
                      ),
                    ),
                    Expanded(
                      child: RichText(
                        text: TextSpan(
                          style: const TextStyle(fontSize: 12, color: AppTheme.ink2, height: 1.5),
                          children: [
                            const TextSpan(text: '我已阅读并同意 '),
                            TextSpan(
                              text: '《用户协议》',
                              style: TextStyle(color: AppTheme.primaryInk),
                            ),
                            const TextSpan(text: ' 和 '),
                            TextSpan(
                              text: '《隐私政策》',
                              style: TextStyle(color: AppTheme.primaryInk),
                            ),
                            const TextSpan(text: '，未注册的手机号将自动创建账号'),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
                const Spacer(),
                Center(
                  child: Padding(
                    padding: const EdgeInsets.only(bottom: 20),
                    child: Text.rich(
                      TextSpan(
                        style: const TextStyle(fontSize: 12, color: AppTheme.ink3),
                        children: [
                          const TextSpan(text: '遇到问题？ '),
                          TextSpan(
                            text: '联系客服',
                            style: TextStyle(color: AppTheme.primaryInk),
                          ),
                          const TextSpan(text: ' · '),
                          TextSpan(
                            text: '游客体验',
                            style: TextStyle(color: AppTheme.primaryInk),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _providerButton(IconData icon, Color color, String label) {
    return Container(
      width: 56,
      height: 56,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: AppTheme.line),
      ),
      child: Icon(icon, color: color, size: 28),
    );
  }
}