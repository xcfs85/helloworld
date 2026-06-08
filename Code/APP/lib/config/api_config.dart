class ApiConfig {
  static const String baseUrl = 'https://dev-api.pindou.com/api/v1';
  static const String wsUrl = 'wss://dev-api.pindou.com/ws';
  
  // Auth
  static const String smsSend = '/auth/sms/send';
  static const String phoneLogin = '/auth/phone/login';
  static const String wechatLogin = '/auth/wechat/login';
  static const String appleLogin = '/auth/apple/login';
  static const String guestLogin = '/auth/guest/login';
  static const String tokenRefresh = '/auth/token/refresh';
  static const String phoneBind = '/auth/phone/bind';
  
  // User
  static const String userProfile = '/user/profile';
  static const String userInfo = '/user/{user_id}/info';
  static const String userFollow = '/user/{user_id}/follow';
  static const String userFollows = '/user/follows';
  static const String userFans = '/user/fans';
  
  // Image
  static const String imageUpload = '/image/upload';
  static const String imageUploadBatch = '/image/upload/batch';
  
  // Diagram
  static const String diagramGenerate = '/diagram/generate';
  static const String diagramTask = '/diagram/task/{task_id}';
  static const String diagramDetail = '/diagram/{id}';
  static const String diagramMy = '/diagram/my';
  static const String diagramColors = '/diagram/{id}/colors';
  static const String diagramAdjust = '/diagram/{id}/adjust';
  static const String diagramExport = '/diagram/{id}/export';
  static const String diagramRegenerate = '/diagram/{id}/regenerate';
  
  // Template
  static const String templateList = '/template/list';
  static const String templateDetail = '/template/{id}';
  static const String templateSearch = '/template/search';
  static const String templateFeatured = '/template/featured';
  static const String templateFavorite = '/template/{id}/favorite';
  static const String templateFavorites = '/template/favorites';
  static const String templateSubmit = '/template/submit';
  
  // Post/Feed
  static const String feedList = '/feed/list';
  static const String postCreate = '/post/create';
  static const String postDetail = '/post/{id}';
  static const String postMy = '/post/my';
  static const String postDrafts = '/post/drafts';
  static const String postDraft = '/post/draft';
  static const String postLike = '/post/{id}/like';
  static const String postFavorite = '/post/{id}/favorite';
  static const String postFavorites = '/post/favorites';
  static const String postComments = '/post/{id}/comments';
  static const String postComment = '/post/{id}/comment';
  static const String commentDelete = '/comment/{id}';
  static const String commentLike = '/comment/{id}/like';
  
  // Message
  static const String messageList = '/message/list';
  static const String messageUnread = '/message/unread';
  static const String messageRead = '/message/read';
  static const String messageSettings = '/message/settings';
}