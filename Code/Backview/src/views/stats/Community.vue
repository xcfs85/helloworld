<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { getCommunityStats } from '@/api/stats'

const data = ref<any>(null)

onMounted(async () => {
  data.value = await getCommunityStats()
  drawPostType()
  drawInteraction()
  drawFeed()
})

function drawPostType() {
  const el = document.getElementById('postType')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const p = data.value.post_types
    chart.setOption({
      series: [{
        type: 'pie', radius: ['55%', '85%'],
        label: { show: true, fontSize: 11, color: '#1F1A16', formatter: (p: any) => p.name + ' ' + p.value + '%' },
        data: [
          { value: p.work, name: '作品', itemStyle: { color: '#FF7A5A' } },
          { value: p.tutorial, name: '教程', itemStyle: { color: '#4FBB8A' } },
          { value: p.question, name: '提问', itemStyle: { color: '#6FA8D4' } }
        ]
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawInteraction() {
  const el = document.getElementById('interactionChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const i = data.value.interactions
    chart.setOption({
      series: [{
        type: 'pie', radius: '70%',
        label: { show: true, fontSize: 11, color: '#1F1A16', formatter: (p: any) => p.name + ' ' + p.value + '%' },
        data: [
          { value: i.like, name: '点赞', itemStyle: { color: '#E07777' } },
          { value: i.comment, name: '评论', itemStyle: { color: '#6FA8D4' } },
          { value: i.favorite, name: '收藏', itemStyle: { color: '#F5C45E' } },
          { value: i.share, name: '分享', itemStyle: { color: '#9A7FCC' } }
        ]
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawFeed() {
  const el = document.getElementById('feedChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const f = data.value.feed
    chart.setOption({
      series: [{
        type: 'pie', radius: '70%',
        label: { show: true, fontSize: 11, color: '#1F1A16', formatter: (p: any) => p.name + ' ' + p.value + '%' },
        data: [
          { value: f.recommend, name: '推荐', itemStyle: { color: '#FF7A5A' } },
          { value: f.follow, name: '关注', itemStyle: { color: '#4FBB8A' } }
        ]
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '数据统计' }, { label: '社区分析' }]"
      title="社区分析"
      sub="本月发布 12,345 · 互动 234,567"
    >
      <template #actions>
        <div class="date-range">
          <input value="2026-05-08" />
          <span class="sep">→</span>
          <input value="2026-06-07" />
        </div>
        <button class="btn btn-primary">导出</button>
      </template>
    </PageHead>

    <div class="kpi-row">
      <div class="kpi-card"><div class="kpi-lbl">发布数</div><div class="kpi">12,345</div><div class="kpi-sub"><span class="up">↑ 15.6%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">互动量</div><div class="kpi">234,567</div><div class="kpi-sub"><span class="up">↑ 10.2%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">日均发帖</div><div class="kpi">1,234</div><div class="kpi-sub"><span class="up">↑ 5.6%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">互动率</div><div class="kpi">19.0%</div><div class="kpi-sub"><span class="down">↓ 1.2%</span></div></div>
    </div>

    <div class="grid-3">
      <div class="panel">
        <div class="panel-head"><div class="ph-title">帖子类型</div></div>
        <div class="panel-body">
          <div id="postType" class="donut" style="height: 200px"></div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">互动构成</div></div>
        <div class="panel-body">
          <div id="interactionChart" class="donut" style="height: 200px"></div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">Feed 分发</div></div>
        <div class="panel-body">
          <div id="feedChart" class="donut" style="height: 200px"></div>
        </div>
      </div>
    </div>

    <div class="panel">
      <div class="panel-head"><div class="ph-title">热门话题 TOP 10</div></div>
      <div class="panel-body">
        <table class="tbl">
          <thead>
            <tr>
              <th style="width: 60px">排名</th>
              <th>话题</th>
              <th>帖子数</th>
              <th>参与用户</th>
              <th>趋势</th>
              <th>状态</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(t, i) in (data?.top_topics || [])" :key="t.name">
              <td>
                <div class="rank" :class="{ gold: i === 0, silver: i === 1, bronze: i === 2 }">{{ i + 1 }}</div>
              </td>
              <td style="font-weight: 600"># {{ t.name }}</td>
              <td>{{ t.post_count.toLocaleString() }}</td>
              <td>{{ t.user_count.toLocaleString() }}</td>
              <td>
                <div class="row" style="gap: 6px">
                  <div style="width: 100px"><div class="progress"><div class="fill" :style="`width: ${Math.min(t.post_count / 30, 100)}%`"></div></div></div>
                  <span class="up" style="font-size: 11px">↑ {{ (10 + Math.random() * 20).toFixed(1) }}%</span>
                </div>
              </td>
              <td><StatusTag variant="primary">推荐中</StatusTag></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { padding: 16px 22px; display: flex; flex-direction: column; gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.kpi-row { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }
.kpi-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; padding: 14px 16px; }
.kpi-card .kpi-lbl { font-size: 12px; color: var(--ink-3); font-weight: 500; }
.kpi-card .kpi { font-size: 24px; margin-top: 6px; }
.kpi-card .kpi-sub { font-size: 11px; color: var(--ink-3); display: flex; align-items: center; gap: 4px; margin-top: 2px; }
.kpi-card .kpi-sub .up { color: var(--mint); }
.kpi-card .kpi-sub .down { color: var(--rose); }
.grid-3 { display: grid; grid-template-columns: repeat(3, 1fr); gap: 14px; }
.donut { width: 100%; }
.rank { width: 26px; height: 26px; border-radius: 50%; background: var(--bg-2); display: grid; place-items: center; font-weight: 700; font-size: 12px; }
.rank.gold { background: linear-gradient(135deg, #F5C45E, #FF8A5A); color: #fff; }
.rank.silver { background: linear-gradient(135deg, #B0A599, #E6DFD2); color: #fff; }
.rank.bronze { background: linear-gradient(135deg, #E59A3A, #B0A599); color: #fff; }
.tbl { width: 100%; border-collapse: collapse; font-size: 12.5px; }
.tbl thead th { background: var(--surface-2); text-align: left; padding: 8px 12px; font-weight: 600; color: var(--ink-2); border-bottom: 1px solid var(--line); font-size: 11.5px; }
.tbl tbody td { padding: 10px 12px; border-bottom: 1px solid var(--line); color: var(--ink-2); }
</style>
