<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import PageHead from '@/components/PageHead.vue'
import { getOverview, getTrends, type DailyStats } from '@/api/stats'

const router = useRouter()
const overview = ref<any>(null)
const trendData = ref<{ dates: string[]; dau: number[]; generation: number[]; posts: number[] } | null>(null)
const loading = ref(false)

interface KpiItem {
  key: string
  label: string
  value: number
  delta: number
  unit?: string
  prefix?: string
  decimals?: number
}

/** 从最新一天与上一天的 daily_stats 中计算指标和环比 */
function buildKpi(label: string, key: string, pick: (d: DailyStats) => number, unit = '', prefix = '', decimals = 0): KpiItem {
  const daily = (overview.value?.daily_stats || []) as DailyStats[]
  const latest = daily[daily.length - 1]
  const prev = daily[daily.length - 2]
  const cur = latest ? pick(latest) : 0
  const yest = prev ? pick(prev) : 0
  const delta = yest === 0 ? 0 : +(((cur - yest) / yest) * 100).toFixed(1)
  return { key, label, value: cur, delta, unit, prefix, decimals }
}

const kpis = computed<KpiItem[]>(() => {
  const daily = (overview.value?.daily_stats || []) as DailyStats[]
  if (!overview.value || daily.length === 0) {
    // 数据未就绪时返回占位，避免模板渲染时访问 undefined.value
    const placeholders = [
      'DAU 日活用户', '新增用户', '活跃率', '次日留存',
      'AI 生成次数', '新增帖子', '互动量（点赞+评论+收藏）', '会员收入'
    ]
    return placeholders.map((label, i) => ({
      key: 'ph_' + i,
      label,
      value: 0,
      delta: 0
    }))
  }
  return [
    buildKpi('DAU 日活用户', 'dau', d => d.dau ?? 0),
    buildKpi('新增用户', 'new_users', d => d.new_user_count ?? 0),
    buildKpi('活跃率', 'active_rate', d => {
      // 活跃率 = 当日 DAU / 总用户数
      const total = overview.value.total_users || 0
      if (!total) return 0
      return +(((d.dau || 0) / total) * 100).toFixed(1)
    }, '%'),
    buildKpi('次日留存', 'retention', d => Number(d.retention_1d ?? 0), '%'),
    buildKpi('AI 生成次数', 'ai_generation', d => d.generation_count ?? 0),
    buildKpi('新增帖子', 'posts', d => d.post_count ?? 0),
    buildKpi('互动量（点赞+评论+收藏）', 'interactions', d => (d.like_count ?? 0) + (d.comment_count ?? 0) + (d.favorite_count ?? 0)),
    buildKpi('会员收入', 'revenue', d => Number(d.member_revenue ?? 0), '', '¥ ', 2)
  ]
})

function formatKpi(k: KpiItem): string {
  const n = Number(k.value || 0)
  if (k.decimals) return n.toFixed(k.decimals)
  return n.toLocaleString()
}

onMounted(async () => {
  loading.value = true
  try {
    // 计算最近 14 天日期范围
    const end = new Date()
    const start = new Date()
    start.setDate(start.getDate() - 13)
    const fmt = (d: Date) => d.toISOString().split('T')[0]
    const range = { start: fmt(start), end: fmt(end) }

    // 并行请求概览 + 趋势
    const [ov, trends] = await Promise.all([
      getOverview(range),
      getTrends(range)
    ])
    overview.value = ov

    const list: DailyStats[] = Array.isArray(trends) ? trends : (ov?.daily_stats || [])
    trendData.value = {
      dates: list.map(d => (d.stat_date || '').slice(5)), // MM-DD
      dau: list.map(d => d.dau ?? 0),
      generation: list.map(d => d.generation_count ?? 0),
      posts: list.map(d => d.post_count ?? 0)
    }

    drawTrendChart()
    drawDonut()
  } catch (e) {
    console.error('[Dashboard] 加载数据失败', e)
  } finally {
    loading.value = false
  }
})

function drawTrendChart() {
  const el = document.getElementById('trendChart')
  if (!el || !trendData.value) return
  // 动态引入 echarts 以减少首屏体积
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      grid: { top: 16, right: 16, bottom: 32, left: 40 },
      tooltip: { trigger: 'axis', backgroundColor: '#fff', borderColor: '#E6DFD2', textStyle: { color: '#1F1A16', fontSize: 12 } },
      legend: { show: false },
      xAxis: {
        type: 'category', data: trendData.value.dates,
        axisLine: { lineStyle: { color: '#E6DFD2' } },
        axisLabel: { color: '#8A7E72', fontSize: 10.5, fontFamily: 'JetBrains Mono' },
        axisTick: { show: false }
      },
      yAxis: {
        type: 'value',
        splitLine: { lineStyle: { color: '#E6DFD2', type: 'dashed' } },
        axisLabel: { color: '#8A7E72', fontSize: 10.5 }
      },
      series: [
        {
          name: 'DAU', type: 'line', data: trendData.value.dau, smooth: true,
          lineStyle: { color: '#FF7A5A', width: 2 },
          itemStyle: { color: '#FF7A5A' },
          areaStyle: { color: { type: 'linear', x: 0, y: 0, x2: 0, y2: 1, colorStops: [
            { offset: 0, color: 'rgba(255, 122, 90, 0.18)' },
            { offset: 1, color: 'rgba(255, 122, 90, 0)' }
          ] } }
        },
        {
          name: 'AI 生成', type: 'line', data: trendData.value.generation, smooth: true,
          lineStyle: { color: '#4FBB8A', width: 2, type: 'dashed' },
          itemStyle: { color: '#4FBB8A' }
        },
        {
          name: '帖子', type: 'line', data: trendData.value.posts, smooth: true,
          lineStyle: { color: '#6FA8D4', width: 2, type: 'dashed' },
          itemStyle: { color: '#6FA8D4' }
        }
      ]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}

function drawDonut() {
  const el = document.getElementById('donutChart')
  if (!el) return
  // 注册方式分布由 register_method 接口获取，目前先用接口返回或保留静态占位
  const registerData = [
    { value: 45, name: '微信', itemStyle: { color: '#4FBB8A' } },
    { value: 35, name: '手机号', itemStyle: { color: '#FF7A5A' } },
    { value: 15, name: 'Apple', itemStyle: { color: '#9A7FCC' } },
    { value: 5, name: '游客', itemStyle: { color: '#B0A599' } }
  ]
  import('echarts').then(echarts => {
    const chart = echarts.init(el)
    chart.setOption({
      series: [{
        type: 'pie',
        radius: ['60%', '85%'],
        avoidLabelOverlap: false,
        label: { show: false },
        data: registerData
      }]
    })
    window.addEventListener('resize', () => chart.resize())
  })
}

const todos = [
  { icon: 'c1', label: '帖', title: '23 条帖子待审核', desc: '最早提交于 09:12 · 等待 1h 12m', tag: '紧急', tagVariant: 'warn' },
  { icon: 'c2', label: '评', title: '45 条评论待审核', desc: '涉及 12 个帖子', tag: '常规', tagVariant: 'info' },
  { icon: 'c3', label: '板', title: '8 个模板待审核', desc: '3 个为达人投稿', tag: '常规', tagVariant: 'info' },
  { icon: 'c4', label: '举', title: '12 条举报待处理', desc: '2 条标记为紧急', tag: '高优', tagVariant: 'danger' },
  { icon: 'c5', label: '系', title: 'AI 队列积压 156 单', desc: '建议调度备用节点', tag: '关注', tagVariant: 'warn' }
]

const activities = [
  { user: '林运营', action: '通过了帖子', target: 'p_87921', text: '「给闺蜜的生日礼物」', time: '2 分钟前', type: '内容审核' },
  { user: '李审核', action: '拒绝了评论', target: 'c_45210', text: '「…」', time: '5 分钟前', type: '评论审核' },
  { user: '系统', action: '自动下架了 1 条命中敏感词的帖子', target: '', text: '', time: '8 分钟前', type: '自动审核' },
  { user: '张管理员', action: '上线了 Banner', target: '「春节拼豆专场」', text: '', time: '15 分钟前', type: '运营配置' },
  { user: '陈客服', action: '重置了用户', target: 'u_10021', text: ' 密码', time: '23 分钟前', type: '用户管理' },
  { user: '系统', action: '数据备份完成，耗时 4 分 12 秒', target: '', text: '', time: '1 小时前', type: '系统' }
]

const systemStatus = [
  { name: 'API 服务', status: 'ok', desc: 'P99 213ms' },
  { name: 'AI 推理', status: 'warn', desc: '队列 156' },
  { name: '数据库', status: 'ok', desc: 'QPS 1.2k' },
  { name: 'Redis 缓存', status: 'ok', desc: '命中率 98%' },
  { name: 'OSS 存储', status: 'ok', desc: '用量 23%' },
  { name: '消息队列', status: 'ok', desc: '积压 0' }
]
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '运营总览' }, { label: '核心指标看板' }]"
      title="核心指标"
      :sub="loading ? '数据加载中…' : '数据更新于 ' + (trendData?.dates?.slice(-1)[0] || '今日') + ' · 每 5 分钟刷新'"
    >
      <template #actions>
        <div class="date-range">
          <input value="2026-05-31" />
          <span class="sep">→</span>
          <input value="2026-06-07" />
        </div>
        <div class="f-select">
          <select><option>日</option><option>周</option><option>月</option></select>
        </div>
        <button class="btn btn-secondary">导出</button>
        <button class="btn btn-primary">刷新</button>
      </template>
    </PageHead>

    <div class="tabs">
      <div class="tab-btn active">核心指标 <span class="ct">8</span></div>
      <div class="tab-btn" @click="router.push('/stats/user')">用户分析</div>
      <div class="tab-btn" @click="router.push('/stats/creation')">创作分析</div>
      <div class="tab-btn" @click="router.push('/stats/community')">社区分析</div>
    </div>

    <div class="dash">
      <div class="kpi-row">
        <div v-for="k in kpis.slice(0, 4)" :key="k.key" class="kpi-card">
          <div class="kpi-lbl">{{ k.label }}</div>
          <div class="kpi">{{ k.prefix || '' }}{{ formatKpi(k) }}{{ k.unit }}</div>
          <div class="kpi-foot">
            <div class="kpi-sub">
              <span :class="k.delta >= 0 ? 'up' : 'down'">{{ k.delta >= 0 ? '↑' : '↓' }} {{ Math.abs(k.delta) }}%</span>
              较昨日
            </div>
          </div>
        </div>
      </div>
      <div class="kpi-row">
        <div v-for="k in kpis.slice(4)" :key="k.key" class="kpi-card">
          <div class="kpi-lbl">{{ k.label }}</div>
          <div class="kpi">{{ k.prefix || '' }}{{ formatKpi(k) }}{{ k.unit }}</div>
          <div class="kpi-foot">
            <div class="kpi-sub">
              <span :class="k.delta >= 0 ? 'up' : 'down'">{{ k.delta >= 0 ? '↑' : '↓' }} {{ Math.abs(k.delta) }}%</span>
              较昨日
            </div>
          </div>
        </div>
      </div>

      <div class="dash-row">
        <div class="panel">
          <div class="panel-head">
            <div class="ph-title">趋势 · DAU / 生成次数 / 帖子数 <span class="ct">近 14 天</span></div>
            <div class="ph-actions">
              <button class="btn btn-xs btn-secondary">DAU</button>
              <button class="btn btn-xs btn-secondary">生成</button>
              <button class="btn btn-xs btn-primary">帖子</button>
              <button class="btn btn-xs btn-ghost">导出</button>
            </div>
          </div>
          <div class="panel-body">
            <div id="trendChart" class="line-chart"></div>
            <div style="display: flex; gap: 14px; margin-top: 12px; font-size: 11.5px; color: var(--ink-2)">
              <span><span class="dot" style="background: #FF7A5A"></span>DAU</span>
              <span><span class="dot" style="background: #4FBB8A"></span>AI 生成</span>
              <span><span class="dot" style="background: #6FA8D4"></span>帖子</span>
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head">
            <div class="ph-title">待办事项 <span class="ct">8</span></div>
            <a class="btn btn-xs btn-ghost">全部</a>
          </div>
          <div class="panel-body">
            <div class="queue-list">
              <div v-for="t in todos" :key="t.title" class="queue-item">
                <div class="av" :class="t.icon">{{ t.label }}</div>
                <div class="qt">
                  <div class="qn">{{ t.title }}</div>
                  <div class="qi">{{ t.desc }}</div>
                </div>
                <span class="tag" :class="t.tagVariant">{{ t.tag }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="dash-row">
        <div class="panel">
          <div class="panel-head">
            <div class="ph-title">最近活动 <span class="ct">实时</span></div>
            <a class="btn btn-xs btn-ghost">查看全部</a>
          </div>
          <div class="panel-body">
            <div class="activity-list">
              <div v-for="(a, i) in activities" :key="i" class="activity">
                <div class="dot-line"></div>
                <div class="ac">
                  <b>{{ a.user }}</b> {{ a.action }} <a v-if="a.target" class="mono" style="color: var(--primary-ink)">{{ a.target }}</a> {{ a.text }}
                  <div class="at">{{ a.time }} · {{ a.type }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="panel">
          <div class="panel-head">
            <div class="ph-title">系统状态</div>
            <span class="tag ok">运行中</span>
          </div>
          <div class="panel-body">
            <div v-for="s in systemStatus" :key="s.name" class="form-row">
              <div class="lbl">{{ s.name }}</div>
              <div class="row" style="gap: 8px">
                <span class="tag" :class="s.status">{{ s.status === 'ok' ? '正常' : '繁忙' }}</span>
                <span class="muted small">{{ s.desc }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { display: flex; flex-direction: column; height: 100%; overflow: hidden; }
.tabs { display: flex; gap: 2px; padding: 0 22px; background: var(--surface); border-bottom: 1px solid var(--line); flex-shrink: 0; }
.tab-btn {
  padding: 10px 14px; font-size: 13px; color: var(--ink-3); font-weight: 500;
  border-bottom: 2px solid transparent; display: inline-flex; align-items: center; gap: 6px;
  cursor: pointer;
}
.tab-btn:hover { color: var(--ink-2); }
.tab-btn .ct { font-size: 10px; background: var(--bg-2); color: var(--ink-2); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.tab-btn.active { color: var(--ink); border-bottom-color: var(--primary); font-weight: 600; }
.tab-btn.active .ct { background: var(--primary-soft); color: var(--primary-ink); }
.dash { padding: 16px 22px; display: flex; flex-direction: column; gap: 14px; flex: 1; overflow: auto; background: var(--bg-2); }
.kpi-row { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }
.kpi-card { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; padding: 14px 16px; }
.kpi-card .kpi-lbl { font-size: 12px; color: var(--ink-3); font-weight: 500; display: flex; align-items: center; gap: 6px; }
.kpi-card .kpi { font-size: 24px; margin-top: 6px; }
.kpi-card .kpi-foot { display: flex; align-items: center; justify-content: space-between; margin-top: 8px; }
.dash-row { display: grid; grid-template-columns: 2fr 1fr; gap: 14px; }
.line-chart { height: 220px; width: 100%; }
.queue-list { display: flex; flex-direction: column; }
.queue-item { display: flex; align-items: center; gap: 10px; padding: 10px 0; border-bottom: 1px solid var(--line); font-size: 12.5px; }
.queue-item:last-child { border-bottom: none; }
.queue-item .qt { flex: 1; }
.queue-item .qn { font-weight: 600; color: var(--ink); margin-bottom: 2px; }
.queue-item .qi { font-size: 11px; color: var(--ink-3); }
.activity-list { display: flex; flex-direction: column; }
.activity { display: flex; gap: 10px; padding: 9px 0; border-bottom: 1px solid var(--line); font-size: 12.5px; }
.activity:last-child { border-bottom: none; }
.activity .dot-line { width: 6px; display: flex; flex-direction: column; align-items: center; gap: 2px; flex-shrink: 0; }
.activity .dot-line::before { content: ""; width: 6px; height: 6px; border-radius: 50%; background: var(--primary); margin-top: 4px; }
.activity .dot-line::after { content: ""; width: 1px; flex: 1; background: var(--line); margin-top: 2px; }
.activity .ac { flex: 1; }
.activity .at { font-size: 10.5px; color: var(--ink-3); margin-top: 2px; }
</style>
