// 图纸Provider
import 'package:flutter/foundation.dart';
import '../services/api_client.dart';

class DiagramProvider extends ChangeNotifier {
  List<Map<String, dynamic>> _diagrams = [];
  Map<String, dynamic>? _currentTask;

  List<Map<String, dynamic>> get diagrams => _diagrams;
  Map<String, dynamic>? get currentTask => _currentTask;

  Future<void> loadDiagrams() async {
    try {
      final res = await ApiClient.instance.get<Map<String, dynamic>>(
        '/diagram/list',
        params: {'page': 1, 'size': 20},
      );
      final data = res['data'] as Map<String, dynamic>;
      _diagrams = List<Map<String, dynamic>>.from(data['list'] ?? []);
      notifyListeners();
    } catch (e) {
      debugPrint('Load diagrams failed: $e');
    }
  }

  Future<String> createGeneration({
    required String sourceImageUrl,
    required String boardSize,
    required String difficulty,
    required String style,
    bool isSync = false,
  }) async {
    final res = await ApiClient.instance.post<Map<String, dynamic>>(
      isSync ? '/diagram/generate/sync' : '/diagram/generate',
      data: {
        'sourceImageUrl': sourceImageUrl,
        'boardSize': boardSize,
        'difficulty': difficulty,
        'style': style,
        'isSync': isSync,
      },
    );
    _currentTask = res['data'];
    notifyListeners();
    return res['data'] as String;
  }

  Future<Map<String, dynamic>> getTaskStatus(String taskId) async {
    final res = await ApiClient.instance.get<Map<String, dynamic>>(
      '/diagram/task/$taskId',
    );
    _currentTask = res['data'];
    notifyListeners();
    return res['data'] as Map<String, dynamic>;
  }
}
