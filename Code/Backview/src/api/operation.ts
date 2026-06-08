/** 运营管理 API（基于 05-运营管理模块详细设计） */

import { delay, M, pageQuery } from './_mock'
import type { PageQuery, Banner, Topic, Special, Push } from '@/types'

// Banner
export function listBanners(query: PageQuery & { position?: string; status?: string }) {
  return delay().then(() => {
    return pageQuery<Banner>(M.mockBanners, query, (b) => {
      if (query.status && query.status !== 'all' && b.status !== query.status) return false
      return true
    })
  })
}
export function addBanner(data: Partial<Banner>) { return delay().then(() => ({ success: true })) }
export function updateBanner(id: string, data: Partial<Banner>) { return delay().then(() => ({ success: true })) }
export function deleteBanner(id: string) { return delay().then(() => ({ success: true })) }

// 话题
export function listTopics(query: PageQuery & { status?: string; is_official?: string }) {
  return delay().then(() => {
    return pageQuery<Topic>(M.mockTopics, query, (t) => {
      if (query.status && query.status !== 'all' && t.status !== query.status) return false
      return true
    })
  })
}
export function addTopic(data: Partial<Topic>) { return delay().then(() => ({ success: true })) }
export function updateTopic(id: string, data: Partial<Topic>) { return delay().then(() => ({ success: true })) }
export function closeTopic(id: string) { return delay().then(() => ({ success: true })) }

// 专题
export function listSpecials(query: PageQuery & { status?: string }) {
  return delay().then(() => {
    return pageQuery<Special>(M.mockSpecials, query, (s) => {
      if (query.status && query.status !== 'all' && s.status !== query.status) return false
      return true
    })
  })
}
export function addSpecial(data: Partial<Special>) { return delay().then(() => ({ success: true })) }
export function updateSpecial(id: string, data: Partial<Special>) { return delay().then(() => ({ success: true })) }

// 推送
export function listPushes(query: PageQuery & { status?: string }) {
  return delay().then(() => {
    return pageQuery<Push>(M.mockPushes, query, (p) => {
      if (query.status && query.status !== 'all' && p.status !== query.status) return false
      return true
    })
  })
}
export function createPush(data: Partial<Push>) { return delay().then(() => ({ success: true })) }
export function sendPush(id: string) { return delay().then(() => ({ success: true })) }
