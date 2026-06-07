<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listAdmins } from '@/api/auth'
import type { AdminAccount } from '@/types'

const loading = ref(false)
const list = ref<AdminAccount[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ keyword: '', status: 'all' })
const showAdd = ref(false)
const newAdmin = ref({ username: '', nickname: '', email: '', role_id: 'r_cs' })

async function load() {
  loading.value = true
  try {
    const res: any = await listAdmins({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}
async function onAdd() {
  if (!newAdmin.value.username) { ElMessage.warning('请输入账号'); return }
  ElMessage.success('已创建（演示）')
  showAdd.value = false
  newAdmin.value = { username: '', nickname: '', email: '', role_id: 'r_cs' }
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '系统管理' }, { label: '账号管理' }]"
      title="账号管理"
      sub="共 5 个管理员账号 · 2 个超级管理员"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 新增账号</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="账号 / 昵称 / 邮箱" @keydown.enter="load" />
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
            <th>邮箱</th>
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
                <div class="av" :class="'c' + ((parseInt(a.id.slice(-1)) % 6) + 1)">{{ a.nickname[0] }}</div>
                <div class="meta"><div class="nm">{{ a.nickname }}</div><div class="id">{{ a.username }}</div></div>
              </div>
            </td>
            <td class="muted">{{ a.email }}</td>
            <td><StatusTag :variant="a.role_id === 'r_admin' ? 'primary' : 'neutral'">{{ a.role_name }}</StatusTag></td>
            <td>
              <span class="switch" :class="{ on: a.status === 'active' }"></span>
              <span class="muted small" style="margin-left: 6px">{{ a.status === 'active' ? '启用' : '禁用' }}</span>
            </td>
            <td class="muted">{{ a.last_login_time }}</td>
            <td class="muted mono">{{ a.last_login_ip }}</td>
            <td class="muted">{{ a.create_time }}</td>
            <td class="col-actions">
              <button class="btn btn-xs btn-ghost">编辑</button>
              <button class="btn btn-xs btn-ghost">重置密码</button>
              <button v-if="a.status === 'active'" class="btn btn-xs btn-ghost" style="color: var(--rose)">禁用</button>
              <button v-else class="btn btn-xs btn-ghost" style="color: var(--mint)">启用</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="新增账号" width="480">
      <div class="form-row"><div class="lbl req">账号</div>
        <div class="input-line"><input v-model="newAdmin.username" placeholder="登录账号" /></div>
      </div>
      <div class="form-row"><div class="lbl req">昵称</div>
        <div class="input-line"><input v-model="newAdmin.nickname" placeholder="昵称" /></div>
      </div>
      <div class="form-row"><div class="lbl">邮箱</div>
        <div class="input-line"><input v-model="newAdmin.email" placeholder="邮箱" /></div>
      </div>
      <div class="form-row"><div class="lbl req">角色</div>
        <div class="f-select" style="width: 100%"><select v-model="newAdmin.role_id">
          <option value="r_admin">超级管理员</option><option value="r_ops">运营</option><option value="r_audit">审核</option><option value="r_cs">客服</option>
        </select></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="onAdd">创建</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
