<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listSensitiveWords, addSensitiveWord, deleteSensitiveWord } from '@/api/post'
import type { SensitiveWord } from '@/types'

const loading = ref(false)
const list = ref<SensitiveWord[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)

const filter = reactive({ keyword: '', level: 'all', type: 'all' })
const showAdd = ref(false)
const newWord = reactive({ word: '', level: 1, type: 'political', replacement: '***' })

const levelNameMap: Record<number, string> = { 1: 'severe', 2: 'medium', 3: 'minor' }
const nameToLevelMap: Record<string, number> = { severe: 1, medium: 2, minor: 3 }

async function load() {
  loading.value = true
  try {
    const params: any = {
      page: page.value,
      page_size: pageSize.value,
      keyword: filter.keyword,
      type: filter.type
    }
    if (filter.level !== 'all') params.level = nameToLevelMap[filter.level] ?? 1
    const res: any = await listSensitiveWords(params)
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

async function onAdd() {
  if (!newWord.word) { ElMessage.warning('请输入敏感词'); return }
  const payload: any = { word: newWord.word, level: newWord.level, type: newWord.type }
  if (newWord.replacement) payload.replace_word = newWord.replacement
  await addSensitiveWord(payload)
  ElMessage.success('已添加')
  showAdd.value = false
  Object.assign(newWord, { word: '', level: 1, type: 'political', replacement: '***' })
  load()
}
async function onDelete(w: SensitiveWord) {
  try {
    await ElMessageBox.confirm(`确定删除敏感词「${w.word}」？`, '提示', { type: 'warning' })
    await deleteSensitiveWord(w.id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

const stats = { total: 1234, today_hit: 56789 }

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '内容管理' }, { label: '敏感词管理' }]"
      title="敏感词管理"
      :sub="`敏感词库总数: ${stats.total} 条 · 今日调用: ${stats.today_hit.toLocaleString()} 次`"
    >
      <template #actions>
        <button class="btn btn-secondary">批量导入</button>
        <button class="btn btn-secondary">导出</button>
        <button class="btn btn-primary" @click="showAdd = true">+ 添加敏感词</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="search-input">
        <el-icon><Search /></el-icon>
        <input v-model="filter.keyword" placeholder="敏感词 / 替换词" @keydown.enter="load" />
      </div>
      <div class="f-select"><select v-model="filter.level" @change="load">
        <option value="all">级别：全部</option>
        <option value="severe">严重</option>
        <option value="medium">中等</option>
        <option value="minor">轻微</option>
      </select></div>
      <div class="f-select"><select v-model="filter.type" @change="load">
        <option value="all">类型：全部</option>
        <option value="political">政治</option>
        <option value="porn">色情</option>
        <option value="violence">暴恐</option>
        <option value="ads">广告</option>
        <option value="copyright">版权</option>
        <option value="other">其他</option>
      </select></div>
      <button class="btn btn-sm btn-secondary" @click="filter.keyword = ''; filter.level = 'all'; filter.type = 'all'; load()">重置</button>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="tbl-wrap">
      <table class="tbl">
        <thead>
          <tr>
            <th style="width: 32px"><span class="ck"></span></th>
            <th>敏感词</th>
            <th>级别</th>
            <th>类型</th>
            <th>替换词</th>
            <th>命中次数</th>
            <th>创建时间</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="w in list" :key="w.id">
            <td><span class="ck"></span></td>
            <td style="font-weight: 600; color: var(--ink)">{{ w.word }}</td>
            <td>
              <StatusTag :variant="({ severe: 'danger', medium: 'warn', minor: 'info' } as any)[w.level]">
                {{ { severe: '严重', medium: '中等', minor: '轻微' }[w.level] }}
              </StatusTag>
            </td>
            <td>
              <StatusTag :variant="({ political: 'danger', porn: 'danger', violence: 'danger', ads: 'warn', copyright: 'purple', other: 'neutral' } as any)[w.type]">
                {{ { political: '政治', porn: '色情', violence: '暴恐', ads: '广告', copyright: '版权', other: '其他' }[w.type] }}
              </StatusTag>
            </td>
            <td class="mono">{{ w.replacement }}</td>
            <td>{{ w.hit_count.toLocaleString() }}</td>
            <td class="muted">{{ w.create_time }}</td>
            <td class="col-actions">
              <button class="btn btn-xs btn-ghost">编辑</button>
              <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onDelete(w)">删除</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" title="添加敏感词" width="480">
      <div class="form-row"><div class="lbl req">敏感词</div><div class="input-line"><input v-model="newWord.word" placeholder="请输入敏感词" /></div></div>
      <div class="form-row"><div class="lbl req">级别</div>
        <div class="f-select" style="width: 100%"><select v-model="newWord.level">
          <option value="severe">严重 - 直接拒绝</option>
          <option value="medium">中等 - 人工审核</option>
          <option value="minor">轻微 - 警告放行</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl req">类型</div>
        <div class="f-select" style="width: 100%"><select v-model="newWord.type">
          <option value="political">政治</option><option value="porn">色情</option><option value="violence">暴恐</option>
          <option value="ads">广告</option><option value="copyright">版权</option><option value="other">其他</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl">替换词</div><div class="input-line"><input v-model="newWord.replacement" placeholder="替换为" /></div></div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="onAdd">添加</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.tbl-wrap { flex: 1; overflow: auto; }
</style>
