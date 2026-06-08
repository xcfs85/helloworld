/** 登录与权限 API（基于 01-登录与权限模块详细设计）
 * 真实对接时使用 request.ts 的 axios 实例 */

import request from '@/utils/request'
import { delay, M } from './_mock'

// 登录
export function login(data: { username: string; password: string; captcha?: string }) {
  // 真实接口: return request.post('/auth/login', data)
  return delay().then(() => {
    if (data.username === 'admin' || data.username === 'admin@pindou') {
      return {
        token: 'mock-jwt-token-' + Date.now(),
        user: M.mockAdmins[0]
      }
    }
    throw new Error('账号或密码错误')
  })
}

// 登出
export function logout() {
  return delay().then(() => ({ success: true }))
}

// 获取当前用户
export function getCurrentUser() {
  return delay().then(() => M.mockAdmins[0])
}

// 账号管理 - 列表
export function listAdmins(query: any) {
  return delay().then(() => {
    const list = [...M.mockAdmins]
    return { list, total: list.length }
  })
}

// 角色管理 - 列表
export function listRoles(query: any) {
  return delay().then(() => {
    return { list: M.mockRoles, total: M.mockRoles.length }
  })
}

// 操作日志 - 列表
export function listLogs(query: any) {
  return delay().then(() => {
    let list = [...M.mockLogs]
    if (query.operation) {
      list = list.filter((l: any) => l.operation === query.operation)
    }
    return { list, total: list.length }
  })
}
