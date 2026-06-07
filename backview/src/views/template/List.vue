<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listTemplates, offlineTemplate } from '@/api/template'
import type { Template } from '@/types'

const router = useRouter()
const tab = ref('all')
const loading = ref(false)
const list = ref<Template[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(12)

const filter = reactive({ keyword: '', category_id: 'all', source: 'all', difficulty: 'all' })

const tabs = [
  { label: '全部', value: 'all', count: 1234 },
  { label: '待审核', value: 'pending', count: 8 },
  { label: '已通过', value: 'approved', count: 1180 },
  { label: '已拒绝', value: 'rejected', count: 12 },
  { label: '已下架', value: 'offline', count: 34 }
]

const covers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)',
  'linear-gradient(135deg,#F5C45E,#FF8A5A)',
  'linear-gradient(135deg,#E07777,#F5C45E)'
]

async function load() {
  loading.value = true
  try {
    const res: any = await listTemplates({ ...filter, status: tab.value, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}
function view(t: Template) { router.push({ name: 'template-review-detail', params: { id: t.id } }) }
async function onOffline(t: Template) {
  await offlineTemplate(t.id)
  ElMessage.success('已下架')
  load()
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '模板管理' }, { label: '模板列表' }]"
      title="模板列表"
      sub="共 1,234 套 · 在线 1,180 · 待审 8"
    >
      <template #actions>
        <button class="btn btn-secondary">分类配置</button>
        <button class="btn btn-secondary">导入</button>
        <button class="btn btn-primary">+ 新建模板</button>
      </template>
    </PageHead>

    <div class="tabs">
      <div v-for="t in tabs" :key="t.value" class="tab-btn" :class="{ active: tab === t.value }" @click="tab = t.value; load()">
        {{ t.label }} <span class="ct">{{ t.count }}</span>
      </div>
    </div>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="模板名称 / 编号 / 作者" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.category_id" @change="load"><option value="all">分类：全部</option><option value="1">节日</option><option value="2">卡通</option><option value="3">风景</option><option value="4">花卉</option></select></div>
      <div class="f-select"><select v-model="filter.source" @change="load"><option value="all">来源：全部</option><option value="official">官方</option><option value="creator">达人</option></select></div>
      <div class="f-select"><select v-model="filter.difficulty" @change="load"><option value="all">难度：全部</option><option value="beginner">入门</option><option value="intermediate">进阶</option><option value="advanced">高阶</option></select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="grid">
      <div v-for="t in list" :key="t.id" class="card" @click="view(t)">
        <div class="card-cover" :style="`background: ${covers[parseInt(t.id.slice(-1)) % covers.length]}`">
          <div class="card-cover-tags">
            <span class="badge-overlay" v-if="t.source === 'official'">官方</span>
            <span class="badge-overlay warn" v-else>达人</span>
            <span class="badge-overlay" style="background: rgba(0,0,0,.4); color: #fff">{{ t.difficulty === 'beginner' ? '入门' : t.difficulty === 'intermediate' ? '进阶' : '高阶' }}</span>
          </div>
          <span class="ribbon" v-if="t.status === 'pending'">待审核</span>
        </div>
        <div class="card-body">
          <div class="row" style="justify-content: space-between">
            <div style="font-weight: 700; font-size: 13.5px">{{ t.name }}</div>
            <div class="row" style="gap: 4px">
              <StatusTag :variant="({ pending: 'warn', approved: 'ok', rejected: 'danger', offline: 'neutral', draft: 'neutral' } as any)[t.status]">
                {{ { pending: '待审', approved: '上线', rejected: '已拒', offline: '已下架', draft: '草稿' }[t.status] }}
              </StatusTag>
            </div>
          </div>
          <div class="muted small" style="margin-top: 3px">{{ t.category_name }} · {{ t.board_size }} · {{ t.color_count }} 色 · {{ t.total_beads.toLocaleString() }} 颗</div>
          <div class="row" style="margin-top: 8px; gap: 6px">
            <span v-for="tag in t.tags" :key="tag" class="chip">#{{ tag }}</span>
          </div>
          <div class="card-foot">
            <div class="row" style="gap: 6px">
              <div class="av sm" :class="'c' + ((parseInt(t.creator_id.slice(-1)) % 6) + 1)">{{ t.creator_name[0] }}</div>
              <div>
                <div style="font-size: 12px; font-weight: 600">{{ t.creator_name }}</div>
                <div class="muted small">使用 {{ t.use_count.toLocaleString() }} 次</div>
              </div>
            </div>
            <div>
              <button class="btn btn-xs btn-ghost">查看</button>
              <button v-if="t.status === 'approved'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click.stop="onOffline(t)">下架</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tabs { display: flex; gap: 2px; padding: 0 22px; background: var(--surface); border-bottom: 1px solid var(--line); flex-shrink: 0; }
.tab-btn { padding: 10px 14px; font-size: 13px; color: var(--ink-3); font-weight: 500; border-bottom: 2px solid transparent; display: inline-flex; align-items: center; gap: 6px; cursor: pointer; }
.tab-btn:hover { color: var(--ink-2); }
.tab-btn .ct { font-size: 10px; background: var(--bg-2); color: var(--ink-2); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.tab-btn.active { color: var(--ink); border-bottom-color: var(--primary); font-weight: 600; }
.tab-btn.active .ct { background: var(--primary-soft); color: var(--primary-ink); }
.grid { padding: 16px 22px; display: grid; grid-template-columns: repeat(auto-fill, minmax(240px, 1fr)); gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; overflow: hidden; cursor: pointer; transition: transform .12s, box-shadow .12s; }
.card:hover { transform: translateY(-2px); box-shadow: var(--shadow); }
.card-cover { aspect-ratio: 1; position: relative; display: grid; place-items: center; }
.card-cover::after { content: ""; position: absolute; inset: 0; background: linear-gradient(180deg, transparent 50%, rgba(0,0,0,.4)); }
.card-cover-tags { position: absolute; top: 8px; left: 8px; display: flex; gap: 4px; z-index: 1; }
.badge-overlay { padding: 2px 6px; border-radius: 4px; font-size: 10px; font-weight: 600; background: rgba(255,255,255,.92); color: var(--ink); }
.badge-overlay.warn { background: var(--warn); color: #fff; }
.ribbon { position: absolute; bottom: 8px; right: 8px; z-index: 1; padding: 3px 8px; background: var(--warn); color: #fff; font-size: 10px; font-weight: 700; border-radius: 4px; }
.card-body { padding: 10px 12px 12px; }
.card-foot { display: flex; align-items: center; justify-content: space-between; margin-top: 10px; padding-top: 8px; border-top: 1px solid var(--line); }
</style>
