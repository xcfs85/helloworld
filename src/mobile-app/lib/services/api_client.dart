// API客户端
import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ApiClient {
  static const String baseUrl = 'https://api.pindou.com/api/v1';

  static ApiClient? _instance;
  late final Dio _dio;

  ApiClient._() {
    _dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: const Duration(seconds: 30),
      receiveTimeout: const Duration(seconds: 30),
    ));

    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final prefs = await SharedPreferences.getInstance();
        final token = prefs.getString('access_token');
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onResponse: (response, handler) {
        final data = response.data;
        if (data is Map && data['code'] == 0) {
          return handler.next(response);
        }
        return handler.reject(DioException(
          requestOptions: response.requestOptions,
          response: response,
          message: data is Map ? data['message'] : '请求失败',
        ));
      },
    ));
  }

  static ApiClient get instance => _instance ??= ApiClient._();

  Future<T> get<T>(String path, {Map<String, dynamic>? params}) async {
    final res = await _dio.get<T>(path, queryParameters: params);
    return res.data as T;
  }

  Future<T> post<T>(String path, {dynamic data}) async {
    final res = await _dio.post<T>(path, data: data);
    return res.data as T;
  }
}
