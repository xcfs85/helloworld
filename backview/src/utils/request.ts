import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'

const CODE_SUCCESS = 0

const service: AxiosInstance = axios.create({
  baseURL: '/api/admin/v1',
  timeout: 20000
})

// 请求拦截器：注入 Token
service.interceptors.request.use(
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
service.interceptors.response.use(
  (response: AxiosResponse) => {
    const res = response.data
    if (res && typeof res === 'object' && 'code' in res) {
      if (res.code === CODE_SUCCESS) return res.data
      ElMessage.error(res.message || '请求失败')
      if (res.code === 401) {
        localStorage.removeItem('admin_token')
        localStorage.removeItem('admin_user')
        if (location.pathname !== '/auth/login') location.href = '/auth/login'
      }
      return Promise.reject(res)
    }
    return res
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

export default service
