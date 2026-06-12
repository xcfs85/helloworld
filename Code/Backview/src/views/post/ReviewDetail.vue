<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { getPost, approvePost, rejectPost, type PostDetailItem } from '@/api/post'

const route = useRoute()
const router = useRouter()
const post = ref<PostDetailItem | null>(null)
const rejectReason = ref('')
const selectedReason = ref('')
const note = ref('')

const reasonOptions = ['内容违规', '广告营销', '色情低俗', '政治敏感', '重复内容', '其他原因']
const activeTab = ref('detail')

onMounted(async () => {
  post.value = await getPost(route.params.id as string)
})

async function onApprove() {
  await approvePost(post.value!.id)
  ElMessage.success('已通过审核')
  router.back()
}
async function onReject() {
  if (!selectedReason.value) { ElMessage.warning('请选择拒绝原因'); return }
  await rejectPost(post.value!.id, selectedReason.value)
  ElMessage.success('已拒绝')
  router.back()
}

const postCovers = [
  'linear-gradient(135deg,#FFD2B0,#FF7A5A)',
  'linear-gradient(135deg,#9DC8E5,#6FA8D4)',
  'linear-gradient(135deg,#F2A6A6,#9A7FCC)',
  'linear-gradient(135deg,#4FBB8A,#6FA8D4)',
  'linear-gradient(135deg,#F5C45E,#FF8A5A)',
  'linear-gradient(135deg,#E07777,#F5C45E)',
  'linear-gradient(135deg,#9A7FCC,#E07777)',
  'linear-gradient(135deg,#4FBB8A,#F5C45E)',
  'linear-gradient(135deg,#FF7A5A,#F5C45E)'
]
function coverFor(i: number) { return postCovers[i % postCovers.length] }
</script>

<template>
  <div class="page-view" v-if="post">
    <PageHead
      :crumbs="[{ label: '内容管理', to: '/post/review/list' }, { label: '帖子审核', to: '/post/review/list' }, { label: post.id }]"
      :title="`帖子审核详情`"
      :sub="`${post.id} · 提交于 ${post.publish_time || post.create_time}`"
    >
      <template #actions>
        <button class="btn btn-secondary" @click="router.back()">返回列表</button>
        <button class="btn btn-secondary">查看作者</button>
      </template>
    </PageHead>

    <div class="review-grid">
      <div class="col-main">
        <div class="panel">
          <div class="panel-head"><div class="ph-title">帖子信息</div></div>
          <div class="panel-body">
            <div class="form-row"><div class="lbl">类型</div><div><StatusTag :variant="({ work: 'primary', tutorial: 'info', question: 'purple' } as any)[post.type] || 'neutral'">{{ { work: '作品', tutorial: '教程', question: '提问' }[post.type as 'work' | 'tutorial' | 'question'] || post.type }}</StatusTag></div></div>
            <div class="form-row"><div class="lbl">标题</div><div style="font-weight: 600">{{ post.title }}</div></div>
            <div class="form-row">
              <div class="lbl">作者</div>
              <div class="row" style="gap: 8px">
                <div class="av sm" :class="'c' + ((parseInt((post.author?.id || 'u_0').slice(2)) % 6) + 1)">{{ post.author?.nickname?.[0] || '?' }}</div>
                <div>{{ post.author?.nickname }} ({{ post.author?.id }})</div>
              </div>
            </div>
            <div class="form-row"><div class="lbl">发布时间</div><div>{{ post.publish_time || post.create_time }}</div></div>
            <div class="form-row"><div class="lbl">IP / 设备</div><div>{{ post.author?.id ? '已记录' : '—' }}</div></div>
            <div class="form-row"><div class="lbl">话题</div><div>{{ (post.topic_ids || []).join(' ') }}</div></div>
            <div v-if="post.diagram_id" class="form-row"><div class="lbl">关联图纸</div><div><a class="mono" style="color: var(--primary-ink)">{{ post.diagram_id }}</a></div></div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head"><div class="ph-title">内容</div></div>
          <div class="panel-body">
            <div class="media-grid">
              <div v-for="i in 9" :key="i" class="media-cell" :style="`background: ${coverFor(i - 1)}`"></div>
            </div>
            <div class="text-block">
              <p>{{ post.content }}</p>
              <p class="muted">#生日礼物 #闺蜜 #拼豆日常</p>
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head"><div class="ph-title">AI 审核结果</div><StatusTag variant="ok">风险等级：低</StatusTag></div>
          <div class="panel-body">
            <div class="dimension">
              <div class="dn">政治敏感</div>
              <div class="dr" style="color: var(--mint)">✓ 通过</div>
            </div>
            <div class="dimension">
              <div class="dn">色情低俗</div>
              <div class="dr" style="color: var(--mint)">✓ 通过</div>
            </div>
            <div class="dimension">
              <div class="dn">暴恐血腥</div>
              <div class="dr" style="color: var(--mint)">✓ 通过</div>
            </div>
            <div class="dimension">
              <div class="dn">广告营销</div>
              <div class="dr" style="color: var(--mint)">✓ 通过</div>
            </div>
            <div class="dimension">
              <div class="dn">版权风险</div>
              <div class="dr" style="color: var(--mint)">✓ 通过</div>
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
          <div class="panel-head"><div class="ph-title">作者信息</div></div>
          <div class="panel-body">
            <div class="form-row">
              <div class="lbl">头像 / 昵称</div>
              <div class="row" style="gap: 10px">
                <div class="av" :class="'c' + ((parseInt((post.author?.id || 'u_0').slice(2)) % 6) + 1)">{{ post.author?.nickname?.[0] || '?' }}</div>
                <div><div style="font-weight: 600">{{ post.author?.nickname }}</div><div class="muted small">{{ post.author?.id }}</div></div>
              </div>
            </div>
            <div class="form-row"><div class="lbl">历史发帖</div><div>23</div></div>
            <div class="form-row"><div class="lbl">违规记录</div><div>0 次</div></div>
            <div class="form-row"><div class="lbl">被举报</div><div>1 次 · 已处理</div></div>
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
.media-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 8px; margin-bottom: 14px; }
.media-cell { aspect-ratio: 1; border-radius: 8px; position: relative; overflow: hidden; }
.dimension { display: flex; align-items: center; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid var(--line); font-size: 12.5px; }
.dimension:last-child { border-bottom: none; }
.dimension .dn { color: var(--ink-2); font-weight: 500; }
.dimension .dr { font-size: 11px; font-weight: 600; display: flex; align-items: center; gap: 4px; }
.reason-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 6px; margin-top: 6px; }
.reason-cell { padding: 6px 8px; background: var(--bg); border: 1px solid var(--line); border-radius: 5px; font-size: 12px; text-align: center; color: var(--ink-2); cursor: pointer; }
.reason-cell:hover { border-color: var(--ink-3); }
.reason-cell.active { background: var(--rose-soft); border-color: var(--rose); color: var(--rose); font-weight: 600; }
.text-block { font-size: 13px; line-height: 1.7; color: var(--ink-2); padding: 8px 0; }
.text-block p { margin: 0 0 6px; }
</style>
