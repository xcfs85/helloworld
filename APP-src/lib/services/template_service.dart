import '../config/api_config.dart';
import '../models/template.dart';
import 'api_client.dart';

class TemplateService {
  final ApiClient _client = ApiClient();

  Future<List<Template>> getList({
    int page = 1,
    int pageSize = 20,
    String? category,
    String? difficulty,
    String? sort,
  }) async {
    final params = <String, String>{
      'page': page.toString(),
      'page_size': pageSize.toString(),
    };
    if (category != null) params['category'] = category;
    if (difficulty != null) params['difficulty'] = difficulty;
    if (sort != null) params['sort'] = sort;
    final data = await _client.get(ApiConfig.templateList, queryParams: params);
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Template.fromJson(item)).toList();
  }

  Future<Template> getDetail(String id) async {
    final path = ApiConfig.templateDetail.replaceFirst('{id}', id);
    final data = await _client.get(path);
    return Template.fromJson(data);
  }

  Future<List<Template>> search(String keyword) async {
    final data = await _client.get(ApiConfig.templateSearch, queryParams: {
      'keyword': keyword,
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Template.fromJson(item)).toList();
  }

  Future<List<Template>> getFeatured() async {
    final data = await _client.get(ApiConfig.templateFeatured);
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Template.fromJson(item)).toList();
  }

  Future<void> favorite(String id) async {
    final path = ApiConfig.templateFavorite.replaceFirst('{id}', id);
    await _client.post(path);
  }

  Future<void> unfavorite(String id) async {
    final path = ApiConfig.templateFavorite.replaceFirst('{id}', id);
    await _client.delete(path);
  }

  Future<List<Template>> getFavorites() async {
    final data = await _client.get(ApiConfig.templateFavorites);
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Template.fromJson(item)).toList();
  }

  Future<Map<String, dynamic>> submit({
    required String name,
    required String category,
    required List<String> tags,
    required String description,
    required Map<String, dynamic> beadParams,
    required String diagramUrl,
    required String coverUrl,
  }) async {
    return await _client.post(ApiConfig.templateSubmit, body: {
      'name': name,
      'category': category,
      'tags': tags,
      'description': description,
      'bead_params': beadParams,
      'diagram_url': diagramUrl,
      'cover_url': coverUrl,
    });
  }
}