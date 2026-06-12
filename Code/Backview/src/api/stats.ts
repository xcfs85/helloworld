/** 数据统计 API（对接后台 /api/admin/v1/statistics 接口） */

import service from '@/utils/request'

/** 每日统计数据（与后端 DailyStatsDto 对应，已在响应拦截器中转为 snake_case） */
export interface DailyStats {
  stat_date: string
  dau: number
  new_user_count: number
  retention_1d?: number
  retention_7d?: number
  retention_30d?: number
  generation_count: number
  avg_bead_count: number
  avg_color_count: number
  export_count: number
  post_count: number
  work_count: number
  tutorial_count: number
  comment_count: number
  like_count: number
  share_count: number
  favorite_count: number
  member_order_count: number
  member_revenue: number
}

/** 概览统计响应（与后端 OverviewDto 对应） */
export interface OverviewData {
  total_users: number
  active_users: number
  new_users: number
  total_diagrams: number
  total_posts: number
  total_templates: number
  member_count: number
  total_revenue: number
  daily_stats: DailyStats[]
}

// 概览统计
export function getOverview(params?: { start?: string; end?: string }) {
  return service.get<OverviewData>('/statistics/overview', { params })
}

// 趋势数据（返回指定时间段内的每日统计）
export function getTrends(params?: { start?: string; end?: string }) {
  return service.get<DailyStats[]>('/statistics/trends', { params })
}

// 每日统计
export function getDailyStats(date?: string) {
  return service.get<DailyStats>('/statistics/daily', { params: { date } })
}

// 范围统计
export function getRangeStats(start: string, end: string) {
  return service.get<DailyStats[]>('/statistics/range', { params: { start, end } })
}

// 导出报表
export function exportStats(params?: { start?: string; end?: string }) {
  return service.get('/statistics/export', {
    params,
    responseType: 'blob' as any
  })
}
