<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listRoles } from '@/api/auth'
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

onMounted(async () => {
  const res: any = await listRoles({})
  list.value = res.list
  activeRole.value = list.value[0]
})

function togglePerm(role: Role, perm: string) {
  if (role.permissions.includes('*')) return
  if (role.permissions.includes(perm)) {
    role.permissions = role.permissions.filter(p => p !== perm)
  } else {
    role.permissions.push(perm)
  }
  ElMessage.success('已更新')
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
        <button class="btn btn-primary">+ 新建角色</button>
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
                <StatusTag v-if="r.id === 'r_admin'" variant="primary" style="margin-left: 4px">最高</StatusTag>
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
                <span class="ck" :class="{ checked: activeRole.permissions.includes(p.key) }"></span>
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
</style>
