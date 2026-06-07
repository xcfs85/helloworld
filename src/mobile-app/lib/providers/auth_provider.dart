// 认证Provider
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../services/api_client.dart';

class AuthProvider extends ChangeNotifier {
  String? _token;
  String? _userId;
  String? _nickname;
  String? _avatar;
  bool _isMember = false;

  String? get token => _token;
  String? get userId => _userId;
  String? get nickname => _nickname;
  String? get avatar => _avatar;
  bool get isMember => _isMember;
  bool get isLoggedIn => _token != null;

  Future<void> init() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString('access_token');
    _userId = prefs.getString('user_id');
    _nickname = prefs.getString('nickname');
    _avatar = prefs.getString('avatar');
    _isMember = prefs.getBool('is_member') ?? false;
    notifyListeners();
  }

  Future<bool> phoneLogin(String phone, String code) async {
    try {
      final res = await ApiClient.instance.post<Map<String, dynamic>>(
        '/auth/phone/login',
        data: {'phone': phone, 'code': code},
      );
      await _saveLoginInfo(res);
      return true;
    } catch (e) {
      return false;
    }
  }

  Future<bool> guestLogin() async {
    try {
      final res = await ApiClient.instance.post<Map<String, dynamic>>(
        '/auth/guest/login',
        data: {},
      );
      await _saveLoginInfo(res);
      return true;
    } catch (e) {
      return false;
    }
  }

  Future<void> _saveLoginInfo(Map<String, dynamic> res) async {
    final prefs = await SharedPreferences.getInstance();
    final data = res['data'] as Map<String, dynamic>;
    _token = data['token'];
    final user = data['user'] as Map<String, dynamic>;
    _userId = user['id'];
    _nickname = user['nickname'];
    _avatar = user['avatar'];
    _isMember = user['isMember'] ?? false;

    await prefs.setString('access_token', _token!);
    await prefs.setString('refresh_token', data['refreshToken']);
    await prefs.setString('user_id', _userId!);
    await prefs.setString('nickname', _nickname ?? '');
    await prefs.setString('avatar', _avatar ?? '');
    await prefs.setBool('is_member', _isMember);
    notifyListeners();
  }

  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove('access_token');
    await prefs.remove('refresh_token');
    _token = null;
    _userId = null;
    notifyListeners();
  }
}
