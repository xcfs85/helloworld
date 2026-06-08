<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listCategories, addCategory, updateCategory, deleteCategory } from '@/api/template'
import type { Category } from '@/types'

const list = ref<Category[]>([])
const loading = ref(false)
const showAdd = ref(false)
const editing = ref<Category | null>(null)
const newCategory = ref({ name: '', sort: 99, status: 'visible' })

async function load() {
  loading.value = true
  try { list.value = await listCategories() } finally { loading.value = false }
}

async function onSave() {
  if (editing.value) {
    await updateCategory(editing.value.id, editing.value)
    ElMessage.success('已更新')
  } else {
    if (!newCategory.value.name) { ElMessage.warning('请输入分类名'); return }
    await addCategory(newCategory.value)
    ElMessage.success('已添加')
  }
  showAdd.value = false
  editing.value = null
  newCategory.value = { name: '', sort: 99, status: 'visible' }
  load()
}

async function onDelete(c: Category) {
  try {
    await ElMessageBox.confirm(`确定删除分类「${c.name}」？关联模板将移至「未分类」`, '提示', { type: 'warning' })
    await deleteCategory(c.id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '模板管理' }, { label: '分类管理' }]"
      title="分类管理"
      sub="共 10 个分类 · 3,221 套模板"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true; editing = null">+ 新建分类</button>
      </template>
    </PageHead>

    <div class="cat-grid">
      <div v-for="c in list" :key="c.id" class="cat-card" :class="{ hidden: c.status === 'hidden' }">
        <div class="cat-cover">
          <div class="cat-emoji">{{ ['🎄','🎨','🌸','🌹','🐱','👤','🍰','✨','©','📦'][parseInt(c.id) - 1] || '📁' }}</div>
          <span v-if="c.status === 'hidden'" class="cat-hidden">已隐藏</span>
        </div>
        <div class="cat-body">
          <div class="cat-name">{{ c.name }}</div>
          <div class="cat-meta">
            <span>{{ c.template_count }} 套</span>
            <span class="dot"></span>
            <span>排序 #{{ c.sort }}</span>
          </div>
          <div class="cat-actions">
            <button class="btn btn-xs btn-ghost" @click="showAdd = true; editing = c">编辑</button>
            <button class="btn btn-xs btn-ghost" @click="updateCategory(c.id, { status: c.status === 'visible' ? 'hidden' : 'visible' }); load()">
              {{ c.status === 'visible' ? '隐藏' : '显示' }}
            </button>
            <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onDelete(c)">删除</button>
          </div>
        </div>
      </div>
    </div>

    <el-dialog v-model="showAdd" :title="editing ? '编辑分类' : '新建分类'" width="420">
      <div class="form-row"><div class="lbl req">分类名</div>
        <div class="input-line"><input v-model="(editing || newCategory).name" placeholder="分类名" /></div>
      </div>
      <div class="form-row"><div class="lbl">排序</div>
        <div class="input-line"><input type="number" v-model="(editing || newCategory).sort" /></div>
      </div>
      <div class="form-row"><div class="lbl">状态</div>
        <div class="f-select" style="width: 100%"><select v-model="(editing || newCategory).status">
          <option value="visible">显示</option><option value="hidden">隐藏</option>
        </select></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="onSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.cat-grid { padding: 16px 22px; display: grid; grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)); gap: 12px; flex: 1; overflow: auto; background: var(--bg-2); }
.cat-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; overflow: hidden; transition: transform .12s, box-shadow .12s; }
.cat-card:hover { transform: translateY(-2px); box-shadow: var(--shadow); }
.cat-card.hidden { opacity: 0.6; }
.cat-cover { height: 90px; background: linear-gradient(135deg, var(--primary-soft), var(--warn-soft)); display: grid; place-items: center; position: relative; }
.cat-emoji { font-size: 36px; }
.cat-hidden { position: absolute; top: 8px; right: 8px; background: var(--ink); color: #fff; font-size: 10px; padding: 2px 6px; border-radius: 4px; }
.cat-body { padding: 12px 14px; }
.cat-name { font-weight: 700; font-size: 14px; }
.cat-meta { display: flex; align-items: center; gap: 6px; font-size: 12px; color: var(--ink-3); margin-top: 3px; }
.cat-actions { display: flex; gap: 4px; margin-top: 10px; padding-top: 8px; border-top: 1px solid var(--line); }
</style>
