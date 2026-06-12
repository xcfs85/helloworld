<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listUsers, disableUser, enableUser, muteUser, getUserStats, createUser, importUsers, exportUsers, type UserListItem } from '@/api/user'

const router = useRouter()
const loading = ref(false)
const list = ref<UserListItem[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const selected = ref<string[]>([])

const filter = reactive<{
  keyword: string
  platform: string
  is_member: string  // 'all' | 'true' | 'false'
  status: string
  register_start_time: string
  register_end_time: string
}>({
  keyword: '',
  platform: 'all',
  is_member: 'all',
  status: 'all',
  register_start_time: '',
  register_end_time: ''
})

interface SideItem { label: string; value: string; count: number; badge?: string }
interface MemberItem { label: string; value: string; count: number; badge?: string }

const sideFilters = ref<SideItem[]>([
  { label: '全部用户', value: 'all', count: 0, badge: '' },
  { label: '正常', value: 'active', count: 0, badge: '' },
  { label: '禁言中', value: 'muted', count: 0, badge: 'warn' },
  { label: '已禁用', value: 'disabled', count: 0, badge: 'danger' }
])
const sideMethod = ref<SideItem[]>([
  { label: '手机号', value: 'phone', count: 0 },
  { label: '微信', value: 'wechat', count: 0 },
  { label: 'Apple ID', value: 'apple', count: 0 },
  { label: '游客', value: 'guest', count: 0 }
])
const sideMember = ref<MemberItem[]>([
  { label: '会员用户', value: 'true', count: 0 },
  { label: '非会员', value: 'false', count: 0 }
])

const activeSide = ref('all')
const activePlatform = ref('all')
const activeMember = ref('all')

// 添加用户弹窗
const showAddUser = ref(false)
const addLoading = ref(false)
const addUserForm = reactive({ nickname: '', phone: '', gender: 'unknown', city: '' })

// 导入用户弹窗
const showImport = ref(false)
const importLoading = ref(false)
const importResult = ref<{ success_count: number; fail_count: number; fail_details: { row: number; reason: string }[] } | null>(null)

function selectStatus(v: string) {
  activeSide.value = v
  activePlatform.value = 'all'
  activeMember.value = 'all'
  filter.status = v
  filter.platform = 'all'
  filter.is_member = 'all'
  page.value = 1
  load()
}

function selectPlatform(v: string) {
  activePlatform.value = v
  activeSide.value = 'all'
  activeMember.value = 'all'
  filter.status = 'all'
  filter.platform = v
  filter.is_member = 'all'
  page.value = 1
  load()
}

function selectMember(v: string) {
  activeMember.value = v
  activeSide.value = 'all'
  activePlatform.value = 'all'
  filter.status = 'all'
  filter.platform = 'all'
  filter.is_member = v
  page.value = 1
  load()
}

async function load() {
  loading.value = true
  try {
    // 转换筛选参数
    const params: any = {
      page: page.value,
      page_size: pageSize.value
    }
    if (filter.keyword) params.keyword = filter.keyword
    if (filter.platform && filter.platform !== 'all') params.platform = filter.platform
    if (filter.is_member && filter.is_member !== 'all') params.is_member = filter.is_member === 'true'
    if (filter.status && filter.status !== 'all') params.status = filter.status === 'normal' ? 'active' : filter.status

    const res: any = await listUsers(params)
    list.value = (res?.list || []) as UserListItem[]
    total.value = res?.total || 0
  } finally { loading.value = false }
}

// 加载侧边栏分类统计
async function loadStats() {
  try {
    const res: any = await getUserStats()
    const data = res?.data || res
    if (!data) return

    // 状态分类（全部/正常/禁言/禁用）
    sideFilters.value[0].count = data.total || 0
    sideFilters.value[1].count = data.active_count || 0
    sideFilters.value[2].count = data.muted_count || 0
    sideFilters.value[3].count = data.disabled_count || 0

    // 注册方式（手机号/微信/Apple/游客）
    const platformMap: Record<string, number> = {}
    if (Array.isArray(data.platform_counts)) {
      for (const item of data.platform_counts) {
        platformMap[item.platform] = item.count
      }
    }
    sideMethod.value[0].count = platformMap['phone'] || 0
    sideMethod.value[1].count = platformMap['wechat'] || 0
    sideMethod.value[2].count = platformMap['apple'] || 0
    sideMethod.value[3].count = platformMap['guest'] || 0

    // 会员状态
    sideMember.value[0].count = data.member_count || 0
    sideMember.value[1].count = data.non_member_count || 0
  } catch (error) {
    console.error('加载用户统计失败:', error)
  }
}

function search() { page.value = 1; load() }
function reset() {
  filter.keyword = ''
  filter.platform = 'all'
  filter.is_member = 'all'
  filter.status = 'all'
  filter.register_start_time = ''
  filter.register_end_time = ''
  activeSide.value = 'all'
  activePlatform.value = 'all'
  activeMember.value = 'all'
  search()
}
function viewDetail(u: UserListItem) { router.push({ name: 'user-detail', params: { id: u.id } }) }

// 导出CSV
async function doExport() {
  loading.value = true
  try {
    const params: any = {
      page: page.value,
      page_size: pageSize.value
    }
    if (filter.keyword) params.keyword = filter.keyword
    if (filter.platform && filter.platform !== 'all') params.platform = filter.platform
    if (filter.is_member && filter.is_member !== 'all') params.is_member = filter.is_member === 'true'
    if (filter.status && filter.status !== 'all') params.status = filter.status === 'normal' ? 'active' : filter.status

    const res: any = await exportUsers(params)
    const exportList = (res?.list || []) as UserListItem[]

    // 生成CSV
    const headers = ['ID', '昵称', '手机号', '性别', '城市', '会员', '会员到期时间', '状态', '注册时间', '最后登录', '帖子数', '作品数']
    const rows = exportList.map(u => [
      u.id,
      u.nickname,
      u.phone || '',
      ({ male: '男', female: '女', unknown: '未知' } as any)[u.gender] || u.gender,
      u.city || '',
      u.is_member ? '是' : '否',
      u.member_expire_time || '',
      ({ active: '正常', normal: '正常', muted: '禁言', disabled: '已禁用' } as any)[u.status] || u.status,
      u.create_time || '',
      u.last_login_time || '',
      u.post_count,
      u.diagram_count || 0
    ])

    const csvContent = '\uFEFF' + [headers, ...rows].map(r => r.map(c => `"${String(c).replace(/"/g, '""')}"`).join(',')).join('\n')
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `用户列表_${new Date().toISOString().slice(0, 10)}.csv`
    a.click()
    URL.revokeObjectURL(url)
    ElMessage.success(`已导出 ${exportList.length} 条用户数据`)
  } catch (error) {
    ElMessage.error('导出失败')
  } finally { loading.value = false }
}

// 添加用户
async function onAddUser() {
  if (!addUserForm.nickname) { ElMessage.warning('请输入昵称'); return }
  addLoading.value = true
  try {
    await createUser({
      nickname: addUserForm.nickname,
      phone: addUserForm.phone || undefined,
      gender: addUserForm.gender,
      city: addUserForm.city || undefined
    })
    ElMessage.success('用户创建成功')
    showAddUser.value = false
    Object.assign(addUserForm, { nickname: '', phone: '', gender: 'unknown', city: '' })
    load()
    loadStats()
  } catch (error: any) {
    ElMessage.error(error?.message || '创建失败')
  } finally { addLoading.value = false }
}

// 导入用户
function onImportFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  importLoading.value = true
  importResult.value = null

  const reader = new FileReader()
  reader.onload = async (evt) => {
    try {
      const text = evt.target?.result as string
      const lines = text.split('\n').filter(l => l.trim())
      if (lines.length < 2) {
        ElMessage.warning('CSV文件至少需要包含标题行和一行数据')
        return
      }

      // 解析CSV（跳过标题行）
      const users: { nickname: string; phone?: string; gender?: string; city?: string }[] = []
      for (let i = 1; i < lines.length; i++) {
        const cols = parseCsvLine(lines[i])
        if (cols.length < 1 || !cols[0]?.trim()) continue
        users.push({
          nickname: cols[0]?.trim() || '',
          phone: cols[1]?.trim() || undefined,
          gender: cols[2]?.trim() || 'unknown',
          city: cols[3]?.trim() || undefined
        })
      }

      if (users.length === 0) {
        ElMessage.warning('未解析到有效用户数据')
        return
      }

      const res: any = await importUsers(users)
      importResult.value = res?.data || res
      ElMessage.success(`导入完成：成功 ${importResult.value?.success_count || 0} 条，失败 ${importResult.value?.fail_count || 0} 条`)
      load()
      loadStats()
    } catch (error: any) {
      ElMessage.error(error?.message || '导入失败')
    } finally {
      importLoading.value = false
      input.value = ''
    }
  }
  reader.readAsText(file)
}

// 简单CSV行解析（处理引号内逗号）
function parseCsvLine(line: string): string[] {
  const result: string[] = []
  let current = ''
  let inQuotes = false
  for (let i = 0; i < line.length; i++) {
    const ch = line[i]
    if (ch === '"') {
      if (inQuotes && line[i + 1] === '"') { current += '"'; i++ }
      else { inQuotes = !inQuotes }
    } else if (ch === ',' && !inQuotes) {
      result.push(current)
      current = ''
    } else {
      current += ch
    }
  }
  result.push(current)
  return result
}

// 下载导入模板
function downloadTemplate() {
  const headers = '昵称,手机号,性别,城市'
  const example = '张三,13800138000,男,北京'
  const csvContent = '\uFEFF' + headers + '\n' + example
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = '用户导入模板.csv'
  a.click()
  URL.revokeObjectURL(url)
}

async function doMute(u: UserListItem) {
  try {
    const { value } = await ElMessageBox.prompt('请输入禁言天数（1-30）', '禁言用户', { inputValue: '3', inputPattern: /^\d+$/ })
    await muteUser(u.id, { days: parseInt(value), reason: '运营禁言' })
    ElMessage.success(`已禁言 ${u.nickname} ${value} 天`)
    load()
    loadStats()
  } catch {}
}
async function doDisable(u: UserListItem) {
  try {
    await ElMessageBox.confirm(`确定禁用用户 ${u.nickname}？禁用后无法登录`, '禁用账号', { type: 'warning' })
    await disableUser(u.id, '违规')
    ElMessage.success('已禁用')
    load()
    loadStats()
  } catch {}
}
async function doEnable(u: UserListItem) {
  await enableUser(u.id)
  ElMessage.success('已解禁')
  load()
  loadStats()
}

function fmtTime(t: string | undefined | null) {
  if (!t) return '—'
  const now = new Date()
  const d = new Date(t.replace(/-/g, '/'))
  const diff = (now.getTime() - d.getTime()) / 1000
  if (diff < 60) return '刚刚'
  if (diff < 3600) return `${Math.floor(diff / 60)} 分钟前`
  if (diff < 86400) return `${Math.floor(diff / 3600)} 小时前`
  if (diff < 86400 * 3) return `${Math.floor(diff / 86400)} 天前`
  return t.slice(5)
}

onMounted(() => {
  load()
  loadStats()
})
</script>

<template>
  <div class="page-view">
    <div class="app-with-aside">
      <aside class="aside">
        <div class="aside-title">筛选条件</div>
        <div v-for="f in sideFilters" :key="f.value" class="aside-item" :class="{ active: activeSide === f.value }" @click="selectStatus(f.value)">
          {{ f.label }}
          <span v-if="f.count" class="badge" :class="(f as any).badge">{{ f.count.toLocaleString() }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">注册方式</div>
        <div v-for="m in sideMethod" :key="m.value" class="aside-item" :class="{ active: activePlatform === m.value }" @click="selectPlatform(m.value)">
          {{ m.label }}
          <span v-if="m.count" class="badge">{{ m.count.toLocaleString() }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">会员状态</div>
        <div v-for="m in sideMember" :key="String(m.value)" class="aside-item" :class="{ active: activeMember === m.value }" @click="selectMember(m.value)">
          {{ m.label }}
          <span v-if="m.count" class="badge">{{ m.count.toLocaleString() }}</span>
        </div>
      </aside>

      <div class="main">
        <PageHead
          :crumbs="[{ label: '用户管理', to: '/user/list' }, { label: '用户列表' }]"
          :title="`用户列表`"
          :sub="`共 ${total.toLocaleString()} 条 · 已选 ${selected.length} 条`"
        >
          <template #actions>
            <button class="btn btn-secondary" @click="doExport">导出 CSV</button>
            <button class="btn btn-secondary" @click="showImport = true; importResult = null">导入</button>
            <button class="btn btn-primary" @click="showAddUser = true">+ 添加用户</button>
          </template>
        </PageHead>

        <div class="toolbar">
          <div class="search-input">
            <el-icon><Search /></el-icon>
            <input v-model="filter.keyword" placeholder="用户 ID / 昵称 / 手机号" @keydown.enter="search" />
          </div>
          <div class="f-select">
            <select v-model="filter.platform" @change="search">
              <option value="all">注册方式：全部</option>
              <option value="wechat">微信</option>
              <option value="phone">手机号</option>
              <option value="apple">Apple</option>
            </select>
          </div>
          <div class="f-select">
            <select v-model="filter.is_member" @change="search">
              <option value="all">会员：全部</option>
              <option value="true">会员</option>
              <option value="false">非会员</option>
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
                  <StatusTag v-if="u.is_member" variant="primary">
                    会员 · 剩 {{ u.member_expire_time ? Math.max(0, Math.ceil((new Date(u.member_expire_time).getTime() - Date.now()) / 86400000)) : 0 }} 天
                  </StatusTag>
                  <StatusTag v-else variant="neutral">非会员</StatusTag>
                </td>
                <td>
                  <StatusTag v-if="(u.status as string) === 'active' || (u.status as string) === 'normal'" variant="ok">正常</StatusTag>
                  <StatusTag v-else-if="(u.status as string) === 'muted'" variant="warn">禁言</StatusTag>
                  <StatusTag v-else variant="danger">已禁用</StatusTag>
                </td>
                <td>{{ u.post_count }}</td>
                <td>{{ (u.diagram_count || 0).toLocaleString() }}</td>
                <td class="muted">{{ fmtTime(u.last_login_time) }}</td>
                <td class="col-actions">
                  <button class="btn btn-xs btn-ghost" @click="viewDetail(u)">查看</button>
                  <button v-if="(u.status as string) === 'active' || (u.status as string) === 'normal'" class="btn btn-xs btn-ghost" @click="doMute(u)">禁言</button>
                  <button v-if="(u.status as string) === 'active' || (u.status as string) === 'normal'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="doDisable(u)">禁用</button>
                  <button v-if="(u.status as string) === 'muted' || (u.status as string) === 'disabled'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="doEnable(u)">解禁</button>
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

    <!-- 添加用户弹窗 -->
    <el-dialog v-model="showAddUser" title="添加用户" width="480">
      <div class="form-row"><div class="lbl req">昵称</div>
        <div class="input-line"><input v-model="addUserForm.nickname" placeholder="请输入用户昵称" /></div>
      </div>
      <div class="form-row"><div class="lbl">手机号</div>
        <div class="input-line"><input v-model="addUserForm.phone" placeholder="请输入手机号（选填）" /></div>
      </div>
      <div class="form-row"><div class="lbl">性别</div>
        <div class="f-select" style="width: 100%">
          <select v-model="addUserForm.gender">
            <option value="unknown">未知</option>
            <option value="male">男</option>
            <option value="female">女</option>
          </select>
        </div>
      </div>
      <div class="form-row"><div class="lbl">城市</div>
        <div class="input-line"><input v-model="addUserForm.city" placeholder="请输入城市（选填）" /></div>
      </div>
      <template #footer>
        <el-button @click="showAddUser = false">取消</el-button>
        <el-button type="primary" :loading="addLoading" @click="onAddUser">创建</el-button>
      </template>
    </el-dialog>

    <!-- 导入用户弹窗 -->
    <el-dialog v-model="showImport" title="批量导入用户" width="520">
      <div style="margin-bottom: 16px">
        <p style="font-size: 13px; color: var(--ink-3); margin-bottom: 8px">请上传 CSV 文件，格式：昵称,手机号,性别,城市</p>
        <button class="btn btn-sm btn-secondary" @click="downloadTemplate">下载导入模板</button>
      </div>
      <div style="border: 2px dashed var(--line); border-radius: 8px; padding: 32px; text-align: center; position: relative; cursor: pointer;">
        <input type="file" accept=".csv" @change="onImportFileChange" style="position: absolute; inset: 0; opacity: 0; cursor: pointer;" />
        <div style="font-size: 14px; color: var(--ink-2)">点击选择 CSV 文件</div>
        <div style="font-size: 12px; color: var(--ink-3); margin-top: 4px">支持 .csv 格式</div>
      </div>
      <div v-if="importLoading" style="text-align: center; margin-top: 16px; color: var(--ink-3)">导入中...</div>
      <div v-if="importResult" style="margin-top: 16px; padding: 12px; background: var(--bg); border-radius: 6px; font-size: 13px;">
        <div style="color: var(--mint)">成功：{{ importResult.success_count }} 条</div>
        <div v-if="importResult.fail_count > 0" style="color: var(--rose); margin-top: 4px">失败：{{ importResult.fail_count }} 条</div>
        <div v-if="importResult.fail_details?.length" style="margin-top: 8px; max-height: 120px; overflow-y: auto;">
          <div v-for="d in importResult.fail_details" :key="d.row" style="color: var(--ink-3); font-size: 12px;">第 {{ d.row }} 行：{{ d.reason }}</div>
        </div>
      </div>
      <template #footer>
        <el-button @click="showImport = false">关闭</el-button>
      </template>
    </el-dialog>
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
