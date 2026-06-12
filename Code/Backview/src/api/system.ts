/** 系统配置 API（基于 07-系统配置模块详细设计）
 *  - 后端 GET /config/list 返回扁平列表：[{config_key, config_value, config_type, ...}, ...]
 *  - 单条更新：PUT /config/{key}   body: { value, type?, description? }
 *  - 批量更新：POST /config/batch    body: [{ key, value, type?, description? }, ...]
 */
import service, { type PageResult } from '@/utils/request'
import type { ColorChip } from '@/types'

// ===== 通用配置 / AI 配置 =====

export interface ConfigItem {
  id?: string
  config_key: string
  config_value: string | null
  config_type?: string
  description?: string
  status?: number
  create_time?: string
  update_time?: string
}

export interface SetConfigPayload {
  key: string
  value: string
  type?: string
  description?: string
}

/** 获取全部配置（通用 + AI 等所有扁平条目） */
export function getGeneralConfig() {
  return service.get<ConfigItem[]>('/config/list')
}

/** 单条保存（用于单 key 修改） */
export function setGeneralConfig(data: SetConfigPayload) {
  return service.put(`/config/${data.key}`, {
    value: data.value,
    type: data.type,
    description: data.description
  })
}

/** 批量保存（用于配置页一次性改多个 key） */
export function batchSetConfig(data: SetConfigPayload[]) {
  return service.post('/config/batch', data)
}

// 色板 - 使用 mard-color 接口
export function listColors(query?: { page?: number; page_size?: number; category?: string; is_common?: number; status?: number }) {
  return service.get<PageResult<any>>('/mard-color/list', { params: query })
}

export function addColor(data: Partial<ColorChip>) {
  return service.post('/mard-color', data)
}

export function updateColor(id: string, data: Partial<ColorChip>) {
  return service.put(`/mard-color/${id}`, data)
}

export function deleteColor(id: string) {
  return service.delete(`/mard-color/${id}`)
}

export function batchImportColors(data: any[]) {
  return service.post('/mard-color/batch-import', data)
}

// 套装 - 使用 bead-kit 接口
export function listKits(query?: { color_count?: number }) {
  return service.get<any[]>('/bead-kit/list', { params: query })
}

export function addKit(data: any) {
  return service.post('/bead-kit', data)
}

export function updateKit(id: string, data: any) {
  return service.put(`/bead-kit/${id}`, data)
}

export function deleteKit(id: string) {
  return service.delete(`/bead-kit/${id}`)
}