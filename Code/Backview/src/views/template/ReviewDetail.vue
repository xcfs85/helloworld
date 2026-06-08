<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { getTemplate, approveTemplate, rejectTemplate } from '@/api/template'
import type { Template } from '@/types'

const route = useRoute()
const router = useRouter()
const tpl = ref<Template | null>(null)
const note = ref('')
const selectedReason = ref('')

const reasonOptions = ['非官方角色', '作品侵权', '低俗违规', '清晰度不达标', '信息有误', '其他原因']

onMounted(async () => { tpl.value = await getTemplate(route.params.id as string) })

async function onApprove() { await approveTemplate(tpl.value!.id); ElMessage.success('已通过'); router.back() }
async function onReject() {
  if (!selectedReason.value) { ElMessage.warning('请选择原因'); return }
  try {
    await ElMessageBox.confirm(`确定拒绝该模板投稿？`, '拒绝模板', { type: 'warning' })
    await rejectTemplate(tpl.value!.id, selectedReason.value)
    ElMessage.success('已拒绝')
    router.back()
  } catch {}
}

const covers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)',
  'linear-gradient(135deg,#F5C45E,#FF8A5A)',
  'linear-gradient(135deg,#E07777,#F5C45E)'
]
function coverFor(i: number) { return covers[i % covers.length] }
</script>

<template>
  <div class="page-view" v-if="tpl">
    <PageHead
      :crumbs="[{ label: '模板管理', to: '/template/list' }, { label: '模板审核', to: '/template/list' }, { label: tpl.name }]"
      :title="`模板审核详情`"
      :sub="`${tpl.id} · 提交于 ${tpl.submit_time}`"
    >
      <template #actions>
        <button class="btn btn-secondary" @click="router.back()">返回</button>
      </template>
    </PageHead>

    <div class="review-grid">
      <div class="col-main">
        <div class="panel">
          <div class="panel-head"><div class="ph-title">模板信息</div></div>
          <div class="panel-body">
            <div class="form-row"><div class="lbl">名称</div><div style="font-weight: 600">{{ tpl.name }}</div></div>
            <div class="form-row"><div class="lbl">分类</div><div>{{ tpl.category_name }}</div></div>
            <div class="form-row"><div class="lbl">标签</div><div class="row" style="gap: 6px"><span v-for="tg in tpl.tags" :key="tg" class="chip">#{{ tg }}</span></div></div>
            <div class="form-row"><div class="lbl">规格</div><div>{{ tpl.board_size }} · {{ tpl.color_count }} 色 · {{ tpl.total_beads.toLocaleString() }} 颗</div></div>
            <div class="form-row"><div class="lbl">难度</div><div><StatusTag :variant="tpl.difficulty === 'beginner' ? 'ok' : tpl.difficulty === 'intermediate' ? 'warn' : 'danger'">{{ { beginner: '入门', intermediate: '进阶', advanced: '高阶' }[tpl.difficulty] }}</StatusTag></div></div>
            <div class="form-row"><div class="lbl">风格</div><div>{{ tpl.style }} · {{ tpl.duration }}</div></div>
            <div class="form-row"><div class="lbl">作者</div>
              <div class="row" style="gap: 8px">
                <div class="av sm" :class="'c' + ((parseInt(tpl.creator_id.slice(-1)) % 6) + 1)">{{ tpl.creator_name[0] }}</div>
                <div>
                  <div style="font-weight: 600">{{ tpl.creator_name }}</div>
                  <div class="muted small">{{ tpl.creator_id }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head"><div class="ph-title">预览</div></div>
          <div class="panel-body">
            <div class="media-grid">
              <div v-for="i in 4" :key="i" class="media-cell" :style="`background: ${coverFor(i - 1)}`"></div>
            </div>
            <div style="padding: 12px 0; font-size: 13px; line-height: 1.7; color: var(--ink-2)">
              {{ tpl.name }} - {{ tpl.category_name }}主题模板
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head"><div class="ph-title">色板预览</div></div>
          <div class="panel-body">
            <div class="palette-preview">
              <div v-for="i in 32" :key="i" class="palette-cell" :style="`background: ${['#FF7A5A','#F5C45E','#4FBB8A','#6FA8D4','#9A7FCC','#E07777','#FFD2B0','#1F1A16','#FAF7F1','#E07777','#4FBB8A','#F5C45E','#6FA8D4','#9A7FCC','#E07777','#FF7A5A'][i % 16]}`"></div>
            </div>
            <div class="palette-info">
              <div class="form-row"><div class="lbl">色号分布</div><div class="row" style="gap: 8px; flex-wrap: wrap">
                <span v-for="c in ['H001 拼豆橙 × 132', 'H002 琥珀黄 × 88', 'H003 薄荷绿 × 65', 'H004 天空蓝 × 42', 'H005 紫罗兰 × 23']" :key="c" class="chip">{{ c }}</span>
              </div></div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-side">
        <div class="panel">
          <div class="panel-head"><div class="ph-title">审核操作</div></div>
          <div class="panel-body">
            <div class="form-row"><div class="lbl req">审核结论</div><div class="row" style="gap: 8px">
              <button class="btn btn-sm" style="background: var(--mint); color: #fff" @click="onApprove">通过</button>
              <button class="btn btn-sm btn-danger" @click="onReject">拒绝</button>
            </div></div>
            <div class="form-row" style="grid-template-columns: 1fr; padding-top: 14px">
              <div class="lbl">拒绝原因 <span class="muted small">（点击选择）</span></div>
              <div class="reason-grid">
                <div v-for="r in reasonOptions" :key="r" class="reason-cell" :class="{ active: selectedReason === r }" @click="selectedReason = r">{{ r }}</div>
              </div>
            </div>
            <div class="form-row" style="grid-template-columns: 1fr">
              <div class="lbl">备注</div>
              <textarea v-model="note" class="textarea" placeholder="可填写补充说明..."></textarea>
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head"><div class="ph-title">数据指标</div></div>
          <div class="panel-body">
            <div class="form-row"><div class="lbl">浏览量</div><div>1,234</div></div>
            <div class="form-row"><div class="lbl">收藏数</div><div>89</div></div>
            <div class="form-row"><div class="lbl">使用量</div><div>12</div></div>
            <div class="form-row"><div class="lbl">评分</div><div>4.8 / 5.0</div></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.review-grid { display: grid; grid-template-columns: 1fr 320px; gap: 14px; padding: 16px 22px; flex: 1; overflow: auto; background: var(--bg-2); }
.col-main, .col-side { display: flex; flex-direction: column; gap: 14px; min-width: 0; }
.media-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 8px; }
.media-cell { aspect-ratio: 1; border-radius: 8px; }
.palette-preview { display: grid; grid-template-columns: repeat(16, 1fr); gap: 4px; margin-bottom: 12px; }
.palette-cell { aspect-ratio: 1; border-radius: 3px; }
.reason-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 6px; margin-top: 6px; }
.reason-cell { padding: 6px 8px; background: var(--bg); border: 1px solid var(--line); border-radius: 5px; font-size: 12px; text-align: center; color: var(--ink-2); cursor: pointer; }
.reason-cell:hover { border-color: var(--ink-3); }
.reason-cell.active { background: var(--rose-soft); border-color: var(--rose); color: var(--rose); font-weight: 600; }
</style>
