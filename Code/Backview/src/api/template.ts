/** 模板管理 API（基于 04-模板管理模块详细设计） */

import { delay, M, pageQuery } from './_mock'
import type { PageQuery, Template, Category, Tag } from '@/types'

export function listTemplates(query: PageQuery & {
  status?: string
  category_id?: string
  source?: string
  difficulty?: string
}) {
  return delay().then(() => {
    return pageQuery<Template>(M.mockTemplates, query, (t) => {
      if (query.status && query.status !== 'all' && t.status !== query.status) return false
      if (query.category_id && query.category_id !== 'all' && t.category_id !== query.category_id) return false
      if (query.source && query.source !== 'all' && t.source !== query.source) return false
      if (query.difficulty && query.difficulty !== 'all' && t.difficulty !== query.difficulty) return false
      return true
    })
  })
}

export function getTemplate(id: string) {
  return delay().then(() => M.mockTemplates.find(t => t.id === id))
}

export function approveTemplate(id: string) { return delay().then(() => ({ success: true })) }
export function rejectTemplate(id: string, reason: string) { return delay().then(() => ({ success: true })) }
export function offlineTemplate(id: string) { return delay().then(() => ({ success: true })) }

// 分类
export function listCategories() {
  return delay().then(() => M.mockCategories)
}
export function addCategory(data: Partial<Category>) { return delay().then(() => ({ success: true })) }
export function updateCategory(id: string, data: Partial<Category>) { return delay().then(() => ({ success: true })) }
export function deleteCategory(id: string) { return delay().then(() => ({ success: true })) }

// 标签
export function listTags(query: PageQuery & { category_id?: string }) {
  return delay().then(() => {
    return pageQuery<Tag>(M.mockTags, query, (t) => {
      if (query.category_id && query.category_id !== 'all' && t.category_id !== query.category_id) return false
      return true
    })
  })
}
export function addTag(data: Partial<Tag>) { return delay().then(() => ({ success: true })) }
export function deleteTag(id: string) { return delay().then(() => ({ success: true })) }
