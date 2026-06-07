import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'config/app_theme.dart';
import 'config/routes.dart';
import 'pages/splash/splash_page.dart';
import 'pages/auth/login_page.dart';
import 'pages/auth/sms_verify_page.dart';
import 'pages/home/home_page.dart';
import 'pages/create/select_image_page.dart';
import 'pages/create/edit_image_page.dart';
import 'pages/create/params_config_page.dart';
import 'pages/create/generating_page.dart';
import 'pages/create/result_preview_page.dart';
import 'pages/colors/colors_table_page.dart';
import 'pages/colors/difficulty_adjust_page.dart';
import 'pages/forum/forum_page.dart';
import 'pages/forum/post_detail_page.dart';
import 'pages/forum/post_create_page.dart';
import 'pages/forum/messages_page.dart';
import 'pages/forum/profile_page.dart';
import 'pages/template/templates_page.dart';
import 'pages/template/template_detail_page.dart';
import 'pages/template/creator_submit_page.dart';
import 'pages/mine/mine_page.dart';
import 'pages/mine/my_diagrams_page.dart';
import 'pages/mine/settings_page.dart';
import 'pages/mine/vip_page.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  SystemChrome.setPreferredOrientations([
    DeviceOrientation.portraitUp,
  ]);
  runApp(const PindouApp());
}

class PindouApp extends StatelessWidget {
  const PindouApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: '拼豆',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      initialRoute: AppRoutes.splash,
      routes: {
        AppRoutes.splash: (context) => const SplashPage(),
        AppRoutes.login: (context) => const LoginPage(),
        AppRoutes.smsVerify: (context) => const SmsVerifyPage(),
        AppRoutes.home: (context) => const HomePage(),
        AppRoutes.selectImage: (context) => const SelectImagePage(),
        AppRoutes.editImage: (context) => const EditImagePage(),
        AppRoutes.paramsConfig: (context) => const ParamsConfigPage(),
        AppRoutes.generating: (context) => const GeneratingPage(),
        AppRoutes.resultPreview: (context) => const ResultPreviewPage(),
        AppRoutes.colorsTable: (context) => const ColorsTablePage(),
        AppRoutes.difficultyAdjust: (context) => const DifficultyAdjustPage(),
        AppRoutes.forum: (context) => const ForumPage(),
        AppRoutes.postDetail: (context) => const PostDetailPage(),
        AppRoutes.postCreate: (context) => const PostCreatePage(),
        AppRoutes.messages: (context) => const MessagesPage(),
        AppRoutes.profile: (context) => const ProfilePage(),
        AppRoutes.templates: (context) => const TemplatesPage(),
        AppRoutes.templateDetail: (context) => const TemplateDetailPage(),
        AppRoutes.creatorSubmit: (context) => const CreatorSubmitPage(),
        AppRoutes.mine: (context) => const MinePage(),
        AppRoutes.myDiagrams: (context) => const MyDiagramsPage(),
        AppRoutes.settings: (context) => const SettingsPage(),
        AppRoutes.vip: (context) => const VipPage(),
      },
    );
  }
}