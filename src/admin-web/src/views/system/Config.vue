<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { configApi } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const load = async () => {
  list.value = await configApi.list()
}

const handleUpdate = async (row: any) => {
  await configApi.update(row.configKey, row.configValue)
  ElMessage.success('已更新')
}
onMounted(load)
</script>
<template>
  <div class="page-container">
    <el-table :data="list" border stripe>
      <el-table-column prop="configKey" label="键" width="200" />
      <el-table-column prop="description" label="描述" width="200" />
      <el-table-column prop="configType" label="类型" width="100" />
      <el-table-column label="值">
        <template #default="{ row }">
          <el-input v-model="row.configValue" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleUpdate(row)">保存</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
