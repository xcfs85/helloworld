<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { kitApi } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', kitId: '', kitName: '', brand: 'MARD', colorCount: 0, beadCount: 0, price: 0, purchaseUrl: '' })

const load = async () => {
  const data = await kitApi.list()
  list.value = data.list
}

const handleSave = async () => {
  if (form.value.id) await kitApi.update(form.value.id, form.value)
  else await kitApi.create(form.value)
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
        <el-button type="primary" @click="form = { id: '', kitId: '', kitName: '', brand: 'MARD', colorCount: 0, beadCount: 0, price: 0, purchaseUrl: '' }; dialogVisible = true">新增套装</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="kitId" label="套装ID" width="120" />
      <el-table-column prop="kitName" label="名称" />
      <el-table-column prop="brand" label="品牌" width="100" />
      <el-table-column prop="colorCount" label="色数" width="80" />
      <el-table-column prop="beadCount" label="颗数" width="100" />
      <el-table-column prop="price" label="价格" width="100" />
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = row; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑套装' : '新增套装'" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="套装ID"><el-input v-model="form.kitId" /></el-form-item>
        <el-form-item label="名称"><el-input v-model="form.kitName" /></el-form-item>
        <el-form-item label="品牌"><el-input v-model="form.brand" /></el-form-item>
        <el-form-item label="色数"><el-input-number v-model="form.colorCount" :min="0" /></el-form-item>
        <el-form-item label="颗数"><el-input-number v-model="form.beadCount" :min="0" /></el-form-item>
        <el-form-item label="价格"><el-input-number v-model="form.price" :min="0" :precision="2" /></el-form-item>
        <el-form-item label="购买链接"><el-input v-model="form.purchaseUrl" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
