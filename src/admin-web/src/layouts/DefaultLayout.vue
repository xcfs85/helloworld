<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  HomeFilled, User, Document, ChatLineRound, Warning, Flag,
  Picture, Check, Menu, PictureFilled, CollectionTag, Files, Promotion,
  DataAnalysis, UserFilled, DataLine, Setting, Brush, Goods, Avatar,
  GoldMedal, List, SwitchButton
} from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const breadcrumb = computed(() => {
  return route.matched.filter(item => item.meta && item.meta.title).map(item => ({
    title: item.meta.title as string,
    path: item.path
  }))
})

const menus = [
  { path: '/dashboard', title: '首页', icon: HomeFilled },
  {
    title: '用户运营', icon: User, children: [
      { path: '/user/list', title: '用户列表', permission: 'user:view' },
      { path: '/member/list', title: '会员管理', permission: 'member:view' },
      { path: '/order/list', title: '订单管理', permission: 'order:view' }
    ]
  },
  {
    title: '内容审核', icon: Document, children: [
      { path: '/post/review', title: '帖子审核', permission: 'post:review' },
      { path: '/comment/review', title: '评论审核', permission: 'comment:review' },
      { path: '/sensitive/word', title: '敏感词', permission: 'sensitive:view' },
      { path: '/report', title: '举报管理', permission: 'report:view' }
    ]
  },
  {
    title: '模板管理', icon: Picture, children: [
      { path: '/template/list', title: '模板列表', permission: 'template:view' },
      { path: '/template/review', title: '模板审核', permission: 'template:review' },
      { path: '/template/category', title: '模板分类', permission: 'template:view' }
    ]
  },
  {
    title: '运营管理', icon: Promotion, children: [
      { path: '/banner', title: 'Banner管理', permission: 'banner:view' },
      { path: '/operation/topic', title: '话题管理', permission: 'topic:view' },
      { path: '/special-topic', title: '专题管理', permission: 'special-topic:view' },
      { path: '/push', title: '消息推送', permission: 'push:view' }
    ]
  },
  {
    title: '数据统计', icon: DataAnalysis, children: [
      { path: '/stats/overview', title: '数据概览', permission: 'stats:view' },
      { path: '/stats/user', title: '用户分析', permission: 'stats:view' },
      { path: '/stats/content', title: '内容分析', permission: 'stats:view' }
    ]
  },
  {
    title: '系统配置', icon: Setting, children: [
      { path: '/system/config', title: '通用配置', permission: 'config:view' },
      { path: '/system/mard', title: 'MARD色号', permission: 'mard:view' },
      { path: '/system/kit', title: '耗材套装', permission: 'kit:view' }
    ]
  },
  {
    title: '系统管理', icon: Avatar, children: [
      { path: '/system/admin', title: '账号管理', permission: 'admin:view' },
      { path: '/system/role', title: '角色管理', permission: 'role:view' },
      { path: '/system/log', title: '操作日志', permission: 'log:view' }
    ]
  }
]

const activeMenu = computed(() => route.path)
const collapsed = ref(false)

const hasMenuPermission = (perm?: string) => {
  if (!perm) return true
  return auth.hasPermission(perm)
}

const handleLogout = async () => {
  try {
    await import('@/api/auth').then(m => m.authApi.logout())
  } catch {}
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <el-container class="layout-container">
    <el-aside :width="collapsed ? '64px' : '220px'" class="sidebar">
      <div class="logo">
        <span v-if="!collapsed">拼豆后台</span>
        <span v-else>拼</span>
      </div>
      <el-menu
        :default-active="activeMenu"
        :collapse="collapsed"
        :collapse-transition="false"
        background-color="#001529"
        text-color="#dcdfe6"
        active-text-color="#409eff"
        router
      >
        <template v-for="(menu, index) in menus" :key="index">
          <template v-if="!menu.children">
            <el-menu-item :index="menu.path" v-if="hasMenuPermission(menu.permission)">
              <el-icon><component :is="menu.icon" /></el-icon>
              <template #title>{{ menu.title }}</template>
            </el-menu-item>
          </template>
          <template v-else>
            <el-sub-menu v-if="menu.children?.some(c => hasMenuPermission(c.permission))">
              <template #title>
                <el-icon><component :is="menu.icon" /></el-icon>
                <span>{{ menu.title }}</span>
              </template>
              <el-menu-item
                v-for="(child, ci) in menu.children"
                :key="ci"
                :index="child.path"
                v-show="hasMenuPermission(child.permission)"
              >
                <el-icon><component :is="child.icon || Menu" /></el-icon>
                <template #title>{{ child.title }}</template>
              </el-menu-item>
            </el-sub-menu>
          </template>
        </template>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="header">
        <div class="left">
          <el-button text @click="collapsed = !collapsed">
            <el-icon><component :is="collapsed ? 'Expand' : 'Fold'" /></el-icon>
          </el-button>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item v-for="(item, i) in breadcrumb" :key="i">
              {{ item.title }}
            </el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <div class="right">
          <el-dropdown>
            <span class="user-info">
              <el-avatar :size="32">{{ auth.user?.nickname?.[0] || 'A' }}</el-avatar>
              <span class="name">{{ auth.user?.nickname || auth.user?.username }}</span>
              <el-tag size="small" type="success" v-if="auth.roleId === 1">超管</el-tag>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="handleLogout">
                  <el-icon><SwitchButton /></el-icon>退出登录
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <el-main class="main">
        <router-view v-slot="{ Component }">
          <transition name="fade">
            <component :is="Component" />
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<style lang="scss" scoped>
.layout-container { height: 100vh; }
.sidebar { background: #001529; transition: width 0.2s; overflow-x: hidden; }
.logo {
  height: 60px; line-height: 60px; text-align: center;
  color: #fff; font-size: 18px; font-weight: 600;
  background: #002140;
}
.header {
  background: #fff; border-bottom: 1px solid #e8e8e8;
  display: flex; align-items: center; justify-content: space-between;
  padding: 0 20px;
  .left { display: flex; align-items: center; gap: 16px; }
  .right { .user-info { display: flex; align-items: center; gap: 8px; cursor: pointer; } }
}
.main { background: #f0f2f5; padding: 20px; }
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
