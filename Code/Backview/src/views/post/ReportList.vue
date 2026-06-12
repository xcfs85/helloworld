<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listReports, handleReport } from '@/api/post'
import type { Report } from '@/types'

const loading = ref(false)
const list = ref<Report[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ keyword: '', status: 'all', type: 'all' })
const tab = ref('pending')

const tabs = [
  { label: '待处理', value: 'pending', count: 12 },
  { label: '已处理', value: 'handled' },
  { label: '已忽略', value: 'ignored' }
]

async function load() {
  loading.value = true
  try {
    const res: any = await listReports({ ...filter, status: tab.value, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

async function onHandle(r: Report, action: string) {
  const actionMap: Record<string, string> = { ignore: '忽略', warn: '警告', mute: '禁言', ban: '封号' }
  try {
    await ElMessageBox.confirm(`确定对举报 ${actionMap[action]} 处理？`, '处理举报', { type: 'warning' })
    await handleReport(r.id, action)
    ElMessage.success('已处理')
    load()
  } catch {}
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '内容管理' }, { label: '举报管理' }]"
      title="举报管理"
      sub="待处理 12 条 · 高优 2 条"
    >
      <template #actions>
        <button class="btn btn-secondary">导出</button>
      </template>
    </PageHead>

    <div class="tabs">
      <div v-for="t in tabs" :key="t.value" class="tab-btn" :class="{ active: tab === t.value }" @click="tab = t.value; load()">
        {{ t.label }} <span class="ct" v-if="t.count">{{ t.count }}</span>
      </div>
    </div>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="举报人 / 被举报内容" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.type" @change="load">
        <option value="all">类型：全部</option>
        <option value="spam">垃圾广告</option>
        <option value="violation">违规内容</option>
        <option value="infringement">侵权</option>
        <option value="fake">虚假信息</option>
        <option value="attack">人身攻击</option>
        <option value="other">其他</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th style="width: 32px"><span class="ck"></span></th>
            <th>类型</th>
            <th>被举报内容</th>
            <th>举报人</th>
            <th>原因</th>
            <th>时间</th>
            <th>状态</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="r in list" :key="r.id">
            <td><span class="ck"></span></td>
            <td>
              <StatusTag :variant="(({ spam: 'warn', violation: 'danger', infringement: 'purple', fake: 'info', attack: 'danger', other: 'neutral' } as any)[r.type] || 'neutral') as any">
                {{ ({ spam: '垃圾广告', violation: '违规内容', infringement: '侵权', fake: '虚假信息', attack: '人身攻击', other: '其他' } as any)[r.type] || r.type }}
              </StatusTag>
            </td>
            <td>
              <div style="font-weight: 600; color: var(--ink); max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">{{ r.target_summary }}</div>
              <div class="muted small mono">{{ { post: '帖子', comment: '评论', user: '用户' }[r.target_type] }} · {{ r.target_id }}</div>
            </td>
            <td>
              <div class="user-cell">
                <div class="av sm" :class="'c' + ((parseInt(r.reporter_id.slice(-1)) % 6) + 1)">{{ r.reporter_name[0] }}</div>
                <div class="meta"><div class="nm" style="font-size: 12px">{{ r.reporter_name }}</div></div>
              </div>
            </td>
            <td class="muted" style="max-width: 180px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">{{ r.reason }}</td>
            <td class="muted">{{ r.create_time.slice(5) }}</td>
            <td>
              <StatusTag v-if="r.status === 'pending'" variant="warn">待处理</StatusTag>
              <StatusTag v-else-if="r.status === 'warned'" variant="info">已警告</StatusTag>
              <StatusTag v-else-if="r.status === 'muted'" variant="info">已禁言</StatusTag>
              <StatusTag v-else-if="r.status === 'banned'" variant="danger">已封号</StatusTag>
              <StatusTag v-else variant="neutral">已忽略</StatusTag>
            </td>
            <td class="col-actions">
              <template v-if="r.status === 'pending'">
                <button class="btn btn-xs btn-ghost" @click="onHandle(r, 'ignore')">忽略</button>
                <button class="btn btn-xs btn-ghost" @click="onHandle(r, 'warn')">警告</button>
                <button class="btn btn-xs btn-ghost" @click="onHandle(r, 'mute')">禁言</button>
                <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onHandle(r, 'ban')">封号</button>
              </template>
              <button v-else class="btn btn-xs btn-ghost">查看</button>
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
