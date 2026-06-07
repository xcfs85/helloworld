<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PageHead from '@/components/PageHead.vue'
import { getCreationStats } from '@/api/stats'

const data = ref<any>(null)

onMounted(async () => {
  data.value = await getCreationStats()
  drawGeneration()
  drawColor()
  drawDifficulty()
  drawStyle()
})

function drawGeneration() {
  const el = document.getElementById('genChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const dates = Array.from({ length: 14 }, (_, i) => {
      const d = new Date(); d.setDate(d.getDate() - (13 - i))
      return `${d.getMonth() + 1}/${d.getDate()}`
    })
    chart.setOption({
      grid: { top: 16, right: 16, bottom: 32, left: 50 },
      tooltip: { trigger: 'axis' },
      xAxis: { type: 'category', data: dates, axisLine: { lineStyle: { color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 }, axisTick: { show: false } },
      yAxis: { type: 'value', splitLine: { lineStyle: { type: 'dashed', color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 } },
      series: [{
        name: 'AI 生成', type: 'bar', data: data.value.generation,
        barWidth: 20,
        itemStyle: { color: { type: 'linear', x: 0, y: 0, x2: 0, y2: 1, colorStops: [
          { offset: 0, color: '#FF8A5A' }, { offset: 1, color: '#F5C45E' }
        ] }, borderRadius: [4, 4, 0, 0] }
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawColor() {
  const el = document.getElementById('colorChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      series: [{
        type: 'pie',
        radius: ['55%', '85%'],
        label: { show: false },
        data: data.value.color_distribution.map((c: any, i: number) => ({
          value: c.value,
          name: c.range,
          itemStyle: { color: ['#FF7A5A', '#F5C45E', '#4FBB8A', '#6FA8D4', '#9A7FCC'][i] }
        }))
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawDifficulty() {
  const el = document.getElementById('diffChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const d = data.value.difficulty
    chart.setOption({
      series: [{
        type: 'pie', radius: '70%',
        label: { show: true, fontSize: 11, color: '#1F1A16' },
        data: [
          { value: d.beginner, name: '入门 ' + d.beginner + '%', itemStyle: { color: '#4FBB8A' } },
          { value: d.intermediate, name: '进阶 ' + d.intermediate + '%', itemStyle: { color: '#F5C45E' } },
          { value: d.advanced, name: '高阶 ' + d.advanced + '%', itemStyle: { color: '#E07777' } }
        ]
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawStyle() {
  const el = document.getElementById('styleChart')
  if (!el) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const s = data.value.style
    chart.setOption({
      series: [{
        type: 'pie', radius: '70%',
        label: { show: true, fontSize: 11, color: '#1F1A16' },
        data: [
          { value: s['写实'], name: '写实 ' + s['写实'] + '%', itemStyle: { color: '#FF7A5A' } },
          { value: s['卡通'], name: '卡通 ' + s['卡通'] + '%', itemStyle: { color: '#F5C45E' } },
          { value: s['写意'], name: '写意 ' + s['写意'] + '%', itemStyle: { color: '#6FA8D4' } },
          { value: s['抽象'], name: '抽象 ' + s['抽象'] + '%', itemStyle: { color: '#9A7FCC' } }
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
      :crumbs="[{ label: '数据统计' }, { label: '创作分析' }]"
      title="创作分析"
      sub="近 30 天 AI 生成 5,678 · 模板使用 12,345"
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
      <div class="kpi-card"><div class="kpi-lbl">AI 生成次数</div><div class="kpi">5,678</div><div class="kpi-sub"><span class="up">↑ 8.9%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">模板使用量</div><div class="kpi">12,345</div><div class="kpi-sub"><span class="up">↑ 12.3%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">导出作品</div><div class="kpi">3,850</div><div class="kpi-sub"><span class="up">↑ 5.6%</span></div></div>
      <div class="kpi-card"><div class="kpi-lbl">导出率</div><div class="kpi">67.8%</div><div class="kpi-sub"><span class="up">↑ 1.2%</span></div></div>
    </div>

    <div class="panel">
      <div class="panel-head"><div class="ph-title">AI 生成趋势 <span class="ct">近 14 天</span></div></div>
      <div class="panel-body">
        <div id="genChart" class="bar-chart"></div>
      </div>
    </div>

    <div class="grid-3">
      <div class="panel">
        <div class="panel-head"><div class="ph-title">色数分布</div></div>
        <div class="panel-body">
          <div id="colorChart" class="donut"></div>
          <div class="legend" style="margin-top: 12px">
            <div v-for="(c, i) in (data?.color_distribution || [])" :key="i" class="lg">
              <span class="dot" :style="`background: ${['#FF7A5A', '#F5C45E', '#4FBB8A', '#6FA8D4', '#9A7FCC'][i]}`"></span>
              <span>{{ c.range }} 色</span>
              <span style="font-weight: 600; margin-left: auto">{{ c.value }}%</span>
            </div>
          </div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">难度分布</div></div>
        <div class="panel-body">
          <div id="diffChart" class="donut" style="height: 200px"></div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">风格偏好</div></div>
        <div class="panel-body">
          <div id="styleChart" class="donut" style="height: 200px"></div>
        </div>
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
.bar-chart { width: 100%; height: 220px; }
.donut { width: 100%; height: 160px; }
.legend { display: flex; flex-direction: column; gap: 6px; font-size: 12px; color: var(--ink-2); }
.lg { display: flex; align-items: center; gap: 8px; }
.lg span:not(:first-child) { color: var(--ink-2); }
</style>
