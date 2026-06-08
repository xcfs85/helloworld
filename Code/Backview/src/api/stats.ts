/** 数据统计 API（基于 06-数据统计模块详细设计） */

import { delay, M } from './_mock'

// 核心指标
export function getOverview() {
  return delay().then(() => M.mockOverview)
}

// 趋势数据
export function getTrend(metric: string, days = 14) {
  return delay().then(() => {
    const dates = Array.from({ length: days }, (_, i) => {
      const d = new Date()
      d.setDate(d.getDate() - (days - 1 - i))
      return `${(d.getMonth() + 1).toString().padStart(2, '0')}-${d.getDate().toString().padStart(2, '0')}`
    })
    const baseMap: Record<string, number> = {
      dau: 11000, generation: 5000, posts: 1000, retention: 38, users: 1000, interactions: 20000
    }
    const base = baseMap[metric] || 1000
    const values = Array.from({ length: days }, () => Math.floor(base * (0.7 + Math.random() * 0.6)))
    return { dates, values }
  })
}

// 用户分析
export function getUserStats() {
  return delay().then(() => ({
    growth: Array.from({ length: 30 }, (_, i) => 800 + Math.random() * 600),
    gender: { male: 23, female: 71, unknown: 6 },
    age: [
      { range: '<18', value: 8 },
      { range: '18-24', value: 32 },
      { range: '25-30', value: 28 },
      { range: '31-40', value: 22 },
      { range: '>40', value: 10 }
    ],
    city: [
      { name: '北京', value: 18 },
      { name: '上海', value: 16 },
      { name: '广州', value: 12 },
      { name: '深圳', value: 11 },
      { name: '成都', value: 8 },
      { name: '其他', value: 35 }
    ],
    register_method: { wechat: 45, phone: 35, apple: 15, guest: 5 }
  }))
}

// 创作分析
export function getCreationStats() {
  return delay().then(() => ({
    generation: Array.from({ length: 14 }, () => 4000 + Math.random() * 2500),
    color_distribution: [
      { range: '1-20', value: 35 },
      { range: '21-30', value: 28 },
      { range: '31-50', value: 22 },
      { range: '51-80', value: 12 },
      { range: '>80', value: 3 }
    ],
    difficulty: { beginner: 45, intermediate: 38, advanced: 17 },
    style: { 写实: 32, 卡通: 45, 写意: 15, 抽象: 8 },
    export_rate: 67.8
  }))
}

// 社区分析
export function getCommunityStats() {
  return delay().then(() => ({
    post_types: { work: 56, tutorial: 22, question: 22 },
    interactions: { like: 45, comment: 25, favorite: 20, share: 10 },
    feed: { recommend: 65, follow: 35 },
    top_topics: [
      { name: '圣诞拼豆挑战', post_count: 1234, user_count: 567 },
      { name: '拼豆作品展示', post_count: 2345, user_count: 1234 },
      { name: '新手第一次', post_count: 890, user_count: 432 },
      { name: '色号讨论', post_count: 567, user_count: 234 }
    ]
  }))
}
