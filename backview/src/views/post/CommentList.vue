<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listCommentReviews, approveComment, hideComment, deleteComment } from '@/api/post'
import type { Comment } from '@/types'

const loading = ref(false)
const list = ref<Comment[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)

const filter = reactive({ keyword: '', status: 'all' })
const tab = ref('pending')

const tabs = [
  { label: '待审核', value: 'pending', count: 45 },
  { label: '已通过', value: 'approved' },
  { label: '已隐藏', value: 'hidden' },
  { label: '已删除', value: 'deleted' }
]

async function load() {
  loading.value = true
  try {
    const res: any = await listCommentReviews({ ...filter, status: tab.value, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

async function onApprove(c: Comment) { await approveComment(c.id); ElMessage.success('已通过'); load() }
async function onHide(c: Comment) { await hideComment(c.id); ElMessage.success('已隐藏'); load() }
async function onDelete(c: Comment) { await deleteComment(c.id); ElMessage.success('已删除'); load() }

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '内容管理' }, { label: '评论审核' }]"
      title="评论审核"
      sub="今日待审 45 条 · 已处理 156 条"
    >
      <template #actions>
        <button class="btn btn-secondary">导出</button>
      </template>
    </PageHead>

    <div class="tabs">
      <div v-for="t in tabs" :key="t.value" class="tab-btn" :class="{ active: tab === t.value }" @click="tab = t.value; load()">
        {{ t.label }} <span class="ct">{{ t.count || '' }}</span>
      </div>
    </div>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="评论内容 / 作者" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option><option value="pending">待审</option><option value="approved">通过</option><option value="hidden">隐藏</option>
      </select></div>
      <button class="btn btn-sm btn-primary">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th style="width: 32px"><span class="ck"></span></th>
            <th>评论内容</th>
            <th>所属帖子</th>
            <th>作者</th>
            <th>时间</th>
            <th>风险</th>
            <th>状态</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in list" :key="c.id">
            <td><span class="ck"></span></td>
            <td style="max-width: 360px">{{ c.content }}</td>
            <td>
              <div style="font-weight: 600; color: var(--ink); max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">{{ c.post_title }}</div>
              <div class="muted small mono">{{ c.post_id }}</div>
            </td>
            <td>
              <div class="user-cell">
                <div class="av sm" :class="'c' + ((parseInt(c.user_id.slice(2)) % 6) + 1)">{{ c.user_nickname[0] }}</div>
                <div class="meta"><div class="nm" style="font-size: 12px">{{ c.user_nickname }}</div><div class="id">{{ c.user_id }}</div></div>
              </div>
            </td>
            <td class="muted">{{ c.create_time.slice(5) }}</td>
            <td>
              <StatusTag v-if="c.risk_level === 'none'" variant="ok">无</StatusTag>
              <StatusTag v-else variant="warn">{{ { low: '低', mid: '中', high: '高' }[c.risk_level] }}</StatusTag>
            </td>
            <td>
              <StatusTag :variant="({ pending: 'warn', approved: 'ok', hidden: 'neutral', deleted: 'danger' } as any)[c.status]">
                {{ { pending: '待审', approved: '通过', hidden: '隐藏', deleted: '已删' }[c.status] }}
              </StatusTag>
            </td>
            <td class="col-actions">
              <button v-if="c.status === 'pending'" class="btn btn-xs" style="color: var(--mint)" @click="onApprove(c)">通过</button>
              <button v-if="c.status === 'pending'" class="btn btn-xs btn-ghost" @click="onHide(c)">隐藏</button>
              <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onDelete(c)">删除</button>
            </td>
          </tr>
        </tbody>
      </table>
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
.tbl-wrap { flex: 1; overflow: auto; }
</style>
