import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi, type AdminUserInfo } from '@/api/auth'

const TOKEN_KEY = 'pindou_admin_token'
const REFRESH_KEY = 'pindou_admin_refresh'
const USER_KEY = 'pindou_admin_user'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string>(localStorage.getItem(TOKEN_KEY) || '')
  const refreshToken = ref<string>(localStorage.getItem(REFRESH_KEY) || '')
  const user = ref<AdminUserInfo | null>(JSON.parse(localStorage.getItem(USER_KEY) || 'null'))

  const isLoggedIn = computed(() => !!token.value)
  const permissions = computed(() => user.value?.permissions || [])
  const roleId = computed(() => user.value?.roleId || 0)

  function setToken(t: string, r: string) {
    token.value = t
    refreshToken.value = r
    localStorage.setItem(TOKEN_KEY, t)
    localStorage.setItem(REFRESH_KEY, r)
  }

  function setUser(u: AdminUserInfo) {
    user.value = u
    localStorage.setItem(USER_KEY, JSON.stringify(u))
  }

  function logout() {
    token.value = ''
    refreshToken.value = ''
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(REFRESH_KEY)
    localStorage.removeItem(USER_KEY)
  }

  async function refresh() {
    try {
      const resp = await authApi.current()
      setUser(resp)
      return true
    } catch {
      logout()
      return false
    }
  }

  function hasPermission(code: string) {
    if (roleId.value === 1) return true // 超级管理员
    return permissions.value.includes('*') || permissions.value.includes(code)
  }

  return { token, refreshToken, user, isLoggedIn, permissions, roleId, setToken, setUser, logout, refresh, hasPermission }
})
