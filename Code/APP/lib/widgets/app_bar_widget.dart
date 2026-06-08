import 'package:flutter/material.dart';
import '../config/app_theme.dart';

class PindouAppBar extends StatelessWidget implements PreferredSizeWidget {
  final String? title;
  final Widget? leading;
  final List<Widget>? actions;
  final bool solid;
  final Color? backgroundColor;
  final Color? foregroundColor;

  const PindouAppBar({
    super.key,
    this.title,
    this.leading,
    this.actions,
    this.solid = false,
    this.backgroundColor,
    this.foregroundColor,
  });

  @override
  Widget build(BuildContext context) {
    return AppBar(
      title: title != null ? Text(title!) : null,
      leading: leading,
      actions: actions,
      backgroundColor: backgroundColor ?? (solid ? AppTheme.surface : Colors.transparent),
      foregroundColor: foregroundColor ?? AppTheme.ink,
      elevation: solid ? 0.5 : 0,
      scrolledUnderElevation: 0.5,
      centerTitle: true,
      titleTextStyle: TextStyle(
        color: foregroundColor ?? AppTheme.ink,
        fontSize: 17,
        fontWeight: FontWeight.w700,
      ),
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(56);
}

class BackButton extends StatelessWidget {
  final VoidCallback? onTap;
  final Color? color;

  const BackButton({super.key, this.onTap, this.color});

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: Icon(Icons.chevron_left, color: color ?? AppTheme.ink2, size: 28),
      onPressed: onTap ?? () => Navigator.of(context).pop(),
      style: IconButton.styleFrom(
        backgroundColor: (color != null) ? Colors.black.withOpacity(0.3) : null,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }
}