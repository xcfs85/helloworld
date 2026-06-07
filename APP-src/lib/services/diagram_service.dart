import '../config/api_config.dart';
import '../models/diagram.dart';
import 'api_client.dart';

class DiagramService {
  final ApiClient _client = ApiClient();

  Future<GenerateTask> generate({
    required String sourceImageUrl,
    required String boardSize,
    required int beadCount,
    required String difficulty,
    required String style,
    Map<String, dynamic>? cropRect,
    int? rotateAngle,
    bool? flipHorizontal,
    bool? flipVertical,
    int? brightness,
    int? contrast,
    int? saturation,
  }) async {
    final body = <String, dynamic>{
      'source_image_url': sourceImageUrl,
      'board_size': boardSize,
      'bead_count': beadCount,
      'difficulty': difficulty,
      'style': style,
    };
    if (cropRect != null) body['crop_rect'] = cropRect;
    if (rotateAngle != null) body['rotate_angle'] = rotateAngle;
    if (flipHorizontal != null) body['flip_horizontal'] = flipHorizontal;
    if (flipVertical != null) body['flip_vertical'] = flipVertical;
    if (brightness != null) body['brightness'] = brightness;
    if (contrast != null) body['contrast'] = contrast;
    if (saturation != null) body['saturation'] = saturation;

    final data = await _client.post(ApiConfig.diagramGenerate, body: body);
    return GenerateTask.fromJson(data);
  }

  Future<TaskProgress> getTaskStatus(String taskId) async {
    final path = ApiConfig.diagramTask.replaceFirst('{task_id}', taskId);
    final data = await _client.get(path);
    return TaskProgress.fromJson(data);
  }

  Future<Diagram> getDiagram(String id) async {
    final path = ApiConfig.diagramDetail.replaceFirst('{id}', id);
    final data = await _client.get(path);
    return Diagram.fromJson(data);
  }

  Future<List<Diagram>> getMyDiagrams({int page = 1, int pageSize = 20, String? status}) async {
    final params = <String, String>{
      'page': page.toString(),
      'page_size': pageSize.toString(),
    };
    if (status != null) params['status'] = status;
    final data = await _client.get(ApiConfig.diagramMy, queryParams: params);
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Diagram.fromJson(item)).toList();
  }

  Future<Map<String, dynamic>> getColors(String id, {String? sort, String? filter}) async {
    final path = ApiConfig.diagramColors.replaceFirst('{id}', id);
    final params = <String, String>{};
    if (sort != null) params['sort'] = sort;
    if (filter != null) params['filter'] = filter;
    final data = await _client.get(path, queryParams: params);
    return data;
  }

  Future<void> adjustDifficulty(String id, int targetColors) async {
    final path = ApiConfig.diagramAdjust.replaceFirst('{id}', id);
    await _client.post(path, body: {'target_colors': targetColors});
  }

  Future<Map<String, dynamic>> exportDiagram(String id, {
    required String format,
    bool includeGrid = true,
    bool includeColorTable = true,
  }) async {
    final path = ApiConfig.diagramExport.replaceFirst('{id}', id);
    return await _client.post(path, body: {
      'format': format,
      'include_grid': includeGrid,
      'include_color_table': includeColorTable,
    });
  }

  Future<void> deleteDiagram(String id) async {
    final path = ApiConfig.diagramDetail.replaceFirst('{id}', id);
    await _client.delete(path);
  }
}