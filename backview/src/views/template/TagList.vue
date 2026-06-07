<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listTags, deleteTag } from '@/api/template'
import type { Tag } from '@/types'

const loading = ref(false)
const list = ref<Tag[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ keyword: '', category_id: 'all' })
const showAdd = ref(false)
const newTag = ref({ name: '', category_id: '1', desc: '' })

async function load() {
  loading.value = true
  try {
    const res: any = await listTags({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

async function onDelete(t: Tag) {
  try {
    await ElMessageBox.confirm(`确定删除标签「${t.name}」？`, '提示', { type: 'warning' })
    await deleteTag(t.id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '模板管理' }, { label: '标签管理' }]"
      title="标签管理"
      sub="共 28 个标签 · 5 个标签热度 ≥ 100"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 新建标签</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="标签名" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.category_id" @change="load">
        <option value="all">分类：全部</option><option value="1">节日</option><option value="2">卡通</option><option value="3">风景</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>标签名</th>
            <th>分类</th>
            <th>使用次数</th>
            <th>热度</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="t in list" :key="t.id">
            <td><span style="font-weight: 600; color: var(--ink)">#{{ t.name }}</span></td>
            <td><span class="chip">{{ t.category_name }}</span></td>
            <td>{{ t.use_count }}</td>
            <td>
              <div class="row" style="gap: 6px">
                <div style="width: 100px"><div class="progress"><div class="fill" :style="`width: ${Math.min(t.use_count, 100)}%`"></div></div></div>
                <span class="muted small">{{ t.use_count > 100 ? '高' : t.use_count > 50 ? '中' : '低' }}</span>
              </div>
            </td>
            <td class="col-actions">
              <button class="btn btn-xs btn-ghost">编辑</button>
              <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onDelete(t)">删除</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="新建标签" width="420">
      <div class="form-row"><div class="lbl req">标签名</div>
        <div class="input-line"><input v-model="newTag.name" placeholder="标签名" /></div>
      </div>
      <div class="form-row"><div class="lbl">所属分类</div>
        <div class="f-select" style="width: 100%"><select v-model="newTag.category_id">
          <option value="1">节日</option><option value="2">卡通</option><option value="3">风景</option><option value="4">花卉</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl">描述</div>
        <div class="input-line"><input v-model="newTag.desc" placeholder="选填" /></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="showAdd = false; ElMessage.success('已创建'); newTag = { name: '', category_id: '1', desc: '' }">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
