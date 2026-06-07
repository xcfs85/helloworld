class Diagram {
  final String id;
  final String userId;
  final String name;
  final String status;
  final String sourceImageUrl;
  final String previewUrl;
  final String previewNoGridUrl;
  final DiagramParams params;
  final ColorSummary colorSummary;
  final int version;
  final String createdAt;
  final String updatedAt;

  Diagram({
    required this.id,
    required this.userId,
    required this.name,
    required this.status,
    required this.sourceImageUrl,
    required this.previewUrl,
    required this.previewNoGridUrl,
    required this.params,
    required this.colorSummary,
    required this.version,
    required this.createdAt,
    required this.updatedAt,
  });

  factory Diagram.fromJson(Map<String, dynamic> json) {
    return Diagram(
      id: json['id'] ?? '',
      userId: json['user_id'] ?? '',
      name: json['name'] ?? '',
      status: json['status'] ?? 'draft',
      sourceImageUrl: json['source_image_url'] ?? '',
      previewUrl: json['preview_url'] ?? '',
      previewNoGridUrl: json['preview_no_grid_url'] ?? '',
      params: DiagramParams.fromJson(json['params'] ?? {}),
      colorSummary: ColorSummary.fromJson(json['color_summary'] ?? {}),
      version: json['version'] ?? 1,
      createdAt: json['create_time'] ?? '',
      updatedAt: json['update_time'] ?? '',
    );
  }
}

class DiagramParams {
  final String boardSize;
  final int beadCount;
  final String difficulty;
  final String style;

  DiagramParams({
    required this.boardSize,
    required this.beadCount,
    required this.difficulty,
    required this.style,
  });

  factory DiagramParams.fromJson(Map<String, dynamic> json) {
    return DiagramParams(
      boardSize: json['board_size'] ?? '50x50',
      beadCount: json['bead_count'] ?? 5000,
      difficulty: json['difficulty'] ?? 'simple',
      style: json['style'] ?? 'realistic',
    );
  }
}

class ColorSummary {
  final int totalColors;
  final int totalBeads;

  ColorSummary({
    required this.totalColors,
    required this.totalBeads,
  });

  factory ColorSummary.fromJson(Map<String, dynamic> json) {
    return ColorSummary(
      totalColors: json['total_colors'] ?? 0,
      totalBeads: json['total_beads'] ?? 0,
    );
  }
}

class ColorInfo {
  final int index;
  final String code;
  final String name;
  final String rgb;
  final int beadCount;
  final double percentage;
  final String position;

  ColorInfo({
    required this.index,
    required this.code,
    required this.name,
    required this.rgb,
    required this.beadCount,
    required this.percentage,
    required this.position,
  });

  factory ColorInfo.fromJson(Map<String, dynamic> json) {
    return ColorInfo(
      index: json['index'] ?? 0,
      code: json['code'] ?? '',
      name: json['name'] ?? '',
      rgb: json['rgb'] ?? '#000000',
      beadCount: json['bead_count'] ?? 0,
      percentage: (json['percentage'] ?? 0).toDouble(),
      position: json['position'] ?? '',
    );
  }
}

class GenerateTask {
  final String taskId;
  final String status;
  final int estimatedTime;
  final bool isSync;

  GenerateTask({
    required this.taskId,
    required this.status,
    required this.estimatedTime,
    required this.isSync,
  });

  factory GenerateTask.fromJson(Map<String, dynamic> json) {
    return GenerateTask(
      taskId: json['task_id'] ?? '',
      status: json['status'] ?? 'pending',
      estimatedTime: json['estimated_time'] ?? 0,
      isSync: json['is_sync'] ?? false,
    );
  }
}

class TaskProgress {
  final String taskId;
  final String status;
  final int progress;
  final String currentStage;
  final List<TaskStage> stages;
  final String? diagramId;
  final String? errorMessage;

  TaskProgress({
    required this.taskId,
    required this.status,
    required this.progress,
    required this.currentStage,
    required this.stages,
    this.diagramId,
    this.errorMessage,
  });

  factory TaskProgress.fromJson(Map<String, dynamic> json) {
    return TaskProgress(
      taskId: json['task_id'] ?? '',
      status: json['status'] ?? 'pending',
      progress: json['progress'] ?? 0,
      currentStage: json['current_stage'] ?? '',
      stages: (json['stages'] as List<dynamic>?)
          ?.map((s) => TaskStage.fromJson(s))
          .toList() ?? [],
      diagramId: json['diagram_id'],
      errorMessage: json['error_message'],
    );
  }
}

class TaskStage {
  final String name;
  final int progress;
  final bool completed;

  TaskStage({
    required this.name,
    required this.progress,
    required this.completed,
  });

  factory TaskStage.fromJson(Map<String, dynamic> json) {
    return TaskStage(
      name: json['name'] ?? '',
      progress: json['progress'] ?? 0,
      completed: json['completed'] ?? false,
    );
  }
}