import '../config/api_config.dart';
import '../models/user.dart';
import 'api_client.dart';

class UserService {
  final ApiClient _client = ApiClient();

  Future<User> getProfile() async {
    final data = await _client.get(ApiConfig.userProfile);
    return User.fromJson(data);
  }

  Future<void> updateProfile({
    String? nickname,
    String? avatar,
    String? gender,
    String? city,
    String? bio,
  }) async {
    final body = <String, dynamic>{};
    if (nickname != null) body['nickname'] = nickname;
    if (avatar != null) body['avatar'] = avatar;
    if (gender != null) body['gender'] = gender;
    if (city != null) body['city'] = city;
    if (bio != null) body['bio'] = bio;
    await _client.put(ApiConfig.userProfile, body: body);
  }

  Future<User> getUserInfo(String userId) async {
    final path = ApiConfig.userInfo.replaceFirst('{user_id}', userId);
    final data = await _client.get(path);
    return User.fromJson(data);
  }

  Future<void> follow(String userId) async {
    final path = ApiConfig.userFollow.replaceFirst('{user_id}', userId);
    await _client.post(path);
  }

  Future<void> unfollow(String userId) async {
    final path = ApiConfig.userFollow.replaceFirst('{user_id}', userId);
    await _client.delete(path);
  }

  Future<List<User>> getFollows({int page = 1, int pageSize = 20}) async {
    final data = await _client.get(ApiConfig.userFollows, queryParams: {
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => User.fromJson(item)).toList();
  }

  Future<List<User>> getFans({int page = 1, int pageSize = 20}) async {
    final data = await _client.get(ApiConfig.userFans, queryParams: {
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => User.fromJson(item)).toList();
  }
}