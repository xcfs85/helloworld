<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessageBox, ElMessage } from 'element-plus'

const router = useRouter()

// 菜单数据 - 根据路由自动生成
const menuGroups = computed(() => {
  const routes = router.options.routes.find( r => r.path === '/')?.children || []
  const groups: Record<string, any[]> = {}
  routes.forEach(r => {
    const group = r.meta?.group as string | undefined
    if (!group || r.meta?.hidden) return
    if (!groups[group]) groups[group] = []
    groups[group].push(r)
  })
  return Object.entries(groups).map(([title, items]) => ({ title, items }))
})

const user = ref({
  nickname: localStorage.getItem('admin_nickname') || '林运营',
  role: '运营组',
  initial: '运'
})

const searchKeyword = ref('')

function handleSearch() {
  if (!searchKeyword.value.trim()) return
  router.push({ name: 'user-list', query: { keyword: searchKeyword.value } })
}

async function handleLogout() {
  try {
    await ElMessageBox.confirm('确定要退出登录吗？', '提示', { type: 'warning' })
    localStorage.removeItem('admin_token')
    localStorage.removeItem('admin_user')
    localStorage.removeItem('admin_nickname')
    ElMessage.success('已退出登录')
    router.push('/auth/login')
  } catch {}
}
</script>

<template>
  <div class="app-layout">
    <!-- 顶栏 -->
    <header class="topbar">
      <div class="brand">
        <div class="brand-mark">
          <svg viewBox="0 0 32 32" width="20" height="20" aria-hidden="true">
            <defs>
              <linearGradient id="brandLg" x1="0" y1="0" x2="1" y2="1">
                <stop offset="0%" stop-color="#FF8A5A" />
                <stop offset="100%" stop-color="#F5C45E" />
              </linearGradient>
            </defs>
            <circle cx="8" cy="8" r="3" fill="url(#brandLg)" />
            <circle cx="16" cy="8" r="3" fill="#2A1F1A" />
            <circle cx="24" cy="8" r="3" fill="url(#brandLg)" />
            <circle cx="8" cy="16" r="3" fill="#2A1F1A" />
            <circle cx="16" cy="16" r="3" fill="url(#brandLg)" />
            <circle cx="24" cy="16" r="3" fill="#2A1F1A" />
            <circle cx="8" cy="24" r="3" fill="url(#brandLg)" />
            <circle cx="16" cy="24" r="3" fill="#2A1F1A" />
            <circle cx="24" cy="24" r="3" fill="url(#brandLg)" />
          </svg>
        </div>
        <div class="brand-text">
          <div class="brand-name">拼豆 <span class="brand-en">PINDOU ADMIN</span></div>
          <div class="brand-sub">后台管理系统 · v0.1</div>
        </div>
      </div>

      <div class="topbar-search">
        <svg viewBox="0 0 24 24" width="14" height="14">
          <path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4" />
        </svg>
        <input
          v-model="searchKeyword"
          type="text"
          placeholder="搜索页面 / 路由 / 菜单…"
          @keydown.enter="handleSearch"
        />
        <span class="kbd">⌘ K</span>
      </div>

      <div class="topbar-actions">
        <span class="pill soft">Vue 3 · Element Plus</span>
        <el-dropdown trigger="click">
          <div class="user-chip">
            <div class="avatar">{{ user.initial }}</div>
            <div class="user-meta">
              <div class="un">{{ user.nickname }}</div>
              <div class="ur">{{ user.role }} · 在线</div>
            </div>
          </div>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item disabled>{{ user.nickname }}（{{ user.role }}）</el-dropdown-item>
              <el-dropdown-item divided @click="handleLogout">
                <el-icon><SwitchButton /></el-icon>退出登录
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </header>

    <!-- 主体：侧栏 + 内容 -->
    <div class="layout-body">
      <aside class="nav">
        <div class="nav-list">
          <div v-for="g in menuGroups" :key="g.title" class="nav-group">
            <div class="nav-group-title">
              <span>{{ g.title }}</span>
              <span class="count">{{ g.items.length }}</span>
            </div>
            <router-link
              v-for="item in g.items"
              :key="item.name"
              :to="{ name: item.name }"
              custom
              v-slot="{ navigate, isActive }"
            >
              <div
                class="nav-item"
                :class="{ active: isActive }"
                @click="navigate"
              >
                <el-icon class="ni-icon">
                  <component :is="(item.meta as any).icon" />
                </el-icon>
                <span class="ni-text">{{ (item.meta as any).title }}</span>
              </div>
            </router-link>
          </div>
        </div>
        <div class="nav-foot">
          <div>设计系统：温暖色系 · 8dp 网格</div>
          <div>主色 <span class="dot" style="background: #FF7A5A"></span> #FF7A5A</div>
        </div>
      </aside>

      <main class="main-content">
        <router-view v-slot="{ Component, route: r }">
          <transition name="fade" mode="out-in">
            <component :is="Component" :key="r.fullPath" />
          </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<style scoped>
.app-layout {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: var(--bg-2);
}

/* 顶栏 */
.topbar {
  display: grid;
  grid-template-columns: 280px 1fr 360px;
  align-items: center;
  padding: 0 22px;
  background: rgba(255, 255, 255, 0.92);
  backdrop-filter: blur(14px);
  border-bottom: 1px solid var(--line);
  height: var(--top-h);
  flex-shrink: 0;
  z-index: 50;
}
.brand { display: flex; align-items: center; gap: 12px; }
.brand-mark {
  width: 36px; height: 36px; border-radius: 10px;
  background: linear-gradient(135deg, #FFE2D3, #FFD2B0);
  display: grid; place-items: center;
  box-shadow: inset 0 0 0 1px rgba(255, 122, 90, 0.18);
}
.brand-name { font-weight: 700; font-size: 15px; letter-spacing: .4px; display: flex; align-items: baseline; gap: 6px; }
.brand-en { font-weight: 600; font-size: 10px; color: var(--ink-3); letter-spacing: 2px; }
.brand-sub { font-size: 11px; color: var(--ink-3); margin-top: 1px; }
.topbar-search {
  display: flex; align-items: center; gap: 8px; padding: 7px 12px;
  background: var(--bg); border: 1px solid var(--line); border-radius: 8px;
  color: var(--ink-3); max-width: 480px; margin: 0 auto; width: 100%;
}
.topbar-search input { border: none; outline: none; background: transparent; flex: 1; font-size: 13px; color: var(--ink); }
.kbd { font-family: var(--mono); font-size: 10px; padding: 2px 6px; background: var(--surface); border: 1px solid var(--line); border-radius: 4px; color: var(--ink-3); }
.topbar-actions { display: flex; gap: 8px; align-items: center; justify-content: flex-end; }
.pill { padding: 4px 10px; border-radius: 999px; background: var(--ink); color: #fff; font-size: 11px; font-weight: 600; letter-spacing: .4px; }
.pill.soft { background: var(--surface); color: var(--ink-2); border: 1px solid var(--line); }
.user- chip { display: flex; align-items: center; gap: 8px; padding: 4px 10px 4px 4px; border-radius: 999px; background: var(--surface); border: 1px solid var(--line); cursor: pointer; }
.avatar { width: 28px; height: 28px; border-radius: 50%; background: linear-gradient(135deg, #FF8A5A, #F5C45E); color: #fff; display: grid; place-items: center; font-size: 12px; font-weight: 700; }
.user-meta .un { font-size: 12px; font-weight: 600; line-height: 1.2; }
.user-meta .ur { font-size: 10px; color: var(--ink-3); line-height: 1.2; }

/* 主体 */
.layout-body {
  display: grid;
  grid-template-columns: var(--aside-w) 1fr;
  flex: 1;
  overflow: hidden;
}

/* 侧栏 */
.nav {
  border-right: 1px solid var(--line);
  background: var(--surface);
  padding: 14px 10px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  overflow-y: auto;
}
.nav-list { flex: 1; margin: 0 -4px; padding: 0 4px; }
.nav-group { margin-bottom: 10px; }
.nav-group-title {
  font-size: 10px; color: var(--ink-3); font-weight: 700; letter-spacing: 1.2px;
  text-transform: uppercase; padding: 6px 10px 4px;
  display: flex; justify-content: space-between; align-items: center;
}
.nav-group-title .count { font-size: 10px; color: var(--ink-4); font-weight: 600; }
.nav-item {
  display: flex; align-items: center; gap: 10px;
  padding: 7px 10px; border-radius: 7px; font-size: 13px; color: var(--ink-2); cursor: pointer;
  transition: background .12s, color .12s; margin-bottom: 1px;
}
.nav-item:hover { background: var(--bg); }
.nav-item.active { background: var(--ink); color: #fff; font-weight: 600; }
.nav-item.active .ni- icon { color: rgba(255, 255, 255, 0.7); }
.nav-item.active .ni-route { color: rgba(255, 255, 255, 0.55); }
.nav-item .ni-icon { width: 14px; height: 14px; color: var(--ink-3); flex-shrink: 0; }
.nav-item .ni-text { flex: 1; }
.nav-item .ni-route { font-size: 10px; color: var(--ink-4); font-family: var(--mono); }

.nav-foot {
  border-top: 1px solid var(--line); padding-top: 10px; font-size: 11px; color: var(--ink-3); line-height: 1.7;
}

/* 主内容 */
.main-content {
  overflow: hidden;
  display: flex;
  flex-direction: column;
  background: var(--bg-2);
  min-width: 0;
}

.fade-enter-active, .fade-leave-active { transition: opacity .15s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>