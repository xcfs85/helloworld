<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listPushes, createPush, sendPush } from '@/api/operation'
import type { Push } from '@/types'

const loading = ref(false)
const list = ref<Push[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ status: 'all' })
const showAdd = ref(false)
const newPush = ref({ title: '', content: '', audience: 'all', channels: ['app'] as string[] })

async function load() {
  loading.value = true
  try {
    const res: any = await listPushes({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

async function onAdd() {
  if (!newPush.value.title) { ElMessage.warning('请输入标题'); return }
  await createPush(newPush.value)
  ElMessage.success('已创建')
  showAdd.value = false
  newPush.value = { title: '', content: '', audience: 'all', channels: ['app'] }
  load()
}

async function onSend(p: Push) {
  try {
    await ElMessageBox.confirm(`确定立即发送推送「${p.title}」？`, '发送推送', { type: 'warning' })
    await sendPush(p.id)
    ElMessage.success('已加入发送队列')
    load()
  } catch {}
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营管理' }, { label: '推送管理' }]"
      title="推送管理"
      sub="近 7 天推送 12 次 · 触达 12.5w 用户"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 创建推送</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option><option value="draft">草稿</option><option value="scheduled">待发送</option><option value="sent">已发送</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>推送标题</th>
            <th>内容</th>
            <th>受众</th>
            <th>渠道</th>
            <th>计划发送</th>
            <th>实际发送</th>
            <th>成功/失败</th>
            <th>状态</th>
            <th>创建人</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in list" :key="p.id">
            <td style="font-weight: 600; color: var(--ink)">{{ p.title }}</td>
            <td class="muted" style="max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">{{ p.content }}</td>
            <td><span class="chip">{{ p.audience_label }}</span></td>
            <td>
              <div class="row" style="gap: 4px">
                <StatusTag v-for="c in p.channels" :key="c" :variant="c === 'app' ? 'info' : c === 'sms' ? 'warn' : 'purple'">
                  {{ c === 'app' ? 'App' : c === 'sms' ? '短信' : c === 'email' ? '邮件' : c }}
                </StatusTag>
              </div>
            </td>
            <td class="muted small">{{ p.scheduled_time }}</td>
            <td class="muted small">{{ p.send_time || '—' }}</td>
            <td>
              <span style="color: var(--mint)">{{ p.success_count.toLocaleString() }}</span>
              <span class="muted"> / </span>
              <span style="color: var(--rose)">{{ p.fail_count.toLocaleString() }}</span>
            </td>
            <td>
              <StatusTag :variant="({ draft: 'neutral', scheduled: 'warn', sending: 'info', sent: 'ok', failed: 'danger' } as any)[p.status]">
                {{ { draft: '草稿', scheduled: '待发送', sending: '发送中', sent: '已发送', failed: '失败' }[p.status] }}
              </StatusTag>
            </td>
            <td class="muted">{{ p.creator }}</td>
            <td class="col-actions">
              <button v-if="p.status === 'draft' || p.status === 'scheduled'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="onSend(p)">发送</button>
              <button v-if="p.status === 'draft'" class="btn btn-xs btn-ghost">编辑</button>
              <button v-else class="btn btn-xs btn-ghost">详情</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="创建推送" width="540">
      <div class="form-row"><div class="lbl req">标题</div>
        <div class="input-line"><input v-model="newPush.title" placeholder="推送标题" /></div>
      </div>
      <div class="form-row"><div class="lbl req">内容</div>
        <textarea v-model="newPush.content" class="textarea" placeholder="推送内容..." style="min-height: 100px"></textarea>
      </div>
      <div class="form-row"><div class="lbl">受众</div>
        <div class="f-select" style="width: 100%"><select v-model="newPush.audience">
          <option value="all">全量用户</option><option value="tag">按标签</option><option value="user">指定用户</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl">渠道</div>
        <div class="row" style="gap: 12px; width: 100%">
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('app')" @change="newPush.channels = newPush.channels.includes('app') ? newPush.channels.filter(x => x !== 'app') : [...newPush.channels, 'app']" /> App 推送</label>
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('sms')" @change="newPush.channels = newPush.channels.includes('sms') ? newPush.channels.filter(x => x !== 'sms') : [...newPush.channels, 'sms']" /> 短信</label>
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('email')" @change="newPush.channels = newPush.channels.includes('email') ? newPush.channels.filter(x => x !== 'email') : [...newPush.channels, 'email']" /> 邮件</label>
        </div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button @click="onAdd">保存草稿</el-button>
        <el-button type="primary" @click="onAdd">立即发送</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
