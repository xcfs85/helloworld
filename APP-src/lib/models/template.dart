class Template {
  final String id;
  final String name;
  final String category;
  final List<String> tags;
  final String coverUrl;
  final List<String> previewUrls;
  final TemplateBeadParams beadParams;
  final TemplateColorSummary colorSummary;
  final String sourceType;
  final String? creatorName;
  final int viewCount;
  final int likeCount;
  final int useCount;
  final String status;
  final String createdAt;

  Template({
    required this.id,
    required this.name,
    required this.category,
    required this.tags,
    required this.coverUrl,
    required this.previewUrls,
    required this.beadParams,
    required this.colorSummary,
    required this.sourceType,
    this.creatorName,
    this.viewCount = 0,
    this.likeCount = 0,
    this.useCount = 0,
    required this.status,
    required this.createdAt,
  });

  factory Template.fromJson(Map<String, dynamic> json) {
    return Template(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      category: json['category'] ?? '',
      tags: (json['tags'] as List<dynamic>?)
          ?.map((t) => t.toString())
          .toList() ?? [],
      coverUrl: json['cover_url'] ?? '',
      previewUrls: (json['preview_urls'] as List<dynamic>?)
          ?.map((u) => u.toString())
          .toList() ?? [],
      beadParams: TemplateBeadParams.fromJson(json['bead_params'] ?? {}),
      colorSummary: TemplateColorSummary.fromJson(json['color_summary'] ?? {}),
      sourceType: json['source_type'] ?? 'official',
      creatorName: json['creator_name'],
      viewCount: json['view_count'] ?? 0,
      likeCount: json['like_count'] ?? 0,
      useCount: json['use_count'] ?? 0,
      status: json['status'] ?? 'approved',
      createdAt: json['created_at'] ?? '',
    );
  }
}

class TemplateBeadParams {
  final String boardSize;
  final int colorCount;
  final int beadCount;
  final String difficulty;
  final String style;

  TemplateBeadParams({
    required this.boardSize,
    required this.colorCount,
    required this.beadCount,
    required this.difficulty,
    required this.style,
  });

  factory TemplateBeadParams.fromJson(Map<String, dynamic> json) {
    return TemplateBeadParams(
      boardSize: json['board_size'] ?? '50x50',
      colorCount: json['color_count'] ?? 0,
      beadCount: json['bead_count'] ?? 0,
      difficulty: json['difficulty'] ?? 'simple',
      style: json['style'] ?? 'realistic',
    );
  }
}

class TemplateColorSummary {
  final int totalColors;
  final int totalBeads;
  final List<ColorPreview> mainColors;

  TemplateColorSummary({
    required this.totalColors,
    required this.totalBeads,
    required this.mainColors,
  });

  factory TemplateColorSummary.fromJson(Map<String, dynamic> json) {
    return TemplateColorSummary(
      totalColors: json['total_colors'] ?? 0,
      totalBeads: json['total_beads'] ?? 0,
      mainColors: (json['main_colors'] as List<dynamic>?)
          ?.map((c) => ColorPreview.fromJson(c))
          .toList() ?? [],
    );
  }
}

class ColorPreview {
  final String code;
  final String name;
  final String rgb;

  ColorPreview({
    required this.code,
    required this.name,
    required this.rgb,
  });

  factory ColorPreview.fromJson(Map<String, dynamic> json) {
    return ColorPreview(
      code: json['code'] ?? '',
      name: json['name'] ?? '',
      rgb: json['rgb'] ?? '#000000',
    );
  }
}