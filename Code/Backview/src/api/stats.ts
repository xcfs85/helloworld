/** 数据统计 API（基于 06-数据统计模块详细设计） */

import service from '@/utils/request'

// 核心指标
export function getOverview(params?: { date_type?: string; start_date?: string; end_date?: string }) {
  return service.get('/statistics/overview', { params })
}

// 趋势数据
export function getTrend(metric: string, days = 14) {
  const endDate = new Date()
  const startDate = new Date()
  startDate.setDate(startDate.getDate() - days)
  return service.get('/statistics/trends', {
    params: {
      start: startDate.toISOString().split('T')[0],
      end: endDate.toISOString().split('T')[0]
    }
  })
}

// 用户分析
export function getUserStats(params?: { date_type?: string; start_date?: string; end_date?: string }) {
  return service.get('/statistics/range', { params })
}

// 创作分析 - 复用趋势数据
export function getCreationStats(params?: { date_type?: string; start_date?: string; end_date?: string }) {
  return service.get('/statistics/trends', { params })
}

// 社区分析 - 复用每日统计
export function getCommunityStats(params?: { date_type?: string }) {
  const date = new Date().toISOString().split('T')[0]
  return service.get('/statistics/daily', { params: { date } })
}

// 导出报表
export function exportStats(params?: { date_type?: string; start_date?: string; end_date?: string }) {
  return service.get('/statistics/export', {
    params,
    responseType: 'blob' as any
  })
}