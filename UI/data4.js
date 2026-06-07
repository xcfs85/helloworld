// 拼豆 · 移动端原型数据 (第四部分)
const SCREENS_4 = [
  {
    id:'template-detail', group:'M4 模板', name:'模板详情', route:'/template/:id',
    desc:'模板详情页：作品大图预览、色号、参数、创作者、同款作品、"按此生成"主按钮。',
    elements:['大图预览（可缩放）','色号 + 板型 + 难度参数','创作者信息 + 来源','色号前 10 个色块','同款作品瀑布流'],
    design:['主按钮"按此生成"用主色，强调转化','收藏 / 分享次要操作','色号前 10 + "查看全部"链接'],
    html:`
      <div class="page tpl-detail">
        <div class="tpl-cover">
          <div class="grid-overlay"></div>
          <div class="appbar" style="position:absolute;top:0;left:0;right:0;color:#fff;z-index:3">
            <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
            <div></div>
            <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><circle cx="5" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="19" cy="12" r="2" fill="currentColor"/></svg></button>
          </div>
          <div style="position:absolute;left:18px;bottom:24px;color:#fff;z-index:2">
            <div style="font-size:11px;background:rgba(0,0,0,.4);padding:4px 10px;border-radius:999px;display:inline-block;backdrop-filter:blur(4px)">节日 · 亲子 · 入门</div>
            <div style="font-size:11px;opacity:.85;margin-top:8px">👀 12,432 · ♡ 1.2k · 已用 234</div>
          </div>
        </div>
        <div class="info">
          <h1>圣诞老人 🎅</h1>
          <div class="stats-line">
            <span>📊 28 色 / 50×50 板</span>
            <span>⏱ 8-10h</span>
            <span>⭐ 4.9</span>
            <span>📌 入门</span>
          </div>
          <div class="creator">
            <div class="avatar"></div>
            <div>
              <div class="name">@拼豆达人 Lily</div>
              <div class="src">小红书：拼豆达人 Lily · 5,432 粉丝</div>
            </div>
            <button class="chip" style="margin-left:auto">+ 关注</button>
          </div>
          <div class="quote">"给孩子的圣诞礼物🎄 30 分钟能拼完简单的部分，剩 1/3 留给他自己完成 ✨"</div>
          <div style="font-size:12px;font-weight:600;margin-bottom:8px">色号预览</div>
          <div class="color-preview">
            <div class="sw" style="background:#FF7A5A"></div>
            <div class="sw" style="background:#2A1F1A"></div>
            <div class="sw" style="background:#F5C45E"></div>
            <div class="sw" style="background:#6BC7A1"></div>
            <div class="sw" style="background:#9DC8E5"></div>
            <div class="sw" style="background:#B49DD8"></div>
            <div class="sw" style="background:#8B5A3C"></div>
            <div class="sw" style="background:#F2A6A6"></div>
            <div class="sw" style="background:#fff;border:1px dashed var(--ink-3);display:grid;place-items:center;font-size:10px;color:var(--ink-3)">+20</div>
            <div style="margin-left:auto;font-size:11px;color:var(--ink-3)">查看全部 ›</div>
          </div>
          <div class="similar">
            <h4>同款作品</h4>
            <div class="similar-grid"><div></div><div></div><div></div></div>
          </div>
        </div>
        <div class="bottom-cta">
          <div class="ic"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg></div>
          <div class="ic"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M18 16l-4-4 4-4M6 8l4 4-4 4"/></svg></div>
          <button class="btn btn-primary">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="#fff" d="M12 2L9 9H2l5.5 4-2 7L12 16l6.5 4-2-7L22 9h-7z"/></svg>
            按此生成
          </button>
        </div>
      </div>`
  },

  {
    id:'creator', group:'M4 模板', name:'创作者投稿', route:'/creator/submit',
    desc:'创作者中心 - 投稿流程：选择文件 → 填写信息 → 上传拼豆图（系统识别）→ 预览 → 提交审核。',
    elements:['4 步进度指示器','模板名 / 分类 / 标签 / 描述 表单','上传拼豆图（系统自动识别色号）','预览 + 提交审核'],
    design:['顶部 4 步步骤条，激活态主色','识别色号结果可手动调整','审核中显示进度 + 排队位置'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">投稿模板</div>
          <button class="btn-ghost">草稿</button>
        </div>
        <div style="display:flex;justify-content:space-between;padding:16px 24px;background:#fff;align-items:center;position:relative">
          <div style="position:absolute;left:50px;right:50px;top:30px;height:2px;background:var(--line);z-index:0"></div>
          <div style="position:absolute;left:50px;right:50px;top:30px;height:2px;background:linear-gradient(90deg,var(--primary) 50%,var(--line) 50%);z-index:0"></div>
          <div class="col" style="align-items:center;gap:4px;z-index:1">
            <div style="width:24px;height:24px;border-radius:50%;background:var(--primary);color:#fff;display:grid;place-items:center;font-size:11px;font-weight:700">1</div>
            <div style="font-size:11px;font-weight:600">选图</div>
          </div>
          <div class="col" style="align-items:center;gap:4px;z-index:1">
            <div style="width:24px;height:24px;border-radius:50%;background:var(--primary);color:#fff;display:grid;place-items:center;font-size:11px;font-weight:700">2</div>
            <div style="font-size:11px;font-weight:600">填信息</div>
          </div>
          <div class="col" style="align-items:center;gap:4px;z-index:1">
            <div style="width:24px;height:24px;border-radius:50%;background:#fff;border:2px solid var(--line);color:var(--ink-3);display:grid;place-items:center;font-size:11px;font-weight:700">3</div>
            <div style="font-size:11px;color:var(--ink-3)">预览</div>
          </div>
          <div class="col" style="align-items:center;gap:4px;z-index:1">
            <div style="width:24px;height:24px;border-radius:50%;background:#fff;border:2px solid var(--line);color:var(--ink-3);display:grid;place-items:center;font-size:11px;font-weight:700">4</div>
            <div style="font-size:11px;color:var(--ink-3)">提交</div>
          </div>
        </div>
        <div class="page-body">
          <div style="padding:18px">
            <div style="font-size:13px;font-weight:600;margin-bottom:10px">📸 拼豆图（系统识别色号）</div>
            <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:8px">
              <div style="aspect-ratio:1;border-radius:12px;background:linear-gradient(135deg,#FFD2B0,#FF7A5A);position:relative">
                <div style="position:absolute;left:6px;top:6px;background:rgba(108,199,161,.9);color:#fff;font-size:10px;padding:2px 6px;border-radius:6px">✓ 已识别 28 色</div>
              </div>
              <div style="aspect-ratio:1;border-radius:12px;background:linear-gradient(135deg,#9DC8E5,#6BC7A1)"></div>
              <div style="aspect-ratio:1;border-radius:12px;background:linear-gradient(135deg,#B49DD8,#F2A6A6)"></div>
            </div>
            <div style="margin-top:10px;padding:12px;background:#DFF5E9;border-radius:12px;font-size:12px;color:#1F7A4B">
              ✓ 系统识别完成：<b>28 色 / 50×50 板 / 5,000 颗</b>，请确认无误
            </div>
          </div>
          <div style="padding:0 18px">
            <div style="font-size:13px;font-weight:600;margin-bottom:10px">基本信息</div>
            <div class="card" style="padding:0">
              <div style="padding:14px 16px;border-bottom:1px solid var(--line)">
                <div style="font-size:11px;color:var(--ink-3);margin-bottom:4px">模板名 *</div>
                <input style="border:none;outline:none;width:100%;font-size:14px;font-family:inherit" value="圣诞老人" />
              </div>
              <div style="padding:14px 16px;border-bottom:1px solid var(--line)">
                <div style="font-size:11px;color:var(--ink-3);margin-bottom:4px">一级分类 *</div>
                <div class="row" style="justify-content:space-between">
                  <div class="row" style="gap:6px;flex-wrap:wrap">
                    <span class="chip active">节日</span>
                    <span class="chip">卡通</span>
                    <span class="chip">二次元</span>
                    <span class="chip">宠物</span>
                  </div>
                  <span class="ch" style="color:var(--ink-3)">›</span>
                </div>
              </div>
              <div style="padding:14px 16px">
                <div style="font-size:11px;color:var(--ink-3);margin-bottom:4px">标签 (最多 5 个)</div>
                <div class="row" style="gap:6px;flex-wrap:wrap">
                  <span class="chip soft">#亲子</span>
                  <span class="chip soft">#入门</span>
                  <span class="chip soft">+ 添加</span>
                </div>
              </div>
            </div>
          </div>
          <div style="padding:18px 18px 0">
            <div style="font-size:13px;font-weight:600;margin-bottom:10px">描述</div>
            <textarea style="width:100%;min-height:80px;padding:12px;background:#fff;border:1px solid var(--line);border-radius:12px;border:none;outline:none;font-size:14px;resize:none;font-family:inherit" placeholder="介绍一下这个模板…">给孩子的圣诞礼物🎄 30 分钟能拼完简单的部分，剩 1/3 留给他自己完成 ✨</textarea>
          </div>
          <div style="padding:18px 18px 30px">
            <button class="btn btn-primary full">下一步：预览效果</button>
          </div>
        </div>
      </div>`
  },

  {
    id:'mine', group:'我的', name:'我的', route:'/mine',
    desc:'个人中心。资料、图纸库、收藏、草稿、设置、会员中心。',
    elements:['头像 / 昵称 / 简介 / 等级','会员入口卡','8 个功能宫格','分组列表：关注 / 粉丝 / 我的帖子 / 草稿'],
    design:['暖色渐变头部 + 会员黑金卡','每个功能图标带主色背景色块','设置 / 退出放在最底部'],
    html:`
      <div class="page mine">
        <div class="page-body">
          <div class="mine-header">
            <div class="row">
              <div class="avatar">小</div>
              <div style="flex:1;margin-left:14px">
                <div class="name">小美 ✨</div>
                <div class="bio">拼豆玩家 · 经验值 1,234</div>
              </div>
              <button class="btn btn-sm" style="background:rgba(255,255,255,.25);color:#fff;border:1px solid rgba(255,255,255,.3);height:32px">编辑资料</button>
            </div>
            <div class="stats">
              <div class="item"><div class="n">23</div><div class="l">作品</div></div>
              <div class="item"><div class="n">156</div><div class="l">关注</div></div>
              <div class="item"><div class="n">89</div><div class="l">粉丝</div></div>
              <div class="item"><div class="n">1.2k</div><div class="l">获赞</div></div>
            </div>
          </div>
          <div class="mine-vip">
            <div class="vip-icon">👑</div>
            <div>
              <div class="t">开通会员 · 解锁无限生成</div>
              <div class="s">首月 ¥9.9 · 5 大特权</div>
            </div>
            <button class="btn">立即开通</button>
          </div>
          <div class="mine-grid">
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M3 3h7v7H3zm11 0h7v7h-7zM3 14h7v7H3zm11 0h7v7h-7z"/></svg></div><span>图纸库</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg></div><span>收藏</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M5 4h14v16H5zM5 4l-1 16m10-16l1 16"/></svg></div><span>草稿箱</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 4v16M4 12h16"/></svg></div><span>创作</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M3 5h18v2H3zm3 5h12v2H6zm3 5h6v2H9z"/></svg></div><span>我的帖子</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z"/></svg></div><span>关注/粉丝</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg></div><span>消息</span></div>
            <div class="g"><div class="ic"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20zm0 5v6M12 16h.01"/></svg></div><span>帮助</span></div>
          </div>
          <div class="mine-list">
            <div class="l-item"><div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg></div>会员中心 <span class="ch">›</span></div>
            <div class="l-item"><div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M3 6h18l-2 12H5L3 6z"/></svg></div>订单记录 <span class="ch">›</span></div>
            <div class="l-item"><div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M19 7h-1V6a3 3 0 0 0-3-3H9a3 3 0 0 0-3 3v1H5a2 2 0 0 0-2 2v11a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2z"/></svg></div>账号安全 <span class="ch">›</span></div>
            <div class="l-item"><div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg></div>设置 <span class="ch">›</span></div>
          </div>
        </div>
        <div class="tabbar">
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 3l9 8h-3v9h-4v-6H10v6H6v-9H3l9-8z"/></svg><div>首页</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M3 5h18v2H3zm0 6h18v2H3zm0 6h12v2H3z"/></svg><div>模板</div></div>
          <div class="tab-fab"><svg viewBox="0 0 24 24"><path fill="none" stroke="#fff" stroke-width="2.5" d="M12 5v14M5 12h14"/></svg></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg><div>社区</div></div>
          <div class="tab active"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z"/></svg><div>我的</div></div>
        </div>
      </div>`
  },

  {
    id:'my-diagrams', group:'我的', name:'我的图纸', route:'/mine/diagrams',
    desc:'用户创作的所有图纸。2 列网格缩略图，可按状态筛选。',
    elements:['Tab 切换：全部 / 草稿 / 已完成 / 已拼豆','2 列网格卡片：缩略图 + 名称 + 参数 + 状态徽章','点击进入预览/编辑'],
    design:['状态徽章置于缩略图左上角','长按弹出更多操作菜单','右上角"+ 新建"按钮'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">我的图纸</div>
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 4v16M4 12h16"/></svg></button>
        </div>
        <div class="forum-tabs" style="background:#fff">
          <div class="forum-tab active">全部 12</div>
          <div class="forum-tab">草稿 3</div>
          <div class="forum-tab">已完成 5</div>
          <div class="forum-tab">已拼豆 4</div>
        </div>
        <div class="page-body">
          <div class="mine-diagrams">
            <div class="md-card"><div class="pic"><div class="status">草稿</div></div><div class="info"><div class="n">小咪肖像</div><div class="p">28 色 / 50×50 · 2 天前</div></div></div>
            <div class="md-card"><div class="pic alt1"><div class="status" style="background:rgba(108,199,161,.85)">已完成</div></div><div class="info"><div class="n">海边日落</div><div class="p">45 色 / 58×58 · 5 天前</div></div></div>
            <div class="md-card"><div class="pic alt2"><div class="status" style="background:rgba(157,200,229,.85)">已拼豆</div></div><div class="info"><div class="n">Hello Kitty</div><div class="p">15 色 / 29×29 · 1 周前</div></div></div>
            <div class="md-card"><div class="pic alt3"><div class="status">草稿</div></div><div class="info"><div class="n">圣诞老人</div><div class="p">28 色 / 50×50 · 1 周前</div></div></div>
            <div class="md-card"><div class="pic alt1"><div class="status" style="background:rgba(108,199,161,.85)">已完成</div></div><div class="info"><div class="n">樱花树</div><div class="p">32 色 / 50×50 · 2 周前</div></div></div>
            <div class="md-card"><div class="pic alt2"><div class="status" style="background:rgba(157,200,229,.85)">已拼豆</div></div><div class="info"><div class="n">星空熊</div><div class="p">20 色 / 29×29 · 3 周前</div></div></div>
          </div>
        </div>
      </div>`
  },

  {
    id:'settings', group:'我的', name:'设置', route:'/settings',
    desc:'系统设置：通知、隐私、缓存、关于、退出登录。',
    elements:['消息通知开关组','隐私设置','清除缓存','关于我们 / 协议 / 反馈','退出登录'],
    design:['分组列表卡片化','开关用主色激活态','退出登录用危险色（红）'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">设置</div>
          <div></div>
        </div>
        <div class="page-body">
          <div class="settings-list" style="margin-top:14px">
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg></div>
              消息通知
              <div class="switch on"></div>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M19 7h-1V6a3 3 0 0 0-3-3H9a3 3 0 0 0-3 3v1H5a2 2 0 0 0-2 2v11a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2z"/></svg></div>
              账号安全
              <span class="ch">›</span>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20z"/></svg></div>
              深色模式
              <div class="switch"></div>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M19 7h-1V6a3 3 0 0 0-3-3H9a3 3 0 0 0-3 3v1H5"/></svg></div>
              清除缓存
              <span class="ch">32.5 MB ›</span>
            </div>
          </div>
          <div class="settings-list">
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20zm-1 5h2v6h-2zm0 8h2v2h-2z"/></svg></div>
              意见反馈
              <span class="ch">›</span>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M5 4h14v16H5z"/></svg></div>
              用户协议
              <span class="ch">›</span>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M19 7h-1V6a3 3 0 0 0-3-3H9a3 3 0 0 0-3 3v1H5"/></svg></div>
              隐私政策
              <span class="ch">›</span>
            </div>
            <div class="s-item">
              <div class="ic"><svg viewBox="0 0 24 24" width="16" height="16"><circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" stroke-width="2"/><path fill="currentColor" d="M12 8v.01M11 11h1v5h1"/></svg></div>
              关于拼豆
              <span class="ch">v0.1.0 ›</span>
            </div>
          </div>
          <div style="padding:18px">
            <button class="btn btn-secondary full" style="color:#C73A1B;border-color:#FFB6A8;background:#FFF5F2">退出登录</button>
          </div>
        </div>
      </div>`
  },

  {
    id:'vip', group:'我的', name:'会员中心', route:'/vip',
    desc:'会员购买与特权展示。月度 / 年度 / 终身三档。',
    elements:['会员徽章 + 倒计时','4-6 个特权卡片','3 档套餐选择','协议勾选 + 立即开通'],
    design:['深色背景 + 金色高光，强调尊贵感','"最受欢迎" 角标标在年度套餐','价格信息层次清晰'],
    html:`
      <div class="page vip">
        <div class="page-body">
          <div class="vip-hero">
            <div class="badge">👑</div>
            <h2>拼豆会员</h2>
            <p>解锁无限 AI 生成 · 全部模板 · 优先客服</p>
          </div>
          <div class="vip-perks">
            <div class="perk"><div class="ic">∞</div>无限 AI 生成</div>
            <div class="perk"><div class="ic">🎨</div>全模板免费</div>
            <div class="perk"><div class="ic">⚡</div>优先处理队列</div>
            <div class="perk"><div class="ic">💎</div>专属色号包</div>
            <div class="perk"><div class="ic">📞</div>VIP 客服</div>
            <div class="perk"><div class="ic">🎁</div>每月色卡</div>
          </div>
          <div class="vip-plans">
            <div class="plan">
              <div class="n">月度</div>
              <div class="p">¥19</div>
              <div class="s">/ 月</div>
            </div>
            <div class="plan featured">
              <div class="ribbon">省 50%</div>
              <div class="n">年度</div>
              <div class="p">¥99</div>
              <div class="s">/ 年 · ¥8.2/月</div>
            </div>
            <div class="plan">
              <div class="n">终身</div>
              <div class="p">¥298</div>
              <div class="s">一次付清</div>
            </div>
          </div>
          <div class="vip-foot">
            <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="#fff" stroke-width="2" d="M12 2L4 6v6c0 5 3.5 9 8 10 4.5-1 8-5 8-10V6l-8-4z"/></svg>
            <span>开通即视为同意《会员服务协议》· 7 天无理由退款</span>
          </div>
        </div>
        <div style="padding:14px 16px 28px">
          <button class="btn btn-primary full" style="background:linear-gradient(135deg,#F5C45E,#FF8A5A);color:#fff;height:54px;font-size:16px">
            立即开通 · ¥99 / 年
          </button>
        </div>
      </div>`
  }
];
