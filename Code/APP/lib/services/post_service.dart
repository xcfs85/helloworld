import '../config/api_config.dart';
import '../models/post.dart';
import 'api_client.dart';

class PostService {
  final ApiClient _client = ApiClient();

  Future<List<Post>> getFeed({
    required String type,
    int page = 1,
    int pageSize = 20,
    String? topicId,
  }) async {
    final params = <String, String>{
      'type': type,
      'page': page.toString(),
      'page_size': pageSize.toString(),
    };
    if (topicId != null) params['topic_id'] = topicId;
    final data = await _client.get(ApiConfig.feedList, queryParams: params);
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Post.fromJson(item)).toList();
  }

  Future<Post> createPost({
    required String type,
    required String title,
    required String content,
    required List<Map<String, dynamic>> media,
    List<String>? topicIds,
    Map<String, dynamic>? beadParams,
    String? diagramId,
  }) async {
    final body = <String, dynamic>{
      'type': type,
      'title': title,
      'content': content,
      'media': media,
    };
    if (topicIds != null) body['topic_ids'] = topicIds;
    if (beadParams != null) body['bead_params'] = beadParams;
    if (diagramId != null) body['diagram_id'] = diagramId;
    final data = await _client.post(ApiConfig.postCreate, body: body);
    return Post.fromJson(data);
  }

  Future<Post> getPostDetail(String id) async {
    final path = ApiConfig.postDetail.replaceFirst('{id}', id);
    final data = await _client.get(path);
    return Post.fromJson(data);
  }

  Future<List<Post>> getMyPosts({int page = 1, int pageSize = 20}) async {
    final data = await _client.get(ApiConfig.postMy, queryParams: {
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Post.fromJson(item)).toList();
  }

  Future<void> likePost(String id) async {
    final path = ApiConfig.postLike.replaceFirst('{id}', id);
    await _client.post(path);
  }

  Future<void> unlikePost(String id) async {
    final path = ApiConfig.postLike.replaceFirst('{id}', id);
    await _client.delete(path);
  }

  Future<void> favoritePost(String id) async {
    final path = ApiConfig.postFavorite.replaceFirst('{id}', id);
    await _client.post(path);
  }

  Future<void> unfavoritePost(String id) async {
    final path = ApiConfig.postFavorite.replaceFirst('{id}', id);
    await _client.delete(path);
  }

  Future<List<Post>> getFavorites({int page = 1, int pageSize = 20}) async {
    final data = await _client.get(ApiConfig.postFavorites, queryParams: {
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Post.fromJson(item)).toList();
  }

  Future<List<Comment>> getComments(String postId, {int page = 1, int pageSize = 20}) async {
    final path = ApiConfig.postComments.replaceFirst('{id}', postId);
    final data = await _client.get(path, queryParams: {
      'page': page.toString(),
      'page_size': pageSize.toString(),
    });
    final list = data['list'] as List<dynamic>? ?? [];
    return list.map((item) => Comment.fromJson(item)).toList();
  }

  Future<Comment> addComment(String postId, {
    required String content,
    String? parentId,
    String? replyToUserId,
  }) async {
    final path = ApiConfig.postComment.replaceFirst('{id}', postId);
    final body = <String, dynamic>{'content': content};
    if (parentId != null) body['parent_id'] = parentId;
    if (replyToUserId != null) body['reply_to_user_id'] = replyToUserId;
    final data = await _client.post(path, body: body);
    return Comment.fromJson(data);
  }

  Future<void> deletePost(String id) async {
    final path = ApiConfig.postDetail.replaceFirst('{id}', id);
    await _client.delete(path);
  }

  Future<void> deleteComment(String id) async {
    final path = ApiConfig.commentDelete.replaceFirst('{id}', id);
    await _client.delete(path);
  }
}