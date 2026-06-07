<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listBanners, addBanner, updateBanner, deleteBanner } from '@/api/operation'
import type { Banner } from '@/types'

const loading = ref(false)
const list = ref<Banner[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)
const filter = reactive({ position: 'all', status: 'all' })
const showAdd = ref(false)
const editing = ref<Banner | null>(null)

const formData = ref({ title: '', image: '', link_type: 'template', link_url: '', position: '首页顶部', start_time: '', end_time: '', sort: 99, status: 'visible' })

async function load() {
  loading.value = true
  try {
    const res: any = await listBanners({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}

function openAdd() {
  showAdd.value = true
  editing.value = null
  formData.value = { title: '', image: '', link_type: 'template', link_url: '', position: '首页顶部', start_time: '', end_time: '', sort: 99, status: 'visible' }
}
function openEdit(b: Banner) {
  showAdd.value = true
  editing.value = b
  Object.assign(formData.value, b)
}
async function onSave() {
  if (editing.value) {
    await updateBanner(editing.value.id, formData.value)
    ElMessage.success('已更新')
  } else {
    if (!formData.value.title) { ElMessage.warning('请输入标题'); return }
    await addBanner(formData.value)
    ElMessage.success('已创建')
  }
  showAdd.value = false
  load()
}
async function onDelete(b: Banner) {
  try {
    await ElMessageBox.confirm(`确定删除 Banner「${b.title}」？`, '提示', { type: 'warning' })
    await deleteBanner(b.id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

const covers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)'
]
function coverFor(b: Banner) { return covers[parseInt(b.id.slice(-1)) % covers.length] }

onMounted(load)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营管理' }, { label: 'Banner 管理' }]"
      title="Banner 管理"
      sub="首页 Banner · 3 个生效中"
    >
      <template #actions>
        <button class="btn btn-primary" @click="openAdd">+ 新建 Banner</button>
      </template>
    </PageHead>

    <div class="toolbar">
      <div class="f-select"><select v-model="filter.position" @change="load">
        <option value="all">位置：全部</option><option value="home_top">首页顶部</option><option value="activity">活动页</option>
      </select></div>
      <div class="f-select"><select v-model="filter.status" @change="load">
        <option value="all">状态：全部</option><option value="visible">显示</option><option value="hidden">隐藏</option>
      </select></div>
      <button class="btn btn-sm btn-primary" @click="load">搜索</button>
    </div>

    <div class="grid">
      <div v-for="b in list" :key="b.id" class="bn-card">
        <div class="bn-cover" :style="`background: ${coverFor(b)}`">
          <span class="bn-status" :class="b.status">{{ b.status === 'visible' ? '显示中' : '已隐藏' }}</span>
        </div>
        <div class="bn-body">
          <div style="font-weight: 700">{{ b.title }}</div>
          <div class="muted small">#{{ b.sort }} · {{ b.start_time }} ~ {{ b.end_time }}</div>
          <div class="row" style="margin-top: 6px; gap: 4px">
            <StatusTag :variant="b.link_type === 'web' ? 'info' : b.link_type === 'template' ? 'primary' : 'purple'">
              {{ { template: '模板', special: '专题', web: 'H5', activity: '活动' }[b.link_type] }}
            </StatusTag>
            <span class="muted small">点击 {{ b.click_count.toLocaleString() }}</span>
          </div>
          <div class="bn-foot">
            <button class="btn btn-xs btn-ghost" @click="openEdit(b)">编辑</button>
            <button class="btn btn-xs btn-ghost" @click="updateBanner(b.id, { status: b.status === 'visible' ? 'hidden' : 'visible' }); load()">
              {{ b.status === 'visible' ? '隐藏' : '显示' }}
            </button>
            <button class="btn btn-xs btn-ghost" style="color: var(--rose)" @click="onDelete(b)">删除</button>
          </div>
        </div>
      </div>
    </div>

    <Pager :total="total" :page="page" :page-size="pageSize"
      @update:page="(v) => { page = v; load() }"
      @update:page-size="(v) => { pageSize = v; load() }" />

    <el-dialog v-model="showAdd" :title="editing ? '编辑 Banner' : '新建 Banner'" width="540">
      <div class="form-row"><div class="lbl req">标题</div>
        <div class="input-line"><input v-model="formData.title" placeholder="Banner 标题" /></div>
      </div>
      <div class="form-row"><div class="lbl req">图片</div>
        <div class="input-line"><input v-model="formData.image" placeholder="图片 URL 或上传" /></div>
      </div>
      <div class="form-row"><div class="lbl">跳转</div>
        <div class="row" style="gap: 6px; width: 100%">
          <div class="f-select" style="width: 100px"><select v-model="formData.link_type">
            <option value="template">模板</option><option value="special">专题</option><option value="web">H5</option><option value="activity">活动</option>
          </select></div>
          <div class="input-line" style="flex: 1"><input v-model="formData.link_url" placeholder="跳转链接 / ID" /></div>
        </div>
      </div>
      <div class="form-row"><div class="lbl">位置</div>
        <div class="f-select" style="width: 100%"><select v-model="formData.position">
          <option value="首页顶部">首页顶部</option><option value="首页中部">首页中部</option><option value="活动页">活动页</option>
        </select></div>
      </div>
      <div class="form-row"><div class="lbl">生效时间</div>
        <div class="row" style="gap: 6px; width: 100%">
          <div class="input-line" style="flex: 1"><input v-model="formData.start_time" placeholder="开始" /></div>
          <span>~</span>
          <div class="input-line" style="flex: 1"><input v-model="formData.end_time" placeholder="结束" /></div>
        </div>
      </div>
      <div class="form-row"><div class="lbl">排序</div>
        <div class="input-line"><input type="number" v-model="formData.sort" /></div>
      </div>
      <template #footer>
        <el-button @click="showAdd = false">取消</el-button>
        <el-button type="primary" @click="onSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.grid { padding: 16px 22px; display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.bn-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; overflow: hidden; transition: transform .12s, box-shadow .12s; }
.bn-card:hover { transform: translateY(-2px); box-shadow: var(--shadow); }
.bn-cover { aspect-ratio: 16/9; position: relative; }
.bn-status { position: absolute; top: 8px; right: 8px; background: rgba(0,0,0,.5); color: #fff; font-size: 10px; padding: 2px 8px; border-radius: 4px; }
.bn-status.hidden { background: var(--ink-3); }
.bn-body { padding: 12px 14px; }
.bn-foot { display: flex; gap: 4px; margin-top: 10px; padding-top: 8px; border-top: 1px solid var(--line); }
</style>
