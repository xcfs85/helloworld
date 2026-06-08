/** 系统配置 API（基于 07-系统配置模块详细设计） */
import service, { type PageResult } from '@/utils/request'
import type { ColorChip } from '@/types'

// 通用配置 - 使用 config 接口
export function getGeneralConfig() {
  return service.get<any, any>('/config/list')
}

export function setGeneralConfig(data: { key: string; value: string; type?: string; description?: string }) {
  return service.put(`/config/${data.key}`, { value: data.value, type: data.type, description: data.description })
}

// AI配置 - 使用 config 接口
export function getAIConfig() {
  return service.get<any, any>('/config/list', { params: { config_type: 'number' } })
}

export function setAIConfig(data: { key: string; value: string }) {
  return service.put(`/config/${data.key}`, { value: data.value })
}

// 色板 - 使用 mard-color 接口
export function listColors(query?: { page?: number; page_size?: number; category?: string; is_common?: number; status?: number }) {
  return service.get<any, PageResult<any>>('/mard-color/list', { params: query })
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
  return service.get<any, any[]>('/bead-kit/list', { params: query })
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