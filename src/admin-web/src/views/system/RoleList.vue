<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { roleApi } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const allPermissions = ref([
  'user:view', 'user:edit', 'user:delete', 'user:disable',
  'member:view', 'member:edit',
  'order:view', 'order:refund',
  'post:view', 'post:review', 'post:delete',
  'comment:view', 'comment:review', 'comment:delete',
  'sensitive:view', 'sensitive:edit',
  'report:view', 'report:handle',
  'template:view', 'template:add', 'template:edit', 'template:delete', 'template:review',
  'banner:view', 'banner:edit',
  'topic:view', 'topic:edit',
  'special-topic:view', 'special-topic:edit',
  'push:view', 'push:edit',
  'stats:view',
  'config:view', 'config:edit',
  'mard:view', 'mard:edit',
  'kit:view', 'kit:edit',
  'admin:view', 'admin:add', 'admin:edit', 'admin:delete', 'admin:reset-password', 'admin:status',
  'role:view', 'role:add', 'role:edit', 'role:delete',
  'log:view', 'log:delete', 'log:clear'
])

const dialogVisible = ref(false)
const form = ref({ id: 0, name: '', code: '', description: '', permissions: [] as string[] })

const load = async () => {
  const data = await roleApi.list()
  list.value = data.list
}

const handleSave = async () => {
  if (form.value.id) await roleApi.update(form.value.id, form.value)
  else await roleApi.create(form.value)
  ElMessage.success('已保存')
  dialogVisible.value = false
  load()
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="primary" @click="form = { id: 0, name: '', code: '', description: '', permissions: [] }; dialogVisible = true">新增角色</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="code" label="编码" width="150" />
      <el-table-column prop="description" label="描述" />
      <el-table-column prop="createTime" label="创建时间" width="170" />
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = { id: row.id, name: row.name, code: row.code, description: row.description, permissions: row.permissions }; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑角色' : '新增角色'" width="700px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="编码"><el-input v-model="form.code" :disabled="form.id > 0" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" /></el-form-item>
        <el-form-item label="权限">
          <el-checkbox-group v-model="form.permissions">
            <el-checkbox
              v-for="p in allPermissions"
              :key="p"
              :label="p"
              :value="p"
            />
          </el-checkbox-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
