import '../config/api_config.dart';
import '../models/user.dart';
import 'api_client.dart';

class AuthService {
  final ApiClient _client = ApiClient();

  Future<Map<String, dynamic>> sendSms(String phone, String scene) async {
    return await _client.post(ApiConfig.smsSend, body: {
      'phone': phone,
      'scene': scene,
    });
  }

  Future<AuthResponse> phoneLogin(String phone, String code, String deviceId) async {
    final data = await _client.post(ApiConfig.phoneLogin, body: {
      'phone': phone,
      'code': code,
      'device_id': deviceId,
      'agree_protocol': true,
    });
    final authResponse = AuthResponse.fromJson(data);
    await _client.setTokens(authResponse.accessToken, authResponse.refreshToken);
    return authResponse;
  }

  Future<AuthResponse> wechatLogin(String code, String deviceId) async {
    final data = await _client.post(ApiConfig.wechatLogin, body: {
      'code': code,
      'device_id': deviceId,
    });
    final authResponse = AuthResponse.fromJson(data);
    await _client.setTokens(authResponse.accessToken, authResponse.refreshToken);
    return authResponse;
  }

  Future<AuthResponse> appleLogin(String identityToken, String deviceId) async {
    final data = await _client.post(ApiConfig.appleLogin, body: {
      'identity_token': identityToken,
      'device_id': deviceId,
    });
    final authResponse = AuthResponse.fromJson(data);
    await _client.setTokens(authResponse.accessToken, authResponse.refreshToken);
    return authResponse;
  }

  Future<AuthResponse> guestLogin(String deviceId) async {
    final data = await _client.post(ApiConfig.guestLogin, body: {
      'device_id': deviceId,
    });
    final authResponse = AuthResponse.fromJson(data);
    await _client.setTokens(authResponse.accessToken, authResponse.refreshToken);
    return authResponse;
  }

  Future<void> refreshToken() async {
    await _client.loadTokens();
    final refreshToken = _client._refreshToken;
    if (refreshToken == null) throw Exception('No refresh token');
    final data = await _client.post(ApiConfig.tokenRefresh, body: {
      'refresh_token': refreshToken,
    });
    await _client.setTokens(
      data['access_token'] ?? '',
      data['refresh_token'] ?? '',
    );
  }

  Future<void> bindPhone(String phone, String code) async {
    await _client.post(ApiConfig.phoneBind, body: {
      'phone': phone,
      'code': code,
    });
  }

  Future<void> logout() async {
    await _client.clearTokens();
  }
}