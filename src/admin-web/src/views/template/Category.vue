<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { templateApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', name: '', icon: '', sort: 0 })

const load = async () => {
  list.value = await templateApi.categories()
}

const handleSave = () => {
  ElMessage.success('保存成功')
  dialogVisible.value = false
  load()
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="primary" @click="form = { id: '', name: '', icon: '', sort: 0 }; dialogVisible = true">新增分类</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="icon" label="图标" width="120" />
      <el-table-column prop="sort" label="排序" width="100" />
      <el-table-column prop="templateCount" label="模板数" width="100" />
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = row; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑分类' : '新增分类'" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="图标URL"><el-input v-model="form.icon" /></el-form-item>
        <el-form-item label="排序"><el-input-number v-model="form.sort" :min="0" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
