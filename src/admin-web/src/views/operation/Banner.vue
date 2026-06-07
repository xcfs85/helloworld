<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { bannerApi } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const dialogVisible = ref(false)
const form = ref({ id: '', title: '', imageUrl: '', linkType: 'url', linkValue: '', position: 'home_top', sort: 0, status: 'active', startTime: '', endTime: '' })

const load = async () => {
  const data = await bannerApi.list()
  list.value = data.list
}

const handleSave = () => {
  ElMessage.success('保存成功')
  dialogVisible.value = false
  load()
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认删除?', '提示', { type: 'warning' })
    await bannerApi.remove(row.id)
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
        <el-button type="primary" @click="form = { id: '', title: '', imageUrl: '', linkType: 'url', linkValue: '', position: 'home_top', sort: 0, status: 'active', startTime: '', endTime: '' }; dialogVisible = true">新增Banner</el-button>
      </div>
    </div>

    <el-table :data="list" border stripe>
      <el-table-column prop="id" label="ID" width="100" />
      <el-table-column label="图片" width="200">
        <template #default="{ row }">
          <el-image :src="row.imageUrl" style="width: 160px; height: 60px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="linkType" label="跳转" width="100" />
      <el-table-column prop="position" label="位置" width="120" />
      <el-table-column prop="sort" label="排序" width="80" />
      <el-table-column prop="status" label="状态" width="100" />
      <el-table-column label="操作" width="160" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="form = row; dialogVisible = true">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="dialogVisible" :title="form.id ? '编辑Banner' : '新增Banner'" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="图片URL"><el-input v-model="form.imageUrl" /></el-form-item>
        <el-form-item label="跳转类型">
          <el-select v-model="form.linkType">
            <el-option label="URL" value="url" />
            <el-option label="帖子" value="post" />
            <el-option label="模板" value="template" />
          </el-select>
        </el-form-item>
        <el-form-item label="跳转值"><el-input v-model="form.linkValue" /></el-form-item>
        <el-form-item label="位置">
          <el-select v-model="form.position">
            <el-option label="首页顶部" value="home_top" />
            <el-option label="模板页" value="template_top" />
          </el-select>
        </el-form-item>
        <el-form-item label="排序"><el-input-number v-model="form.sort" :min="0" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>
