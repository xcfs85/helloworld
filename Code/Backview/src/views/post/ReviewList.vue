<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listPostReviews, approvePost, rejectPost, batchApprovePosts, batchRejectPosts, type PostReviewItem } from '@/api/post'

const router = useRouter()
const tab = ref('pending')
const loading = ref(false)
const list = ref<PostReviewItem[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const selected = ref<string[]>([])

const filter = reactive({ keyword: '', type: 'all', risk_level: 'all' })

const tabs = [
  { label: '待审核', value: 'pending', count: 23 },
  { label: '已通过', value: 'approved', count: 0 },
  { label: '已拒绝', value: 'rejected', count: 5 },
  { label: '已下架', value: 'offline', count: 0 }
]

const sideStatus = [
  { label: '待审核', value: 'pending', count: 23, variant: 'primary' },
  { label: '已通过', value: 'approved' },
  { label: '已拒绝', value: 'rejected' },
  { label: '已下架', value: 'offline' }
]
const sideType = [
  { label: '作品', value: 'work', count: 15 },
  { label: '教程', value: 'tutorial', count: 5 },
  { label: '提问', value: 'question', count: 3 }
]
const sideRisk = [
  { label: '无风险', value: 'none', count: 18, variant: 'ok' },
  { label: '低风险', value: 'low', count: 3, variant: 'warn' },
  { label: '中风险', value: 'mid', count: 1, variant: 'warn' },
  { label: '高风险', value: 'high', count: 1, variant: 'danger' }
]

async function load() {
  loading.value = true
  try {
    const res = await listPostReviews({
      page: page.value,
      page_size: pageSize.value
    }) as any
    list.value = res?.list || []
    total.value = res?.total || 0
  } catch (e) {
    console.error('加载审核列表失败:', e)
    list.value = []
    total.value = 0
  } finally { loading.value = false }
}
function search() { page.value = 1; load() }
function reset() { filter.keyword = ''; filter.type = 'all'; filter.risk_level = 'all'; search() }

async function onApprove(p: PostReviewItem) {
  await approvePost(p.id)
  ElMessage.success('已通过')
  load()
}
async function onReject(p: PostReviewItem) {
  try {
    const { value } = await ElMessageBox.prompt('请输入拒绝原因', '拒绝帖子', { inputValue: '内容不符合规范' })
    await rejectPost(p.id, value)
    ElMessage.success('已拒绝')
    load()
  } catch {}
}
async function onBatchApprove() {
  if (selected.value.length === 0) { ElMessage.warning('请先选择帖子'); return }
  await batchApprovePosts(selected.value)
  ElMessage.success(`已通过 ${selected.value.length} 条`)
  load()
}
async function onBatchReject() {
  if (selected.value.length === 0) { ElMessage.warning('请先选择帖子'); return }
  try {
    const { value } = await ElMessageBox.prompt('请输入拒绝原因', '批量拒绝', { inputValue: '内容不符合规范' })
    await batchRejectPosts(selected.value, value)
    ElMessage.success(`已拒绝 ${selected.value.length} 条`)
    load()
  } catch {}
}

function viewDetail(p: PostReviewItem) { router.push({ name: 'post-review-detail', params: { id: p.id } }) }

const postCovers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)',
  'linear-gradient(135deg,#F5C45E,#FF8A5A)',
  'linear-gradient(135deg,#E07777,#F5C45E)'
]

function coverFor(p: PostReviewItem) {
  const idx = parseInt(p.id.slice(-1)) % postCovers.length
  return postCovers[idx]
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <div class="app-with-aside">
      <aside class="aside">
        <div class="aside-title">审核状态</div>
        <div v-for="s in sideStatus" :key="s.value" class="aside-item" :class="{ active: tab === s.value }" @click="tab = s.value; search()">
          {{ s.label }}
          <span v-if="s.count" class="badge" :class="s.variant">{{ s.count }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">内容类型</div>
        <div v-for="t in sideType" :key="t.value" class="aside-item" @click="filter.type = t.value; search()">
          {{ t.label }}
          <span class="badge">{{ t.count }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">AI 风险等级</div>
        <div v-for="r in sideRisk" :key="r.value" class="aside-item" @click="filter.risk_level = r.value; search()">
          {{ r.label }}
          <span v-if="r.count" class="badge" :class="r.variant">{{ r.count }}</span>
        </div>
      </aside>

      <div class="main">
        <PageHead
          :crumbs="[{ label: '内容管理' }, { label: '帖子审核' }]"
          title="帖子审核"
          sub="今日已审 156 · 累计待审 23"
        >
          <template #actions>
            <button class="btn btn-secondary">审核规则</button>
            <button class="btn btn-primary">审核</button>
          </template>
        </PageHead>

        <div class="tabs">
          <div v-for="t in tabs" :key="t.value" class="tab-btn" :class="{ active: tab === t.value }" @click="tab = t.value; search()">
            {{ t.label }} <span class="ct">{{ t.count }}</span>
          </div>
        </div>

        <div class="toolbar">
          <div class="search-input">
            <el-icon><Search /></el-icon>
            <input v-model="filter.keyword" placeholder="标题 / 作者 / 关键词" @keydown.enter="search" />
          </div>
          <div class="f-select"><select v-model="filter.type" @change="search">
            <option value="all">类型：全部</option><option value="work">作品</option><option value="tutorial">教程</option><option value="question">提问</option>
          </select></div>
          <div class="f-select"><select v-model="filter.risk_level" @change="search">
            <option value="all">AI 风险：全部</option><option value="none">无</option><option value="low">低</option><option value="mid">中</option><option value="high">高</option>
          </select></div>
          <div class="date-range">
            <input value="2026-06-01" />
            <span class="sep">→</span>
            <input value="2026-06-07" />
          </div>
          <button class="btn btn-sm btn-secondary" @click="reset">重置</button>
          <button class="btn btn-sm btn-primary" @click="search">搜索</button>
          <div class="f-spacer"></div>
          <div class="batch-actions">
            <span class="muted small">已选 {{ selected.length }} 条</span>
            <button class="btn btn-sm" style="background: var(--mint); color: #fff" @click="onBatchApprove">批量通过</button>
            <button class="btn btn-sm btn-danger" @click="onBatchReject">批量拒绝</button>
          </div>
        </div>

        <div class="tbl-wrap">
          <table class="tbl">
            <thead>
              <tr>
                <th style="width: 32px"><span class="ck"></span></th>
                <th>帖子</th>
                <th>类型</th>
                <th>作者</th>
                <th>发布时间</th>
                <th>AI 风险</th>
                <th>状态</th>
                <th class="col-actions">操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="p in list" :key="p.id">
                <td><span class="ck"></span></td>
                <td>
                  <div class="row" style="gap: 10px">
                    <div :style="`width: 36px; height: 36px; border-radius: 6px; background: ${coverFor(p)}; flex-shrink: 0`"></div>
                    <div style="min-width: 0">
                      <div style="font-weight: 600; color: var(--ink); overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 280px">{{ p.title }}</div>
                      <div class="muted small">{{ p.media?.filter((m: any) => m.type === 'image').length || 0 }} 图</div>
                    </div>
                  </div>
                </td>
                <td>
                  <StatusTag :variant="(({ work: 'primary', tutorial: 'info', question: 'purple' } as any)[p.type] || 'neutral') as any">
                    {{ ({ work: '作品', tutorial: '教程', question: '提问' } as any)[p.type] || p.type }}
                  </StatusTag>
                </td>
                <td>
                  <div class="user-cell">
                    <div class="av sm" :class="'c' + ((parseInt((p.author?.id || 'u_0').slice(2)) % 6) + 1)">{{ p.author?.nickname?.[0] || '?' }}</div>
                    <div class="meta"><div class="nm" style="font-size: 12px">{{ p.author?.nickname }}</div><div class="id">{{ p.author?.id }}</div></div>
                  </div>
                </td>
                <td class="muted">{{ (p.publish_time || p.create_time).slice(5) }}</td>
                <td>
                  <StatusTag v-if="(p.risk_level || 'none') === 'none'" variant="ok">无</StatusTag>
                  <StatusTag v-else-if="p.risk_level === 'low'" variant="warn">低</StatusTag>
                  <StatusTag v-else-if="p.risk_level === 'mid'" variant="warn">中</StatusTag>
                  <StatusTag v-else variant="danger">高 · {{ p.risk_tags?.[0] || '风险' }}</StatusTag>
                </td>
                <td>
                  <StatusTag :variant="(({ pending: 'warn', approved: 'ok', rejected: 'danger', offline: 'neutral' } as any)[(p.review_status || p.status) as string] || 'neutral') as any">
                    {{ ({ pending: '待审核', approved: '已通过', rejected: '已拒绝', offline: '已下架' } as any)[(p.review_status || p.status) as string] }}
                  </StatusTag>
                </td>
                <td class="col-actions">
                  <button class="btn btn-xs btn-ghost" @click="viewDetail(p)">预览</button>
                  <button v-if="p.review_status === 'pending'" class="btn btn-xs" style="color: var(--mint)" @click="onApprove(p)">通过</button>
                  <button v-if="p.review_status === 'pending'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onReject(p)">拒绝</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <Pager :total="total" :page="page" :page-size="pageSize"
          @update:page="(v) => { page = v; load() }"
          @update:page-size="(v) => { pageSize = v; load() }" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.app-with-aside { display: grid; grid-template-columns: var(--sub-aside-w) 1fr; height: 100%; background: var(--bg-2); flex: 1; overflow: hidden; }
.aside { background: var(--surface); border-right: 1px solid var(--line); padding: 14px 10px; display: flex; flex-direction: column; gap: 2px; overflow-y: auto; }
.aside-title { font-size: 10px; font-weight: 700; color: var(--ink-3); letter-spacing: 1.2px; text-transform: uppercase; padding: 6px 10px 4px; }
.aside-item { display: flex; align-items: center; justify-content: space-between; gap: 6px; padding: 7px 10px; border-radius: 6px; font-size: 12.5px; color: var(--ink-2); cursor: pointer; transition: background .12s, color .12s; }
.aside-item:hover { background: var(--bg); }
.aside-item.active { background: var(--ink); color: #fff; font-weight: 600; }
.aside-item .badge { font-size: 10px; background: var(--primary-soft); color: var(--primary-ink); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.aside-item.active .badge { background: rgba(255,255,255,.2); color: #fff; }
.aside-item .badge.ok { background: var(--mint-soft); color: #1F7A4B; }
.aside-item .badge.warn { background: var(--warn-soft); color: var(--warn); }
.aside-item .badge.danger { background: var(--rose-soft); color: var(--rose); }
.main { display: flex; flex-direction: column; overflow: hidden; min-width: 0; }
.tabs { display: flex; gap: 2px; padding: 0 22px; background: var(--surface); border-bottom: 1px solid var(--line); flex-shrink: 0; }
.tab-btn { padding: 10px 14px; font-size: 13px; color: var(--ink-3); font-weight: 500; border-bottom: 2px solid transparent; display: inline-flex; align-items: center; gap: 6px; cursor: pointer; }
.tab-btn:hover { color: var(--ink-2); }
.tab-btn .ct { font-size: 10px; background: var(--bg-2); color: var(--ink-2); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.tab-btn.active { color: var(--ink); border-bottom-color: var(--primary); font-weight: 600; }
.tab-btn.active .ct { background: var(--primary-soft); color: var(--primary-ink); }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
