<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { userApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const loading = ref(false)
const list = ref<any[]>([])
const total = ref(0)
const query = ref({ page: 1, size: 20, keyword: '' })

const loadData = async () => {
  loading.value = true
  try {
    const data = await userApi.list(query.value)
    list.value = data.list
    total.value = data.total
  } finally {
    loading.value = false
  }
}

const handleDisable = async (row: any) => {
  try {
    const { value } = await ElMessageBox.prompt('请输入禁用原因', '禁用用户', { inputPattern: /.+/, inputErrorMessage: '原因不能为空' })
    await userApi.disable(row.id, value)
    ElMessage.success('已禁用')
    loadData()
  } catch {}
}

const handleEnable = async (row: any) => {
  await userApi.enable(row.id)
  ElMessage.success('已启用')
  loadData()
}

const handleOpenMember = async (row: any) => {
  // 开通会员对话框
  ElMessage.info('请使用会员管理页')
}

onMounted(loadData)
</script>

<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="left">
        <el-input v-model="query.keyword" placeholder="搜索昵称/手机号" clearable style="width: 240px" @keyup.enter="loadData" />
      </div>
      <div class="right">
        <el-button type="primary" @click="loadData">搜索</el-button>
        <el-button @click="loadData">刷新</el-button>
      </div>
    </div>

    <el-table v-loading="loading" :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column label="用户" width="240">
        <template #default="{ row }">
          <div style="display:flex;align-items:center;gap:8px">
            <el-avatar :size="32" :src="row.avatar">{{ row.nickname?.[0] }}</el-avatar>
            <div>
              <div>{{ row.nickname }}</div>
              <div style="color:#909399;font-size:12px">{{ row.phone || '-' }}</div>
            </div>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="gender" label="性别" width="80" />
      <el-table-column prop="city" label="城市" width="120" />
      <el-table-column label="会员" width="120">
        <template #default="{ row }">
          <el-tag v-if="row.isMember" type="success">会员</el-tag>
          <span v-else>-</span>
        </template>
      </el-table-column>
      <el-table-column prop="diagramCount" label="图纸" width="80" />
      <el-table-column prop="postCount" label="帖子" width="80" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.status === 'active'" type="success">正常</el-tag>
          <el-tag v-else type="danger">禁用</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="注册时间" width="170" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button v-if="row.status === 'active'" type="danger" size="small" @click="handleDisable(row)">禁用</el-button>
          <el-button v-else type="success" size="small" @click="handleEnable(row)">启用</el-button>
          <el-button type="primary" size="small" @click="handleOpenMember(row)">开通会员</el-button>
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
