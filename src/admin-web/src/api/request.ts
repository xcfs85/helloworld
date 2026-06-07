import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE || '/api',
  timeout: 30000
})

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    const auth = useAuthStore()
    if (auth.token) {
      config.headers.Authorization = `Bearer ${auth.token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse) => {
    const data = response.data
    if (data.code === 0) {
      return data.data
    }
    if (data.code === 2001 || data.code === 2002) {
      // Token失效
      const auth = useAuthStore()
      auth.logout()
      router.push('/login')
      ElMessage.error(data.message || '未授权')
      return Promise.reject(new Error(data.message))
    }
    ElMessage.error(data.message || '请求失败')
    return Promise.reject(new Error(data.message))
  },
  (error) => {
    if (error.response) {
      const { status, data } = error.response
      if (status === 401) {
        const auth = useAuthStore()
        auth.logout()
        router.push('/login')
      } else if (status === 403) {
        ElMessage.error('无操作权限')
      } else {
        ElMessage.error(data?.message || `请求错误 ${status}`)
      }
    } else {
      ElMessage.error('网络错误')
    }
    return Promise.reject(error)
  }
)

export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
  timestamp: number
}

export interface PageRequest {
  page?: number
  size?: number
  keyword?: string
  orderBy?: string
  orderDir?: 'asc' | 'desc'
}

export interface PagedResult<T> {
  list: T[]
  total: number
  page: number
  size: number
}

export function get<T = any>(url: string, params?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.get(url, { params, ...config })
}

export function post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.post(url, data, config)
}

export function put<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.put(url, data, config)
}

export function del<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
  return service.delete(url, config)
}

export default service
