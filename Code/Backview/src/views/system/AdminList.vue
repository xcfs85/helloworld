<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listAdmins, createAdmin, updateAdmin, updateAdminStatus, resetAdminPassword, getAllRoles, type AdminUserListItem } from '@/api/auth'
import type { RoleItem } from '@/types'

const loading = ref(false)
const list = ref<AdminUserListItem[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ keyword: '', status: 'all' })

// 新增账号
const showAdd = ref(false)
const newAdmin = ref({ username: '', nickname: '', password: '', role_id: 1, status: 1 })
const addLoading = ref(false)

// 编辑账号
const showEdit = ref(false)
const editingAdmin = ref<AdminUserListItem | null>(null)
const editForm = reactive({ nickname: '', role_id: 1, status: 1 })
const editLoading = ref(false)

// 重置密码
const showResetPwd = ref(false)
const resetPwdLoading = ref(false)
const newPassword = ref('')

// 角色列表
const roles = ref<RoleItem[]>([])

async function load() {
  loading.value = true
  try {
    // status 转换: all -> 不传, active -> 1, disabled -> 0
    const statusMap: Record<string, number | undefined> = { all: undefined, active: 1, disabled: 0 }
    const res: any = await listAdmins({
      keyword: filter.keyword,
      status: statusMap[filter.status as string],
      page: page.value,
      page_size: pageSize.value
    })
    list.value = (res?.list || []) as AdminUserListItem[]
    total.value = res?.total || 0
  } finally { loading.value = false }
}

// 加载角色列表
async function loadRoles() {
  const res: any = await getAllRoles()
  roles.value = Array.isArray(res) ? res : (res?.list || [])
}

// 新增账号
async function onAdd() {
  if (!newAdmin.value.username) { ElMessage.warning('请输入账号'); return }
  if (!newAdmin.value.password) { ElMessage.warning('请输入密码'); return }
  if (!newAdmin.value.nickname) { ElMessage.warning('请输入昵称'); return }
  addLoading.value = true
  try {
    await createAdmin(newAdmin.value)
    ElMessage.success('创建成功')
    showAdd.value = false
    newAdmin.value = { username: '', nickname: '', password: '', role_id: 1, status: 1 }
    load()
  } finally { addLoading.value = false }
}

// 打开编辑弹窗
function onEdit(row: AdminUserListItem) {
  editingAdmin.value = row
  editForm.nickname = row.nickname
  editForm.role_id = Number(row.role_id)
  editForm.status = Number(row.status)
  showEdit.value = true
}

// 提交编辑
async function onUpdate() {
  if (!editingAdmin.value) return
  editLoading.value = true
  try {
    await updateAdmin(Number(editingAdmin.value.id), editForm)
    ElMessage.success('修改成功')
    showEdit.value = false
    load()
  } finally { editLoading.value = false }
}

// 启用/禁用
async function onToggleStatus(row: AdminUserListItem) {
  const newStatus = Number(row.status) === 1 ? 0 : 1
  const action = newStatus === 1 ? '启用' : '禁用'
  try {
    await ElMessageBox.confirm(`确定要${action}该账号吗？`, '提示', { type: 'warning' })
    await updateAdminStatus(Number(row.id), newStatus)
    ElMessage.success(`${action}成功`)
    load()
  } catch (e) { /* 用户取消 */ }
}

// 打开重置密码弹窗
function onResetPassword(row: AdminUserListItem) {
  editingAdmin.value = row
  newPassword.value = ''
  showResetPwd.value = true
}

// 提交重置密码
async function onConfirmResetPwd() {
  if (!editingAdmin.value) return
  if (!newPassword.value) { ElMessage.warning('请输入新密码'); return }
  if (newPassword.value.length < 6) { ElMessage.warning('密码长度至少6位'); return }
  resetPwdLoading.value = true
  try {
    await resetAdminPassword(Number(editingAdmin.value.id), newPassword.value)
    ElMessage.success('密码重置成功')
    showResetPwd.value = false
  } finally { resetPwdLoading.value = false }
}

function idHash(id: number | string): number {
  const n = Number(id)
  return Number.isFinite(n) ? n : Array.from(String(id)).reduce((acc, ch) => acc + ch.charCodeAt(0), 0)
}

onMounted(() => { load(); loadRoles() })
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '系统管理' }, { label: '账号管理' }]"
      title="账号管理"
      :sub="`共 ${total} 个管理员账号`"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 新增账号</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="账号 / 昵称" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option><option value="active">启用</option><option value="disabled">禁用</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>账号</th>
            <th>角色</th>
            <th>状态</th>
            <th>最后登录</th>
            <th>IP</th>
            <th>创建时间</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="a in list" :key="a.id">
            <td>
              <div class="user-cell">
                <div class="av" :class="'c' + ((idHash(a.id) % 6) + 1)">{{ a.nickname?.[0] || '?' }}</div>
                <div class="meta"><div class="nm">{{ a.nickname }}</div><div class="id">{{ a.username }}</div></div>
              </div>
            </td>
            <td><StatusTag :variant="Number(a.role_id) === 1 ? 'primary' : 'neutral'">{{ a.role_name }}</StatusTag></td>
            <td>
              <StatusTag :variant="Number(a.status) ? 'ok' : 'neutral'">{{ Number(a.status) ? '启用' : '禁用' }}</StatusTag>
            </td>
            <td class="muted">{{ a.last_login_time || '-' }}</td>
            <td class="muted mono">{{ a.last_login_ip || '-' }}</td>
            <td class="muted">{{ a.create_time }}</td>
            <td class="col-actions">
              <button class="btn btn-xs btn-ghost" @click="onEdit(a)">编辑</button>
              <button class="btn btn-xs btn-ghost" @click="onResetPassword(a)">重置密码</button>
              <button v-if="Number(a.status)" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onToggleStatus(a)">禁用</button>
              <button v-else class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="onToggleStatus(a)">启用</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <!-- 新增账号弹窗 -->
    <el-dialog v-model="showAdd" title="新增账号" width="480">
      <div class="form-row"><div class="lbl req">账号</div>
        <div class="input-line"><input v-model="newAdmin.username" placeholder="登录账号" /></div>
      </div>
      <div class="form-row"><div class="lbl req">密码</div>
        <div class="input-line"><input v-model="newAdmin.password" type="password" placeholder="密码" show-password /></div>
      </div>
      <div class="form-row"><div class="lbl req">昵称</div>
        <div class="input-line"><input v-model="newAdmin.nickname" placeholder="昵称" /></div>
      </div>
      <div class="form-row"><div class="lbl req">角色</div>
        <div class="f-select" style="width: 100%">
          <select v-model="newAdmin.role_id">
            <option v-for="r in roles" :key="r.id" :value="r.id">{{ r.name }}</option>
          </select>
        </div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" :loading="addLoading" @click="onAdd">创建</el-button>
      </template>
    </el-dialog>

    <!-- 编辑账号弹窗 -->
    <el-dialog v-model="showEdit" title="编辑账号" width="480">
      <div class="form-row"><div class="lbl">账号</div>
        <div class="input-line" style="color: var(--text-muted)">{{ editingAdmin?.username }}</div>
      </div>
      <div class="form-row"><div class="lbl req">昵称</div>
        <div class="input-line"><input v-model="editForm.nickname" placeholder="昵称" /></div>
      </div>
      <div class="form-row"><div class="lbl req">角色</div>
        <div class="f-select" style="width: 100%">
          <select v-model="editForm.role_id">
            <option v-for="r in roles" :key="r.id" :value="r.id">{{ r.name }}</option>
          </select>
        </div>
      </div>
      <div class="form-row"><div class="lbl req">状态</div>
        <div class="f-select" style="width: 100%">
          <select v-model="editForm.status">
            <option :value="1">启用</option>
            <option :value="0">禁用</option>
          </select>
        </div>
      </div>
      <template #footer>
        <el-button @click="showEdit = false">取消</el-button>
        <el-button type="primary" :loading="editLoading" @click="onUpdate">保存</el-button>
      </template>
    </el-dialog>

    <!-- 重置密码弹窗 -->
    <el-dialog v-model="showResetPwd" title="重置密码" width="400">
      <div class="form-row"><div class="lbl">账号</div>
        <div class="input-line" style="color: var(--text-muted)">{{ editingAdmin?.username }}</div>
      </div>
      <div class="form-row"><div class="lbl req">新密码</div>
        <div class="input-line"><input v-model="newPassword" type="password" placeholder="请输入新密码" /></div>
      </div>
      <template #footer>
        <el-button @click="showResetPwd = false">取消</el-button>
        <el-button type="primary" :loading="resetPwdLoading" @click="onConfirmResetPwd">确认重置</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page- view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>