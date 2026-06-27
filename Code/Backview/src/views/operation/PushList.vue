<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listPushes, sendPush, schedulePush, cancelPush, retryPush, createPush } from '@/api/operation'
import type { Push } from '@/types'

const loading = ref(false)
const list = ref<Push[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ status: 'all' })
const showAdd = ref(false)
const newPush = ref({
  title: '',
  content: '',
  push_type: 'system',
  target_type: 'all',
  target_ids: '' as string,
  channels: ['app'] as string[],
  schedule_time: '' as string
})

// 受众标签映射
const audienceLabel: Record<string, string> = { all: '全量用户', tag: '按标签', user: '指定用户' }
// 推送类型映射
const pushTypeLabel: Record<string, string> = { system: '系统', activity: '活动', marketing: '营销' }
// 状态映射
const statusLabel: Record<string, string> = { draft: '草稿', pending: '待发送', sending: '发送中', sent: '已发送', failed: '失败', canceled: '已取消' }
const statusVariant: Record<string, string> = { draft: 'neutral', pending: 'warn', sending: 'info', sent: 'ok', failed: 'danger', canceled: 'neutral' }
// 渠道映射
const channelLabel: Record<string, string> = { app: 'App', sms: '短信', email: '邮件' }
const channelVariant: Record<string, string> = { app: 'info', sms: 'warn', email: 'purple' }

async function load() {
  loading.value = true
  try {
    const res: any = await listPushes({
      ...filter,
      page: page.value,
      size: pageSize.value
    })
    list.value = res.list || []
    total.value = res.total || 0
  } finally { loading.value = false }
}

function resetForm() {
  newPush.value = {
    title: '',
    content: '',
    push_type: 'system',
    target_type: 'all',
    target_ids: '',
    channels: ['app'],
    schedule_time: ''
  }
}

async function onSaveDraft() {
  if (!newPush.value.title) { ElMessage.warning('请输入标题'); return }
  if (!newPush.value.content) { ElMessage.warning('请输入内容'); return }
  try {
    await createPush(buildRequest(newPush.value))
    ElMessage.success('草稿已保存')
    showAdd.value = false
    resetForm()
    load()
  } catch {}
}

async function onSendDirect() {
  if (!newPush.value.title) { ElMessage.warning('请输入标题'); return }
  if (!newPush.value.content) { ElMessage.warning('请输入内容'); return }
  try {
    await ElMessageBox.confirm('确定立即发送推送？', '发送推送', { type: 'warning' })
    await sendPush(buildRequest(newPush.value))
    ElMessage.success('已加入发送队列')
    showAdd.value = false
    resetForm()
    load()
  } catch {}
}

async function onSchedule() {
  if (!newPush.value.title) { ElMessage.warning('请输入标题'); return }
  if (!newPush.value.content) { ElMessage.warning('请输入内容'); return }
  if (!newPush.value.schedule_time) { ElMessage.warning('请选择定时发送时间'); return }
  try {
    await ElMessageBox.confirm(`确定定时发送推送「${newPush.value.title}」？`, '定时推送', { type: 'warning' })
    await schedulePush(buildScheduleRequest(newPush.value))
    ElMessage.success('定时推送已创建')
    showAdd.value = false
    resetForm()
    load()
  } catch {}
}

async function onSendExisting(p: Push) {
  try {
    await ElMessageBox.confirm(`确定立即发送推送「${p.title}」？`, '发送推送', { type: 'warning' })
    await sendPush({
      title: p.title,
      content: p.content,
      push_type: p.push_type,
      target_type: p.target_type,
      channels: p.channels
    })
    ElMessage.success('已加入发送队列')
    load()
  } catch {}
}

async function onCancel(p: Push) {
  try {
    await ElMessageBox.confirm(`确定取消推送「${p.title}」？`, '取消推送', { type: 'warning' })
    await cancelPush(p.id)
    ElMessage.success('已取消')
    load()
  } catch {}
}

async function onRetry(p: Push) {
  try {
    await ElMessageBox.confirm(`确定重试推送「${p.title}」？`, '重试推送', { type: 'warning' })
    await retryPush(p.id)
    ElMessage.success('已重新发送')
    load()
  } catch {}
}

function buildRequest(form: typeof newPush.value) {
  const data: any = {
    title: form.title,
    content: form.content,
    push_type: form.push_type,
    target_type: form.target_type,
    channels: form.channels
  }
  if (form.target_type === 'user' && form.target_ids) {
    data.target_ids = form.target_ids.split(',').map((s: string) => s.trim()).filter(Boolean)
  }
  if (form.target_type === 'tag' && form.target_ids) {
    data.target_param = form.target_ids
  }
  return data
}

function buildScheduleRequest(form: typeof newPush.value) {
  const data = buildRequest(form)
  data.schedule_time = form.schedule_time
  return data
}

function toggleChannel(ch: string) {
  const idx = newPush.value.channels.indexOf(ch)
  if (idx >= 0) {
    if (newPush.value.channels.length > 1) {
      newPush.value.channels.splice(idx, 1)
    }
  } else {
    newPush.value.channels.push(ch)
  }
}

function formatTime(t?: string) {
  if (!t) return '—'
  return t.replace('T', ' ').substring(0, 16)
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营管理' }, { label: '推送管理' }]"
      title="推送管理"
      sub="管理推送消息，支持App/短信/邮件多渠道"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 创建推送</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option>
        <option value="draft">草稿</option>
        <option value="pending">待发送</option>
        <option value="sent">已发送</option>
        <option value="failed">失败</option>
        <option value="canceled">已取消</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th>推送标题</th>
            <th>内容</th>
            <th>类型</th>
            <th>受众</th>
            <th>渠道</th>
            <th>计划发送</th>
            <th>实际发送</th>
            <th>成功/失败</th>
            <th>状态</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in list" :key="p.id">
            <td style="font-weight: 600; color: var(--ink)">{{ p.title }}</td>
            <td class="muted" style="max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap">{{ p.content }}</td>
            <td><span class="chip">{{ pushTypeLabel[p.push_type] || p.push_type }}</span></td>
            <td><span class="chip">{{ audienceLabel[p.target_type] || p.target_type }}</span></td>
            <td>
              <div class="row" style="gap: 4px">
                <StatusTag v-for="c in p.channels" :key="c" :variant="(channelVariant[c] as any) || 'neutral'">
                  {{ channelLabel[c] || c }}
                </StatusTag>
              </div>
            </td>
            <td class="muted small">{{ formatTime(p.scheduled_time) }}</td>
            <td class="muted small">{{ formatTime(p.send_time) }}</td>
            <td>
              <span style="color: var(--mint)">{{ (p.success_count || 0).toLocaleString() }}</span>
              <span class="muted"> / </span>
              <span style="color: var(--rose)">{{ (p.fail_count || 0).toLocaleString() }}</span>
            </td>
            <td>
              <StatusTag :variant="(statusVariant[p.status] as any) || 'neutral'">
                {{ statusLabel[p.status] || p.status }}
              </StatusTag>
            </td>
            <td class="col-actions">
              <button v-if="p.status === 'draft'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="onSendExisting(p)">发送</button>
              <button v-if="p.status === 'pending'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="onSendExisting(p)">立即发送</button>
              <button v-if="p.status === 'pending'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onCancel(p)">取消</button>
              <button v-if="p.status === 'failed'" class="btn btn-xs btn-ghost" style="color: var(--mint)" @click="onRetry(p)">重试</button>
            </td>
          </tr>
          <tr v-if="!loading && list.length === 0">
            <td colspan="10" style="text-align: center; padding: 40px; color: var(--ink-2)">暂无推送记录</td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="创建推送" width="560">
      <div class="form-row"><div class="lbl req">标题</div>
        <div class="input-line"><input v-model="newPush.title" placeholder="推送标题" /></div>
      </div>
      <div class="form-row"><div class="lbl req">内容</div>
        <textarea v-model="newPush.content" class="textarea" placeholder="推送内容..." style="min-height: 100px"></textarea>
      </div>
      <div class="form-row"><div class="lbl">推送类型</div>
        <div class="f-select" style="width: 100%"><select v-model="newPush.push_type">
          <option value="system">系统通知</option>
          <option value="activity">活动推送</option>
          <option value="marketing">营销推送</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl">受众</div>
        <div class="f-select" style="width: 100%"><select v-model="newPush.target_type">
          <option value="all">全量用户</option>
          <option value="tag">按标签</option>
          <option value="user">指定用户</option>
        </select></div>
      </div>
      <div v-if="newPush.target_type === 'user'" class="form-row">
        <div class="lbl">用户ID</div>
        <div class="input-line"><input v-model="newPush.target_ids" placeholder="用户ID，多个用逗号分隔" /></div>
      </div>
      <div v-if="newPush.target_type === 'tag'" class="form-row">
        <div class="lbl">标签</div>
        <div class="input-line"><input v-model="newPush.target_ids" placeholder="推送标签" /></div>
      </div>
      <div class="form-row"><div class="lbl req">渠道</div>
        <div class="row" style="gap: 12px; width: 100%">
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('app')" @change="toggleChannel('app')" /> App 推送</label>
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('sms')" @change="toggleChannel('sms')" /> 短信</label>
          <label class="row" style="gap: 4px; font-size: 12.5px"><input type="checkbox" class="ck" :checked="newPush.channels.includes('email')" @change="toggleChannel('email')" /> 邮件</label>
        </div>
      </div>
      <div class="form-row"><div class="lbl">定时发送</div>
        <div class="input-line"><input type="datetime-local" v-model="newPush.schedule_time" /></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button @click="onSaveDraft">保存草稿</el-button>
        <el-button v-if="newPush.schedule_time" type="warning" @click="onSchedule">定时发送</el-button>
        <el-button type="primary" @click="onSendDirect">立即发送</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
