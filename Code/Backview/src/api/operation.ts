/** 运营管理 API（基于 05-运营管理模块详细设计） */

import service, { type PageResult } from '@/utils/request'
import type { PageQuery, Banner, Topic, Special, Push } from '@/types'

// Banner
export function listBanners(query: PageQuery & { position?: string; status?: string }) {
  return service.get<PageResult<Banner>>('/banner/list', { params: query })
}
export function addBanner(data: Partial<Banner>) {
  return service.post('/banner', data)
}
export function updateBanner(id: string, data: Partial<Banner>) {
  return service.put(`/banner/${id}`, data)
}
export function deleteBanner(id: string) {
  return service.delete(`/banner/${id}`)
}

// 话题
export function listTopics(query: PageQuery & { status?: string; is_official?: string; keyword?: string }) {
  return service.get<PageResult<Topic>>('/topic/list', { params: query })
}
export function addTopic(data: Partial<Topic>) {
  return service.post('/topic', data)
}
export function updateTopic(id: string, data: Partial<Topic>) {
  return service.put(`/topic/${id}`, data)
}
export function closeTopic(id: string) {
  return service.post(`/topic/${id}/close`)
}
export function openTopic(id: string) {
  return service.post(`/topic/${id}/open`)
}

// 专题
/** 专题列表查询参数 */
export interface SpecialListQuery {
  page?: number
  size?: number
  status?: number
  keyword?: string
}

/** 创建/更新专题参数 */
export interface SpecialFormData {
  name: string
  description?: string
  cover_url: string
  template_ids: string[]
  start_time: string
  end_time: string
  status?: number
}

export function listSpecials(query: SpecialListQuery) {
  return service.get<PageResult<Special>>('/special-topic/list', { params: query })
}
export function getSpecial(id: string) {
  return service.get<Special>(`/special-topic/${id}`)
}
export function addSpecial(data: SpecialFormData) {
  return service.post<string>('/special-topic', data)
}
export function updateSpecial(id: string, data: SpecialFormData) {
  return service.put(`/special-topic/${id}`, data)
}
export function deleteSpecial(id: string) {
  return service.delete(`/special-topic/${id}`)
}

// 推送
export function listPushes(query: PageQuery & { status?: string; push_type?: string; target_type?: string; keyword?: string }) {
  return service.get<PageResult<Push>>('/push/list', { params: query })
}
export function getPush(id: string) {
  return service.get<Push>(`/push/${id}`)
}
export function createPush(data: { title: string; content: string; push_type?: string; target_type?: string; target_ids?: string[]; channels: string[] }) {
  return service.post<string>('/push', data)
}
export function sendPush(data: { title: string; content: string; push_type?: string; target_type?: string; target_ids?: string[]; channels: string[] }) {
  return service.post<string>('/push/send', data)
}
export function schedulePush(data: { title: string; content: string; push_type?: string; target_type?: string; target_ids?: string[]; channels: string[]; schedule_time: string }) {
  return service.post<string>('/push/schedule', data)
}
export function cancelPush(id: string) {
  return service.post(`/push/${id}/cancel`)
}
export function retryPush(id: string) {
  return service.post(`/push/${id}/retry`)
}
