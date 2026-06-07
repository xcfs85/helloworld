class User {
  final String userId;
  final String nickname;
  final String avatar;
  final String? phone;
  final String? unionid;
  final String? appleUserId;
  final String registerMethod;
  final String registerTime;
  final String lastLoginTime;
  final String status;
  final bool isMember;
  final String? memberExpireTime;
  final String? gender;
  final String? city;
  final String? bio;
  final int followCount;
  final int fansCount;
  final int postCount;

  User({
    required this.userId,
    required this.nickname,
    required this.avatar,
    this.phone,
    this.unionid,
    this.appleUserId,
    required this.registerMethod,
    required this.registerTime,
    required this.lastLoginTime,
    required this.status,
    required this.isMember,
    this.memberExpireTime,
    this.gender,
    this.city,
    this.bio,
    this.followCount = 0,
    this.fansCount = 0,
    this.postCount = 0,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      userId: json['user_id'] ?? '',
      nickname: json['nickname'] ?? '',
      avatar: json['avatar'] ?? '',
      phone: json['phone'],
      unionid: json['unionid'],
      appleUserId: json['apple_user_id'],
      registerMethod: json['register_method'] ?? 'phone',
      registerTime: json['register_time'] ?? '',
      lastLoginTime: json['last_login_time'] ?? '',
      status: json['status'] ?? 'active',
      isMember: json['is_member'] ?? false,
      memberExpireTime: json['member_expire_time'],
      gender: json['gender'],
      city: json['city'],
      bio: json['bio'],
      followCount: json['follow_count'] ?? 0,
      fansCount: json['fans_count'] ?? 0,
      postCount: json['post_count'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'user_id': userId,
      'nickname': nickname,
      'avatar': avatar,
      'phone': phone,
      'unionid': unionid,
      'apple_user_id': appleUserId,
      'register_method': registerMethod,
      'register_time': registerTime,
      'last_login_time': lastLoginTime,
      'status': status,
      'is_member': isMember,
      'member_expire_time': memberExpireTime,
      'gender': gender,
      'city': city,
      'bio': bio,
      'follow_count': followCount,
      'fans_count': fansCount,
      'post_count': postCount,
    };
  }
}

class AuthResponse {
  final String userId;
  final String accessToken;
  final String refreshToken;
  final int expiresIn;
  final bool isNewUser;
  final String nickname;
  final String avatar;

  AuthResponse({
    required this.userId,
    required this.accessToken,
    required this.refreshToken,
    required this.expiresIn,
    required this.isNewUser,
    required this.nickname,
    required this.avatar,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      userId: json['user_id'] ?? '',
      accessToken: json['access_token'] ?? '',
      refreshToken: json['refresh_token'] ?? '',
      expiresIn: json['expires_in'] ?? 7200,
      isNewUser: json['is_new_user'] ?? false,
      nickname: json['nickname'] ?? '',
      avatar: json['avatar'] ?? '',
    );
  }
}