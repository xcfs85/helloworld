<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { templateApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const load = async () => {
  const data = await templateApi.pending({ page: 1, size: 20 })
  list.value = data.list
}

const handleApprove = async (row: any) => {
  await templateApi.approve(row.id)
  ElMessage.success('已通过')
  load()
}

const handleReject = async (row: any) => {
  try {
    const { value } = await ElMessageBox.prompt('请输入拒绝原因', '拒绝', { inputPattern: /.+/, inputErrorMessage: '原因不能为空' })
    await templateApi.reject(row.id, value)
    ElMessage.success('已拒绝')
    load()
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column label="封面" width="120">
        <template #default="{ row }">
          <el-image :src="row.coverUrl" style="width: 80px; height: 80px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="creatorName" label="创作者" width="120" />
      <el-table-column prop="boardSize" label="规格" width="100" />
      <el-table-column prop="beadCount" label="颗数" width="100" />
      <el-table-column prop="createTime" label="提交时间" width="170" />
      <el-table-column label="操作" width="180" fixed="right">
        <template #default="{ row }">
          <el-button type="success" size="small" @click="handleApprove(row)">通过</el-button>
          <el-button type="danger" size="small" @click="handleReject(row)">拒绝</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
