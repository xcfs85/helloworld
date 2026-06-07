<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { operationLogApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const query = ref({ page: 1, size: 20 })

const load = async () => {
  const data = await operationLogApi.list(query.value)
  list.value = data.list
}

const handleClear = async () => {
  try {
    await ElMessageBox.confirm('确认清空所有日志?', '提示', { type: 'warning' })
    await operationLogApi.clear()
    ElMessage.success('已清空')
    load()
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="danger" @click="handleClear">清空日志</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="username" label="用户" width="120" />
      <el-table-column prop="operation" label="操作" width="150" />
      <el-table-column prop="content" label="内容" />
      <el-table-column prop="method" label="方法" width="100" />
      <el-table-column prop="ip" label="IP" width="120" />
      <el-table-column prop="createTime" label="时间" width="170" />
    </el-table>
  </div>
</template>
