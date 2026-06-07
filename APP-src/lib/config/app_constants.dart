class AppConstants {
  static const String appName = '拼豆';
  static const String appNameEn = 'PINDOU';
  static const String slogan = '用 AI 1 分钟把照片变成拼豆图纸';
  static const String appVersion = 'v0.1.0';
  
  // Screen
  static const double screenWidth = 375.0;
  static const double screenHeight = 812.0;
  
  // Token
  static const int accessTokenExpire = 7200;
  static const int refreshTokenExpire = 2592000;
  
  // SMS
  static const int smsInterval = 60;
  static const int smsDailyLimit = 10;
  
  // Image
  static const int maxImageSize = 20 * 1024 * 1024;
  static const int maxImageLongEdge = 8192;
  
  // Diagram
  static const List<int> beadCountOptions = [500, 1000, 2000, 3000, 5000, 8000, 10000];
  static const List<String> boardSizeOptions = ['29×29', '50×50', '58×58'];
  static const List<String> difficultyOptions = ['简单', '进阶', '写实'];
  static const List<String> difficultyValues = ['simple', 'advanced', 'realistic'];
  static const List<String> styleOptions = ['写实', '卡通'];
  static const List<String> styleValues = ['realistic', 'cartoon'];
  static const int maxVersionCount = 5;
  
  // Template categories
  static const List<String> templateCategories = [
    '推荐', '节日', '卡通', '二次元', '宠物', '风景', '像素游戏', '国风', '表情包', '文字'
  ];
  
  // Post types
  static const List<String> postTypes = ['作品', '求图', '教程', '讨论'];
  static const List<String> postTypeValues = ['work', 'request', 'tutorial', 'discussion'];
  
  // Color difficulty
  static const List<Map<String, dynamic>> colorDifficulties = [
    {'name': '极简', 'range': '8-12', 'min': 8, 'max': 12},
    {'name': '简单', 'range': '13-20', 'min': 13, 'max': 20},
    {'name': '标准', 'range': '21-35', 'min': 21, 'max': 35},
    {'name': '精细', 'range': '36-60', 'min': 36, 'max': 60},
    {'name': '极致', 'range': '60-100', 'min': 60, 'max': 100},
  ];
  
  // Supply recommendations
  static const List<Map<String, dynamic>> supplyRecommendations = [
    {'maxColors': 30, 'name': 'MARD 168 色套装', 'price': '¥89'},
    {'maxColors': 80, 'name': 'MARD 288 色套装', 'price': '¥168'},
    {'maxColors': 999, 'name': 'MARD 500 色套装', 'price': '¥298'},
  ];
}