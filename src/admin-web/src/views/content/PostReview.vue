<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { postApi, commentApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const loading = ref(false)
const list = ref<any[]>([])
const total = ref(0)
const activeTab = ref('post')
const query = ref({ page: 1, size: 20 })

const loadData = async () => {
  loading.value = true
  try {
    const data = activeTab.value === 'post'
      ? await postApi.pendingList(query.value)
      : await commentApi.pendingList(query.value)
    list.value = data.list
    total.value = data.total
  } finally {
    loading.value = false
  }
}

const handleApprove = async (row: any) => {
  if (activeTab.value === 'post') {
    await postApi.approve(row.id)
  } else {
    await commentApi.approve(row.id)
  }
  ElMessage.success('已通过')
  loadData()
}

const handleReject = async (row: any) => {
  try {
    const { value } = await ElMessageBox.prompt('请输入拒绝原因', '拒绝', { inputPattern: /.+/, inputErrorMessage: '原因不能为空' })
    if (activeTab.value === 'post') {
      await postApi.reject(row.id, value)
    } else {
      await commentApi.reject(row.id)
    }
    ElMessage.success('已拒绝')
    loadData()
  } catch {}
}

onMounted(loadData)
</script>

<template>
  <div class="page-container">
    <el-tabs v-model="activeTab" @tab-change="loadData">
      <el-tab-pane label="帖子审核" name="post" />
      <el-tab-pane label="评论审核" name="comment" />
    </el-tabs>

    <el-table v-loading="loading" :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="content" label="内容" show-overflow-tooltip />
      <el-table-column prop="userId" label="作者" width="120" />
      <el-table-column prop="publishTime" label="提交时间" width="170" />
      <el-table-column label="操作" width="180" fixed="right">
        <template #default="{ row }">
          <el-button type="success" size="small" @click="handleApprove(row)">通过</el-button>
          <el-button type="danger" size="small" @click="handleReject(row)">拒绝</el-button>
        </template>
      </el-table-column>
    </el-table>

    <div class="pagination-wrapper">
      <el-pagination
        v-model:current-page="query.page"
        v-model:page-size="query.size"
        :total="total"
        layout="total, sizes, prev, pager, next, jumper"
        :page-sizes="[10, 20, 50, 100]"
        @current-change="loadData"
        @size-change="loadData"
      />
    </div>
  </div>
</template>
