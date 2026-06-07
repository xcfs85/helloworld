// 拼豆 · 后台管理系统原型数据 · 第一部分
// 屏幕：登录、仪表盘、用户管理、会员管理
window.PINDOU = window.PINDOU || {};
PINDOU.screens = [
  // 1. 登录页
  {
    id:'login', group:'登录与权限', name:'登录页', route:'/auth/login',
    desc:'管理员账号密码登录入口。密码错误 3 次后显示验证码，支持记住登录状态 7 天。',
    elements:['账号/密码输入','图形验证码（错误3次后）','登录按钮 + 快捷入口','品牌侧栏与版本信息'],
    design:['左品牌色块右表单的经典后台布局','主色 #FF7A5A 单一强调','圆角 7px 输入控件，40px 行高','避免任何营销话术，聚焦操作'],
    html:`
      <div class="app no-aside">
        <div class="auth-page" style="height:100%">
          <div class="auth-left">
            <div class="auth-head">
              <div class="mark">
                <svg viewBox="0 0 32 32" width="20" height="20">
                  <circle cx="8" cy="8" r="3" fill="#fff"/><circle cx="16" cy="8" r="3" fill="#fff" opacity=".7"/>
                  <circle cx="24" cy="8" r="3" fill="#fff"/><circle cx="8" cy="16" r="3" fill="#fff" opacity=".7"/>
                  <circle cx="16" cy="16" r="3" fill="#fff"/><circle cx="24" cy="16" r="3" fill="#fff" opacity=".7"/>
                  <circle cx="8" cy="24" r="3" fill="#fff" opacity=".7"/><circle cx="16" cy="24" r="3" fill="#fff"/>
                  <circle cx="24" cy="24" r="3" fill="#fff" opacity=".7"/>
                </svg>
              </div>
              <div class="nm">拼豆<small>PINDOU ADMIN</small></div>
            </div>
            <div class="auth-form">
              <h1>登录控制台</h1>
              <div class="sub">使用管理员账号登录 · 默认 7 天 Token 有效期</div>
              <div class="auth-tabs">
                <div class="auth-tab active">账号密码</div>
                <div class="auth-tab">手机验证</div>
                <div class="auth-tab">SSO</div>
              </div>
              <div class="auth-field">
                <label>账号</label>
                <div class="auth-input">
                  <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="1.8" d="M4 8a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v8a4 4 0 0 1-4 4H8a4 4 0 0 1-4-4V8z"/><circle cx="12" cy="10" r="2.5" fill="none" stroke="currentColor" stroke-width="1.8"/><path fill="none" stroke="currentColor" stroke-width="1.8" d="M6 18c1-2 3-3 6-3s5 1 6 3"/></svg>
                  <input value="admin@pindou" />
                </div>
              </div>
              <div class="auth-field">
                <label>密码</label>
                <div class="auth-input">
                  <svg viewBox="0 0 24 24" width="16" height="16"><rect x="5" y="11" width="14" height="9" rx="2" fill="none" stroke="currentColor" stroke-width="1.8"/><path fill="none" stroke="currentColor" stroke-width="1.8" d="M8 11V8a4 4 0 0 1 8 0v3"/></svg>
                  <input type="password" value="••••••••••" />
                  <svg viewBox="0 0 24 24" width="16" height="16" style="color:var(--ink-3)"><path fill="none" stroke="currentColor" stroke-width="1.8" d="M2 12s4-7 10-7 10 7 10 7-4 7-10 7S2 12 2 12z"/><circle cx="12" cy="12" r="3" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>
                </div>
              </div>
              <div class="auth-field">
                <label>验证码 <span class="muted small">密码错误 3 次后必填</span></label>
                <div class="auth-row">
                  <div class="auth-input">
                    <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="1.8" d="M3 12h6m4 0h8M9 8l4 4-4 4"/></svg>
                    <input placeholder="输入 4 位验证码" />
                  </div>
                  <div class="auth-captcha">A7K9</div>
                </div>
              </div>
              <div class="auth-meta">
                <label class="row" style="gap:6px;font-size:12px;color:var(--ink-2)">
                  <span class="ck" style="background:var(--primary);border-color:var(--primary)"></span>
                  记住登录状态 7 天
                </label>
                <a>忘记密码？</a>
              </div>
              <div class="auth-btn">登 录 控制台</div>
              <div class="row" style="justify-content:center;gap:6px;margin-top:18px;font-size:11.5px;color:var(--ink-3)">
                <svg viewBox="0 0 24 24" width="12" height="12"><circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" stroke-width="1.5"/><path fill="none" stroke="currentColor" stroke-width="1.5" d="M12 8v4M12 16h.01"/></svg>
                登录即同意《管理员协议》与《操作日志规范》
              </div>
            </div>
          </div>
          <div class="auth-right">
            <div class="auth-head" style="margin-bottom:0">
              <div class="mark" style="background:rgba(255,255,255,.9)">
                <svg viewBox="0 0 32 32" width="20" height="20">
                  <circle cx="8" cy="8" r="3" fill="#B83B1B"/><circle cx="16" cy="8" r="3" fill="#2A1F1A"/>
                  <circle cx="24" cy="8" r="3" fill="#B83B1B"/><circle cx="8" cy="16" r="3" fill="#2A1F1A"/>
                  <circle cx="16" cy="16" r="3" fill="#F5C45E"/><circle cx="24" cy="16" r="3" fill="#2A1F1A"/>
                  <circle cx="8" cy="24" r="3" fill="#B83B1B"/><circle cx="16" cy="24" r="3" fill="#2A1F1A"/>
                  <circle cx="24" cy="24" r="3" fill="#B83B1B"/>
                </svg>
              </div>
              <div class="nm" style="color:var(--ink)">PINDOU ADMIN<small style="color:var(--ink-2)">v0.1</small></div>
            </div>
            <div class="auth-pitch">
              <h1>运营工作台</h1>
              <p>用户、内容、模板、运营配置、数据统计 · 7 大模块 · 25 个核心页面 · 为拼豆团队日常运营提供稳定高效的操作体验。</p>
              <div style="margin-top:24px;display:grid;grid-template-columns:repeat(3,1fr);gap:12px;max-width:380px">
                <div><div class="kpi" style="font-size:22px">12.5k</div><div class="kpi-sub">日活用户</div></div>
                <div><div class="kpi" style="font-size:22px">1.2k</div><div class="kpi-sub">待审内容</div></div>
                <div><div class="kpi" style="font-size:22px">5.6k</div><div class="kpi-sub">在线模板</div></div>
              </div>
            </div>
            <div class="auth-foot">
              © 2026 PINDOU · 拼豆团队 · 工单支持 ops@pindou.work
            </div>
          </div>
        </div>
      </div>`
  },

  // 2. 核心指标看板
  {
    id:'dashboard', group:'运营总览', name:'核心指标看板', route:'/dashboard',
    desc:'管理员登录后的默认首页。提供 DAU/新增/留存/创作/社区/收入 8 个核心指标的日/周/月视图与趋势图，以及待办和最近活动。',
    elements:['8 个核心 KPI（DAU/新增/活跃率/留存/生成/帖子/互动/收入）','趋势折线图（DAU/生成/帖子）','待审核队列快捷入口','最近活动流与系统状态'],
    design:['横向 4 列 KPI 紧凑排版，不做卡片堆叠','KPI 卡内嵌迷你 sparkline 节省空间','左右双栏：趋势 + 待办 / 活动','单一主色 #FF7A5A 用于趋势高亮'],
    html:`
      <div class="app">
        <div class="aside">
          <div class="aside-title">主导航</div>
          <div class="aside-item active">核心指标</div>
          <div class="aside-item">用户分析<span class="badge">4</span></div>
          <div class="aside-item">创作分析</div>
          <div class="aside-item">社区分析</div>
          <div class="aside-title" style="margin-top:14px">数据维度</div>
          <div class="aside-item">按时间（日/周/月）</div>
          <div class="aside-item">按渠道（iOS/Android）</div>
          <div class="aside-item">按用户分层</div>
          <div class="aside-item">导出报表</div>
        </div>
        <div class="main">
          <div class="page-head">
            <div>
              <div class="crumbs"><span>运营总览</span><span class="sep">/</span><span class="current">核心指标看板</span></div>
              <div class="page-title">核心指标 <span class="sub">数据更新于 2026-06-07 10:32 · 每 5 分钟刷新</span></div>
            </div>
            <div class="head-actions">
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
            </div>
          </div>
          <div class="tabs">
            <div class="tab-btn active">核心指标 <span class="ct">8</span></div>
            <div class="tab-btn">用户分析</div>
            <div class="tab-btn">创作分析</div>
            <div class="tab-btn">社区分析</div>
          </div>
          <div class="dash">
            <div class="kpi-row">
              <div class="kpi-card">
                <div class="kpi-lbl">DAU 日活用户</div>
                <div class="kpi">12,567</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 5.6%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#FF7A5A" stroke-width="1.5" points="0,22 8,18 16,20 24,14 32,16 40,10 48,12 56,8 64,10 72,6 80,4"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">新增用户</div>
                <div class="kpi">1,234</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 12.3%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#4FBB8A" stroke-width="1.5" points="0,24 8,22 16,18 24,20 32,16 40,12 48,14 56,10 64,8 72,6 80,4"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">活跃率</div>
                <div class="kpi">45.6<span class="kpi-sub" style="display:inline;margin-left:2px">%</span></div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 2.1%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#4FBB8A" stroke-width="1.5" points="0,18 8,16 16,18 24,14 32,12 40,14 48,10 56,12 64,8 72,10 80,8"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">次日留存</div>
                <div class="kpi">38.5<span class="kpi-sub" style="display:inline;margin-left:2px">%</span></div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="down">↓ 1.2%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#E07777" stroke-width="1.5" points="0,10 8,12 16,10 24,14 32,12 40,16 48,14 56,18 64,16 72,20 80,18"/></svg></div>
                </div>
              </div>
            </div>
            <div class="kpi-row">
              <div class="kpi-card">
                <div class="kpi-lbl">AI 生成次数</div>
                <div class="kpi">5,678</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 8.9%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#FF7A5A" stroke-width="1.5" points="0,26 8,20 16,22 24,16 32,18 40,12 48,14 56,8 64,10 72,6 80,4"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">新增帖子</div>
                <div class="kpi">1,234</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 15.6%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#4FBB8A" stroke-width="1.5" points="0,24 8,22 16,18 24,20 32,16 40,12 48,14 56,10 64,8 72,6 80,4"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">互动量（点赞+评论+收藏）</div>
                <div class="kpi">23,456</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 10.2%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#4FBB8A" stroke-width="1.5" points="0,22 8,18 16,20 24,16 32,18 40,12 48,14 56,10 64,12 72,8 80,6"/></svg></div>
                </div>
              </div>
              <div class="kpi-card">
                <div class="kpi-lbl">会员收入</div>
                <div class="kpi">¥ 12,567</div>
                <div class="kpi-foot">
                  <div class="kpi-sub"><span class="up">↑ 25.3%</span> 较昨日</div>
                  <div class="spark-wrap"><svg viewBox="0 0 80 32" class="spark" preserveAspectRatio="none"><polyline fill="none" stroke="#4FBB8A" stroke-width="1.5" points="0,28 8,24 16,20 24,22 32,16 40,12 48,14 56,8 64,10 72,6 80,4"/></svg></div>
                </div>
              </div>
            </div>
            <div class="dash-row">
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">趋势 · DAU / 生成次数 / 帖子数
                    <span class="ct">近 14 天</span>
                  </div>
                  <div class="ph-actions">
                    <button class="btn btn-xs btn-secondary">DAU</button>
                    <button class="btn btn-xs btn-secondary">生成</button>
                    <button class="btn btn-xs btn-primary">帖子</button>
                    <button class="btn btn-xs btn-ghost">导出</button>
                  </div>
                </div>
                <div class="panel-body">
                  <div class="line-chart">
                    <svg viewBox="0 0 800 200" preserveAspectRatio="none">
                      <defs>
                        <linearGradient id="g1" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#FF7A5A" stop-opacity="0.18"/>
                          <stop offset="100%" stop-color="#FF7A5A" stop-opacity="0"/>
                        </linearGradient>
                      </defs>
                      <g stroke="#E6DFD2" stroke-width="1" stroke-dasharray="3 3">
                        <line x1="0" y1="40" x2="800" y2="40"/>
                        <line x1="0" y1="100" x2="800" y2="100"/>
                        <line x1="0" y1="160" x2="800" y2="160"/>
                      </g>
                      <path d="M0,140 L60,120 L120,130 L180,100 L240,110 L300,80 L360,90 L420,60 L480,70 L540,50 L600,40 L660,55 L720,30 L780,20 L780,200 L0,200 Z" fill="url(#g1)"/>
                      <polyline fill="none" stroke="#FF7A5A" stroke-width="2" points="0,140 60,120 120,130 180,100 240,110 300,80 360,90 420,60 480,70 540,50 600,40 660,55 720,30 780,20"/>
                      <polyline fill="none" stroke="#4FBB8A" stroke-width="2" stroke-dasharray="4 3" points="0,150 60,140 120,135 180,115 240,118 300,95 360,98 420,80 480,82 540,68 600,55 660,68 720,42 780,32"/>
                      <polyline fill="none" stroke="#6FA8D4" stroke-width="2" stroke-dasharray="2 3" points="0,170 60,165 120,168 180,150 240,148 300,130 360,135 420,118 480,120 540,108 600,98 660,108 720,85 780,78"/>
                    </svg>
                    <div style="display:flex;justify-content:space-between;margin-top:6px;font-size:10.5px;color:var(--ink-3);font-family:var(--mono)">
                      <span>05-25</span><span>05-27</span><span>05-29</span><span>05-31</span><span>06-02</span><span>06-04</span><span>06-07</span>
                    </div>
                    <div style="display:flex;gap:14px;margin-top:8px;font-size:11.5px;color:var(--ink-2)">
                      <span><span class="dot" style="background:#FF7A5A"></span>DAU</span>
                      <span><span class="dot" style="background:#4FBB8A"></span>AI 生成</span>
                      <span><span class="dot" style="background:#6FA8D4"></span>帖子</span>
                    </div>
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
                    <div class="queue-item">
                      <div class="av c1 sm">帖</div>
                      <div class="qt"><div class="qn">23 条帖子待审核</div><div class="qi">最早提交于 09:12 · 等待 1h 12m</div></div>
                      <span class="tag warn">紧急</span>
                    </div>
                    <div class="queue-item">
                      <div class="av c2 sm">评</div>
                      <div class="qt"><div class="qn">45 条评论待审核</div><div class="qi">涉及 12 个帖子</div></div>
                      <span class="tag info">常规</span>
                    </div>
                    <div class="queue-item">
                      <div class="av c3 sm">板</div>
                      <div class="qt"><div class="qn">8 个模板待审核</div><div class="qi">3 个为达人投稿</div></div>
                      <span class="tag info">常规</span>
                    </div>
                    <div class="queue-item">
                      <div class="av c4 sm">举</div>
                      <div class="qt"><div class="qn">12 条举报待处理</div><div class="qi">2 条标记为紧急</div></div>
                      <span class="tag danger">高优</span>
                    </div>
                    <div class="queue-item">
                      <div class="av c5 sm">系</div>
                      <div class="qt"><div class="qn">AI 队列积压 156 单</div><div class="qi">建议调度备用节点</div></div>
                      <span class="tag warn">关注</span>
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
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>林运营</b> 通过了帖子 <a class="mono" style="color:var(--primary-ink)">p_87921</a> 「给闺蜜的生日礼物」<div class="at">2 分钟前 · 内容审核</div></div></div>
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>李审核</b> 拒绝了评论 <a class="mono" style="color:var(--primary-ink)">c_45210</a>「…」<div class="at">5 分钟前 · 评论审核</div></div></div>
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>系统</b> 自动下架了 1 条命中敏感词的帖子<div class="at">8 分钟前 · 自动审核</div></div></div>
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>张管理员</b> 上线了 Banner「春节拼豆专场」<div class="at">15 分钟前 · 运营配置</div></div></div>
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>陈客服</b> 重置了用户 <a class="mono" style="color:var(--primary-ink)">u_10021</a> 密码<div class="at">23 分钟前 · 用户管理</div></div></div>
                    <div class="activity"><div class="dot-line"></div><div class="ac"><b>系统</b> 数据备份完成，耗时 4 分 12 秒<div class="at">1 小时前 · 系统</div></div></div>
                  </div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">系统状态</div>
                  <span class="tag ok">运行中</span>
                </div>
                <div class="panel-body">
                  <div class="form-row"><div class="lbl">API 服务</div><div class="row" style="gap:8px"><span class="tag ok">正常</span><span class="muted small">P99 213ms</span></div></div>
                  <div class="form-row"><div class="lbl">AI 推理</div><div class="row" style="gap:8px"><span class="tag warn">繁忙</span><span class="muted small">队列 156</span></div></div>
                  <div class="form-row"><div class="lbl">数据库</div><div class="row" style="gap:8px"><span class="tag ok">正常</span><span class="muted small">QPS 1.2k</span></div></div>
                  <div class="form-row"><div class="lbl">Redis 缓存</div><div class="row" style="gap:8px"><span class="tag ok">正常</span><span class="muted small">命中率 98%</span></div></div>
                  <div class="form-row"><div class="lbl">OSS 存储</div><div class="row" style="gap:8px"><span class="tag ok">正常</span><span class="muted small">用量 23%</span></div></div>
                  <div class="form-row"><div class="lbl">消息队列</div><div class="row" style="gap:8px"><span class="tag ok">正常</span><span class="muted small">积压 0</span></div></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>`
  },

  // 3. 用户列表
  {
    id:'user-list', group:'用户管理', name:'用户列表', route:'/user/list',
    desc:'C 端用户主列表。支持按 ID/昵称/手机/注册方式/会员/状态/时间筛选，提供批量禁用/解禁/打标操作。',
    elements:['多维筛选 + 搜索','用户主表（ID/昵称/手机/注册/会员/状态/操作）','批量操作条','分页与导出'],
    design:['左 200px 筛选 + 主表布局','行高紧凑、列分隔清晰','状态用色块 + 文字','操作列右对齐，操作按钮化为文本按钮'],
    html:`
      <div class="app">
        <div class="aside">
          <div class="aside-title">筛选条件</div>
          <div class="aside-item active">全部用户 <span class="badge">1,234</span></div>
          <div class="aside-item">正常 <span class="badge">1,180</span></div>
          <div class="aside-item">禁言中 <span class="badge">31</span></div>
          <div class="aside-item">已禁用 <span class="badge">23</span></div>
          <div class="aside-title" style="margin-top:14px">注册方式</div>
          <div class="aside-item">手机号 <span class="badge">687</span></div>
          <div class="aside-item">微信 <span class="badge">456</span></div>
          <div class="aside-item">Apple ID <span class="badge">78</span></div>
          <div class="aside-item">游客 <span class="badge">13</span></div>
          <div class="aside-title" style="margin-top:14px">会员状态</div>
          <div class="aside-item">会员用户 <span class="badge">312</span></div>
          <div class="aside-item">非会员 <span class="badge">922</span></div>
          <div class="aside-item">即将到期 <span class="badge">28</span></div>
        </div>
        <div class="main">
          <div class="page-head">
            <div>
              <div class="crumbs"><span>用户管理</span><span class="sep">/</span><span class="current">用户列表</span></div>
              <div class="page-title">用户列表 <span class="sub">共 1,234 条 · 已选 0 条</span></div>
            </div>
            <div class="head-actions">
              <button class="btn btn-secondary">导出 CSV</button>
              <button class="btn btn-secondary">导入</button>
              <button class="btn btn-primary">+ 添加用户</button>
            </div>
          </div>
          <div class="toolbar">
            <div class="search-input">
              <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg>
              <input placeholder="用户 ID / 昵称 / 手机号" />
            </div>
            <div class="f-select">
              <select><option>注册方式：全部</option><option>手机号</option><option>微信</option><option>Apple</option><option>游客</option></select>
            </div>
            <div class="f-select">
              <select><option>会员：全部</option><option>会员</option><option>非会员</option></select>
            </div>
            <div class="f-select">
              <select><option>状态：全部</option><option>正常</option><option>禁言</option><option>禁用</option></select>
            </div>
            <div class="date-range">
              <input value="2026-05-01" />
              <span class="sep">→</span>
              <input value="2026-06-07" />
            </div>
            <button class="btn btn-sm btn-secondary">重置</button>
            <button class="btn btn-sm btn-primary">搜索</button>
            <div class="f-spacer"></div>
            <div class="batch-actions">
              <button class="btn btn-sm btn-secondary">批量打标</button>
              <button class="btn btn-sm btn-secondary">批量禁言</button>
              <button class="btn btn-sm btn-danger">批量禁用</button>
            </div>
          </div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead>
                <tr>
                  <th style="width:32px"><span class="ck"></span></th>
                  <th>用户</th>
                  <th>手机号</th>
                  <th>注册方式</th>
                  <th>会员</th>
                  <th>状态</th>
                  <th>帖子</th>
                  <th>粉丝</th>
                  <th>最后登录</th>
                  <th class="col-actions">操作</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c1">小</div>
                      <div class="meta"><div class="nm">小美</div><div class="id">u_10001</div></div>
                    </div>
                  </td>
                  <td class="mono">138****8888</td>
                  <td><span class="tag info">微信</span></td>
                  <td><span class="tag primary">VIP1 · 剩 207 天</span></td>
                  <td><span class="tag ok">正常</span></td>
                  <td>23</td>
                  <td>89</td>
                  <td class="muted">10 分钟前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">禁言</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c2">小</div>
                      <div class="meta"><div class="nm">小红</div><div class="id">u_10002</div></div>
                    </div>
                  </td>
                  <td class="mono">139****1234</td>
                  <td><span class="tag neutral">手机号</span></td>
                  <td><span class="tag neutral">非会员</span></td>
                  <td><span class="tag ok">正常</span></td>
                  <td>5</td>
                  <td>23</td>
                  <td class="muted">1 小时前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">禁言</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c3">豆</div>
                      <div class="meta"><div class="nm">豆豆爱拼</div><div class="id">u_10003</div></div>
                    </div>
                  </td>
                  <td class="mono">136****5566</td>
                  <td><span class="tag info">微信</span></td>
                  <td><span class="tag primary">VIP2 · 剩 89 天</span></td>
                  <td><span class="tag warn">禁言 3 天</span></td>
                  <td>156</td>
                  <td>2,341</td>
                  <td class="muted">昨天 21:14</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">解禁</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c4">拼</div>
                      <div class="meta"><div class="nm">拼图小子</div><div class="id">u_10004</div></div>
                    </div>
                  </td>
                  <td class="mono">187****0011</td>
                  <td><span class="tag purple">Apple</span></td>
                  <td><span class="tag primary">VIP1 · 剩 14 天</span></td>
                  <td><span class="tag ok">正常</span></td>
                  <td>42</td>
                  <td>321</td>
                  <td class="muted">2 天前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">禁言</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c5">阿</div>
                      <div class="meta"><div class="nm">阿狸的店</div><div class="id">u_10005</div></div>
                    </div>
                  </td>
                  <td class="mono">188****7788</td>
                  <td><span class="tag neutral">手机号</span></td>
                  <td><span class="tag neutral">非会员</span></td>
                  <td><span class="tag danger">已禁用</span></td>
                  <td>0</td>
                  <td>0</td>
                  <td class="muted">5 天前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--mint)">解禁</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">删除</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c6">星</div>
                      <div class="meta"><div class="nm">星空物语</div><div class="id">u_10006</div></div>
                    </div>
                  </td>
                  <td class="mono">132****4499</td>
                  <td><span class="tag info">微信</span></td>
                  <td><span class="tag primary">VIP3 · 剩 365 天</span></td>
                  <td><span class="tag ok">正常</span></td>
                  <td>289</td>
                  <td>5,672</td>
                  <td class="muted">3 小时前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">禁言</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c2">毛</div>
                      <div class="meta"><div class="nm">毛线球</div><div class="id">u_10007</div></div>
                    </div>
                  </td>
                  <td class="mono">186****3322</td>
                  <td><span class="tag info">微信</span></td>
                  <td><span class="tag neutral">非会员</span></td>
                  <td><span class="tag ok">正常</span></td>
                  <td>8</td>
                  <td>15</td>
                  <td class="muted">6 小时前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">禁言</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
                <tr>
                  <td><span class="ck"></span></td>
                  <td>
                    <div class="user-cell">
                      <div class="av c3">小</div>
                      <div class="meta"><div class="nm">小小酥</div><div class="id">u_10008</div></div>
                    </div>
                  </td>
                  <td class="mono">—</td>
                  <td><span class="tag neutral">游客</span></td>
                  <td><span class="tag neutral">非会员</span></td>
                  <td><span class="tag warn">禁言 7 天</span></td>
                  <td>0</td>
                  <td>0</td>
                  <td class="muted">3 天前</td>
                  <td class="col-actions">
                    <button class="btn btn-xs btn-ghost">查看</button>
                    <button class="btn btn-xs btn-ghost">解禁</button>
                    <button class="btn btn-xs btn-ghost" style="color:var(--rose)">禁用</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="pager">
            <div>共 <b style="color:var(--ink)">1,234</b> 条 · 第 1-20 条</div>
            <div class="pages">
              <div class="pg arrow">‹</div>
              <div class="pg active">1</div>
              <div class="pg">2</div>
              <div class="pg">3</div>
              <div class="pg">4</div>
              <div class="pg">…</div>
              <div class="pg">62</div>
              <div class="pg arrow">›</div>
              <select class="f-select" style="height:26px"><option>20 条/页</option><option>50 条/页</option><option>100 条/页</option></select>
            </div>
          </div>
        </div>
      </div>`
  },

  // 4. 用户详情
  {
    id:'user-detail', group:'用户管理', name:'用户详情', route:'/user/:id',
    desc:'查看用户的完整档案：基本信息、会员信息、行为统计、最近发布内容、订单记录、风险操作记录。',
    elements:['基本信息卡（头像+昵称+ID+注册信息）','会员信息与到期时间','行为数据卡','最近发布/订单时间线','操作区：禁言/禁用/重置密码'],
    design:['左侧主信息 + 右侧操作面板的详情布局','基本信息一栏式展示，不用卡片堆叠','操作按钮全部右对齐单色扁平'],
    html:`
      <div class="app">
        <div class="aside">
          <div class="aside-title">返回</div>
          <div class="aside-item">‹ 用户列表</div>
          <div class="aside-title" style="margin-top:14px">用户档案</div>
          <div class="aside-item active">基本信息</div>
          <div class="aside-item">会员信息</div>
          <div class="aside-item">数据统计</div>
          <div class="aside-item">发布内容</div>
          <div class="aside-item">订单记录</div>
          <div class="aside-item">操作日志</div>
          <div class="aside-item">登录设备</div>
          <div class="aside-title" style="margin-top:14px">快捷操作</div>
          <div class="aside-item">发送站内信</div>
          <div class="aside-item">调整会员</div>
          <div class="aside-item">手动赠送</div>
        </div>
        <div class="main">
          <div class="page-head">
            <div>
              <div class="crumbs"><span>用户管理</span><span class="sep">/</span><span>用户列表</span><span class="sep">/</span><span class="current">u_10001</span></div>
              <div class="page-title">小美 <span class="sub">u_10001 · 注册于 2026-01-01</span></div>
            </div>
            <div class="head-actions">
              <button class="btn btn-secondary">发送站内信</button>
              <button class="btn btn-secondary">重置密码</button>
              <button class="btn btn-secondary">禁言</button>
              <button class="btn btn-danger">禁用账号</button>
            </div>
          </div>
          <div class="detail">
            <div class="col-main">
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">基本信息</div>
                  <button class="btn btn-xs btn-ghost">编辑</button>
                </div>
                <div class="panel-body">
                  <div class="form-row"><div class="lbl">头像 / 昵称</div><div class="row" style="gap:10px"><div class="av c1 lg">小</div><div><div style="font-weight:600">小美</div><div class="muted small">ID u_10001 · 北京 · 女</div></div></div></div>
                  <div class="form-row"><div class="lbl">手机号</div><div class="mono">138 8888 8888</div></div>
                  <div class="form-row"><div class="lbl">注册方式</div><div><span class="tag info">微信</span> · 微信昵称「小美」</div></div>
                  <div class="form-row"><div class="lbl">注册时间</div><div>2026-01-01 10:00 <span class="muted small">已使用 158 天</span></div></div>
                  <div class="form-row"><div class="lbl">最后登录</div><div>2026-06-07 09:30 · IP 123.125.*.* · iPhone 14 Pro</div></div>
                  <div class="form-row"><div class="lbl">账号状态</div><div><span class="tag ok">正常</span> <span class="muted small">· 无未处理举报</span></div></div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">会员信息</div>
                  <button class="btn btn-xs btn-ghost">调整</button>
                </div>
                <div class="panel-body">
                  <div class="form-row"><div class="lbl">会员状态</div><div><span class="tag primary">有效</span> · VIP1 · 2026-12-31 到期</div></div>
                  <div class="form-row"><div class="lbl">累计付费</div><div>¥ 168.00</div></div>
                  <div class="form-row"><div class="lbl">开通渠道</div><div>App Store · 微信支付 · 支付宝</div></div>
                  <div class="form-row"><div class="lbl">自动续费</div><div><span class="switch on"></span> <span class="muted small">将于 2026-12-24 自动续费</span></div></div>
                  <div class="form-row"><div class="lbl">本期权益使用</div><div><div class="row" style="gap:10px"><div style="flex:1"><div class="muted small">AI 生成</div><div class="progress"><div class="fill" style="width:34%"></div></div></div><div style="flex:1"><div class="muted small">高级模板</div><div class="progress"><div class="fill mint" style="width:62%"></div></div></div><div style="flex:1"><div class="muted small">云存储</div><div class="progress"><div class="fill warn" style="width:88%"></div></div></div></div></div></div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">最近发布</div>
                  <a class="btn btn-xs btn-ghost">查看全部 23</a>
                </div>
                <div class="panel-body">
                  <div class="timeline">
                    <div class="tl-item"><div class="tl-time">06-07 09:30</div><div class="tl-body"><div class="what">帖子「给闺蜜的生日礼物」</div><div class="who"><span class="tag ok">已通过</span> · 9 张图 · 1,234 互动</div></div></div>
                    <div class="tl-item"><div class="tl-time">06-05 18:21</div><div class="tl-body"><div class="what">教程「圣诞老人入门教程」</div><div class="who"><span class="tag ok">已通过</span> · 关联图纸 d_xxxxx</div></div></div>
                    <div class="tl-item"><div class="tl-time">06-03 14:08</div><div class="tl-body"><div class="what">AI 生成「樱花树」</div><div class="who"><span class="tag neutral">未发布</span> · 50×50 · 32 色</div></div></div>
                    <div class="tl-item"><div class="tl-time">06-01 22:14</div><div class="tl-body"><div class="what">评论于「星空物语」帖子</div><div class="who"><span class="tag warn">被举报 1 次</span> · 已处理</div></div></div>
                  </div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head">
                  <div class="ph-title">数据统计</div>
                </div>
                <div class="panel-body">
                  <div class="metric-strip">
                    <div class="ms"><div class="ml">作品数</div><div class="mv">23</div></div>
                    <div class="ms"><div class="ml">教程数</div><div class="mv">5</div></div>
                    <div class="ms"><div class="ml">粉丝数</div><div class="mv">89</div></div>
                    <div class="ms"><div class="ml">获赞数</div><div class="mv">1,234</div></div>
                    <div class="ms"><div class="ml">收藏数</div><div class="mv">456</div></div>
                    <div class="ms"><div class="ml">分享数</div><div class="mv">78</div></div>
                    <div class="ms"><div class="ml">AI 生成</div><div class="mv">42</div></div>
                    <div class="ms"><div class="ml">本月互动</div><div class="mv">+312</div></div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-side">
              <div class="panel">
                <div class="panel-head"><div class="ph-title">风险评估</div></div>
                <div class="panel-body">
                  <div class="form-row"><div class="lbl">信用分</div><div><b style="color:var(--mint);font-size:18px">92</b> <span class="muted small">/ 100</span></div></div>
                  <div class="form-row"><div class="lbl">违规记录</div><div>0 次</div></div>
                  <div class="form-row"><div class="lbl">被举报</div><div>1 次 · 已处理</div></div>
                  <div class="form-row"><div class="lbl">敏感词命中</div><div>0 次</div></div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head"><div class="ph-title">订单记录</div></div>
                <div class="panel-body">
                  <div class="timeline">
                    <div class="tl-item"><div class="tl-time">05-24</div><div class="tl-body"><div class="what">VIP1 月卡</div><div class="who muted small">¥ 18 · 微信支付</div></div></div>
                    <div class="tl-item"><div class="tl-time">04-12</div><div class="tl-body"><div class="what">VIP1 月卡</div><div class="who muted small">¥ 18 · 微信支付</div></div></div>
                    <div class="tl-item"><div class="tl-time">01-01</div><div class="tl-body"><div class="what">VIP1 年卡</div><div class="who muted small">¥ 168 · 支付宝</div></div></div>
                  </div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-head"><div class="ph-title">登录设备</div></div>
                <div class="panel-body">
                  <div class="form-row"><div class="lbl">iPhone 14 Pro</div><div class="muted small">iOS 17.4 · 当前</div></div>
                  <div class="form-row"><div class="lbl">iPad Air</div><div class="muted small">iPadOS 17 · 7 天前</div></div>
                  <div class="form-row"><div class="lbl">MacBook Pro</div><div class="muted small">macOS 14 · 30 天前</div></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>`
  },

  // 5. 会员列表
  {
    id:'member-list', group:'用户管理', name:'会员列表', route:'/member/list',
    desc:'会员用户管理页。支持按到期时间、会员等级、支付方式筛选，查看会员详情、手动开通、配置会员权益。',
    elements:['会员等级分布卡','会员列表（等级/到期/支付/续费）','手动开通入口','会员权益配置入口'],
    design:['顶部 4 个会员等级 KPI','列表展示到期时间和续费状态','即将到期突出显示','主操作：手动开通/调整权益'],
    html:`
      <div class="app">
        <div class="aside">
          <div class="aside-title">会员等级</div>
          <div class="aside-item active">全部 <span class="badge">312</span></div>
          <div class="aside-item">VIP1 <span class="badge">182</span></div>
          <div class="aside-item">VIP2 <span class="badge">86</span></div>
          <div class="aside-item">VIP3 <span class="badge">38</span></div>
          <div class="aside-item">SVIP <span class="badge">6</span></div>
          <div class="aside-title" style="margin-top:14px">到期状态</div>
          <div class="aside-item">已过期</div>
          <div class="aside-item">7 天内到期 <span class="badge" style="background:var(--warn-soft);color:var(--warn)">28</span></div>
          <div class="aside-item">30 天内到期</div>
          <div class="aside-item">长期有效</div>
          <div class="aside-title" style="margin-top:14px">支付方式</div>
          <div class="aside-item">微信支付</div>
          <div class="aside-item">支付宝</div>
          <div class="aside-item">App Store</div>
          <div class="aside-item">后台开通</div>
        </div>
        <div class="main">
          <div class="page-head">
            <div>
              <div class="crumbs"><span>用户管理</span><span class="sep">/</span><span class="current">会员管理</span></div>
              <div class="page-title">会员列表 <span class="sub">共 312 个会员 · 月收入 ¥ 12,567</span></div>
            </div>
            <div class="head-actions">
              <button class="btn btn-secondary">会员权益配置</button>
              <button class="btn btn-primary">+ 手动开通</button>
            </div>
          </div>
          <div class="toolbar">
            <div class="search-input">
              <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg>
              <input placeholder="用户 ID / 昵称 / 手机号" />
            </div>
            <div class="f-select"><select><option>等级：全部</option><option>VIP1</option><option>VIP2</option><option>VIP3</option><option>SVIP</option></select></div>
            <div class="f-select"><select><option>到期：全部</option><option>7 天内</option><option>30 天内</option><option>已过期</option></select></div>
            <div class="f-select"><select><option>支付：全部</option><option>微信</option><option>支付宝</option><option>App Store</option></select></div>
            <button class="btn btn-sm btn-secondary">重置</button>
            <button class="btn btn-sm btn-primary">搜索</button>
            <div class="f-spacer"></div>
            <button class="btn btn-sm btn-secondary">导出</button>
          </div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead>
                <tr>
                  <th style="width:32px"><span class="ck"></span></th>
                  <th>用户</th>
                  <th>等级</th>
                  <th>到期时间</th>
                  <th>剩余</th>
                  <th>自动续费</th>
                  <th>累计付费</th>
                  <th>开通渠道</th>
                  <th class="col-actions">操作</th>
                </tr>
              </thead>
              <tbody>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c1">小</div><div class="meta"><div class="nm">小美</div><div class="id">u_10001</div></div></div></td><td><span class="tag primary">VIP1</span></td><td>2026-12-31</td><td><span class="muted">207 天</span></td><td><span class="switch on"></span></td><td>¥ 168</td><td>App Store</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-ghost">调整</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c6">星</div><div class="meta"><div class="nm">星空物语</div><div class="id">u_10006</div></div></div></td><td><span class="tag" style="background:var(--accent);color:var(--ink)">SVIP</span></td><td>2027-06-07</td><td><span class="muted">365 天</span></td><td><span class="switch on"></span></td><td>¥ 1,288</td><td>微信支付</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-ghost">调整</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c3">豆</div><div class="meta"><div class="nm">豆豆爱拼</div><div class="id">u_10003</div></div></div></td><td><span class="tag primary">VIP2</span></td><td>2026-09-04</td><td><span class="tag warn">89 天</span></td><td><span class="switch on"></span></td><td>¥ 268</td><td>微信支付</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-ghost">调整</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c4">拼</div><div class="meta"><div class="nm">拼图小子</div><div class="id">u_10004</div></div></div></td><td><span class="tag primary">VIP1</span></td><td>2026-06-21</td><td><span class="tag danger">14 天</span></td><td><span class="switch"></span></td><td>¥ 168</td><td>App Store</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-primary" style="background:var(--warn)">提醒续费</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c5">布</div><div class="meta"><div class="nm">布丁猫</div><div class="id">u_10201</div></div></div></td><td><span class="tag primary">VIP1</span></td><td>2026-06-12</td><td><span class="tag danger">5 天</span></td><td><span class="switch on"></span></td><td>¥ 168</td><td>支付宝</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-primary" style="background:var(--warn)">提醒续费</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c2">甜</div><div class="meta"><div class="nm">甜甜圈</div><div class="id">u_10312</div></div></div></td><td><span class="tag primary">VIP3</span></td><td>2026-12-01</td><td><span class="muted">177 天</span></td><td><span class="switch on"></span></td><td>¥ 588</td><td>微信支付</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-ghost">调整</button></td></tr>
                <tr><td><span class="ck"></span></td><td><div class="user-cell"><div class="av c4">小</div><div class="meta"><div class="nm">小黄鸭</div><div class="id">u_10421</div></div></div></td><td><span class="tag primary">VIP1</span></td><td>2026-06-09</td><td><span class="tag danger">2 天</span></td><td><span class="switch"></span></td><td>¥ 18</td><td>App Store</td><td class="col-actions"><button class="btn btn-xs btn-ghost">续费记录</button><button class="btn btn-xs btn-primary" style="background:var(--warn)">提醒续费</button></td></tr>
              </tbody>
            </table>
          </div>
          <div class="pager">
            <div>共 <b style="color:var(--ink)">312</b> 条</div>
            <div class="pages"><div class="pg arrow">‹</div><div class="pg active">1</div><div class="pg">2</div><div class="pg">3</div><div class="pg arrow">›</div></div>
          </div>
        </div>
      </div>`
  },

  // 6. 帖子审核列表
  {
    id:'post-review-list', group:'内容审核', name:'帖子审核', route:'/post/review/list',
    desc:'帖子审核主列表。展示待审核 / 已通过 / 已拒绝 / 已下架 4 种状态的内容，支持批量通过/拒绝、关键词过滤、AI 风险标记。',
    elements:['顶部 4 个状态 Tab（待审/通过/拒绝/下架）','帖子主表（类型/标题/作者/时间/AI 风险/状态）','批量审核操作','AI 风险等级标识'],
    design:['紧凑表行 + 左侧 9 图缩略','AI 风险等级用色块标识','批量操作常驻工具栏'],
    html:`
      <div class="app">
        <div class="aside">
          <div class="aside-title">审核状态</div>
          <div class="aside-item active">待审核 <span class="badge" style="background:var(--primary-soft);color:var(--primary-ink)">23</span></div>
          <div class="aside-item">已通过</div>
          <div class="aside-item">已拒绝</div>
          <div class="aside-item">已下架</div>
          <div class="aside-title" style="margin-top:14px">内容类型</div>
          <div class="aside-item">作品 <span class="badge">15</span></div>
          <div class="aside-item">教程 <span class="badge">5</span></div>
          <div class="aside-item">提问 <span class="badge">3</span></div>
          <div class="aside-title" style="margin-top:14px">AI 风险等级</div>
          <div class="aside-item">无风险 <span class="badge" style="background:var(--mint-soft);color:#1F7A4B">18</span></div>
          <div class="aside-item">低风险 <span class="badge" style="background:var(--warn-soft);color:var(--warn)">3</span></div>
          <div class="aside-item">中风险 <span class="badge" style="background:#FBE1D0;color:#A86515">1</span></div>
          <div class="aside-item">高风险 <span class="badge" style="background:var(--rose-soft);color:var(--rose)">1</span></div>
        </div>
        <div class="main">
          <div class="page-head">
            <div>
              <div class="crumbs"><span>内容管理</span><span class="sep">/</span><span class="current">帖子审核</span></div>
              <div class="page-title">帖子审核 <span class="sub">今日已审 156 · 累计待审 23</span></div>
            </div>
            <div class="head-actions">
              <button class="btn btn-secondary">审核规则</button>
              <button class="btn btn-primary">审核</button>
            </div>
          </div>
          <div class="tabs">
            <div class="tab-btn active">待审核 <span class="ct">23</span></div>
            <div class="tab-btn">已通过</div>
            <div class="tab-btn">已拒绝 <span class="ct">5</span></div>
            <div class="tab-btn">已下架</div>
          </div>
          <div class="toolbar">
            <div class="search-input">
              <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg>
              <input placeholder="标题 / 作者 / 关键词" />
            </div>
            <div class="f-select"><select><option>类型：全部</option><option>作品</option><option>教程</option><option>提问</option></select></div>
            <div class="f-select"><select><option>AI 风险：全部</option><option>无</option><option>低</option><option>中</option><option>高</option></select></div>
            <div class="date-range"><input value="2026-06-01" /><span class="sep">→</span><input value="2026-06-07" /></div>
            <button class="btn btn-sm btn-secondary">重置</button>
            <button class="btn btn-sm btn-primary">搜索</button>
            <div class="f-spacer"></div>
            <div class="batch-actions">
              <span class="muted small">已选 0 条</span>
              <button class="btn btn-sm" style="background:var(--mint);color:#fff">批量通过</button>
              <button class="btn btn-sm btn-danger">批量拒绝</button>
            </div>
          </div>
          <div class="tbl-wrap">
            <table class="tbl">
              <thead>
                <tr>
                  <th style="width:32px"><span class="ck"></span></th>
                  <th>帖子</th>
                  <th>类型</th>
                  <th>作者</th>
                  <th>发布时间</th>
                  <th>AI 风险</th>
                  <th>状态</th>
                  <th class="col-actions">操作</th>
                </tr>
              </thead>
              <tbody>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#FFD2B0,#FF7A5A);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">给闺蜜的生日礼物 🎁</div><div class="muted small">9 图 · #生日礼物 #闺蜜</div></div>
                    </div>
                  </td>
                  <td><span class="tag primary">作品</span></td>
                  <td><div class="user-cell"><div class="av c1 sm">小</div><div class="meta"><div class="nm" style="font-size:12px">小美</div><div class="id">u_10001</div></div></div></td>
                  <td class="muted">06-07 09:30</td>
                  <td><span class="tag ok">无</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#9DC8E5,#6FA8D4);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">拼豆技巧：渐变色的取色思路</div><div class="muted small">教程 · 12 步</div></div>
                    </div>
                  </td>
                  <td><span class="tag info">教程</span></td>
                  <td><div class="user-cell"><div class="av c2 sm">小</div><div class="meta"><div class="nm" style="font-size:12px">小红</div><div class="id">u_10002</div></div></div></td>
                  <td class="muted">06-07 08:14</td>
                  <td><span class="tag ok">无</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#F2A6A6,#9A7FCC);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">求助：MARD 色板 H12 缺货哪里买？</div><div class="muted small">1 图 · 提问</div></div>
                    </div>
                  </td>
                  <td><span class="tag purple">提问</span></td>
                  <td><div class="user-cell"><div class="av c3 sm">豆</div><div class="meta"><div class="nm" style="font-size:12px">豆豆爱拼</div><div class="id">u_10003</div></div></div></td>
                  <td class="muted">06-07 07:50</td>
                  <td><span class="tag warn">低</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#4FBB8A,#6FA8D4);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">樱花树 50×50 完工啦！</div><div class="muted small">3 图 · #樱花 #春</div></div>
                    </div>
                  </td>
                  <td><span class="tag primary">作品</span></td>
                  <td><div class="user-cell"><div class="av c4 sm">拼</div><div class="meta"><div class="nm" style="font-size:12px">拼图小子</div><div class="id">u_10004</div></div></div></td>
                  <td class="muted">06-06 22:18</td>
                  <td><span class="tag ok">无</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#F5C45E,#FF8A5A);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">加我微信拼豆交流群（看图）</div><div class="muted small">1 图 · 含联系方式</div></div>
                    </div>
                  </td>
                  <td><span class="tag primary">作品</span></td>
                  <td><div class="user-cell"><div class="av c5 sm">阿</div><div class="meta"><div class="nm" style="font-size:12px">阿狸的店</div><div class="id">u_10005</div></div></div></td>
                  <td class="muted">06-06 20:43</td>
                  <td><span class="tag danger">高 · 广告</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#9A7FCC,#E07777);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">星空物语 80×80 大作分享</div><div class="muted small">5 图 · #星空</div></div>
                    </div>
                  </td>
                  <td><span class="tag primary">作品</span></td>
                  <td><div class="user-cell"><div class="av c6 sm">星</div><div class="meta"><div class="nm" style="font-size:12px">星空物语</div><div class="id">u_10006</div></div></div></td>
                  <td class="muted">06-06 18:11</td>
                  <td><span class="tag ok">无</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
                <tr><td><span class="ck"></span></td>
                  <td>
                    <div class="row" style="gap:10px">
                      <div style="width:36px;height:36px;border-radius:6px;background:linear-gradient(135deg,#4FBB8A,#F5C45E);flex-shrink:0"></div>
                      <div style="min-width:0"><div style="font-weight:600;color:var(--ink);overflow:hidden;text-overflow:ellipsis;white-space:nowrap;max-width:260px">第一次做拼豆 求指点</div><div class="muted small">1 图 · 提问</div></div>
                    </div>
                  </td>
                  <td><span class="tag purple">提问</span></td>
                  <td><div class="user-cell"><div class="av c2 sm">毛</div><div class="meta"><div class="nm" style="font-size:12px">毛线球</div><div class="id">u_10007</div></div></div></td>
                  <td class="muted">06-06 16:32</td>
                  <td><span class="tag ok">无</span></td>
                  <td><span class="tag warn">待审核</span></td>
                  <td class="col-actions"><button class="btn btn-xs btn-ghost">预览</button><button class="btn btn-xs" style="color:var(--mint)">通过</button><button class="btn btn-xs btn-ghost" style="color:var(--rose)">拒绝</button></td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="pager"><div>共 <b style="color:var(--ink)">23</b> 条待审</div><div class="pages"><div class="pg arrow">‹</div><div class="pg active">1</div><div class="pg">2</div><div class="pg arrow">›</div></div></div>
        </div>
      </div>`
  },
];
