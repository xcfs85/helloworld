<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { templateApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const loading = ref(false)
const query = ref({ page: 1, size: 20, keyword: '' })

const load = async () => {
  loading.value = true
  try {
    const data = await templateApi.list(query.value)
    list.value = data.list
  } finally {
    loading.value = false
  }
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认删除?', '提示', { type: 'warning' })
    ElMessage.success('已删除')
    load()
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="left">
        <el-input v-model="query.keyword" placeholder="搜索模板" clearable style="width: 240px" />
      </div>
      <div class="right">
        <el-button type="primary" @click="load">搜索</el-button>
      </div>
    </div>

    <el-table v-loading="loading" :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column label="封面" width="120">
        <template #default="{ row }">
          <el-image :src="row.coverUrl" style="width: 60px; height: 60px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="name" label="名称" />
      <el-table-column prop="boardSize" label="规格" width="100" />
      <el-table-column prop="beadCount" label="颗数" width="100" />
      <el-table-column prop="difficulty" label="难度" width="100" />
      <el-table-column prop="useCount" label="使用数" width="100" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.reviewStatus === 'approved'" type="success">已上架</el-tag>
          <el-tag v-else-if="row.reviewStatus === 'pending'" type="warning">待审核</el-tag>
          <el-tag v-else type="danger">已拒绝</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small">查看</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
