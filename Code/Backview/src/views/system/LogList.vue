<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listLogs } from '@/api/auth'
import type { OperationLog } from '@/types'

const loading = ref(false)
const list = ref<OperationLog[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ keyword: '', operation: 'all', date_range: '' })

async function load() {
  loading.value = true
  try {
    const res: any = await listLogs({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

const operationTypes = [
  { value: 'all', label: '全部', count: 0 },
  { value: '登录', label: '登录', count: 0 },
  { value: '审核', label: '审核', count: 0 },
  { value: '新增', label: '新增', count: 0 },
  { value: '修改', label: '修改', count: 0 },
  { value: '删除', label: '删除', count: 0 },
  { value: '重置', label: '重置', count: 0 }
]

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '系统管理' }, { label: '操作日志' }]"
      title="操作日志"
      sub="近 7 天 156 条 · 今日 7 条"
    >
      <template #actions>
        <button class="btn btn-secondary">导出</button>
      </template>
    </PageHead>

    <div class="tabs">
      <div v-for="t in operationTypes" :key="t.value" class="tab-btn" :class="{ active: filter.operation === t.value }" @click="filter.operation = t.value; load()">
        {{ t.label }}
      </div>
    </div>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="账号 / 内容 / IP" @keydown.enter="load" />
      </div>
      <div class="date-range">
        <input value="2026-05-08" />
        <span class="sep">→</span>
        <input value="2026-06-07" />
      </div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>操作员</th>
            <th>类型</th>
            <th>内容</th>
            <th>参数</th>
            <th>IP</th>
            <th>时间</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="l in list" :key="l.id">
            <td>
              <div class="user-cell">
                <div class="av sm" :class="'c' + ((parseInt(l.user_id.slice(-1)) % 6) + 1)">{{ l.username[0] }}</div>
                <div class="meta"><div class="nm" style="font-size: 12px">{{ l.username }}</div><div class="id">{{ l.user_id }}</div></div>
              </div>
            </td>
            <td>
              <StatusTag :variant="({ 登录: 'info', 审核: 'primary', 新增: 'ok', 修改: 'warn', 删除: 'danger', 重置: 'purple' } as any)[l.operation] || 'neutral'">
                {{ l.operation }}
              </StatusTag>
            </td>
            <td>{{ l.content }}</td>
            <td class="muted mono small" style="max-width: 220px; overflow: hidden; text-overflow: ellipsis">{{ l.params }}</td>
            <td class="muted mono">{{ l.ip }}</td>
            <td class="muted">{{ l.create_time }}</td>
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
.tab-btn { padding: 10px 14px; font-size: 13px; color: var(--ink-3); font-weight: 500; border-bottom: 2px solid transparent; cursor: pointer; }
.tab-btn:hover { color: var(--ink-2); }
.tab-btn.active { color: var(--ink); border-bottom-color: var(--primary); font-weight: 600; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
