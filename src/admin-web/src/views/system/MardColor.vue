<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { mardApi } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', colorNo: '', colorName: '', rgb: '', category: '', isCommon: 0 })

const load = async () => {
  const data = await mardApi.list()
  list.value = data.list
}

const handleSave = async () => {
  if (form.value.id) await mardApi.update(form.value.id, form.value)
  else await mardApi.create(form.value)
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
        <el-button type="primary" @click="form = { id: '', colorNo: '', colorName: '', rgb: '', category: '', isCommon: 0 }; dialogVisible = true">新增色号</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="colorNo" label="色号" width="100" />
      <el-table-column label="颜色" width="80">
        <template #default="{ row }">
          <div :style="{ width: '40px', height: '20px', background: `rgb(${row.rgb})`, border: '1px solid #ddd', borderRadius: '2px' }" />
        </template>
      </el-table-column>
      <el-table-column prop="rgb" label="RGB" width="120" />
      <el-table-column prop="colorName" label="名称" />
      <el-table-column prop="category" label="分类" width="100" />
      <el-table-column label="常用" width="80">
        <template #default="{ row }">
          <el-tag v-if="row.isCommon === 1" type="success">是</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = row; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑色号' : '新增色号'" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="色号"><el-input v-model="form.colorNo" /></el-form-item>
        <el-form-item label="名称"><el-input v-model="form.colorName" /></el-form-item>
        <el-form-item label="RGB"><el-input v-model="form.rgb" placeholder="255,255,255" /></el-form-item>
        <el-form-item label="分类">
          <el-select v-model="form.category">
            <el-option label="红" value="red" />
            <el-option label="橙" value="orange" />
            <el-option label="黄" value="yellow" />
            <el-option label="绿" value="green" />
            <el-option label="蓝" value="blue" />
            <el-option label="紫" value="purple" />
            <el-option label="灰" value="gray" />
            <el-option label="黑" value="black" />
            <el-option label="白" value="white" />
            <el-option label="特殊" value="special" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
