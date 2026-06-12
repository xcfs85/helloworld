<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listRoles, createRole, updateRole, type CreateRoleParams } from '@/api/auth'
import type { Role } from '@/types'

const list = ref<Role[]>([])

const allPermissions = [
  { key: 'user', name: '用户管理' },
  { key: 'post', name: '帖子管理' },
  { key: 'comment', name: '评论管理' },
  { key: 'sensitive', name: '敏感词管理' },
  { key: 'report', name: '举报管理' },
  { key: 'template', name: '模板管理' },
  { key: 'banner', name: 'Banner' },
  { key: 'topic', name: '话题' },
  { key: 'special', name: '专题' },
  { key: 'push', name: '推送' },
  { key: 'stats', name: '数据统计' },
  { key: 'config', name: '系统配置' },
  { key: 'admin', name: '账号管理' },
  { key: 'log', name: '操作日志' }
]

const activeRole = ref<Role | null>(null)

// 正在更新的权限 key（避免重复点击）
const updatingPerm = ref<string | null>(null)

// 新建角色弹窗
const showCreateDialog = ref(false)
const createForm = ref<CreateRoleParams>({
  name: '',
  code: '',
  description: '',
  permissions: []
})
const isSubmitting = ref(false)

async function handleCreate() {
  if (!createForm.value.name.trim()) {
    ElMessage.warning('请输入角色名称')
    return
  }
  if (!createForm.value.code.trim()) {
    ElMessage.warning('请输入角色编码')
    return
  }
  isSubmitting.value = true
  try {
    await createRole(createForm.value)
    ElMessage.success('创建成功')
    showCreateDialog.value = false
    // 重置表单
    createForm.value = { name: '', code: '', description: '', permissions: [] }
    // 刷新列表
    const res: any = await listRoles({})
    list.value = res.list
    if (list.value.length > 0) {
      activeRole.value = list.value[0]
    }
  } catch (e: any) {
    ElMessage.error(e.message || '创建失败')
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  const res: any = await listRoles({})
  list.value = res.list
  activeRole.value = list.value[0]
})

function togglePerm(role: Role, perm: string) {
  if (role.permissions.includes('*')) return
  if (updatingPerm.value) return

  // 计算新的权限列表
  const newPermissions = role.permissions.includes(perm)
    ? role.permissions.filter(p => p !== perm)
    : [...role.permissions, perm]

  const roleId = Number(role.id)
  if (!roleId || Number.isNaN(roleId)) {
    ElMessage.error('角色ID无效')
    return
  }

  updatingPerm.value = perm
  // 乐观更新：先更新本地状态
  const oldPermissions = role.permissions
  role.permissions = newPermissions

  updateRole(roleId, {
    name: role.name,
    code: role.code || '',
    description: role.description || '',
    permissions: newPermissions
  })
    .then(() => {
      ElMessage.success(newPermissions.length > oldPermissions.length ? '权限已添加' : '权限已删除')
    })
    .catch((e: any) => {
      // 失败时回滚
      role.permissions = oldPermissions
      ElMessage.error(e?.message || '更新失败')
    })
    .finally(() => {
      if (updatingPerm.value === perm) {
        updatingPerm.value = null
      }
    })
}
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '系统管理' }, { label: '角色权限' }]"
      title="角色权限"
      sub="4 个角色 · 27 个账号"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showCreateDialog = true">+ 新建角色</button>
      </template>
    </PageHead>

    <div class="role-grid">
      <div class="role-list">
        <div
          v-for="r in list"
          :key="r.id"
          class="role-cell"
          :class="{ active: activeRole?.id === r.id }"
          @click="activeRole = r"
        >
          <div class="row" style="justify-content: space-between">
            <div>
              <div style="font-weight: 700">
                {{ r.name }}
                <StatusTag v-if="r.id === 'r_super' || r.id === '1'" variant="primary" style="margin-left: 4px">最高</StatusTag>
              </div>
              <div class="muted small" style="margin-top: 2px">{{ r.description }}</div>
            </div>
            <div style="text-align: right">
              <div style="font-size: 18px; font-weight: 700">{{ r.user_count }}</div>
              <div class="muted small">账号</div>
            </div>
          </div>
        </div>
      </div>
      <div class="role-detail" v-if="activeRole">
        <div class="panel">
          <div class="panel-head">
            <div class="ph-title">{{ activeRole.name }} · 权限配置</div>
            <span class="muted small">{{ activeRole.user_count }} 个账号使用</span>
          </div>
          <div class="panel-body">
            <div v-if="activeRole.permissions.includes('*')" class="all-perm">
              <el-icon><Star /></el-icon> 超级管理员拥有全部权限
            </div>
            <div v-else class="perm-grid">
              <div
                v-for="p in allPermissions"
                :key="p.key"
                class="perm-cell"
                :class="{ on: activeRole.permissions.includes(p.key) }"
                @click="togglePerm(activeRole, p.key)"
              >
                <span class="ck" :class="{ checked: activeRole.permissions.includes(p.key) }">
                  <el-icon v-if="updatingPerm === p.key" class="is-loading"><Loading /></el-icon>
                </span>
                <div>
                  <div style="font-weight: 600; font-size: 13px">{{ p.name }}</div>
                  <div class="muted small">权限标识 {{ p.key }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 新建角色弹窗 -->
    <el-dialog
      v-model="showCreateDialog"
      title="新建角色"
      width="480px"
      :close-on-click-modal="false"
      :append-to-body="false"
      destroy-on-close
    >
      <el-form label-width="80px">
        <el-form-item label="角色名称" required>
          <el-input v-model="createForm.name" placeholder="请输入角色名称，如：内容审核员" />
        </el-form-item>
        <el-form-item label="角色编码" required>
          <el-input v-model="createForm.code" placeholder="请输入角色编码，如：moderator" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="createForm.description" type="textarea" :rows="2" placeholder="请输入角色描述" />
        </el-form-item>
        <el-form-item label="权限">
          <div class="perm-select-grid">
            <div
              v-for="p in allPermissions"
              :key="p.key"
              class="perm-select-cell"
              :class="{ on: createForm.permissions.includes(p.key) }"
              @click="() => {
                if (createForm.permissions.includes(p.key)) {
                  createForm.permissions = createForm.permissions.filter(x => x !== p.key)
                } else {
                  createForm.permissions.push(p.key)
                }
              }"
            >
              <span class="ck" :class="{ checked: createForm.permissions.includes(p.key) }"></span>
              <span>{{ p.name }}</span>
            </div>
          </div>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showCreateDialog = false">取消</el-button>
        <el-button type="primary" :loading="isSubmitting" @click="handleCreate">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.role-grid { display: grid; grid-template-columns: 320px 1fr; gap: 16px; padding: 16px 22px; flex: 1; overflow: hidden; background: var(--bg-2); }
.role-list { display: flex; flex-direction: column; gap: 8px; overflow-y: auto; }
.role-cell { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; padding: 12px 14px; cursor: pointer; transition: border-color .12s, transform .08s; }
.role-cell:hover { border-color: var(--ink-3); }
.role-cell.active { border-color: var(--ink); background: var(--surface-2); }
.role-detail { overflow-y: auto; }
.perm-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 8px; }
.perm-cell { display: flex; align-items: center; gap: 10px; padding: 10px 12px; border: 1px solid var(--line); border-radius: 8px; cursor: pointer; transition: border-color .12s, background .12s; }
.perm-cell:hover { border-color: var(--ink-3); }
.perm-cell.on { background: var(--primary-soft); border-color: var(--primary); }
.ck.checked { background: var(--primary); border-color: var(--primary); }
.all-perm { padding: 40px; text-align: center; font-size: 14px; color: var(--ink-2); background: var(--primary-soft); border-radius: 10px; }
.perm-select-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 8px; }
.perm-select-cell { display: flex; align-items: center; gap: 8px; padding: 8px 10px; border: 1px solid var(--line); border-radius: 6px; cursor: pointer; transition: border-color .12s, background .12s; font-size: 13px; }
.perm-select-cell:hover { border-color: var(--ink-3); }
.perm-select-cell.on { background: var(--primary-soft); border-color: var(--primary); }
</style>