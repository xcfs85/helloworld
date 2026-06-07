<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { adminApi, roleApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const roles = ref<any[]>([])
const dialogVisible = ref(false)
const dialogTitle = ref('新增账号')
const form = ref({ id: 0, username: '', password: '', nickname: '', roleId: 0, status: 1 })

const load = async () => {
  const data = await adminApi.list({ page: 1, size: 20 })
  list.value = data.list
  roles.value = await roleApi.all()
}

const handleAdd = () => {
  dialogTitle.value = '新增账号'
  form.value = { id: 0, username: '', password: '', nickname: '', roleId: roles.value[0]?.id || 0, status: 1 }
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  dialogTitle.value = '编辑账号'
  form.value = { ...row, password: '' }
  dialogVisible.value = true
}

const handleSave = async () => {
  if (form.value.id) {
    await adminApi.update(form.value.id, form.value)
  } else {
    await adminApi.create(form.value)
  }
  ElMessage.success('已保存')
  dialogVisible.value = false
  load()
}

const handleResetPassword = async (row: any) => {
  try {
    const { value } = await ElMessageBox.prompt('新密码', '重置密码', { inputPattern: /.{6,}/, inputErrorMessage: '密码至少6位' })
    await adminApi.resetPassword(row.id, value)
    ElMessage.success('已重置')
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button v-permission="'admin:add'" type="primary" @click="handleAdd">新增账号</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="username" label="用户名" />
      <el-table-column prop="nickname" label="昵称" />
      <el-table-column prop="roleName" label="角色" width="120" />
      <el-table-column label="状态" width="80">
        <template #default="{ row }">
          <el-tag v-if="row.status === 1" type="success">启用</el-tag>
          <el-tag v-else type="danger">禁用</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="lastLoginTime" label="最后登录" width="170" />
      <el-table-column prop="lastLoginIp" label="登录IP" width="120" />
      <el-table-column label="操作" width="240" fixed="right">
        <template #default="{ row }">
          <el-button v-permission="'admin:edit'" type="primary" size="small" @click="handleEdit(row)">编辑</el-button>
          <el-button v-permission="'admin:reset-password'" type="warning" size="small" @click="handleResetPassword(row)">重置密码</el-button>
          <el-button v-permission="'admin:delete'" type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="用户名">
          <el-input v-model="form.username" :disabled="form.id > 0" />
        </el-form-item>
        <el-form-item label="昵称">
          <el-input v-model="form.nickname" />
        </el-form-item>
        <el-form-item label="密码" v-if="!form.id">
          <el-input v-model="form.password" type="password" show-password />
        </el-form-item>
        <el-form-item label="角色">
          <el-select v-model="form.roleId">
            <el-option v-for="r in roles" :key="r.id" :label="r.name" :value="r.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-radio-group v-model="form.status">
            <el-radio :value="1">启用</el-radio>
            <el-radio :value="0">禁用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
