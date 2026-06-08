/** 用户管理 API（基于 02-用户管理模块详细设计） */

import { delay, M, pageQuery } from './_mock'
import type { PageQuery, User } from '@/types'

// 用户列表
export function listUsers(query: PageQuery & {
  register_method?: string
  is_member?: string
  status?: string
  date_range?: string[]
}) {
  return delay().then(() => {
    return pageQuery<User>(M.mockUsers, query, (u) => {
      if (query.register_method && query.register_method !== 'all' && u.register_method !== query.register_method) return false
      if (query.is_member === 'yes' && !u.is_member) return false
      if (query.is_member === 'no' && u.is_member) return false
      if (query.status && query.status !== 'all' && u.status !== query.status) return false
      return true
    })
  })
}

// 用户详情
export function getUser(id: string) {
  return delay().then(() => M.mockUsers.find(u => u.id === id))
}

// 禁用/解禁
export function disableUser(id: string, reason: string) {
  return delay().then(() => ({ success: true }))
}
export function enableUser(id: string) {
  return delay().then(() => ({ success: true }))
}
// 禁言
export function muteUser(id: string, days: number, reason: string) {
  return delay().then(() => ({ success: true }))
}
// 重置密码
export function resetPassword(id: string) {
  return delay().then(() => ({ success: true, newPassword: 'pindou' + Math.floor(Math.random() * 1000) }))
}

// 会员列表
export function listMembers(query: PageQuery & {
  level?: string
  expire?: string
  pay_channel?: string
}) {
  return delay().then(() => {
    return pageQuery(M.mockMembers, query, (m: any) => {
      if (query.level && query.level !== 'all' && m.level !== query.level) return false
      return true
    })
  })
}

// 手动开通会员
export function openMember(user_id: string, level: string, days: number) {
  return delay().then(() => ({ success: true }))
}
