// 路由
import 'package:flutter/material.dart';
import '../views/splash_view.dart';
import '../views/auth/login_view.dart';
import '../views/main/main_tab_view.dart';
import '../views/diagram/diagram_list_view.dart';
import '../views/diagram/diagram_detail_view.dart';
import '../views/diagram/diagram_create_view.dart';

class AppRouter {
  static const splash = '/splash';
  static const login = '/login';
  static const home = '/home';
  static const diagramList = '/diagram/list';
  static const diagramDetail = '/diagram/detail';
  static const diagramCreate = '/diagram/create';

  static Route<dynamic> onGenerateRoute(RouteSettings settings) {
    Widget page;
    switch (settings.name) {
      case splash:
        page = const SplashView();
        break;
      case login:
        page = const LoginView();
        break;
      case home:
        page = const MainTabView();
        break;
      case diagramList:
        page = const DiagramListView();
        break;
      case diagramDetail:
        final id = settings.arguments as String;
        page = DiagramDetailView(diagramId: id);
        break;
      case diagramCreate:
        page = const DiagramCreateView();
        break;
      default:
        page = const SplashView();
    }
    return MaterialPageRoute(builder: (_) => page, settings: settings);
  }
}
