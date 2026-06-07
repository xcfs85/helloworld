<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { reportApi } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const query = ref({ page: 1, size: 20, status: 'pending' })

const load = async () => {
  const data = await reportApi.list(query.value)
  list.value = data.list
}

const handle = async (row: any, action: string) => {
  await reportApi.handle(row.id, { action })
  ElMessage.success('已处理')
  load()
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <el-table :data="list" border stripe>
      <el-table-column prop="reportId" label="举报ID" width="120" />
      <el-table-column prop="reporterName" label="举报人" width="100" />
      <el-table-column prop="targetType" label="类型" width="80" />
      <el-table-column prop="reason" label="原因" width="100" />
      <el-table-column prop="content" label="详情" show-overflow-tooltip />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.status === 'pending'" type="warning">待处理</el-tag>
          <el-tag v-else-if="row.status === 'ignored'" type="info">已忽略</el-tag>
          <el-tag v-else-if="row.status === 'warned'" type="success">已警告</el-tag>
          <el-tag v-else-if="row.status === 'ban_content'" type="danger">封禁内容</el-tag>
          <el-tag v-else type="danger">封禁用户</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="举报时间" width="170" />
      <el-table-column label="操作" width="240" fixed="right">
        <template #default="{ row }">
          <el-button v-if="row.status === 'pending'" type="success" size="small" @click="handle(row, 'warned')">警告</el-button>
          <el-button v-if="row.status === 'pending'" type="danger" size="small" @click="handle(row, 'ban_content')">封禁内容</el-button>
          <el-button v-if="row.status === 'pending'" type="danger" size="small" @click="handle(row, 'ban_user')">封禁用户</el-button>
          <el-button v-if="row.status === 'pending'" size="small" @click="handle(row, 'ignored')">忽略</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
