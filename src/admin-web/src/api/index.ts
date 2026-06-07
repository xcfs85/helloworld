import { get, post, put, del, type PagedResult, type PageRequest } from './request'

// 用户管理
export const userApi = {
  list: (params: any) => get<PagedResult<any>>('/admin/v1/user/list', params),
  detail: (id: string) => get<any>(`/admin/v1/user/${id}`),
  disable: (id: string, reason: string) => post(`/admin/v1/user/${id}/disable`, { reason }),
  enable: (id: string) => post(`/admin/v1/user/${id}/enable`),
  openMember: (data: any) => post('/admin/v1/member/open', data),
  orders: (params: PageRequest) => get<PagedResult<any>>('/admin/v1/order/list', params)
}

// 帖子/评论
export const postApi = {
  pendingList: (params: PageRequest) => get<PagedResult<any>>('/admin/v1/post/pending', params),
  approve: (id: string) => post(`/admin/v1/post/${id}/approve`),
  reject: (id: string, reason: string) => post(`/admin/v1/post/${id}/reject`, { reason }),
  detail: (id: string) => get<any>(`/admin/v1/post/${id}`)
}

export const commentApi = {
  pendingList: (params: PageRequest) => get<PagedResult<any>>('/admin/v1/comment/pending', params),
  approve: (id: string) => post(`/admin/v1/comment/${id}/approve`),
  reject: (id: string) => post(`/admin/v1/comment/${id}/reject`)
}

export const sensitiveApi = {
  list: (params?: any) => get<PagedResult<any>>('/admin/v1/sensitive/list', params),
  add: (data: any) => post<string>('/admin/v1/sensitive', data),
  update: (id: string, data: any) => put(`/admin/v1/sensitive/${id}`, data),
  remove: (id: string) => del(`/admin/v1/sensitive/${id}`)
}

export const reportApi = {
  list: (params: any) => get<PagedResult<any>>('/admin/v1/report/list', params),
  handle: (id: string, data: any) => post(`/admin/v1/report/${id}/handle`, data)
}

// 模板
export const templateApi = {
  list: (params: any) => get<PagedResult<any>>('/admin/v1/template/list', params),
  pending: (params: PageRequest) => get<PagedResult<any>>('/admin/v1/template/pending', params),
  approve: (id: string) => post(`/admin/v1/template/${id}/approve`),
  reject: (id: string, reason: string) => post(`/admin/v1/template/${id}/reject`, { reason }),
  categories: () => get<any[]>('/admin/v1/template/categories')
}

// 运营
export const bannerApi = {
  list: (params?: any) => get<PagedResult<any>>('/admin/v1/banner/list', params),
  create: (data: any) => post<string>('/admin/v1/banner', data),
  update: (id: string, data: any) => put(`/admin/v1/banner/${id}`, data),
  remove: (id: string) => del(`/admin/v1/banner/${id}`)
}

export const pushApi = {
  list: (params?: any) => get<PagedResult<any>>('/admin/v1/push/list', params),
  create: (data: any) => post<string>('/admin/v1/push', data),
  send: (id: string) => post(`/admin/v1/push/${id}/send`)
}

// 数据统计
export const statsApi = {
  overview: (params?: any) => get<any>('/admin/v1/stats/overview', params),
  userStats: (params?: any) => get<any>('/admin/v1/stats/user', params),
  contentStats: (params?: any) => get<any>('/admin/v1/stats/content', params),
  daily: (params?: any) => get<any[]>('/admin/v1/stats/daily', params)
}

// 系统
export const configApi = {
  list: () => get<any[]>('/admin/v1/config/list'),
  update: (key: string, value: any) => post(`/admin/v1/config/${key}`, { value })
}

export const mardApi = {
  list: (params?: any) => get<PagedResult<any>>('/admin/v1/mard/list', params),
  create: (data: any) => post<string>('/admin/v1/mard', data),
  update: (id: string, data: any) => put(`/admin/v1/mard/${id}`, data),
  remove: (id: string) => del(`/admin/v1/mard/${id}`)
}

export const kitApi = {
  list: (params?: any) => get<PagedResult<any>>('/admin/v1/kit/list', params),
  create: (data: any) => post<string>('/admin/v1/kit', data),
  update: (id: string, data: any) => put(`/admin/v1/kit/${id}`, data),
  remove: (id: string) => del(`/admin/v1/kit/${id}`)
}

// 管理员/角色/日志
export const adminApi = {
  list: (params: any) => get<PagedResult<any>>('/admin/v1/admin/list', params),
  create: (data: any) => post<number>('/admin/v1/admin', data),
  update: (id: number, data: any) => put(`/admin/v1/admin/${id}`, data),
  remove: (id: number) => del(`/admin/v1/admin/${id}`),
  resetPassword: (id: number, newPassword: string) => post(`/admin/v1/admin/${id}/reset-password`, { newPassword })
}

export const roleApi = {
  list: (params?: PageRequest) => get<PagedResult<any>>('/admin/v1/role/list', params),
  all: () => get<any[]>('/admin/v1/role/all'),
  create: (data: any) => post<number>('/admin/v1/role', data),
  update: (id: number, data: any) => put(`/admin/v1/role/${id}`, data),
  remove: (id: number) => del(`/admin/v1/role/${id}`)
}

export const operationLogApi = {
  list: (params: any) => get<PagedResult<any>>('/admin/v1/log/list', params),
  clear: (beforeTime?: string) => post('/admin/v1/log/clear', null, { params: { beforeTime } })
}
