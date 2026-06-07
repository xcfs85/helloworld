/** 系统配置 API（基于 07-系统配置模块详细设计） */

import { delay, M } from './_mock'
import type { ColorChip } from '@/types'

// 通用配置
export function getGeneralConfig() {
  return delay().then(() => ({
    app_name: '拼豆',
    app_version: '1.0.0',
    user_agreement: '...',
    privacy_policy: '...',
    customer_service: { email: 'support@pindou.work', phone: '400-xxx-xxxx' },
    copyright: '© 2026 PINDOU',
    sensitive_word_enabled: true,
    comment_enabled: true
  }))
}

export function setGeneralConfig(data: any) { return delay().then(() => ({ success: true })) }

// AI 配置
export function getAIConfig() {
  return delay().then(() => ({
    default_bead_count: 50 * 50,
    default_difficulty: 'beginner',
    free_daily_quota: 3,
    timeout: 30,
    retry_count: 2,
    fallback: { enabled: true, queue_threshold: 100 }
  }))
}

export function setAIConfig(data: any) { return delay().then(() => ({ success: true })) }

// 色板
export function listColors(query: any) {
  return delay().then(() => {
    const list = [...M.mockColors]
    const start = ((query.page || 1) - 1) * (query.page_size || 80)
    return { list: list.slice(start, start + (query.page_size || 80)), total: list.length }
  })
}

export function addColor(data: Partial<ColorChip>) { return delay().then(() => ({ success: true })) }
export function updateColor(id: string, data: Partial<ColorChip>) { return delay().then(() => ({ success: true })) }
export function deleteColor(id: string) { return delay().then(() => ({ success: true })) }

// 套装
export function listKits() {
  return delay().then(() => [
    { id: 'kit_168', name: '168 色套装', color_count: 168, desc: '入门推荐', price: 198 },
    { id: 'kit_288', name: '288 色套装', color_count: 288, desc: '进阶套装', price: 358 },
    { id: 'kit_500', name: '500 色套装', color_count: 500, desc: '完整色板', price: 588 }
  ])
}
