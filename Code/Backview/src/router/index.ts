import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/auth/login',
    name: 'login',
    component: () => import('@/views/auth/Login.vue'),
    meta: { title: '登录', public: true }
  },
  {
    path: '/',
    component: () => import('@/layouts/DefaultLayout.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'dashboard',
        component: () => import('@/views/dashboard/Index.vue'),
        meta: { title: '核心指标看板', icon: 'DataAnalysis', group: '运营总览' }
      },

      // 用户管理
      {
        path: 'user/list',
        name: 'user-list',
        component: () => import('@/views/user/List.vue'),
        meta: { title: '用户列表', icon: 'User', group: '用户管理' }
      },
      {
        path: 'user/:id',
        name: 'user-detail',
        component: () => import('@/views/user/Detail.vue'),
        meta: { title: '用户详情', icon: 'User', group: '用户管理', hidden: true }
      },
      {
        path: 'member/list',
        name: 'member-list',
        component: () => import('@/views/user/MemberList.vue'),
        meta: { title: '会员管理', icon: 'GoldMedal', group: '用户管理' }
      },

      // 内容管理
      {
        path: 'post/review/list',
        name: 'post-review-list',
        component: () => import('@/views/post/ReviewList.vue'),
        meta: { title: '帖子审核', icon: 'ChatLineRound', group: '内容管理' }
      },
      {
        path: 'post/review/:id',
        name: 'post-review-detail',
        component: () => import('@/views/post/ReviewDetail.vue'),
        meta: { title: '帖子审核详情', icon: 'ChatLineRound', group: '内容管理', hidden: true }
      },
      {
        path: 'comment/review/list',
        name: 'comment-review-list',
        component: () => import('@/views/post/CommentList.vue'),
        meta: { title: '评论审核', icon: 'ChatDotRound', group: '内容管理' }
      },
      {
        path: 'sensitive/word/list',
        name: 'sensitive-word-list',
        component: () => import('@/views/post/SensitiveWordList.vue'),
        meta: { title: '敏感词管理', icon: 'Warning', group: '内容管理' }
      },
      {
        path: 'report/list',
        name: 'report-list',
        component: () => import('@/views/post/ReportList.vue'),
        meta: { title: '举报管理', icon: 'CircleClose', group: '内容管理' }
      },

      // 模板管理
      {
        path: 'template/list',
        name: 'template-list',
        component: () => import('@/views/template/List.vue'),
        meta: { title: '模板列表', icon: 'Grid', group: '模板管理' }
      },
      {
        path: 'template/review/:id',
        name: 'template-review-detail',
        component: () => import('@/views/template/ReviewDetail.vue'),
        meta: { title: '模板审核详情', icon: 'Grid', group: '模板管理', hidden: true }
      },
      {
        path: 'template/category/list',
        name: 'template-category-list',
        component: () => import('@/views/template/CategoryList.vue'),
        meta: { title: '分类管理', icon: 'Menu', group: '模板管理' }
      },
      {
        path: 'template/tag/list',
        name: 'template-tag-list',
        component: () => import('@/views/template/TagList.vue'),
        meta: { title: '标签管理', icon: 'PriceTag', group: '模板管理' }
      },

      // 运营管理
      {
        path: 'banner/list',
        name: 'banner-list',
        component: () => import('@/views/operation/BannerList.vue'),
        meta: { title: 'Banner 管理', icon: 'Picture', group: '运营管理' }
      },
      {
        path: 'topic/list',
        name: 'topic-list',
        component: () => import('@/views/operation/TopicList.vue'),
        meta: { title: '话题管理', icon: 'ChatLineSquare', group: '运营管理' }
      },
      {
        path: 'topic/special/list',
        name: 'special-list',
        component: () => import('@/views/operation/SpecialList.vue'),
        meta: { title: '专题管理', icon: 'Files', group: '运营管理' }
      },
      {
        path: 'push/list',
        name: 'push-list',
        component: () => import('@/views/operation/PushList.vue'),
        meta: { title: '推送管理', icon: 'Promotion', group: '运营管理' }
      },

      // 数据统计
      {
        path: 'stats/user',
        name: 'stats-user',
        component: () => import('@/views/stats/User.vue'),
        meta: { title: '用户分析', icon: 'TrendCharts', group: '数据统计' }
      },
      {
        path: 'stats/creation',
        name: 'stats-creation',
        component: () => import('@/views/stats/Creation.vue'),
        meta: { title: '创作分析', icon: 'Brush', group: '数据统计' }
      },
      {
        path: 'stats/community',
        name: 'stats-community',
        component: () => import('@/views/stats/Community.vue'),
        meta: { title: '社区分析', icon: 'Histogram', group: '数据统计' }
      },

      // 系统管理
      {
        path: 'system/admin/list',
        name: 'admin-list',
        component: () => import('@/views/system/AdminList.vue'),
        meta: { title: '账号管理', icon: 'Avatar', group: '系统管理' }
      },
      {
        path: 'system/role/list',
        name: 'role-list',
        component: () => import('@/views/system/RoleList.vue'),
        meta: { title: '角色权限', icon: 'Lock', group: '系统管理' }
      },
      {
        path: 'system/log/list',
        name: 'log-list',
        component: () => import('@/views/system/LogList.vue'),
        meta: { title: '操作日志', icon: 'Document', group: '系统管理' }
      },
      {
        path: 'config/general',
        name: 'config-general',
        component: () => import('@/views/system/ConfigGeneral.vue'),
        meta: { title: '系统配置', icon: 'Setting', group: '系统管理' }
      },
      {
        path: 'config/ai',
        name: 'config-ai',
        component: () => import('@/views/system/ConfigAI.vue'),
        meta: { title: 'AI 配置', icon: 'Cpu', group: '系统管理', hidden: true }
      },
      {
        path: 'config/color',
        name: 'config-color',
        component: () => import('@/views/system/ConfigColor.vue'),
        meta: { title: '色板配置', icon: 'BrushFilled', group: '系统管理', hidden: true }
      }
    ]
  },
  { path: '/:pathMatch(.*)*', redirect: '/dashboard' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  if (to.meta?.title) document.title = `${to.meta.title} · 拼豆后台`
  if (to.meta?.public) return next()
  const token = localStorage.getItem('admin_token')
  if (!token) return next('/auth/login')
  next()
})

export default router
