/** 登录与权限 API
 * 真实对接后台接口 */

import service from '@/utils/request'

// 验证码相关类型
export interface CaptchaResponse {
  captcha_key: string
  captcha_image: string
}

// 登录请求
export interface LoginParams {
  username: string
  password: string
  captcha?: string
  captcha_key?: string
}

// 登录响应用户信息
export interface AdminUserInfo {
  id: number
  username: string
  nickname: string
  role_id: number
  role_name: string
  permissions: string[]
  last_login_time?: string
  last_login_ip?: string
}

// 登录响应
export interface LoginResponse {
  token: string
  refresh_token: string
  expire_time: string
  user: AdminUserInfo
}

// 获取验证码
export function getCaptcha() {
  return service.get<CaptchaResponse>('/auth/captcha')
}

// 登录
export function login(data: LoginParams) {
  return service.post<LoginResponse>('/auth/login', data)
}

// 登出
export function logout() {
  return service.post('/auth/logout')
}

// 获取当前用户
export function getCurrentUser() {
  return service.get<AdminUserInfo>('/auth/current')
}

// ===== 账号管理 =====

export interface AdminQuery {
  page?: number
  page_size?: number
  role_id?: number
  status?: number | string
  keyword?: string
}

export interface AdminUserListItem {
  id: number
  username: string
  nickname: string
  role_id: number
  role_name: string
  status: number
  last_login_time?: string
  last_login_ip?: string
  create_time: string
}

// 账号管理 - 列表
export function listAdmins(query: AdminQuery) {
  return service.get<{ list: AdminUserListItem[]; total: number }>('/admin/list', { params: query })
}

// 账号管理 - 新增
export interface CreateAdminParams {
  username: string
  password: string
  nickname: string
  role_id: number
  status?: number
}

export function createAdmin(data: CreateAdminParams) {
  return service.post<number>('/admin', data)
}

// 账号管理 - 编辑
export interface UpdateAdminParams {
  nickname: string
  role_id: number
  status: number
}

export function updateAdmin(id: number, data: UpdateAdminParams) {
  return service.put(`/admin/${id}`, data)
}

// 账号管理 - 修改状态
export function updateAdminStatus(id: number, status: number) {
  return service.post(`/admin/${id}/status`, { status })
}

// 账号管理 - 重置密码
export function resetAdminPassword(id: number, newPassword: string) {
  return service.post(`/admin/${id}/reset-password`, { newPassword })
}

// 账号管理 - 删除
export function deleteAdmin(id: number) {
  return service.delete(`/admin/${id}`)
}

// ===== 角色管理 =====

export interface RoleItem {
  id: number
  name: string
  code: string
  description?: string
  permissions: string[]
  create_time: string
}

// 角色管理 - 列表
export function listRoles(query: { page?: number; page_size?: number } = {}) {
  return service.get<{ list: RoleItem[]; total: number }>('/role/list', { params: query })
}

// 角色管理 - 所有角色
export function getAllRoles() {
  return service.get<RoleItem[]>('/role/all')
}

// 角色管理 - 创建角色
export interface CreateRoleParams {
  name: string
  code: string
  description?: string
  permissions: string[]
}

export function createRole(data: CreateRoleParams) {
  return service.post<number>('/role', data)
}

// 角色管理 - 更新角色
export function updateRole(id: number, data: CreateRoleParams) {
  return service.put(`/role/${id}`, data)
}

// 角色管理 - 删除角色
export function deleteRole(id: number) {
  return service.delete(`/role/${id}`)
}

// ===== 操作日志 =====

export interface LogQuery {
  page?: number
  page_size?: number
  user_id?: number
  operation?: string
  start_time?: string
  end_time?: string
}

export interface OperationLogItem {
  id: number
  user_id: number
  username: string
  nickname?: string
  operation: string
  content?: string
  method?: string
  params?: string
  ip?: string
  create_time: string
}

// 操作日志 - 列表
export function listLogs(query: LogQuery) {
  return service.get<{ list: OperationLogItem[]; total: number }>('/log/list', { params: query })
}
