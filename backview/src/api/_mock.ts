/** 通用请求函数（使用 mock 模拟）
 * 真实接入后端时，替换内部实现为调用 request */

import * as Mock from '@/mock'
import type { PageQuery, PageResult } from '@/types'

const delay = (ms = 200) => new Promise(r => setTimeout(r, ms))

export function pageQuery<T>(list: T[], query: PageQuery, filter?: (item: T) => boolean): PageResult<T> {
  const page = query.page || 1
  const pageSize = query.page_size || 20
  const keyword = (query.keyword || '').trim().toLowerCase()
  let filtered = filter ? list.filter(filter) : list
  if (keyword && (list as any[])[0]) {
    filtered = filtered.filter((item: any) => {
      return Object.values(item).some(v => String(v).toLowerCase().includes(keyword))
    })
  }
  const total = filtered.length
  const start = (page - 1) * pageSize
  return { list: filtered.slice(start, start + pageSize) as T[], total, page, page_size: pageSize }
}

export const M = Mock
export { delay }
