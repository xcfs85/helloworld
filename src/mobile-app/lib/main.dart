// 拼豆App主入口
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'router/app_router.dart';
import 'providers/auth_provider.dart';
import 'providers/diagram_provider.dart';

void main() {
  runApp(const PindouApp());
}

class PindouApp extends StatelessWidget {
  const PindouApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => DiagramProvider()),
      ],
      child: MaterialApp(
        title: '拼豆',
        debugShowCheckedModeBanner: false,
        theme: ThemeData(
          colorSchemeSeed: const Color(0xFFFF6B6B),
          useMaterial3: true,
          fontFamily: 'PingFang SC',
        ),
        onGenerateRoute: AppRouter.onGenerateRoute,
        initialRoute: AppRouter.splash,
      ),
    );
  }
}
