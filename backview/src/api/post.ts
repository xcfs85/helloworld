/** 内容管理 API（基于 03-内容管理模块详细设计） */

import { delay, M, pageQuery } from './_mock'
import type { PageQuery, Post, Comment, SensitiveWord, Report } from '@/types'

// 帖子审核列表
export function listPostReviews(query: PageQuery & {
  status?: string
  type?: string
  risk_level?: string
}) {
  return delay().then(() => {
    return pageQuery<Post>(M.mockPosts, query, (p) => {
      if (query.status && query.status !== 'all' && p.status !== query.status) return false
      if (query.type && query.type !== 'all' && p.type !== query.type) return false
      if (query.risk_level && query.risk_level !== 'all' && p.risk_level !== query.risk_level) return false
      return true
    })
  })
}

export function getPost(id: string) {
  return delay().then(() => M.mockPosts.find(p => p.id === id))
}

export function approvePost(id: string, note?: string) {
  return delay().then(() => ({ success: true }))
}

export function rejectPost(id: string, reason: string, note?: string) {
  return delay().then(() => ({ success: true }))
}

export function batchApprovePosts(ids: string[]) {
  return delay().then(() => ({ success: true, count: ids.length }))
}

export function batchRejectPosts(ids: string[], reason: string) {
  return delay().then(() => ({ success: true, count: ids.length }))
}

// 评论审核
export function listCommentReviews(query: PageQuery & { status?: string }) {
  return delay().then(() => {
    return pageQuery<Comment>(M.mockComments, query, (c) => {
      if (query.status && query.status !== 'all' && c.status !== query.status) return false
      return true
    })
  })
}

export function approveComment(id: string) { return delay().then(() => ({ success: true })) }
export function hideComment(id: string) { return delay().then(() => ({ success: true })) }
export function deleteComment(id: string) { return delay().then(() => ({ success: true })) }

// 敏感词
export function listSensitiveWords(query: PageQuery & { level?: string; type?: string }) {
  return delay().then(() => {
    return pageQuery<SensitiveWord>(M.mockSensitiveWords, query, (w) => {
      if (query.level && query.level !== 'all' && w.level !== query.level) return false
      if (query.type && query.type !== 'all' && w.type !== query.type) return false
      return true
    })
  })
}
export function addSensitiveWord(data: Partial<SensitiveWord>) { return delay().then(() => ({ success: true })) }
export function updateSensitiveWord(id: string, data: Partial<SensitiveWord>) { return delay().then(() => ({ success: true })) }
export function deleteSensitiveWord(id: string) { return delay().then(() => ({ success: true })) }

// 举报
export function listReports(query: PageQuery & { status?: string; type?: string }) {
  return delay().then(() => {
    return pageQuery<Report>(M.mockReports, query, (r) => {
      if (query.status && query.status !== 'all' && r.status !== query.status) return false
      if (query.type && query.type !== 'all' && r.type !== query.type) return false
      return true
    })
  })
}
export function handleReport(id: string, action: string) { return delay().then(() => ({ success: true })) }
