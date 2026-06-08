/** 运营管理 API（基于 05-运营管理模块详细设计） */

import service, { type PageResult } from '@/utils/request'
import type { PageQuery, Banner, Topic, Special, Push } from '@/types'

// Banner
export function listBanners(query: PageQuery & { position?: string; status?: string }) {
  return service.get<any, PageResult<Banner>>('/banner/list', { params: query })
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
  return service.get<any, PageResult<Topic>>('/topic/list', { params: query })
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
export function listSpecials(query: PageQuery & { status?: string }) {
  return service.get<any, PageResult<Special>>('/special-topic/list', { params: query })
}
export function addSpecial(data: Partial<Special>) {
  return service.post('/special-topic', data)
}
export function updateSpecial(id: string, data: Partial<Special>) {
  return service.put(`/special-topic/${id}`, data)
}
export function deleteSpecial(id: string) {
  return service.delete(`/special-topic/${id}`)
}

// 推送
export function listPushes(query: PageQuery & { status?: string; push_type?: string; target_type?: string }) {
  return service.get<any, PageResult<Push>>('/push/list', { params: query })
}
export function createPush(data: { title: string; content: string; target_type?: string; target_ids?: string[] }) {
  return service.post('/push', data)
}
export function sendPush( data: { title: string; content: string; target_type?: string; target_ids?: string[]; schedule_time?: string }) {
  return service.post('/push/send', data)
}
export function schedulePush(data: { title: string; content: string; target_type?: string; target_ids?: string[]; schedule_time: string }) {
  return service.post('/push/schedule', data)
}
export function cancelPush(id: string) {
  return service.post(`/push/${id}/cancel`)
}
