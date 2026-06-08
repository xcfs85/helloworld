<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listTopics, addTopic, closeTopic } from '@/api/operation'
import type { Topic } from '@/types'

const loading = ref(false)
const list = ref<Topic[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ status: 'all', is_official: 'all' })
const showAdd = ref(false)
const newTopic = ref({ name: '', desc: '', is_official: false })

async function load() {
  loading.value = true
  try {
    const res: any = await listTopics({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}
async function onAdd() {
  if (!newTopic.value.name) { ElMessage.warning('请输入话题名'); return }
  await addTopic(newTopic.value)
  ElMessage.success('已创建')
  showAdd.value = false
  newTopic.value = { name: '', desc: '', is_official: false }
  load()
}
async function onClose(t: Topic) {
  try {
    await ElMessageBox.confirm(`确定结束话题「${t.name}」？结束后将停止推荐`, '提示', { type: 'warning' })
    await closeTopic(t.id)
    ElMessage.success('已结束')
    load()
  } catch {}
}

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营管理' }, { label: '话题管理' }]"
      title="话题管理"
      sub="共 4 个话题 · 1 个进行中"
    >
      <template #actions>
        <button class="btn btn-primary" @click="showAdd = true">+ 新建话题</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option><option value="active">进行中</option><option value="recommended">推荐中</option><option value="ended">已结束</option>
      </select></div>
      <div class="f-select"><select v-model="filter.is_official" @change="load">
        <option value="all">类型：全部</option><option value="true">官方</option><option value="false">用户</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tp-grid">
      <div v-for="t in list" :key="t.id" class="tp-card" :class="{ ended: t.status === 'ended' }">
        <div class="tp-head">
          <div class="row" style="gap: 6px">
            <StatusTag v-if="t.is_official" variant="primary">官方</StatusTag>
            <StatusTag :variant="({ active: 'ok', recommended: 'primary', ended: 'neutral' } as any)[t.status]">
              {{ { active: '进行中', recommended: '推荐中', ended: '已结束' }[t.status] }}
            </StatusTag>
          </div>
          <span class="muted small">{{ t.create_time }}</span>
        </div>
        <div class="tp-name"># {{ t.name }}</div>
        <div class="tp-desc">{{ t.desc }}</div>
        <div class="tp-stats">
          <div class="tps">
            <div class="tps-v">{{ t.post_count.toLocaleString() }}</div>
            <div class="tps-l">帖子</div>
          </div>
          <div class="tps">
            <div class="tps-v">{{ t.user_count.toLocaleString() }}</div>
            <div class="tps-l">参与</div>
          </div>
          <div class="tps">
            <div class="tps-v">12.5k</div>
            <div class="tps-l">曝光</div>
          </div>
        </div>
        <div class="tp-foot">
          <button class="btn btn-xs btn-ghost">编辑</button>
          <button class="btn btn-xs btn-ghost">推荐</button>
          <button v-if="t.status !== 'ended'" class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onClose(t)">结束</button>
        </div>
      </div>
    </div>

    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="新建话题" width="480">
      <div class="form-row"><div class="lbl req">话题名</div>
        <div class="input-line"><input v-model="newTopic.name" placeholder="话题名" /></div>
      </div>
      <div class="form-row"><div class="lbl">描述</div>
        <div class="input-line"><input v-model="newTopic.desc" placeholder="话题描述" /></div>
      </div>
      <div class="form-row"><div class="lbl">官方</div>
        <label class="row" style="gap: 6px; font-size: 13px"><input type="checkbox" class="ck" v-model="newTopic.is_official" /> 标记为官方话题</label>
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
.tp-grid { padding: 16px 22px; display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.tp-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; padding: 14px 16px; transition: transform .12s, box-shadow .12s; }
.tp-card:hover { transform: translateY(-2px); box-shadow: var(--shadow); }
.tp-card.ended { opacity: 0.65; }
.tp-head { display: flex; align-items: center; justify-content: space-between; margin-bottom: 8px; }
.tp-name { font-size: 16px; font-weight: 700; color: var(--ink); margin-bottom: 4px; }
.tp-desc { font-size: 12.5px; color: var(--ink-3); line-height: 1.5; min-height: 36px; }
.tp-stats { display: grid; grid-template-columns: repeat(3, 1fr); gap: 8px; margin: 12px 0; padding: 10px 0; border-top: 1px solid var(--line); border-bottom: 1px solid var(--line); }
.tps .tps-v { font-size: 18px; font-weight: 700; color: var(--ink); }
.tps .tps-l { font-size: 11px; color: var(--ink-3); }
.tp-foot { display: flex; gap: 4px; }
</style>
