-- 拼豆数据库初始化脚本
-- PostgreSQL

-- 用户表
CREATE TABLE IF NOT EXISTS users (
    id VARCHAR(36) PRIMARY KEY,
    nickname VARCHAR(50) NOT NULL DEFAULT '',
    avatar VARCHAR(500),
    phone VARCHAR(20),
    union_id VARCHAR(100),
    apple_user_id VARCHAR(100),
    gender VARCHAR(20) NOT NULL DEFAULT 'unknown',
    city VARCHAR(50),
    bio VARCHAR(200),
    is_member BOOLEAN NOT NULL DEFAULT FALSE,
    member_expire_time TIMESTAMP,
    status VARCHAR(20) NOT NULL DEFAULT 'active',
    last_login_time TIMESTAMP,
    last_login_ip VARCHAR(50),
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time TIMESTAMP
);
CREATE INDEX idx_users_phone ON users(phone);
CREATE INDEX idx_users_union_id ON users(union_id);
CREATE INDEX idx_users_status ON users(status);

-- 令牌表
CREATE TABLE IF NOT EXISTS tokens (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    access_token VARCHAR(500) NOT NULL,
    refresh_token VARCHAR(500) NOT NULL,
    device_id VARCHAR(100) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_tokens_user_id ON tokens(user_id);

-- 设备表
CREATE TABLE IF NOT EXISTS devices (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    device_id VARCHAR(100) NOT NULL,
    platform VARCHAR(20) NOT NULL,
    push_token VARCHAR(500),
    app_version VARCHAR(20),
    last_active_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 图纸表
CREATE TABLE IF NOT EXISTS diagrams (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    name VARCHAR(100) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'draft',
    source_image_url VARCHAR(500) NOT NULL,
    preview_url VARCHAR(500),
    preview_no_grid_url VARCHAR(500),
    board_size VARCHAR(20) NOT NULL,
    bead_count INTEGER NOT NULL DEFAULT 0,
    difficulty VARCHAR(20) NOT NULL,
    style VARCHAR(20) NOT NULL,
    total_colors INTEGER NOT NULL DEFAULT 0,
    total_beads INTEGER NOT NULL DEFAULT 0,
    tags TEXT,
    version INTEGER NOT NULL DEFAULT 1,
    source_type VARCHAR(20) NOT NULL DEFAULT 'create',
    template_id VARCHAR(36),
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time TIMESTAMP
);
CREATE INDEX idx_diagrams_user_id ON diagrams(user_id);

-- 色号信息表
CREATE TABLE IF NOT EXISTS color_infos (
    id VARCHAR(36) PRIMARY KEY,
    diagram_id VARCHAR(36) NOT NULL,
    color_index INTEGER NOT NULL,
    color_code VARCHAR(20) NOT NULL,
    color_name VARCHAR(50) NOT NULL,
    rgb VARCHAR(20) NOT NULL,
    bead_count INTEGER NOT NULL,
    percentage DECIMAL(10,2) NOT NULL,
    position VARCHAR(50),
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_color_infos_diagram_id ON color_infos(diagram_id);

-- 任务表
CREATE TABLE IF NOT EXISTS diagram_tasks (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    diagram_id VARCHAR(36),
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    progress INTEGER NOT NULL DEFAULT 0,
    current_stage VARCHAR(50),
    source_image_url VARCHAR(500) NOT NULL,
    params TEXT NOT NULL,
    error_message VARCHAR(500),
    is_sync BOOLEAN NOT NULL DEFAULT FALSE,
    complete_time TIMESTAMP,
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 帖子表
CREATE TABLE IF NOT EXISTS posts (
    id VARCHAR(36) PRIMARY KEY,
    user_id VARCHAR(36) NOT NULL,
    type VARCHAR(20) NOT NULL,
    title VARCHAR(100) NOT NULL,
    content TEXT NOT NULL,
    media TEXT NOT NULL DEFAULT '[]',
    topic_ids TEXT,
    bead_params TEXT,
    diagram_id VARCHAR(36),
    like_count INTEGER NOT NULL DEFAULT 0,
    comment_count INTEGER NOT NULL DEFAULT 0,
    favorite_count INTEGER NOT NULL DEFAULT 0,
    view_count INTEGER NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'active',
    review_status VARCHAR(20) NOT NULL DEFAULT 'pending',
    review_reason VARCHAR(200),
    publish_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_posts_user_id ON posts(user_id);
CREATE INDEX idx_posts_status ON posts(review_status, status);
CREATE INDEX idx_posts_publish_time ON posts(publish_time DESC);

-- 评论表
CREATE TABLE IF NOT EXISTS comments (
    id VARCHAR(36) PRIMARY KEY,
    post_id VARCHAR(36) NOT NULL,
    user_id VARCHAR(36) NOT NULL,
    parent_id VARCHAR(36),
    reply_to_user_id VARCHAR(36),
    content VARCHAR(500) NOT NULL,
    like_count INTEGER NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'active',
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_comments_post_id ON comments(post_id);

-- 模板表
CREATE TABLE IF NOT EXISTS templates (
    id VARCHAR(36) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    category_id VARCHAR(36) NOT NULL,
    tags TEXT,
    cover_url VARCHAR(500) NOT NULL,
    preview_urls TEXT NOT NULL DEFAULT '[]',
    board_size VARCHAR(20) NOT NULL,
    bead_count INTEGER NOT NULL,
    difficulty VARCHAR(20) NOT NULL,
    total_colors INTEGER NOT NULL DEFAULT 0,
    source_type VARCHAR(20) NOT NULL DEFAULT 'official',
    creator_id VARCHAR(36),
    creator_name VARCHAR(50),
    view_count INTEGER NOT NULL DEFAULT 0,
    like_count INTEGER NOT NULL DEFAULT 0,
    use_count INTEGER NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    review_status VARCHAR(20) NOT NULL DEFAULT 'pending',
    review_reason VARCHAR(200),
    is_featured BOOLEAN NOT NULL DEFAULT FALSE,
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time TIMESTAMP
);

-- 管理员
CREATE TABLE IF NOT EXISTS admin_users (
    id BIGSERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    nickname VARCHAR(50),
    role_id BIGINT NOT NULL,
    status INTEGER NOT NULL DEFAULT 1,
    last_login_time TIMESTAMP,
    last_login_ip VARCHAR(50),
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_time TIMESTAMP
);

-- 角色
CREATE TABLE IF NOT EXISTS roles (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255),
    permissions TEXT NOT NULL DEFAULT '[]',
    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 默认角色
INSERT INTO roles (id, name, code, description, permissions) VALUES
(1, '超级管理员', 'super_admin', '拥有全部权限', '["*"]'),
(2, '运营', 'operator', '用户、模板、运营、统计', '["user:view","template:*","operation:*","stats:view"]'),
(3, '审核', 'reviewer', '内容审核、模板审核', '["post:review","comment:review","template:review","report:handle"]'),
(4, '客服', 'customer_service', '内容查看、举报处理', '["post:view","comment:view","report:handle"]')
ON CONFLICT (id) DO NOTHING;

-- 默认超级管理员 密码: admin123 (BCrypt)
INSERT INTO admin_users (username, password, nickname, role_id, status) VALUES
('admin', '$2a$11$8K1p/a0dL1LXMIgZ.oPa7OaFz7iMqWQF5h2Kq.2kP1nGcR7Kxg7g.', '系统管理员', 1, 1)
ON CONFLICT (username) DO NOTHING;
