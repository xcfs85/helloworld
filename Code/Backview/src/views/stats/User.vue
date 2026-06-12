<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PageHead from '@/components/PageHead.vue'
import { getTrends, type DailyStats } from '@/api/stats'

const data = ref<any>({
  growth: [] as number[],
  gender: { male: 0, female: 0, unknown: 0 },
  age: [] as { range: string; value: number }[],
  city: [] as { name: string; value: number }[],
  register_method: {} as Record<string, number>
})

onMounted(async () => {
  try {
    const end = new Date()
    const start = new Date()
    start.setDate(start.getDate() - 29) // 近 30 天
    const fmt = (d: Date) => d.toISOString().split('T')[0]
    const list: DailyStats[] = await getTrends({ start: fmt(start), end: fmt(end) })

    // 从 daily_stats 派生可用的数据
    data.value = {
      ...data.value,
      growth: list.map(d => d.new_user_count ?? 0)
    }

    drawLine()
    drawGender()
    drawAge()
    drawCity()
  } catch (e) {
    console.error('[Stats/User] 加载数据失败', e)
  }
})

function drawLine() {
  const el = document.getElementById('userLine')
  if (!el || !data.value) return
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    const dates = Array.from({ length: 30 }, (_, i) => {
      const d = new Date()
      d.setDate(d.getDate() - (29 - i))
      return `${d.getMonth() + 1}/${d.getDate()}`
    })
    chart.setOption({
      grid: { top: 16, right: 16, bottom: 32, left: 40 },
      tooltip: { trigger: 'axis' },
      xAxis: { type: 'category', data: dates, axisLine: { lineStyle: { color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 }, axisTick: { show: false } },
      yAxis: { type: 'value', splitLine: { lineStyle: { type: 'dashed', color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 } },
      series: [{
        name: '新增用户', type: 'line', smooth: true, data: data.value.growth,
        lineStyle: { color: '#FF7A5A', width: 2 },
        itemStyle: { color: '#FF7A5A' },
        areaStyle: { color: { type: 'linear', x: 0, y: 0, x2: 0, y2: 1, colorStops: [
          { offset: 0, color: 'rgba(255, 122, 90, 0.18)' },
          { offset: 1, color: 'rgba(255, 122, 90, 0)' }
        ] } }
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawGender() {
  const el = document.getElementById('genderChart')
  if (!el) return
  const g = data.value?.gender || { male: 0, female: 0, unknown: 0 }
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      series: [{
        type: 'pie',
        radius: ['55%', '85%'],
        avoidLabelOverlap: false,
        label: { show: false },
        data: [
          { value: g.male, name: '男', itemStyle: { color: '#6FA8D4' } },
          { value: g.female, name: '女', itemStyle: { color: '#FF7A5A' } },
          { value: g.unknown, name: '未设置', itemStyle: { color: '#B0A599' } }
        ]
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawAge() {
  const el = document.getElementById('ageChart')
  if (!el) return
  const a = data.value?.age || []
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      grid: { top: 16, right: 8, bottom: 24, left: 50 },
      xAxis: { type: 'category', data: a.map((x: any) => x.range), axisLine: { lineStyle: { color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 }, axisTick: { show: false } },
      yAxis: { type: 'value', splitLine: { lineStyle: { type: 'dashed', color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 } },
      tooltip: { trigger: 'axis' },
      series: [{
        type: 'bar', data: a.map((x: any) => x.value),
        barWidth: 30,
        itemStyle: { color: { type: 'linear', x: 0, y: 0, x2: 0, y2: 1, colorStops: [
          { offset: 0, color: '#FF8A5A' }, { offset: 1, color: '#F5C45E' }
        ] }, borderRadius: [4, 4, 0, 0] }
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
function drawCity() {
  const el = document.getElementById('cityChart')
  if (!el) return
  const c = data.value?.city || []
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      grid: { top: 16, right: 30, bottom: 24, left: 80 },
      xAxis: { type: 'value', splitLine: { lineStyle: { type: 'dashed', color: '#E6DFD2' } }, axisLabel: { color: '#8A7E72', fontSize: 10.5 } },
      yAxis: { type: 'category', data: c.map((x: any) => x.name).reverse(), axisLine: { show: false }, axisLabel: { color: '#4A3F36', fontSize: 11 }, axisTick: { show: false } },
      tooltip: { trigger: 'axis' },
      series: [{
        type: 'bar', data: c.map((x: any) => x.value).reverse(),
        barWidth: 18,
        itemStyle: { color: { type: 'linear', x: 0, y: 0, x2: 1, y2: 0, colorStops: [
          { offset: 0, color: '#6FA8D4' }, { offset: 1, color: '#4FBB8A' }
        ] }, borderRadius: [0, 4, 4, 0] }
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '数据统计' }, { label: '用户分析' }]"
      title="用户分析"
      sub="用户增长与构成 · 后端暂未提供维度数据时显示为空"
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
      <div class="kpi-card">
        <div class="kpi-lbl">总用户</div>
        <div class="kpi">--</div>
        <div class="kpi-sub"><span class="muted small">后端未提供</span></div>
      </div>
      <div class="kpi-card">
        <div class="kpi-lbl">新增用户（近 30 天）</div>
        <div class="kpi">{{ (data?.growth || []).reduce((a: number, b: number) => a + b, 0).toLocaleString() }}</div>
        <div class="kpi-sub"><span class="up">实时</span> 自趋势数据</div>
      </div>
      <div class="kpi-card">
        <div class="kpi-lbl">会员用户</div>
        <div class="kpi">--</div>
        <div class="kpi-sub"><span class="muted small">后端未提供</span></div>
      </div>
      <div class="kpi-card">
        <div class="kpi-lbl">会员渗透率</div>
        <div class="kpi">--</div>
        <div class="kpi-sub"><span class="muted small">后端未提供</span></div>
      </div>
    </div>

    <div class="grid-2">
      <div class="panel">
        <div class="panel-head">
          <div class="ph-title">新增用户趋势 <span class="ct">近 30 天</span></div>
        </div>
        <div class="panel-body">
          <div id="userLine" class="line-chart"></div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head">
          <div class="ph-title">注册方式 <span class="ct">--</span></div>
        </div>
        <div class="panel-body">
          <div class="empty-hint">注册方式分布待后端提供接口（候选：<code>GET /api/admin/v1/statistics/register-method</code>）</div>
        </div>
      </div>
    </div>

    <div class="grid-3">
      <div class="panel">
        <div class="panel-head"><div class="ph-title">性别分布</div></div>
        <div class="panel-body">
          <div id="genderChart" class="donut"></div>
          <div class="row" style="justify-content: center; gap: 16px; margin-top: 12px; font-size: 11.5px; color: var(--ink-2)">
            <span><span class="dot" style="background: #6FA8D4"></span>男 {{ data?.gender?.male || 0 }}%</span>
            <span><span class="dot" style="background: #FF7A5A"></span>女 {{ data?.gender?.female || 0 }}%</span>
            <span><span class="dot" style="background: #B0A599"></span>未设置 {{ data?.gender?.unknown || 0 }}%</span>
          </div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">年龄分布</div></div>
        <div class="panel-body">
          <div id="ageChart" class="bar-chart"></div>
        </div>
      </div>
      <div class="panel">
        <div class="panel-head"><div class="ph-title">TOP 6 城市</div></div>
        <div class="panel-body">
          <div id="cityChart" class="bar-chart"></div>
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
.grid-2 { display: grid; grid-template-columns: 2fr 1fr; gap: 14px; }
.grid-3 { display: grid; grid-template-columns: repeat(3, 1fr); gap: 14px; }
.line-chart, .donut, .bar-chart { width: 100%; }
.line-chart { height: 220px; }
.donut { height: 160px; }
.bar-chart { height: 200px; }
.empty-hint { padding: 24px 12px; color: var(--ink-3); font-size: 12.5px; text-align: center; }
.empty-hint code { font-family: var(--mono); font-size: 11.5px; background: var(--bg-2); padding: 1px 6px; border-radius: 4px; }
.muted.small { font-size: 11px; color: var(--ink-3); }
</style>
