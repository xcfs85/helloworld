/** 模板管理 API
 * 对接真实后台接口 */

import service from '@/utils/request'
import type { PageQuery } from '@/types'

// 创建模板
export function createTemplate(data: {
  name: string
  category_id: string
  difficulty?: string
  board_size?: string
  total_colors?: number
}) {
  return service.post<{ id: string }>('/template', data)
}

// ===== 模板管理 =====

// 模板列表
export function listTemplates(query: PageQuery & {
  status?: string
  category?: string
  source_type?: string
  difficulty?: string
  is_featured?: number
  keyword?: string
} = {}) {
  return service.get<{ list: TemplateItem[]; total: number }>('/template/list', { params: query })
}

// 待审核模板列表
export function listPendingTemplates(query: PageQuery & {
  status?: string
  category?: string
} = {}) {
  return service.get<{ list: TemplateItem[]; total: number }>('/template/pending', { params: query })
}

export interface TemplateItem {
  id: string
  template_id: string
  name: string
  description?: string
  category: string
  category_name: string
  tags: string[]
  cover_url: string
  source_type: 'official' | 'creator'
  board_size: string
  bead_count: number
  total_colors: number
  difficulty: string
  is_featured: boolean
  status: string
  use_count: number
  create_time: string
}

// 模板详情
export function getTemplate(id: string) {
  return service.get<TemplateDetailItem>('/template/' + id)
}

export interface TemplateDetailItem {
  id: string
  template_id: string
  name: string
  description: string
  category: string
  category_name: string
  tags: string[]
  cover_url: string
  preview_urls: string[]
  source_type: string
  source?: 'official' | 'creator'
  creator?: { id: string; nickname: string }
  creator_id?: string
  creator_name?: string
  board_size: string
  bead_count?: number
  total_beads?: number
  total_colors?: number
  color_count?: number
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  style?: string
  duration?: string
  is_featured: boolean
  status: string
  use_count: number
  submit_time?: string
  create_time: string
  publish_time?: string
}

// 审核通过模板
export function approveTemplate(id: string) {
  return service.post('/template/' + id + '/approve')
}

// 驳回模板
export function rejectTemplate(id: string, reason?: string) {
  return service.post('/template/' + id + '/reject', { reason })
}

// 发布/上架模板
export function publishTemplate(id: string) {
  return service.post('/template/' + id + '/publish')
}

// 下架模板
export function unpublishTemplate(id: string) {
  return service.post('/template/' + id + '/unpublish')
}

// 设为精选
export function featureTemplate(id: string) {
  return service.post('/template/' + id + '/feature')
}

// 取消精选
export function unfeatureTemplate(id: string) {
  return service.post('/template/' + id + '/unfeature')
}

// 下架模板（别名）
export function offlineTemplate(id: string) {
  return service.post('/template/' + id + '/unpublish')
}

// ===== 分类管理 =====

// 分类列表
export function listCategories() {
  return service.get<{ list: CategoryItem[]; total: number }>('/template-category/list')
}

export interface CategoryItem {
  id: string
  name: string
  code: string
  icon?: string
  sort: number
  status: number
  template_count: number
  create_time: string
}

// 创建分类
export function addCategory(data: { name: string; icon?: string; sort?: number }) {
  return service.post<string>('/template-category', data)
}

// 更新分类
export function updateCategory(id: string, data: { name?: string; icon?: string; sort?: number }) {
  return service.put('/template-category/' + id, data)
}

// 删除分类
export function deleteCategory(id: string) {
  return service.delete('/template-category/' + id)
}

// ===== 标签管理 =====

// 标签列表
export function listTags(query: PageQuery & { type?: string } = {}) {
  return service.get<{ list: TagItem[]; total: number }>('/template-tag/list', { params: query })
}

export interface TagItem {
  id: string
  name: string
  category?: string
  type?: string
  use_count: number
  status: number
  create_time: string
}

// 创建标签
export function addTag(data: { name: string; category?: string; type?: string }) {
  return service.post<string>('/template-tag', data)
}

// 更新标签
export function updateTag(id: string, data: { name?: string; category?: string; type?: string }) {
  return service.put('/template-tag/' + id, data)
}

// 删除标签
export function deleteTag(id: string) {
  return service.delete('/template-tag/' + id)
}