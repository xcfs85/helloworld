/** 通用类型 */

export interface PageQuery {
  page?: number
  page_size?: number
  keyword?: string
  [key: string]: any
}

export interface PageResult<T = any> {
  list: T[]
  total: number
  page: number
  page_size: number
}

/* ===== 用户 ===== */
export interface User {
  id: string
  nickname: string
  avatar?: string
  phone: string
  register_method: 'wechat' | 'phone' | 'apple' | 'guest'
  register_time: string
  last_login_time: string
  is_member: boolean
  member_level?: 'VIP1' | 'VIP2' | 'VIP3' | 'SVIP'
  member_expire_time?: string
  status: 'normal' | 'muted' | 'disabled'
  mute_expire?: string
  post_count: number
  follower_count: number
  diagram_count?: number
  gender?: 'male' | 'female' | 'unknown'
  city?: string
  create_time?: string
  email?: string
}

/* ===== 帖子 ===== */
export interface PostAuthor {
  id: string
  nickname: string
  avatar?: string
}

export interface Post {
  id: string
  type: 'work' | 'tutorial' | 'question'
  title: string
  cover: string
  images: string[]
  desc: string
  author: PostAuthor
  create_time: string
  publish_time?: string
  content?: string
  media?: { type: 'image' | 'video'; url: string; width?: number; height?: number }[]
  ip?: string
  device?: string
  risk_level?: 'none' | 'low' | 'mid' | 'high'
  risk_tags?: string[]
  topics: string[]
  topic_ids?: string[]
  diagram_id?: string
  status: 'pending' | 'approved' | 'rejected' | 'offline' | 'published'
  review_status?: 'pending' | 'approved' | 'rejected' | 'offline'
  review_note?: string
  reject_reason?: string
  is_favorited?: boolean
  view_count?: number
  like_count?: number
  comment_count?: number
  bead_params?: string
}

/* ===== 评论 ===== */
export interface Comment {
  id: string
  post_id: string
  post_title: string
  user_id: string
  user_nickname: string
  content: string
  create_time: string
  status: 'pending' | 'approved' | 'hidden' | 'deleted'
  risk_level: 'none' | 'low' | 'mid' | 'high'
}

/* ===== 模板 ===== */
export interface Template {
  id: string
  template_id?: string
  name: string
  cover?: string
  cover_url?: string
  previews?: string[]
  preview_urls?: string[]
  description?: string
  desc?: string
  category: string
  category_id?: string
  category_name: string
  tags: string[]
  source?: 'official' | 'creator'
  source_type?: 'official' | 'creator'
  creator_id?: string
  creator_name?: string
  creator?: { id: string; nickname: string }
  board_size: string
  color_count?: number
  total_colors?: number
  total_beads: number
  bead_count?: number
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  difficulty_name?: string
  style?: string
  duration?: string
  status: 'draft' | 'pending' | 'approved' | 'rejected' | 'offline'
  submit_time?: string
  publish_time?: string
  create_time?: string
  is_featured?: boolean
  use_count: number
}

/* ===== 敏感词 ===== */
export interface SensitiveWord {
  id: string
  word: string
  level: 'severe' | 'medium' | 'minor'
  type: 'political' | 'porn' | 'violence' | 'ads' | 'copyright' | 'other'
  replacement: string
  hit_count: number
  create_time: string
}

/* ===== 举报 ===== */
export interface Report {
  id: string
  type: 'spam' | 'violation' | 'infringement' | 'fake' | 'attack' | 'other'
  target_type: 'post' | 'comment' | 'user'
  target_id: string
  target_summary: string
  target_content?: string
  images?: string[]
  reporter: {
    id: string
    nickname: string
  }
  reason: string
  create_time: string
  status: 'pending' | 'ignored' | 'warned' | 'muted' | 'banned'
  handler?: string
  handle_time?: string
}

/* ===== Banner ===== */
export interface Banner {
  id: string
  title: string
  image_url: string
  link_type: 'url' | 'post' | 'template' | 'special' | 'activity'
  link_value: string
  position: string
  start_time: string
  end_time: string
  sort: number
  status: 'active' | 'inactive'
  create_time: string
  update_time?: string
}

/* ===== 话题 ===== */
export interface Topic {
  id: string
  topic_id: string
  name: string
  description: string
  cover_url?: string
  is_official: number
  status: 'active' | 'closed'
  post_count: number
  participant_count: number
  create_time: string
}

/* ===== 专题 ===== */
export interface Special {
  id: string
  name: string
  description: string
  cover_url: string
  template_ids: string[]
  start_time: string
  end_time: string
  /** 状态: 0-下架 1-上架 */
  status: number
  create_time: string
}

/* ===== 推送 ===== */
export interface Push {
  id: string
  title: string
  content: string
  audience: 'all' | 'tag' | 'user'
  audience_label: string
  channels: string[]
  scheduled_time: string
  send_time?: string
  status: 'draft' | 'scheduled' | 'sending' | 'sent' | 'failed'
  success_count: number
  fail_count: number
  click_count: number
  creator: string
}

/* ===== 角色 ===== */
export interface Role {
  id: string
  name: string
  code?: string
  description: string
  permissions: string[]
  user_count: number
  create_time: string
}

export interface RoleItem {
  id: string
  name: string
  code?: string
  description?: string
  permissions: string[]
  create_time: string
}

/* ===== 管理员账号 ===== */
export interface AdminAccount {
  id: string
  username: string
  nickname: string
  email?: string
  role_id: string | number
  role_name: string
  status: string | number
  last_login_time?: string
  last_login_ip?: string
  create_time: string
}

/* ===== 操作日志 ===== */
export interface OperationLog {
  id: string
  user_id: string
  username: string
  operation: string
  content: string
  ip: string
  params: string
  create_time: string
}

/* ===== 会员 ===== */
// 会员列表项（与后端 UserListDto 对应）
export interface Member {
  id: string
  user_id: string
  user_nickname: string
  user_avatar?: string
  level: 'VIP1' | 'VIP2' | 'VIP3' | 'SVIP'
  expire_time: string
  auto_renew: boolean
  total_paid: number
  pay_channel: 'wechat' | 'alipay' | 'appstore' | 'backend'
  create_time: string
  // 兼容字段（后端 UserListDto 字段）
  nickname?: string
  is_member?: boolean
  member_expire_time?: string
  member_level?: string
}

/* ===== 色号 ===== */
export interface ColorChip {
  id: string
  code: string
  name: string
  hex: string
  rgb: string
  lab: string
  status: 'active' | 'inactive'
}

/* ===== 分类 ===== */
export interface Category {
  id: string
  name: string
  code?: string
  icon?: string
  template_count: number
  sort: number
  status: 'visible' | 'hidden' | number
  create_time?: string
}

/* ===== 标签 ===== */
export interface Tag {
  id: string
  name: string
  /** 所属分类: style/theme/difficulty */
  category?: string
  /** 标签类型: style/theme/difficulty */
  type?: string
  use_count: number
  /** 状态: 0-禁用 1-启用 */
  status: number
  create_time: string
  desc?: string
}
