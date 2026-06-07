import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/auth/Login.vue'),
    meta: { title: '登录', anonymous: true }
  },
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/Dashboard.vue'),
        meta: { title: '首页', icon: 'HomeFilled' }
      },
      // 用户管理
      {
        path: 'user/list',
        name: 'UserList',
        component: () => import('@/views/user/UserList.vue'),
        meta: { title: '用户列表', icon: 'User', permission: 'user:view' }
      },
      {
        path: 'member/list',
        name: 'MemberList',
        component: () => import('@/views/user/MemberList.vue'),
        meta: { title: '会员管理', icon: 'GoldMedal', permission: 'member:view' }
      },
      {
        path: 'order/list',
        name: 'OrderList',
        component: () => import('@/views/user/OrderList.vue'),
        meta: { title: '订单管理', icon: 'List', permission: 'order:view' }
      },
      // 内容管理
      {
        path: 'post/review',
        name: 'PostReview',
        component: () => import('@/views/content/PostReview.vue'),
        meta: { title: '帖子审核', icon: 'Document', permission: 'post:review' }
      },
      {
        path: 'comment/review',
        name: 'CommentReview',
        component: () => import('@/views/content/CommentReview.vue'),
        meta: { title: '评论审核', icon: 'ChatLineRound', permission: 'comment:review' }
      },
      {
        path: 'sensitive/word',
        name: 'SensitiveWord',
        component: () => import('@/views/content/SensitiveWord.vue'),
        meta: { title: '敏感词', icon: 'Warning', permission: 'sensitive:view' }
      },
      {
        path: 'report',
        name: 'Report',
        component: () => import('@/views/content/Report.vue'),
        meta: { title: '举报管理', icon: 'Flag', permission: 'report:view' }
      },
      // 模板管理
      {
        path: 'template/list',
        name: 'TemplateList',
        component: () => import('@/views/template/TemplateList.vue'),
        meta: { title: '模板列表', icon: 'Picture', permission: 'template:view' }
      },
      {
        path: 'template/review',
        name: 'TemplateReview',
        component: () => import('@/views/template/TemplateReview.vue'),
        meta: { title: '模板审核', icon: 'Check', permission: 'template:review' }
      },
      {
        path: 'template/category',
        name: 'TemplateCategory',
        component: () => import('@/views/template/Category.vue'),
        meta: { title: '模板分类', icon: 'Menu', permission: 'template:view' }
      },
      // 运营管理
      {
        path: 'banner',
        name: 'Banner',
        component: () => import('@/views/operation/Banner.vue'),
        meta: { title: 'Banner管理', icon: 'PictureFilled', permission: 'banner:view' }
      },
      {
        path: 'operation/topic',
        name: 'OperationTopic',
        component: () => import('@/views/operation/Topic.vue'),
        meta: { title: '话题管理', icon: 'CollectionTag', permission: 'topic:view' }
      },
      {
        path: 'special-topic',
        name: 'SpecialTopic',
        component: () => import('@/views/operation/SpecialTopic.vue'),
        meta: { title: '专题管理', icon: 'Files', permission: 'special-topic:view' }
      },
      {
        path: 'push',
        name: 'Push',
        component: () => import('@/views/operation/Push.vue'),
        meta: { title: '消息推送', icon: 'Promotion', permission: 'push:view' }
      },
      // 数据统计
      {
        path: 'stats/overview',
        name: 'StatsOverview',
        component: () => import('@/views/stats/Overview.vue'),
        meta: { title: '数据概览', icon: 'DataAnalysis', permission: 'stats:view' }
      },
      {
        path: 'stats/user',
        name: 'StatsUser',
        component: () => import('@/views/stats/UserStats.vue'),
        meta: { title: '用户分析', icon: 'UserFilled', permission: 'stats:view' }
      },
      {
        path: 'stats/content',
        name: 'StatsContent',
        component: () => import('@/views/stats/ContentStats.vue'),
        meta: { title: '内容分析', icon: 'DataLine', permission: 'stats:view' }
      },
      // 系统配置
      {
        path: 'system/config',
        name: 'SystemConfig',
        component: () => import('@/views/system/Config.vue'),
        meta: { title: '通用配置', icon: 'Setting', permission: 'config:view' }
      },
      {
        path: 'system/mard',
        name: 'MardColor',
        component: () => import('@/views/system/MardColor.vue'),
        meta: { title: 'MARD色号', icon: 'Brush', permission: 'mard:view' }
      },
      {
        path: 'system/kit',
        name: 'BeadKit',
        component: () => import('@/views/system/BeadKit.vue'),
        meta: { title: '耗材套装', icon: 'Goods', permission: 'kit:view' }
      },
      // 系统管理
      {
        path: 'system/admin',
        name: 'AdminList',
        component: () => import('@/views/system/AdminList.vue'),
        meta: { title: '账号管理', icon: 'Avatar', permission: 'admin:view' }
      },
      {
        path: 'system/role',
        name: 'RoleList',
        component: () => import('@/views/system/RoleList.vue'),
        meta: { title: '角色管理', icon: 'UserFilled', permission: 'role:view' }
      },
      {
        path: 'system/log',
        name: 'OperationLog',
        component: () => import('@/views/log/OperationLog.vue'),
        meta: { title: '操作日志', icon: 'Document', permission: 'log:view' }
      }
    ]
  },
  { path: '/:pathMatch(.*)*', redirect: '/dashboard' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to, _from, next) => {
  const auth = useAuthStore()
  document.title = `${to.meta.title || '拼豆'} - 拼豆后台管理系统`

  if (to.meta.anonymous) {
    next()
    return
  }

  if (!auth.isLoggedIn) {
    next({ path: '/login', query: { redirect: to.fullPath } })
    return
  }

  // 权限验证
  const perm = to.meta.permission as string | undefined
  if (perm && !auth.hasPermission(perm)) {
    next({ path: '/dashboard' })
    return
  }

  next()
})

export default router
