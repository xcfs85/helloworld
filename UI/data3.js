// 拼豆 · 移动端原型数据 (第三部分)
const SCREENS_3 = [
  {
    id:'post-create', group:'M3 论坛', name:'发布帖子', route:'/post/create',
    desc:'4 种帖子类型：作品 / 求图 / 教程 / 讨论。支持图文 / 视频 / 富文本教程。',
    elements:['4 种帖子类型切换','图片 1-9 张','标题 + 描述输入','拼豆参数自动带入','关联图纸 + 话题标签'],
    design:['顶部固定 关闭/草稿/发布','图片九宫格 + 添加按钮','参数行右箭头可展开','自动保存草稿（30 秒）'],
    html:`
      <div class="page">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M6 6l12 12M6 18L18 6"/></svg></button>
          <div class="appbar-title">发布</div>
          <div class="row" style="gap:6px">
            <button class="chip">草稿</button>
            <button class="btn btn-primary btn-sm">发布</button>
          </div>
        </div>
        <div class="publish-type">
          <div class="pt active">作品</div>
          <div class="pt">求图</div>
          <div class="pt">教程</div>
          <div class="pt">讨论</div>
        </div>
        <div class="page-body" style="padding-bottom:80px">
          <div class="publish-editor">
            <div class="img-grid">
              <div class="ig"></div>
              <div class="ig"></div>
              <div class="ig"></div>
              <div class="ig"></div>
              <div class="ig"></div>
              <div class="add">+</div>
            </div>
            <input style="width:100%;border:none;outline:none;font-size:16px;font-weight:600;font-family:inherit" placeholder="给作品起个标题…" value="给闺蜜的生日礼物🎁" />
            <textarea placeholder="说点什么… 支持 @用户 和 #话题">50×50 板 / 28 色 / 拼了 8 小时终于完成！闺蜜看到哭了一小时 😭</textarea>
          </div>
          <div class="publish-params">
            <div class="row"><span>📐 拼豆参数</span><span class="v">50×50 板 / 28 色 / 8h <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M9 6l6 6-6 6"/></svg></span></div>
            <div class="row"><span>🔗 关联图纸</span><span class="v">小咪 50×50 <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M9 6l6 6-6 6"/></svg></span></div>
            <div class="row"><span>#️⃣ 话题</span><span class="v">#生日礼物 #闺蜜 <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M9 6l6 6-6 6"/></svg></span></div>
            <div class="row"><span>📍 位置</span><span class="v">不显示 <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M9 6l6 6-6 6"/></svg></span></div>
          </div>
        </div>
      </div>`
  },

  {
    id:'messages', group:'M3 论坛', name:'消息中心', route:'/messages',
    desc:'5 类消息聚合：评论 / 点赞 / 关注 / @我 / 系统。每类未读红点提示。',
    elements:['评论 / 点赞 / 关注 / @我 / 系统 5 个分类','未读红点 + 数量','最新消息预览','右上角设置入口'],
    design:['每类用主色 + 线性图标徽标','长按可关闭某类通知','点击进入对应消息列表'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <div class="appbar-title">消息</div>
          <div class="right">
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M3 5h18v2H3zm3 5h12v2H6zm3 5h6v2H9z"/></svg></button>
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg></button>
          </div>
        </div>
        <div class="page-body" style="background:#fff">
          <div class="msg-row">
            <div class="ic" style="background:linear-gradient(135deg,#FF7A5A,#F5C45E)">💬<div class="badge">12</div></div>
            <div class="info"><div class="t">评论</div><div class="s">Lily 老师：太厉害啦！第 5 行能看出…</div></div>
            <span class="chev">›</span>
          </div>
          <div class="msg-row">
            <div class="ic" style="background:linear-gradient(135deg,#F2A6A6,#FF7A5A)">❤<div class="badge">234</div></div>
            <div class="info"><div class="t">收到的赞</div><div class="s">你的作品"给闺蜜的生日礼物🎁"收获了 234 个赞</div></div>
            <span class="chev">›</span>
          </div>
          <div class="msg-row">
            <div class="ic" style="background:linear-gradient(135deg,#6BC7A1,#9DC8E5)">➕<div class="badge">5</div></div>
            <div class="info"><div class="t">新增关注</div><div class="s">求图达人 等 5 人关注了你</div></div>
            <span class="chev">›</span>
          </div>
          <div class="msg-row">
            <div class="ic" style="background:linear-gradient(135deg,#9DC8E5,#B49DD8)">🔔<div class="badge">2</div></div>
            <div class="info"><div class="t">@我的</div><div class="s">求图达人 @你 求这个图纸！</div></div>
            <span class="chev">›</span>
          </div>
          <div class="msg-row">
            <div class="ic" style="background:linear-gradient(135deg,#F5C45E,#FF8A5A)">📢</div>
            <div class="info"><div class="t">系统通知</div><div class="s">【活动】新用户首单免费生成</div></div>
            <span class="chev">›</span>
          </div>
        </div>
      </div>`
  },

  {
    id:'profile', group:'M3 论坛', name:'个人主页', route:'/user/:id',
    desc:'用户主页：作品 / 教程 / 收藏三 Tab，3 列网格作品墙。',
    elements:['头像 + 昵称 + 简介 + 关注按钮','粉丝 / 关注 / 作品数','作品 / 教程 / 收藏 Tab','3 列网格瀑布流'],
    design:['大图头图 160dp','创作者徽章可叠加','双列可改为 3 列网格'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div style="height:160px;background:linear-gradient(135deg,#FFB088 0%,#FF7A5A 60%,#E25D3E 100%);position:relative">
          <div class="appbar" style="color:#fff;position:absolute;top:0;left:0;right:0">
            <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
            <div></div>
            <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><circle cx="5" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="19" cy="12" r="2" fill="currentColor"/></svg></button>
          </div>
        </div>
        <div style="background:#fff;padding:0 18px 16px;margin-top:-50px;border-radius:24px 24px 0 0;position:relative;z-index:2">
          <div class="row" style="align-items:flex-end;margin-top:-32px">
            <div style="width:72px;height:72px;border-radius:50%;background:linear-gradient(135deg,#FFB088,#FF7A5A);border:4px solid #fff;display:grid;place-items:center;font-size:28px;color:#fff;font-weight:700">小</div>
            <div style="flex:1;padding-bottom:6px">
              <div style="font-size:18px;font-weight:700;display:flex;align-items:center;gap:6px">小美 ✨ <span class="tag warn" style="font-size:9px">认证创作者</span></div>
              <div style="font-size:12px;color:var(--ink-3)">ID: pd_87231 · 北京</div>
            </div>
            <button class="btn btn-primary btn-sm" style="margin-bottom:6px">+ 关注</button>
          </div>
          <div style="font-size:13px;color:var(--ink-2);line-height:1.6;margin-top:10px">拼豆玩家 / 喜欢小动物 / 作品以肖像为主 🐱🐶</div>
          <div class="row" style="justify-content:space-around;margin-top:14px;padding:12px 0;background:var(--bg-2);border-radius:14px">
            <div class="center"><div style="font-size:18px;font-weight:700">23</div><div style="font-size:11px;color:var(--ink-3)">作品</div></div>
            <div class="center"><div style="font-size:18px;font-weight:700">89</div><div style="font-size:11px;color:var(--ink-3)">粉丝</div></div>
            <div class="center"><div style="font-size:18px;font-weight:700">156</div><div style="font-size:11px;color:var(--ink-3)">关注</div></div>
            <div class="center"><div style="font-size:18px;font-weight:700">1.2k</div><div style="font-size:11px;color:var(--ink-3)">获赞</div></div>
          </div>
        </div>
        <div class="forum-tabs" style="background:#fff;border-top:1px solid var(--line);margin-top:8px">
          <div class="forum-tab active">作品 23</div>
          <div class="forum-tab">教程 5</div>
          <div class="forum-tab">收藏 87</div>
        </div>
        <div class="page-body" style="background:#fff">
          <div class="tpl-grid" style="padding:12px 12px">
            <div class="tpl-card"><div class="pic"></div><div class="info"><div class="n">小咪肖像</div><div class="r"><span>28 色 / 50×50</span><span class="likes">♡ 234</span></div></div></div>
            <div class="tpl-card"><div class="pic alt1"></div><div class="info"><div class="n">海边日落</div><div class="r"><span>45 色 / 58×58</span><span class="likes">♡ 567</span></div></div></div>
            <div class="tpl-card"><div class="pic alt2"></div><div class="info"><div class="n">樱花树</div><div class="r"><span>32 色 / 50×50</span><span class="likes">♡ 189</span></div></div></div>
            <div class="tpl-card"><div class="pic alt3"></div><div class="info"><div class="n">星空熊</div><div class="r"><span>20 色 / 29×29</span><span class="likes">♡ 1024</span></div></div></div>
            <div class="tpl-card"><div class="pic alt4"></div><div class="info"><div class="n">雪山小屋</div><div class="r"><span>56 色 / 58×58</span><span class="likes">♡ 432</span></div></div></div>
            <div class="tpl-card"><div class="pic alt5"></div><div class="info"><div class="n">薰衣草田</div><div class="r"><span>42 色 / 50×50</span><span class="likes">♡ 678</span></div></div></div>
          </div>
        </div>
      </div>`
  },

  {
    id:'templates', group:'M4 模板', name:'模板库', route:'/template',
    desc:'M4 模板浏览入口。10 大分类、双列瀑布流、搜索 / 筛选 / 排序。',
    elements:['搜索栏 + 筛选 + 排序','10 大分类横向滚动','双列瀑布流卡片','难度 + 色号 + 板型筛选'],
    design:['搜索栏 + 横向分类 + 2 列卡片，节奏清晰','卡片显示难度徽章 + 收藏图标','下拉刷新 + 触底加载'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <div class="appbar-title">模板</div>
          <div class="right">
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg></button>
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M4 6h16M7 12h10M10 18h4"/></svg></button>
          </div>
        </div>
        <div class="tpl-search">
          <div class="inp">
            <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg>
            <input placeholder="搜索模板 / 创作者 / 标签" />
            <svg viewBox="0 0 24 24" width="16" height="16"><path fill="currentColor" d="M3 5h18v2H3zm3 5h12v2H6zm3 5h6v2H9z"/></svg>
          </div>
        </div>
        <div class="tpl-cats">
          <div class="cat active">推荐</div>
          <div class="cat">节日</div>
          <div class="cat">卡通</div>
          <div class="cat">二次元</div>
          <div class="cat">宠物</div>
          <div class="cat">风景</div>
          <div class="cat">像素游戏</div>
          <div class="cat">国风</div>
          <div class="cat">表情包</div>
          <div class="cat">文字</div>
        </div>
        <div class="page-body">
          <div class="tpl-grid">
            <div class="tpl-card"><div class="pic"><div class="c-chip">节日</div><svg viewBox="0 0 24 24" width="14" height="14" style="position:absolute;right:8px;top:8px;color:#fff" fill="currentColor"><path d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg></div><div class="info"><div class="n">圣诞老人</div><div class="r"><span>28 色 / 50×50</span><span class="likes">♡ 1.2k</span></div></div></div>
            <div class="tpl-card"><div class="pic alt1"><div class="c-chip">宠物</div></div><div class="info"><div class="n">小柯基</div><div class="r"><span>22 色 / 29×29</span><span class="likes">♡ 892</span></div></div></div>
            <div class="tpl-card"><div class="pic alt2"><div class="c-chip">卡通</div></div><div class="info"><div class="n">Hello Kitty</div><div class="r"><span>15 色 / 29×29</span><span class="likes">♡ 2.1k</span></div></div></div>
            <div class="tpl-card"><div class="pic alt3"><div class="c-chip">节日</div></div><div class="info"><div class="n">中秋玉兔</div><div class="r"><span>34 色 / 50×50</span><span class="likes">♡ 543</span></div></div></div>
            <div class="tpl-card"><div class="pic alt4"><div class="c-chip">二次元</div></div><div class="info"><div class="n">初音未来</div><div class="r"><span>42 色 / 50×50</span><span class="likes">♡ 3.4k</span></div></div></div>
            <div class="tpl-card"><div class="pic alt5"><div class="c-chip">风景</div></div><div class="info"><div class="n">海边日落</div><div class="r"><span>38 色 / 50×50</span><span class="likes">♡ 678</span></div></div></div>
            <div class="tpl-card"><div class="pic alt6"><div class="c-chip">国风</div></div><div class="info"><div class="n">青绿山水</div><div class="r"><span>52 色 / 58×58</span><span class="likes">♡ 432</span></div></div></div>
            <div class="tpl-card"><div class="pic alt2"><div class="c-chip">表情</div></div><div class="info"><div class="n">熊猫头</div><div class="r"><span>8 色 / 29×29</span><span class="likes">♡ 5.6k</span></div></div></div>
          </div>
        </div>
        <div class="tabbar">
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 3l9 8h-3v9h-4v-6H10v6H6v-9H3l9-8z"/></svg><div>首页</div></div>
          <div class="tab active"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M3 5h18v2H3zm0 6h18v2H3zm0 6h12v2H3z"/></svg><div>模板</div></div>
          <div class="tab-fab"><svg viewBox="0 0 24 24"><path fill="none" stroke="#fff" stroke-width="2.5" d="M12 5v14M5 12h14"/></svg></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg><div>社区</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z"/></svg><div>我的</div></div>
        </div>
      </div>`
  }
];
