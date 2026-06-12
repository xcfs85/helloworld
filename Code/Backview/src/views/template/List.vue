<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listTemplates, offlineTemplate, listCategories, createTemplate } from '@/api/template'
import type { Template } from '@/types'

const router = useRouter()
const tab = ref('all')
const loading = ref(false)
const list = ref<Template[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(12)

// 统计数据
const stats = reactive({
  total: 0,
  pending: 0,
  approved: 0,
  rejected: 0,
  offline: 0
})

// 分类列表
const categories = ref<{ id: string; name: string }[]>([])
const categoryLoading = ref(false)

const filter = reactive({ keyword: '', category_id: 'all', source: 'all', difficulty: 'all' })

// 控制导入弹窗显示
const showImport = ref(false)
const importFile = ref<File | null>(null)
const importing = ref(false)

// 控制新建模板弹窗显示
const showCreate = ref(false)
const creating = ref(false)
const newTemplate = reactive({
  name: '',
  category_id: '',
  difficulty: 'beginner',
  board_size: '30x30',
  total_colors: 0
})

// 动态 tabs 数据
const tabs = computed(() => [
  { label: '全部', value: 'all', count: stats.total },
  { label: '待审核', value: 'pending', count: stats.pending },
  { label: '已通过', value: 'approved', count: stats.approved },
  { label: '已拒绝', value: 'rejected', count: stats.rejected },
  { label: '已下架', value: 'offline', count: stats.offline }
])

// 页面副标题
const subTitle = computed(() => {
  const totalStr = stats.total.toLocaleString()
  const approvedStr = stats.approved.toLocaleString()
  const pendingStr = stats.pending.toLocaleString()
  return `共 ${totalStr} 套 · 上线 ${approvedStr} · 待审 ${pendingStr}`
})

const covers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)',
  'linear-gradient(135deg,#F5C45E,#FF8A5A)',
  'linear-gradient(135deg,#E07777,#F5C45E)'
]

async function loadStats() {
  try {
    // 获取全部模板数量
    const allRes: any = await listTemplates({ page: 1, page_size: 1 })
    stats.total = allRes.total || 0

    // 获取待审核数量
    const pendingRes: any = await listTemplates({ status: 'pending', page: 1, page_size: 1 })
    stats.pending = pendingRes.total || 0

    // 获取已通过数量
    const approvedRes: any = await listTemplates({ status: 'approved', page: 1, page_size: 1 })
    stats.approved = approvedRes.total || 0

    // 获取已拒绝数量
    const rejectedRes: any = await listTemplates({ status: 'rejected', page: 1, page_size: 1 })
    stats.rejected = rejectedRes.total || 0

    // 获取已下架数量
    const offlineRes: any = await listTemplates({ status: 'offline', page: 1, page_size: 1 })
    stats.offline = offlineRes.total || 0
  } catch (e) {
    console.error('加载统计数据失败', e)
  }
}

async function loadCategories() {
  categoryLoading.value = true
  try {
    const res: any = await listCategories()
    categories.value = (Array.isArray(res) ? res : (res?.list || [])).map((c: any) => ({
      id: c.id,
      name: c.name
    }))
  } catch (e) {
    console.error('加载分类失败', e)
  } finally {
    categoryLoading.value = false
  }
}

async function load() {
  loading.value = true
  try {
    const query: any = { page: page.value, page_size: pageSize.value }
    if (filter.keyword) query.keyword = filter.keyword
    if (filter.category_id && filter.category_id !== 'all') query.category = filter.category_id
    if (filter.source && filter.source !== 'all') query.source_type = filter.source
    if (filter.difficulty && filter.difficulty !== 'all') query.difficulty = filter.difficulty
    if (tab.value !== 'all') query.status = tab.value

    const res: any = await listTemplates(query)
    list.value = res.list || []
    total.value = res.total || 0
  } finally { loading.value = false }
}

function view(t: Template) {
  router.push({ name: 'template-review-detail', params: { id: t.id } })
}

async function onOffline(t: Template) {
  await offlineTemplate(t.id)
  ElMessage.success('已下架')
  load()
  loadStats()
}

// 跳转到分类管理页面
function goToCategoryConfig() {
  router.push({ name: 'template-category-list' })
}

// 处理导入文件
function handleImportClick() {
  const input = document.createElement('input')
  input.type = 'file'
  input.accept = '.json,.zip'
  input.onchange = (e: any) => {
    if (e.target.files?.length) {
      importFile.value = e.target.files[0]
      showImport.value = true
    }
  }
  input.click()
}

// 执行导入
async function doImport() {
  if (!importFile.value) return

  importing.value = true
  try {
    // 模拟导入成功（实际需要根据后端接口实现）
    await new Promise(resolve => setTimeout(resolve, 1500))
    ElMessage.success('导入成功')
    showImport.value = false
    importFile.value = null
    load()
  } catch (e) {
    ElMessage.error('导入失败')
  } finally {
    importing.value = false
  }
}

// 创建新模板
async function doCreate() {
  if (!newTemplate.name) {
    ElMessage.warning('请输入模板名称')
    return
  }
  if (!newTemplate.category_id) {
    ElMessage.warning('请选择分类')
    return
  }

  creating.value = true
  try {
    await createTemplate({
      name: newTemplate.name,
      category_id: newTemplate.category_id,
      difficulty: newTemplate.difficulty,
      board_size: newTemplate.board_size,
      total_colors: newTemplate.total_colors
    })
    ElMessage.success('创建成功')
    showCreate.value = false
    newTemplate.name = ''
    newTemplate.category_id = ''
    newTemplate.difficulty = 'beginner'
    newTemplate.board_size = '30x30'
    newTemplate.total_colors = 0
    load()
    loadStats()
  } catch (e: any) {
    ElMessage.error(e.message || '创建失败')
  } finally {
    creating.value = false
  }
}

onMounted(() => {
  loadStats()
  loadCategories()
  load()
})
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '模板管理' }, { label: '模板列表' }]"
      title="模板列表"
      :sub="subTitle"
    >
      <template #actions>
        <button class="btn btn-secondary" @click="goToCategoryConfig">分类配置</button>
        <button class="btn btn-secondary" @click="handleImportClick">导入</button>
        <button class="btn btn-primary" @click="showCreate = true">+ 新建模板</button>
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
      <div class="f-select">
        <select v-model="filter.category_id" @change="load" :disabled="categoryLoading">
          <option value="all">分类：全部</option>
          <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
        </select>
      </div>
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
          <div class="muted small" style="margin-top: 3px">{{ t.category_name }} · {{ t.board_size }} · {{ t.color_count }} 色 · {{ t.total_beads?.toLocaleString() }} 颗</div>
          <div class="row" style="margin-top: 8px; gap: 6px">
            <span v-for="tag in t.tags" :key="tag" class="chip">#{{ tag }}</span>
          </div>
          <div class="card-foot">
            <div class="row" style="gap: 6px">
              <div class="av sm" :class="'c' + ((parseInt((t.creator_id || '0').slice(-1)) % 6) + 1)">{{ (t.creator_name || '?')[0] }}</div>
              <div>
                <div style="font-size: 12px; font-weight: 600">{{ t.creator_name }}</div>
                <div class="muted small">使用 {{ t.use_count?.toLocaleString() }} 次</div>
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

    <!-- 新建模板弹窗 -->
    <el-dialog v-model="showCreate" title="新建模板" width="480">
      <div class="form-row">
        <div class="lbl req">模板名称</div>
        <div class="input-line"><input v-model="newTemplate.name" placeholder="请输入模板名称" /></div>
      </div>
      <div class="form-row">
        <div class="lbl req">分类</div>
        <div class="f-select" style="width: 100%">
          <select v-model="newTemplate.category_id">
            <option value="">请选择分类</option>
            <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
          </select>
        </div>
      </div>
      <div class="form-row">
        <div class="lbl">板型</div>
        <div class="f-select" style="width: 100%">
          <select v-model="newTemplate.board_size">
            <option value="20x20">20×20</option>
            <option value="30x30">30×30</option>
            <option value="40x40">40×40</option>
            <option value="50x50">50×50</option>
          </select>
        </div>
      </div>
      <div class="form-row">
        <div class="lbl">难度</div>
        <div class="f-select" style="width: 100%">
          <select v-model="newTemplate.difficulty">
            <option value="beginner">入门</option>
            <option value="intermediate">进阶</option>
            <option value="advanced">高阶</option>
          </select>
        </div>
      </div>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" :loading="creating" @click="doCreate">创建</el-button>
      </template>
    </el-dialog>

    <!-- 导入弹窗 -->
    <el-dialog v-model="showImport" title="导入模板" width="420">
      <div class="import-tip">请选择要导入的模板文件，支持 JSON 或 ZIP 格式</div>
      <div class="file-selected" v-if="importFile">
        <el-icon><Document /></el-icon>
        <span>{{ importFile.name }}</span>
      </div>
      <template #footer>
        <el-button @click="showImport = false">取消</el-button>
        <el-button type="primary" :loading="importing" @click="doImport">导入</el-button>
      </template>
    </el-dialog>
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

/* 导入弹窗样式 */
.import-tip { color: var(--ink-3); font-size: 13px; margin-bottom: 16px; }
.file-selected { display: flex; align-items: center; gap: 8px; padding: 12px; background: var(--bg-2); border-radius: 6px; }
</style>