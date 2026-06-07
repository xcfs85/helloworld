<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listSpecials, addSpecial, updateSpecial } from '@/api/operation'
import type { Special } from '@/types'

const loading = ref(false)
const list = ref<Special[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const showAdd = ref(false)
const newSpecial = ref({ title: '', cover: '', desc: '', template_ids: [] as string[] })

async function load() {
  loading.value = true
  try {
    const res: any = await listSpecials({ page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}
async function onAdd() {
  if (!newSpecial.value.title) { ElMessage.warning('请输入专题名'); return }
  await addSpecial(newSpecial.value)
  ElMessage.success('已创建')
  showAdd.value = false
  load()
}
async function onToggleStatus(s: Special) {
  await updateSpecial(s.id, { status: s.status === 'online' ? 'offline' : 'online' })
  ElMessage.success('已切换')
  load()
}

const covers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)'
]
function coverFor(s: Special) { return covers[parseInt(s.id.slice(-1)) % covers.length] }

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营管理' }, { label: '专题管理' }]"
      title="专题管理"
      sub="共 2 个专题 · 1 个上线中"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 新建专题</button>
      </template>
    </PageHead>

    <div class="sp-grid">
      <div v-for="s in list" :key="s.id" class="sp-card" :class="{ offline: s.status === 'offline' }">
        <div class="sp-cover" :style="`background: ${coverFor(s)}`">
          <span class="sp-status" :class="s.status === 'online' ? 'ok' : 'offline'">
            {{ s.status === 'online' ? '上线中' : '已下线' }}
          </span>
        </div>
        <div class="sp-body">
          <div class="sp-title">{{ s.title }}</div>
          <div class="sp-desc">{{ s.desc }}</div>
          <div class="row" style="margin-top: 8px; gap: 4px; flex-wrap: wrap">
            <span v-for="id in s.template_ids" :key="id" class="chip">{{ id }}</span>
          </div>
          <div class="sp-foot">
            <span class="muted small">{{ s.create_time }}</span>
            <div class="row" style="gap: 4px">
              <button class="btn btn-xs btn-ghost">编辑</button>
              <button class="btn btn-xs btn-ghost" @click="onToggleStatus(s)">{{ s.status === 'online' ? '下线' : '上线' }}</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="新建专题" width="540">
      <div class="form-row"><div class="lbl req">专题标题</div>
        <div class="input-line"><input v-model="newSpecial.title" placeholder="专题标题" /></div>
      </div>
      <div class="form-row"><div class="lbl">封面</div>
        <div class="input-line"><input v-model="newSpecial.cover" placeholder="封面图片 URL" /></div>
      </div>
      <div class="form-row"><div class="lbl">描述</div>
        <div class="input-line"><input v-model="newSpecial.desc" placeholder="专题描述" /></div>
      </div>
      <div class="form-row"><div class="lbl">模板</div>
        <div class="input-line"><input placeholder="关联模板 ID（逗号分隔）" /></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="onAdd">创建</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.sp-grid { padding: 16px 22px; display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.sp-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; overflow: hidden; transition: transform .12s, box-shadow .12s; }
.sp-card:hover { transform: translateY(-2px); box-shadow: var(--shadow); }
.sp-card.offline { opacity: 0.65; }
.sp-cover { aspect-ratio: 16/9; position: relative; }
.sp-status { position: absolute; top: 8px; right: 8px; padding: 2px 8px; border-radius: 4px; font-size: 10px; font-weight: 600; }
.sp-status.ok { background: var(--mint); color: #fff; }
.sp-status.offline { background: var(--ink-3); color: #fff; }
.sp-body { padding: 12px 14px; }
.sp-title { font-weight: 700; font-size: 14px; margin-bottom: 4px; }
.sp-desc { font-size: 12px; color: var(--ink-3); line-height: 1.5; min-height: 32px; }
.sp-foot { display: flex; align-items: center; justify-content: space-between; margin-top: 10px; padding-top: 8px; border-top: 1px solid var(--line); }
</style>
