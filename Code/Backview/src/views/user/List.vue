<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listUsers, disableUser, enableUser, muteUser } from '@/api/user'
import type { User } from '@/types'

const router = useRouter()
const loading = ref(false)
const list = ref<User[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const selected = ref<string[]>([])

const filter = reactive({
  keyword: '',
  register_method: 'all',
  is_member: 'all',
  status: 'all'
})

const sideFilters = [
  { label: '全部用户', value: 'all', count: 1234, badge: '' },
  { label: '正常', value: 'normal', count: 1180, badge: '' },
  { label: '禁言中', value: 'muted', count: 31, badge: 'warn' },
  { label: '已禁用', value: 'disabled', count: 23, badge: 'danger' }
]
const sideMethod = [
  { label: '手机号', value: 'phone', count: 687 },
  { label: '微信', value: 'wechat', count: 456 },
  { label: 'Apple ID', value: 'apple', count: 78 },
  { label: '游客', value: 'guest', count: 13 }
]
const sideMember = [
  { label: '会员用户', value: 'yes', count: 312 },
  { label: '非会员', value: 'no', count: 922 },
  { label: '即将到期', value: 'expire', count: 28, badge: 'warn' }
]

const activeSide = ref('all')

async function load() {
  loading.value = true
  try {
    const res: any = await listUsers({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

function search() { page.value = 1; load() }
function reset() { filter.keyword = ''; filter.register_method = 'all'; filter.is_member = 'all'; filter.status = 'all'; search() }
function viewDetail(u: User) { router.push({ name: 'user-detail', params: { id: u.id } }) }

async function doMute(u: User) {
  try {
    const { value } = await ElMessageBox.prompt('请输入禁言天数（1-30）', '禁言用户', { inputValue: '3', inputPattern: /^\d+$/ })
    await muteUser(u.id, parseInt(value), '运营禁言')
    ElMessage.success(`已禁言 ${u.nickname} ${value} 天`)
    load()
  } catch {}
}
async function doDisable(u: User) {
  try {
    await ElMessageBox.confirm(`确定禁用用户 ${u.nickname}？禁用后无法登录`, '禁用账号', { type: 'warning' })
    await disableUser(u.id, '违规')
    ElMessage.success('已禁用')
    load()
  } catch {}
}
async function doEnable(u: User) {
  await enableUser(u.id)
  ElMessage.success('已解禁')
  load()
}

function fmtTime(t: string) {
  const now = new Date()
  const d = new Date(t.replace(/-/g, '/'))
  const diff = (now.getTime() - d.getTime()) / 1000
  if (diff < 60) return '刚刚'
  if (diff < 3600) return `${Math.floor(diff / 60)} 分钟前`
  if (diff < 86400) return `${Math.floor(diff / 3600)} 小时前`
  if (diff < 86400 * 3) return `${Math.floor(diff / 86400)} 天前`
  return t.slice(5)
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <div class="app-with-aside">
      <aside class="aside">
        <div class="aside-title">筛选条件</div>
        <div v-for="f in sideFilters" :key="f.value" class="aside-item" :class="{ active: activeSide === f.value }" @click="activeSide = f.value; filter.status = f.value; search()">
          {{ f.label }}
          <span v-if="f.count" class="badge" :class="f.badge">{{ f.count.toLocaleString() }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">注册方式</div>
        <div v-for="m in sideMethod" :key="m.value" class="aside-item" @click="filter.register_method = m.value; search()">
          {{ m.label }}
          <span class="badge">{{ m.count }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">会员状态</div>
        <div v-for="m in sideMember" :key="m.value" class="aside-item" @click="filter.is_member = m.value; search()">
          {{ m.label }}
          <span v-if="m.count" class="badge" :class="m.badge">{{ m.count }}</span>
        </div>
      </aside>

      <div class="main">
        <PageHead
          :crumbs="[{ label: '用户管理', to: '/user/list' }, { label: '用户列表' }]"
          :title="`用户列表`"
          :sub="`共 ${total.toLocaleString()} 条 · 已选 ${selected.length} 条`"
        >
          <template #actions>
            <button class="btn btn-secondary">导出 CSV</button>
            <button class="btn btn-secondary">导入</button>
            <button class="btn btn-primary">+ 添加用户</button>
          </template>
        </PageHead>

        <div class="toolbar">
          <div class="search-input">
            <el-icon><Search /></el-icon>
            <input v-model="filter.keyword" placeholder="用户 ID / 昵称 / 手机号" @keydown.enter="search" />
          </div>
          <div class="f-select">
            <select v-model="filter.register_method" @change="search">
              <option value="all">注册方式：全部</option>
              <option value="wechat">微信</option>
              <option value="phone">手机号</option>
              <option value="apple">Apple</option>
              <option value="guest">游客</option>
            </select>
          </div>
          <div class="f-select">
            <select v-model="filter.is_member" @change="search">
              <option value="all">会员：全部</option>
              <option value="yes">会员</option>
              <option value="no">非会员</option>
            </select>
          </div>
          <div class="f-select">
            <select v-model="filter.status" @change="search">
              <option value="all">状态：全部</option>
              <option value="normal">正常</option>
              <option value="muted">禁言</option>
              <option value="disabled">禁用</option>
            </select>
          </div>
          <div class="date-range">
            <input value="2026-05-01" />
            <span class="sep">→</span>
            <input value="2026-06-07" />
          </div>
          <button class="btn btn-sm btn-secondary" @click="reset">重置</button>
          <button class="btn btn-sm btn-primary" @click="search">搜索</button>
          <div class="f-spacer"></div>
          <div class="batch-actions">
            <span class="muted small">已选 {{ selected.length }} 条</span>
            <button class="btn btn-sm btn-secondary">批量打标</button>
            <button class="btn btn-sm btn-secondary">批量禁言</button>
            <button class="btn btn-sm btn-danger">批量禁用</button>
          </div>
        </div>

        <div class="tbl-wrap">
          <table class="tbl">
            <thead>
              <tr>
                <th style="width: 32px"><span class="ck"></span></th>
                <th>用户</th>
                <th>手机号</th>
                <th>注册方式</th>
                <th>会员</th>
                <th>状态</th>
                <th>帖子</th>
                <th>粉丝</th>
                <th>最后登录</th>
                <th class="col-actions">操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="u in list" :key="u.id">
                <td><span class="ck"></span></td>
                <td>
                  <div class="user-cell">
                    <div class="av" :class="['c' + ((parseInt(u.id.slice(2)) % 6) + 1)]">{{ u.nickname[0] }}</div>
                    <div class="meta">
                      <div class="nm">{{ u.nickname }}</div>
                      <div class="id">{{ u.id }}</div>
                    </div>
                  </div>
                </td>
                <td class="mono">{{ u.phone || '—' }}</td>
                <td>
                  <StatusTag :variant="u.register_method === 'wechat' ? 'info' : u.register_method === 'apple' ? 'purple' : 'neutral'">
                    {{ { wechat: '微信', phone: '手机号', apple: 'Apple', guest: '游客' }[u.register_method] }}
                  </StatusTag>
                </td>
                <td>
                  <StatusTag v-if="u.is_member" variant="primary">{{ u.member_level }} · 剩 {{ u.member_expire_time ? (Math.ceil((new Date(u.member_expire_time).getTime() - Date.now()) / 86400000)) : 0 }} 天</StatusTag>
                  <StatusTag v-else variant="neutral">非会员</StatusTag>
                </td>
                <td>
                  <StatusTag v-if="u.status === 'normal'" variant="ok">正常</StatusTag>
                  <StatusTag v-else-if="u.status === 'muted'" variant="warn">禁言</StatusTag>
                  <StatusTag v-else variant="danger">已禁用</StatusTag>
                </td>
                <td>{{ u.post_count }}</td>
                <td>{{ u.follower_count.toLocaleString() }}</td>
                <td class="muted">{{ fmtTime(u.last_login_time) }}</td>
                <td class="col-actions">
                  <button class="btn btn-xs btn-ghost" @click="viewDetail(u)">查看</button>
                  <button v-if="u.status === 'normal'" class="btn btn-xs btn-ghost" @click="doMute(u)">禁言</button>
                  <button v-else-if="u.status === 'muted'" class="btn btn-xs btn-ghost" @click="doEnable(u)">解禁</button>
                  <button v-if="u.status === 'disabled'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="doEnable(u)">解禁</button>
                  <button v-if="u.status !== 'disabled'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="doDisable(u)">禁用</button>
                  <button v-else class="btn btn-xs btn-ghost" style="color: var(--rose)">删除</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <Pager :total="total" :page="page" :page-size="pageSize"
          @update:page="(v) => { page = v; load() }"
          @update:page-size="(v) => { pageSize = v; load() }" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.app-with-aside { display: grid; grid-template-columns: var(--sub-aside-w) 1fr; height: 100%; background: var(--bg-2); flex: 1; overflow: hidden; }
.aside { background: var(--surface); border-right: 1px solid var(--line); padding: 14px 10px; display: flex; flex-direction: column; gap: 2px; overflow-y: auto; }
.aside-title { font-size: 10px; font-weight: 700; color: var(--ink-3); letter-spacing: 1.2px; text-transform: uppercase; padding: 6px 10px 4px; }
.aside-item {
  display: flex; align-items: center; justify-content: space-between; gap: 6px;
  padding: 7px 10px; border-radius: 6px; font-size: 12.5px; color: var(--ink-2); cursor: pointer;
  transition: background .12s, color .12s;
}
.aside-item:hover { background: var(--bg); }
.aside-item.active { background: var(--ink); color: #fff; font-weight: 600; }
.aside-item .badge { font-size: 10px; background: var(--primary-soft); color: var(--primary-ink); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.aside-item.active .badge { background: rgba(255,255,255,.2); color: #fff; }
.aside-item .badge.warn { background: var(--warn-soft); color: var(--warn); }
.aside-item .badge.danger { background: var(--rose-soft); color: var(--rose); }
.main { display: flex; flex-direction: column; overflow: hidden; min-width: 0; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
