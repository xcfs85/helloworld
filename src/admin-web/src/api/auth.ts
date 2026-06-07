import { post, get } from './request'

export interface LoginRequest {
  username: string
  password: string
  captcha?: string
  captchaKey?: string
}

export interface AdminLoginResponse {
  token: string
  refreshToken: string
  expireTime: string
  user: AdminUserInfo
}

export interface AdminUserInfo {
  id: number
  username: string
  nickname?: string
  roleId: number
  roleName?: string
  permissions: string[]
  lastLoginTime?: string
  lastLoginIp?: string
}

export const authApi = {
  captcha: () => get<{ captchaKey: string; captchaImage: string }>('/admin/v1/auth/captcha'),
  login: (data: LoginRequest) => post<AdminLoginResponse>('/admin/v1/auth/login', data),
  logout: () => post('/admin/v1/auth/logout'),
  current: () => get<AdminUserInfo>('/admin/v1/auth/current')
}
