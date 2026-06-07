<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { sensitiveApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', word: '', level: 2, type: 'other', replaceWord: '', status: 1 })

const load = async () => {
  const data = await sensitiveApi.list()
  list.value = data.list
}

const handleAdd = () => {
  form.value = { id: '', word: '', level: 2, type: 'other', replaceWord: '', status: 1 }
  dialogVisible.value = true
}

const handleSave = async () => {
  if (form.value.id) {
    await sensitiveApi.update(form.value.id, form.value)
  } else {
    await sensitiveApi.add(form.value)
  }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  load()
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认删除?', '提示', { type: 'warning' })
    await sensitiveApi.remove(row.id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="primary" @click="handleAdd">新增敏感词</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="word" label="敏感词" />
      <el-table-column label="级别" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.level === 1">警告</el-tag>
          <el-tag v-else-if="row.level === 2" type="warning">替换</el-tag>
          <el-tag v-else type="danger">拦截</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="type" label="类型" width="120" />
      <el-table-column prop="replaceWord" label="替换词" width="120" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.status === 1" type="success">启用</el-tag>
          <el-tag v-else type="info">禁用</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = row; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑敏感词' : '新增敏感词'" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="敏感词">
          <el-input v-model="form.word" />
        </el-form-item>
        <el-form-item label="级别">
          <el-select v-model="form.level">
            <el-option label="警告" :value="1" />
            <el-option label="替换" :value="2" />
            <el-option label="拦截" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="form.type">
            <el-option label="政治" value="politics" />
            <el-option label="色情" value="porn" />
            <el-option label="暴力" value="violence" />
            <el-option label="广告" value="ad" />
            <el-option label="其他" value="other" />
          </el-select>
        </el-form-item>
        <el-form-item label="替换词">
          <el-input v-model="form.replaceWord" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
