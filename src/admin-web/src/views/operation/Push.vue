<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { pushApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', title: '', content: '', pushType: 'system', targetType: 'all', targetParam: '', scheduleTime: '' })

const load = async () => {
  const data = await pushApi.list()
  list.value = data.list
}

const handleSend = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认发送?', '提示', { type: 'warning' })
    await pushApi.send(row.id)
    ElMessage.success('已发送')
    load()
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="primary" @click="form = { id: '', title: '', content: '', pushType: 'system', targetType: 'all', targetParam: '', scheduleTime: '' }; dialogVisible = true">新建推送</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="pushId" label="ID" width="100" />
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="content" label="内容" show-overflow-tooltip />
      <el-table-column prop="pushType" label="类型" width="100" />
      <el-table-column prop="targetType" label="目标" width="100" />
      <el-table-column prop="totalCount" label="总数" width="100" />
      <el-table-column prop="successCount" label="成功" width="100" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.status === 'sent'" type="success">已发送</el-tag>
          <el-tag v-else-if="row.status === 'pending'" type="warning">待发送</el-tag>
          <el-tag v-else type="info">{{ row.status }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="120" fixed="right">
        <template #default="{ row }">
          <el-button v-if="row.status === 'pending'" type="primary" size="small" @click="handleSend(row)">发送</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" title="新建推送" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="内容"><el-input v-model="form.content" type="textarea" :rows="4" /></el-form-item>
        <el-form-item label="类型">
          <el-select v-model="form.pushType">
            <el-option label="系统" value="system" />
            <el-option label="活动" value="activity" />
          </el-select>
        </el-form-item>
        <el-form-item label="目标">
          <el-select v-model="form.targetType">
            <el-option label="全部" value="all" />
            <el-option label="标签" value="tag" />
            <el-option label="用户" value="user" />
          </el-select>
        </el-form-item>
        <el-form-item label="定时时间">
          <el-date-picker v-model="form.scheduleTime" type="datetime" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="ElMessage.success('已保存')">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
