import 'package:flutter/material.dart';

class AppTheme {
  // Colors from UI design system
  static const Color primary = Color(0xFFFF7A5A);
  static const Color primary2 = Color(0xFFFF9876);
  static const Color primaryInk = Color(0xFFB83B1B);
  static const Color accent = Color(0xFFF5C45E);
  static const Color bg = Color(0xFFFBF7F2);
  static const Color bg2 = Color(0xFFF4ECE0);
  static const Color ink = Color(0xFF2A1F1A);
  static const Color ink2 = Color(0xFF5A4A40);
  static const Color ink3 = Color(0xFF8E7E72);
  static const Color line = Color(0xFFE8DDD0);
  static const Color line2 = Color(0xFFD9CCBC);
  static const Color surface = Color(0xFFFFFFFF);
  static const Color surface2 = Color(0xFFFFF8F0);
  static const Color mint = Color(0xFF6BC7A1);
  static const Color rose = Color(0xFFF2A6A6);
  static const Color sky = Color(0xFF9DC8E5);
  static const Color violet = Color(0xFFB49DD8);
  static const Color brown = Color(0xFF8B5A3C);

  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      fontFamily: 'Noto Sans SC',
      scaffoldBackgroundColor: bg,
      colorScheme: ColorScheme.fromSeed(
        seedColor: primary,
        primary: primary,
        secondary: accent,
        surface: surface,
        background: bg,
        brightness: Brightness.light,
      ),
      appBarTheme: AppBarTheme(
        backgroundColor: surface,
        foregroundColor: ink,
        elevation: 0,
        centerTitle: true,
        titleTextStyle: TextStyle(
          color: ink,
          fontSize: 17,
          fontWeight: FontWeight.w700,
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: primary,
          foregroundColor: Colors.white,
          elevation: 6,
          shadowColor: primary.withOpacity(0.32),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(24),
          ),
          minimumSize: Size(double.infinity, 48),
          textStyle: TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: ink,
          side: BorderSide(color: line),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(24),
          ),
          minimumSize: Size(double.infinity, 48),
          textStyle: TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: surface,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: BorderSide(color: line),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: BorderSide(color: line),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(14),
          borderSide: BorderSide(color: primary, width: 2),
        ),
        contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: surface,
        selectedColor: ink,
        labelStyle: TextStyle(fontSize: 12, color: ink2),
        secondaryLabelStyle: TextStyle(fontSize: 12, color: Colors.white),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(999),
          side: BorderSide(color: line),
        ),
        padding: EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      ),
      bottomNavigationBarTheme: BottomNavigationBarThemeData(
        backgroundColor: Colors.white.withOpacity(0.92),
        selectedItemColor: primaryInk,
        unselectedItemColor: ink3,
        type: BottomNavigationBarType.fixed,
        elevation: 0,
      ),
    );
  }
}