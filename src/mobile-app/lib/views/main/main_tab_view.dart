// 主页 - 包含TabBar
import 'package:flutter/material.dart';
import 'home_view.dart';
import 'creation_view.dart';
import 'community_view.dart';
import 'profile_view.dart';

class MainTabView extends StatefulWidget {
  const MainTabView({super.key});
  @override
  State<MainTabView> createState() => _MainTabViewState();
}

class _MainTabViewState extends State<MainTabView> {
  int _current = 0;
  final _pages = const [HomeView(), CreationView(), CommunityView(), ProfileView()];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(index: _current, children: _pages),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _current,
        onDestinationSelected: (i) => setState(() => _current = i),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.home), label: '首页'),
          NavigationDestination(icon: Icon(Icons.brush), label: '创作'),
          NavigationDestination(icon: Icon(Icons.people), label: '社区'),
          NavigationDestination(icon: Icon(Icons.person), label: '我的'),
        ],
      ),
    );
  }
}
