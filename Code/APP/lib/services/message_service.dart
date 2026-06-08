import '../config/api_config.dart';
import '../models/message.dart';
import 'api_client.dart';

class MessageService {
  final ApiClient _client = ApiClient();

  Future<List<Message>> getMessages({
    required String type,
    int page = 1,
    int pageSize = 20,
  }) async {
    final data = await _client.get(ApiConfig.messageList, queryParams: {
      'type': type,
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Message.fromJson(item)).toList();
  }

  Future<UnreadCount> getUnreadCount() async {
    final data = await _client.get(ApiConfig.messageUnread);
    return UnreadCount.fromJson(data);
  }

  Future<void> markAsRead({required String type, String? messageId}) async {
    final body = <String, dynamic>{'type': type};
    if (messageId != null) body['message_id'] = messageId;
    await _client.post(ApiConfig.messageRead, body: body);
  }

  Future<MessageSettings> getSettings() async {
    final data = await _client.get(ApiConfig.messageSettings);
    return MessageSettings.fromJson(data);
  }

  Future<void> updateSettings(MessageSettings settings) async {
    await _client.put(ApiConfig.messageSettings, body: settings.toJson());
  }
}