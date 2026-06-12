/**
 *@Author: 西成峰
 *@CreateTime: 2026-06-09
 *@Description: 用户管理 API（对接后端真实接口）
 */

import service from '@/utils/request'
import type { PageResult } from '@/types'

// 用户列表查询参数
export interface UserListParams {
  page?: number
  page_size?: number
  keyword?: string
  platform?: string    // 注册方式：wechat/phone/apple
  is_member?: boolean // 是否会员
  status?: string     // 状态：active/disabled
  register_start_time?: string
  register_end_time?: string
}

// 用户列表项（与后端 UserListDto 对应）
export interface UserListItem {
  id: string
  nickname: string
  avatar?: string
  phone?: string
  gender: string
  city?: string
  register_method?: 'wechat' | 'phone' | 'apple' | 'guest'
  register_time?: string
  is_member: boolean
  member_level?: 'VIP1' | 'VIP2' | 'VIP3' | 'SVIP'
  member_expire_time?: string
  status: string
  create_time: string
  last_login_time?: string
  diagram_count?: number
  post_count: number
  follower_count?: number
}

// 用户列表响应
export type UserListResponse = PageResult<UserListItem>

// 用户列表
export function listUsers(query: UserListParams) {
  // 转换前端参数到后端参数
  const params: Record<string, any> = {
    page: query.page || 1,
    size: query.page_size || 20
  }

  if (query.keyword) params.keyword = query.keyword
  if (query.platform && query.platform !== 'all') params.platform = query.platform
  if (query.is_member !== undefined && query.is_member !== null) {
    params.isMember = query.is_member
  }
  if (query.status && query.status !== 'all') {
    // 前端 normal → 后端 active，muted 需要特殊处理
    params.status = query.status === 'normal' ? 'active' : query.status
  }
  if (query.register_start_time) params.registerStartTime = query.register_start_time
  if (query.register_end_time) params.registerEndTime = query.register_end_time

  return service.get<UserListResponse>('/user/list', { params })
}

// 用户详情
export function getUser(id: string) {
  return service.get<UserListItem>(`/user/${id}`)
}

// 重置用户密码
export interface ResetPasswordResult {
  newPassword: string
}

export function resetPassword(id: string) {
  return service.post<ResetPasswordResult>(`/user/${id}/reset-password`)
}

// 禁用用户
export function disableUser(id: string, reason: string) {
  return service.post(`/user/${id}/disable`, { reason })
}

// 启用用户
export function enableUser(id: string) {
  return service.post(`/user/${id}/enable`)
}

// 禁言用户
export interface MuteUserParams {
  days: number
  reason: string
}

export function muteUser(id: string, params: MuteUserParams) {
  return service.post(`/user/${id}/mute`, params)
}

// 用户帖子列表
export function getUserPosts(userId: string, query: { page?: number; page_size?: number }) {
  return service.get<PageResult<any>>(`/user/${userId}/posts`, { params: query })
}

/* ===== 会员相关 ===== */

// 会员列表查询
export interface MemberListParams {
  page?: number
  page_size?: number
  keyword?: string
  /** 会员等级:VIP1/VIP2/VIP3/SVIP */
  level?: string
  /** 到期状态:7d-7天内到期 / 30d-30天内到期 / expired-已过期 / long-长期有效 */
  expire?: string
  /** 支付渠道:wechat/alipay/appstore/backend */
  pay_channel?: string
}

// 会员列表
export function listMembers(query: MemberListParams) {
  const params: Record<string, any> = {
    page: query.page || 1,
    size: query.page_size || 20
  }
  if (query.keyword) params.keyword = query.keyword
  if (query.level && query.level !== 'all') params.level = query.level
  if (query.expire && query.expire !== 'all') params.expire = query.expire
  if (query.pay_channel && query.pay_channel !== 'all') params.pay_channel = query.pay_channel
  return service.get<UserListResponse>('/member/list', { params })
}

// 开通会员请求
export interface OpenMemberRequest {
  user_id: string
  level: string
  days: number
}

// 开通会员
export function openMember(data: OpenMemberRequest) {
  return service.post('/member/open', data)
}

// 会员统计
export interface MemberStats {
  total: number
  level_counts: { level: string; count: number }[]
  channel_counts: { channel: string; count: number }[]
  expiring_soon_count: number
  expiring_30d_count: number
  long_term_count: number
  expired_count: number
}

export function getMemberStats() {
  return service.get<MemberStats>('/member/stats')
}

// 会员等级分布（专门用于侧边栏分类计数）
export interface MemberLevelStats {
  total: number
  level_counts: { level: string; count: number }[]
}

export function getMemberLevelStats() {
  return service.get<MemberLevelStats>('/member/level-stats')
}

// 用户统计（用于侧边栏分类计数）
export interface UserStats {
  total: number
  active_count: number
  muted_count: number
  disabled_count: number
  member_count: number
  non_member_count: number
  platform_counts: { platform: string; count: number }[]
}

export function getUserStats() {
  return service.get<UserStats>('/user/stats')
}

// 创建用户
export interface CreateUserParams {
  nickname: string
  phone?: string
  gender?: string
  city?: string
}

export function createUser(data: CreateUserParams) {
  return service.post<UserListItem>('/user/create', data)
}

// 批量导入用户
export interface ImportUserResult {
  success_count: number
  fail_count: number
  fail_details: { row: number; reason: string }[]
}

export function importUsers(users: CreateUserParams[]) {
  return service.post<ImportUserResult>('/user/import', users)
}

// 导出用户CSV
export function exportUsers(query: UserListParams) {
  const params: Record<string, any> = {
    page: 1,
    size: 10000
  }
  if (query.keyword) params.keyword = query.keyword
  if (query.platform && query.platform !== 'all') params.platform = query.platform
  if (query.is_member !== undefined && query.is_member !== null) params.isMember = query.is_member
  if (query.status && query.status !== 'all') params.status = query.status === 'normal' ? 'active' : query.status
  if (query.register_start_time) params.registerStartTime = query.register_start_time
  if (query.register_end_time) params.registerEndTime = query.register_end_time
  return service.get<UserListResponse>('/user/export', { params })
}