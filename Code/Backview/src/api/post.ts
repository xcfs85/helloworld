/** 内容管理 API
 * 对接真实后台接口 */

import service from '@/utils/request'
import type { PageQuery } from '@/types'

// ===== 帖子审核 =====

// 待审核帖子列表
export function listPostReviews(query: PageQuery & {
  type?: string
  start_time?: string
  end_time?: string
}) {
  return service.get<{ list: PostReviewItem[]; total: number; page: number; size: number }>('/content/posts/pending', { params: query })
}

// 后端返回的帖子列表项结构
export interface PostReviewItem {
  id: string
  type: string
  title: string
  content: string
  desc?: string
  cover?: string
  media: MediaItem[]
  diagram_id?: string
  like_count: number
  comment_count: number
  favorite_count: number
  view_count: number
  review_status: string
  status: string
  risk_level?: 'none' | 'low' | 'mid' | 'high'
  risk_tags?: string[]
  create_time: string
  publish_time: string
  ip?: string
  device?: string
  topic_ids?: string[]
  topics?: string[]
  bead_params?: string
  author: {
    id: string
    nickname: string
    avatar?: string
    is_member?: boolean
  }
  is_liked: boolean
  is_favorited: boolean
}

export interface MediaItem {
  type: 'image' | 'video'
  url: string
  width?: number
  height?: number
}

// 帖子详情
export function getPost(id: string) {
  return service.get<PostDetailItem>('/content/posts/' + id)
}

export interface PostDetailItem extends PostReviewItem {
  topic_ids?: string[]
  bead_params?: string
}

// 审核通过帖子
export function approvePost(id: string) {
  return service.post('/content/posts/' + id + '/approve')
}

// 驳回帖子
export function rejectPost(id: string, reason?: string) {
  return service.post('/content/posts/' + id + '/reject', { reason })
}

// 批量审核通过
export function batchApprovePosts(ids: string[]) {
  return service.post('/content/posts/batch-approve', { post_ids: ids })
}

// 批量驳回
export function batchRejectPosts(ids: string[], reason?: string) {
  return service.post('/content/posts/batch-reject', { post_ids: ids, reason })
}

// ===== 评论管理 =====

// 评论列表
export function listCommentReviews(query: PageQuery & {
  post_id?: string
  status?: string
}) {
  return service.get<{ list: CommentItem[]; total: number }>('/content/comments', { params: query })
}

export interface CommentItem {
  id: string
  post_id: string
  post_title: string
  user: {
    id: string
    nickname: string
    avatar?: string
  }
  content: string
  ip: string
  status: string
  create_time: string
}

// 隐藏评论
export function hideComment(id: string) {
  return service.post('/content/comments/' + id + '/hide')
}

// 审核通过评论（与隐藏相同逻辑）
export function approveComment(id: string) {
  return service.post('/content/comments/' + id + '/hide')
}

// 删除评论
export function deleteComment(id: string) {
  return service.delete('/content/comments/' + id)
}

// ===== 敏感词管理 =====

// 敏感词列表
export function listSensitiveWords(query: { page?: number; page_size?: number; level?: number; type?: string; keyword?: string } = {}) {
  return service.get<{ list: SensitiveWordItem[]; total: number }>('/content/sensitive-words', { params: query })
}

export interface SensitiveWordItem {
  id: string
  word: string
  level: number
  type: string
  replace_word?: string
  status: number
  create_time: string
}

// 添加敏感词
export function addSensitiveWord(data: { word: string; level: number; type: string; replace_word?: string }) {
  return service.post<{ id: string }>('/content/sensitive-words', data)
}

// 更新敏感词
export function updateSensitiveWord(id: string, data: { word?: string; level?: number; type?: string; replace_word?: string }) {
  return service.put('/content/sensitive-words/' + id, data)
}

// 删除敏感词
export function deleteSensitiveWord(id: string) {
  return service.delete('/content/sensitive-words/' + id)
}

// ===== 举报管理 =====

// 举报列表
export function listReports(query: PageQuery & {
  status?: string
  type?: string
}) {
  return service.get<{ list: ReportItem[]; total: number }>('/content/reports', { params: query })
}

export interface ReportItem {
  id: string
  report_id: string
  reporter: {
    id: string
    nickname: string
  }
  target_type: string
  target_id: string
  target_content?: string
  reason: string
  images?: string[]
  status: string
  create_time: string
}

// 处理举报
export function handleReport(id: string, action: string, result?: string) {
  return service.post('/content/reports/' + id + '/handle', { action, result })
}