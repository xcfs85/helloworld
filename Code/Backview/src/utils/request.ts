import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'

const CODE_SUCCESS = 0

// 将 PascalCase 转为 snake_case（用于后端字段转换）
function toSnakeCase(obj: any): any {
  if (obj === null || obj === undefined) return obj
  if (Array.isArray(obj)) return obj.map(toSnakeCase)
  if (typeof obj !== 'object') return obj

  const result: Record<string, any> = {}
  for (const key in obj) {
    const snakeKey = key.replace(/([A-Z])/g, '_$1').toLowerCase()
    result[snakeKey] = toSnakeCase(obj[key])
  }
  return result
}

const serviceAxios: AxiosInstance = axios.create({
  baseURL: '/api/admin/v1',
  timeout: 20000
})

// 请求拦截器：注入 Token
serviceAxios.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('admin_token')
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// 响应拦截器：统一处理错误码
serviceAxios.interceptors.response.use(
  (response) => {
    const res = response.data
    if (res && typeof res === 'object' && 'code' in res) {
      if (res.code === CODE_SUCCESS) return toSnakeCase(res.data)
      ElMessage.error(res.message || '请求失败')
      if (res.code === 401) {
        localStorage.removeItem('admin_token')
        localStorage.removeItem('admin_user')
        if (location.pathname !== '/auth/login') location.href = '/auth/login'
      }
      return Promise.reject(res)
    }
    return toSnakeCase(res)
  },
  (error) => {
    const msg = error?.response?.data?.message || error.message || '网络异常'
    ElMessage.error(msg)
    if (error?.response?.status === 401) {
      localStorage.removeItem('admin_token')
      localStorage.removeItem('admin_user')
      if (location.pathname !== '/auth/login') location.href = '/auth/login'
    }
    return Promise.reject(error)
  }
)

export interface PageParams {
  page?: number
  page_size?: number
  keyword?: string
  [key: string]: any
}

export interface PageResult<T = any> {
  list: T[]
  total: number
  page: number
  page_size: number
}

/**
 * HTTP 客户端：方法返回的 Promise 直接 resolve 业务数据（已被响应拦截器解包）。
 * 与底层 axios 行为不同，service.get<T>(url) 解析为 T 而非 AxiosResponse<T>。
 */
interface HttpClient {
  get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T>
  post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>
  put<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>
  delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T>
}

const service: HttpClient = {
  get: (url, config) => serviceAxios.get(url, config) as unknown as Promise<any>,
  post: (url, data, config) => serviceAxios.post(url, data, config) as unknown as Promise<any>,
  put: (url, data, config) => serviceAxios.put(url, data, config) as unknown as Promise<any>,
  delete: (url, config) => serviceAxios.delete(url, config) as unknown as Promise<any>
}

export default service
