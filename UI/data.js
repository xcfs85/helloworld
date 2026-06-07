// 拼豆 · 移动端原型数据
// 20 个核心页面
const SCREENS = [
  {
    id:'splash', group:'启动 / 登录', name:'启动页 / 闪屏', route:'/splash',
    desc:'应用启动时展示品牌 Logo 与 Slogan，1.5s 内自动跳转到登录页或首页。',
    elements:['品牌 Logo 108×108 圆角','产品名 "拼豆" 30pt','Slogan "AI 照片转拼豆图纸"','加载状态指示器'],
    design:['暖色径向渐变背景（粉橘→暖黄）','Logo 浮动阴影带呼吸感','底部三段式 loading 圆点'],
    html:`
      <div class="page splash">
        <div class="splash-logo">
          <svg viewBox="0 0 32 32">
            <circle cx="8" cy="8" r="3" fill="#FF7A5A"/><circle cx="16" cy="8" r="3" fill="#2A1F1A"/><circle cx="24" cy="8" r="3" fill="#FF7A5A"/>
            <circle cx="8" cy="16" r="3" fill="#2A1F1A"/><circle cx="16" cy="16" r="3" fill="#F5C45E"/><circle cx="24" cy="16" r="3" fill="#2A1F1A"/>
            <circle cx="8" cy="24" r="3" fill="#FF7A5A"/><circle cx="16" cy="24" r="3" fill="#2A1F1A"/><circle cx="24" cy="24" r="3" fill="#FF7A5A"/>
          </svg>
        </div>
        <h1>拼豆</h1>
        <div class="slogan">用 AI 1 分钟<br/>把照片变成拼豆图纸</div>
        <div class="splash-dots"><span></span><span></span><span></span></div>
      </div>`
  },

  {
    id:'login', group:'启动 / 登录', name:'登录页', route:'/login',
    desc:'M1 注册登录模块主入口。提供手机号验证码、微信、Apple ID、游客四种方式。',
    elements:['手机号快捷登录主按钮','微信 / Apple / 游客 入口','用户协议 / 隐私政策勾选','一键登录辅助按钮'],
    design:['暖色径向渐变头部，乳白卡片下沉','三种第三方登录圆形入口，线性图标','底部"游客体验"小字链接','勾选框用主色实心态'],
    html:`
      <div class="page login">
        <div class="login-logo">
          <div class="mark">
            <svg viewBox="0 0 32 32" width="22" height="22">
              <circle cx="8" cy="8" r="3" fill="#fff"/><circle cx="16" cy="8" r="3" fill="#fff" opacity=".7"/>
              <circle cx="24" cy="8" r="3" fill="#fff"/><circle cx="8" cy="16" r="3" fill="#fff" opacity=".7"/>
              <circle cx="16" cy="16" r="3" fill="#fff"/><circle cx="24" cy="16" r="3" fill="#fff" opacity=".7"/>
              <circle cx="8" cy="24" r="3" fill="#fff"/><circle cx="16" cy="24" r="3" fill="#fff" opacity=".7"/>
              <circle cx="24" cy="24" r="3" fill="#fff"/>
            </svg>
          </div>
          <div class="name">拼豆</div>
        </div>
        <div class="login-title">欢迎来到拼豆</div>
        <div class="login-sub">用 AI 把喜欢的照片变成可拼的拼豆图纸<br/>新手也能完成第一个作品</div>
        <div class="login-phone">
          <div class="row">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="#8E7E72" stroke-width="2" d="M3 5h2l3 7-2 1a11 11 0 0 0 5 5l1-2 7 3v2a2 2 0 0 1-2 2A16 16 0 0 1 3 7V5z"/></svg>
            <input placeholder="请输入手机号" value="138 8888 8888" />
          </div>
          <div class="row">
            <svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="#8E7E72" stroke-width="2" d="M21 12a9 9 0 1 1-3-6.7M21 4v5h-5"/></svg>
            <input placeholder="6 位验证码" />
            <span class="send">59s 后重发</span>
          </div>
        </div>
        <button class="btn btn-primary full">手机号快捷登录</button>
        <div class="login-divider">其他方式登录</div>
        <div class="login-providers">
          <div class="pp" title="微信">
            <svg viewBox="0 0 32 32"><circle cx="16" cy="16" r="14" fill="#1AAD19"/><path fill="#fff" d="M12 11c-2.8 0-5 1.8-5 4 0 1.3.8 2.4 2 3.2l-.5 1.8 2-1c.5.1 1 .1 1.5.1.3 0 .6 0 .9-.1-.1-.4-.2-.8-.2-1.2 0-2.7 2.5-4.8 5.5-4.8h.6C18.3 11.8 15.4 11 12 11zm-2 2.2a.8.8 0 1 1 0-1.6.8.8 0 0 1 0 1.6zm4 0a.8.8 0 1 1 0-1.6.8.8 0 0 1 0 1.6zm10 4.5c0-1.9-2-3.5-4.5-3.5s-4.5 1.6-4.5 3.5 2 3.5 4.5 3.5c.4 0 .8 0 1.2-.1l1.6.9-.4-1.4c1-.7 1.6-1.6 1.6-2.9zm-6-.3a.6.6 0 1 1 0-1.2.6.6 0 0 1 0 1.2zm3 0a.6.6 0 1 1 0-1.2.6.6 0 0 1 0 1.2z"/></svg>
          </div>
          <div class="pp" title="Apple">
            <svg viewBox="0 0 32 32"><circle cx="16" cy="16" r="14" fill="#000"/><path fill="#fff" d="M19.4 17c0-2 1.6-3 1.7-3-.9-1.3-2.3-1.5-2.8-1.5-1.2-.1-2.3.7-2.9.7-.6 0-1.5-.7-2.5-.7-1.3 0-2.5.7-3.1 1.9-1.3 2.3-.3 5.7 1 7.5.6.9 1.4 1.9 2.3 1.9.9 0 1.3-.6 2.4-.6 1.1 0 1.4.6 2.4.6 1 0 1.6-.9 2.2-1.8.7-1 1-2 1-2.1-.1 0-2-.8-2-3zm-1.7-5.6c.5-.6.8-1.5.7-2.4-.7 0-1.6.5-2.1 1.1-.5.5-.9 1.4-.7 2.3.8 0 1.6-.4 2.1-1z"/></svg>
          </div>
          <div class="pp" title="游客">
            <svg viewBox="0 0 32 32"><circle cx="16" cy="16" r="14" fill="none" stroke="#5A4A40" stroke-width="2"/><circle cx="16" cy="13" r="5" fill="none" stroke="#5A4A40" stroke-width="2"/><path fill="none" stroke="#5A4A40" stroke-width="2" d="M7 25c1.5-4 5-6 9-6s7.5 2 9 6"/></svg>
          </div>
        </div>
        <div class="login-check">
          <div class="cb">
            <svg viewBox="0 0 16 16" width="11" height="11"><path fill="none" stroke="#fff" stroke-width="2.5" d="M3 8l3 3 7-7"/></svg>
          </div>
          <span>我已阅读并同意 <a>《用户协议》</a> 和 <a>《隐私政策》</a>，未注册的手机号将自动创建账号</span>
        </div>
        <div class="login-foot">遇到问题？ <a>联系客服</a> · <a>游客体验</a></div>
      </div>`
  },

  {
    id:'sms-verify', group:'启动 / 登录', name:'验证码输入', route:'/login/verify',
    desc:'手机号登录第二步：输入 6 位验证码。60s 倒计时 + 重发。',
    elements:['手机号回显 + 编辑','6 格验证码输入','60s 倒计时重发','语音验证码兜底'],
    design:['6 个独立方格，自动跳格','激活方格主色边框','倒计时与"重发"切换态'],
    html:`
      <div class="page" style="background:#fff">
        <div class="appbar">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div></div><div></div>
        </div>
        <div style="padding:60px 24px 30px">
          <div style="font-size:24px;font-weight:700;margin-bottom:6px">输入验证码</div>
          <div style="font-size:13px;color:var(--ink-3);line-height:1.6;margin-bottom:6px">已发送至 +86 138 8888 8888</div>
          <button class="btn-ghost" style="padding:0;font-size:12px;margin-bottom:30px">更换手机号 ›</button>
          <div style="display:flex;gap:10px;justify-content:space-between;margin-bottom:24px">
            <div style="flex:1;height:56px;background:var(--bg-2);border:2px solid var(--bg-2);border-radius:14px;display:grid;place-items:center;font-size:24px;font-weight:700">8</div>
            <div style="flex:1;height:56px;background:var(--bg-2);border:2px solid var(--bg-2);border-radius:14px;display:grid;place-items:center;font-size:24px;font-weight:700">2</div>
            <div style="flex:1;height:56px;background:var(--bg-2);border:2px solid var(--bg-2);border-radius:14px;display:grid;place-items:center;font-size:24px;font-weight:700">3</div>
            <div style="flex:1;height:56px;background:#fff;border:2px solid var(--primary);border-radius:14px;display:grid;place-items:center">
              <div style="width:2px;height:28px;background:var(--primary);animation:spin 1s linear infinite"></div>
            </div>
            <div style="flex:1;height:56px;background:#fff;border:2px solid var(--line);border-radius:14px"></div>
            <div style="flex:1;height:56px;background:#fff;border:2px solid var(--line);border-radius:14px"></div>
          </div>
          <div style="text-align:center;font-size:13px;color:var(--ink-3)">59s 后重新发送</div>
        </div>
        <div style="position:absolute;left:0;right:0;bottom:0;background:#fff;padding:14px 16px 28px;border-top:1px solid var(--line);text-align:center">
          <button class="btn-ghost">收不到？试试 语音验证码</button>
        </div>
      </div>`
  },

  {
    id:'home', group:'首页 / 导航', name:'首页', route:'/home',
    desc:'应用主入口。集合创作、社区、模板三大功能入口，运营位与最近作品。',
    elements:['顶部品牌 + 搜索 + 消息','主推 CTA "开始创作"','4 大功能入口宫格','社区动态信息流'],
    design:['暖色渐变 Hero 区，主推"AI 1 分钟"','运营 Banner 横向陈列','4 个入口图标带色块','TabBar 中央悬浮按钮（+）'],
    html:`
      <div class="page home">
        <div class="appbar">
          <div class="left">
            <div class="brand-mark" style="width:30px;height:30px;border-radius:9px;background:linear-gradient(135deg,#FFE2D3,#FFD2B0);display:grid;place-items:center">
              <svg viewBox="0 0 32 32" width="18" height="18">
                <circle cx="8" cy="8" r="3" fill="#FF7A5A"/><circle cx="16" cy="16" r="3" fill="#F5C45E"/><circle cx="24" cy="24" r="3" fill="#FF7A5A"/>
              </svg>
            </div>
            <div style="font-size:15px;font-weight:700;margin-left:6px">拼豆</div>
          </div>
          <div class="right">
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg></button>
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M6 8a6 6 0 1 1 12 0c0 7 3 7 3 9H3c0-2 3-2 3-9zM10 21a2 2 0 0 0 4 0"/></svg></button>
          </div>
        </div>
        <div class="page-body">
          <div class="home-hero">
            <h2>把照片变成拼豆</h2>
            <p>AI 一键生成 · 1 分钟得到可拼图纸</p>
            <button class="btn btn-sm">开始创作</button>
          </div>
          <div class="home-grid">
            <div class="home-tile">
              <div class="icon" style="background:#FFE2D3;color:#FF7A5A">
                <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M9 3l-1.5 4.5L3 9l4.5 1.5L9 15l1.5-4.5L15 9l-4.5-1.5L9 3zm9 8l-.8 2.4L15 14l2.2.6.8 2.4.8-2.4L21 14l-2.2-.6L18 11zm-2 5l-.5 1.5L14 18l1.5.5.5 1.5.5-1.5L18 18l-1.5-.5L16 16z"/></svg>
              </div>
              <div class="label">AI 生成</div>
            </div>
            <div class="home-tile">
              <div class="icon" style="background:#FFE0DC;color:#F2A6A6">
                <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M4 4h16v12H7l-3 4V4z"/></svg>
              </div>
              <div class="label">社区</div>
            </div>
            <div class="home-tile">
              <div class="icon" style="background:#DDE9FF;color:#5A8AFF">
                <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M3 3h7v7H3zm11 0h7v7h-7zM3 14h7v7H3zm11 0h7v7h-7z"/></svg>
              </div>
              <div class="label">模板库</div>
            </div>
            <div class="home-tile">
              <div class="icon" style="background:#DFF5E9;color:#6BC7A1">
                <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg>
              </div>
              <div class="label">色号推荐</div>
            </div>
          </div>
          <div class="home-banner">
            <div>
              <div class="b-t">新手福利</div>
              <div class="b-s">首次生成免费 · 赠 5 张色卡</div>
            </div>
            <div class="b-arrow">
              <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="#fff" stroke-width="2.5" d="M9 6l6 6-6 6"/></svg>
            </div>
          </div>
          <div class="section-title">
            <h3>社区精选</h3>
            <span class="more">查看更多 ›</span>
          </div>
          <div class="feed-list">
            <div class="feed-item">
              <div style="aspect-ratio:1.6/1;background:linear-gradient(135deg,#FFD2B0,#F5C45E);position:relative">
                <div style="position:absolute;left:12px;bottom:10px;background:rgba(0,0,0,.4);color:#fff;padding:4px 10px;border-radius:999px;font-size:11px;backdrop-filter:blur(4px)">作品</div>
              </div>
              <div class="meta">
                <div class="title">给小咪的拼豆肖像 🐱</div>
                <div class="info">@小美 · 50×50 板 · 28 色 · 拼了 8h</div>
                <div class="row-icons">
                  <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg> 1.2k</span>
                  <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg> 89</span>
                  <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg> 234</span>
                </div>
              </div>
            </div>
            <div class="feed-item">
              <div style="aspect-ratio:1.6/1;background:linear-gradient(135deg,#9DC8E5,#6BC7A1);position:relative">
                <div style="position:absolute;left:12px;bottom:10px;background:rgba(0,0,0,.4);color:#fff;padding:4px 10px;border-radius:999px;font-size:11px;backdrop-filter:blur(4px)">教程</div>
              </div>
              <div class="meta">
                <div class="title">新手必看 · 0 基础入门拼豆</div>
                <div class="info">@Lily 老师 · 8 分钟看完</div>
                <div class="row-icons">
                  <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg> 567</span>
                  <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg> 23</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="tabbar">
          <div class="tab active"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 3l9 8h-3v9h-4v-6H10v6H6v-9H3l9-8z"/></svg><div>首页</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M3 5h18v2H3zm0 6h18v2H3zm0 6h12v2H3z"/></svg><div>模板</div></div>
          <div class="tab-fab"><svg viewBox="0 0 24 24"><path fill="none" stroke="#fff" stroke-width="2.5" d="M12 5v14M5 12h14"/></svg></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg><div>社区</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z"/></svg><div>我的</div></div>
        </div>
      </div>`
  },

  {
    id:'select-image', group:'M2 创作', name:'选图页', route:'/create/select',
    desc:'M2 入口第一步。用户从相册、相机或内置示例图中选择一张照片。',
    elements:['拍照 / 相册 / 示例 三大来源','最近 6 张相册预览','6-8 张内置示例图','客户端预检：≤20MB，长边≤8192px'],
    design:['3 列网格预览图，圆形悬浮 + 按钮','示例图区域用斜纹底色区隔','底部固定主操作区'],
    html:`
      <div class="page">
        <div class="appbar">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">选择照片</div>
          <div></div>
        </div>
        <div class="page-body">
          <div class="section-title"><h3>从相册选择</h3><span class="more">全部 ›</span></div>
          <div class="select-grid">
            <div class="select-cell"><span class="lbl">小咪</span><div class="badge">最近</div></div>
            <div class="select-cell alt"></div>
            <div class="select-cell alt2"></div>
            <div class="select-cell alt3"></div>
            <div class="select-cell alt4"></div>
            <div class="select-cell alt5"></div>
            <div class="select-cell"></div>
            <div class="select-cell alt3"></div>
            <div class="select-cell alt2"></div>
          </div>
          <div class="section-title"><h3>推荐示例图</h3><span class="more">更多 ›</span></div>
          <div class="select-grid">
            <div class="select-cell examples"><span>宠物合集<br/>8 张</span></div>
            <div class="select-cell examples" style="background:repeating-linear-gradient(45deg,#DDE9FF,#DDE9FF 10px,#9DC8E5 10px,#9DC8E5 20px)"><span>卡通角色<br/>12 张</span></div>
            <div class="select-cell examples" style="background:repeating-linear-gradient(45deg,#DFF5E9,#DFF5E9 10px,#6BC7A1 10px,#6BC7A1 20px)"><span>风景<br/>6 张</span></div>
            <div class="select-cell examples" style="background:repeating-linear-gradient(45deg,#FFE2D3,#FFE2D3 10px,#F2A6A6 10px,#F2A6A6 20px)"><span>情侣<br/>8 张</span></div>
            <div class="select-cell examples" style="background:repeating-linear-gradient(45deg,#FFF1E2,#FFF1E2 10px,#F5C45E 10px,#F5C45E 20px)"><span>节日<br/>10 张</span></div>
            <div class="select-cell examples" style="background:repeating-linear-gradient(45deg,#EFE6FF,#EFE6FF 10px,#B49DD8 10px,#B49DD8 20px)"><span>头像<br/>14 张</span></div>
          </div>
        </div>
        <div style="position:absolute;left:0;right:0;bottom:0;background:rgba(255,255,255,.95);backdrop-filter:blur(10px);border-top:1px solid var(--line);padding:12px 16px 24px;display:flex;gap:10px">
          <button class="btn btn-secondary"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M3 7h4l2-3h6l2 3h4v13H3V7zm9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8z"/></svg> 拍照</button>
          <button class="btn btn-primary"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="#fff" stroke-width="2" d="M4 5h16v14H4zM4 15l5-5 4 4 3-3 4 4"/></svg> 从相册选择</button>
        </div>
      </div>`
  },

  {
    id:'edit-image', group:'M2 创作', name:'编辑页', route:'/create/edit',
    desc:'M2 第二步：裁剪与基础图像调整。支持自由/方形裁剪、旋转、翻转、亮度对比度饱和度。',
    elements:['画布 + 裁剪框 + 四角拖拽手柄','工具栏：裁剪 / 旋转 / 翻转 / 微调','亮度 / 对比度 / 饱和度 滑杆','撤销 / 重做 / 下一步'],
    design:['200ms 实时预览，滑杆 8dp 网格','工具横向滚动，当前态深色填充','裁剪手柄白底阴影 18×18'],
    html:`
      <div class="page">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">编辑</div>
          <button class="btn-ghost">下一步</button>
        </div>
        <div class="page-body">
          <div class="edit-canvas">
            <div class="checker"></div>
            <div class="crop">
              <div class="handle h-tl"></div><div class="handle h-tr"></div>
              <div class="handle h-bl"></div><div class="handle h-br"></div>
            </div>
          </div>
          <div class="edit-tools">
            <div class="tool active">
              <div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M6 2v4H2v16h16v-4h4V2H6zm10 16H6V6h10v12z"/></svg></div>
              <span>裁剪</span>
            </div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 4V2L8 6l4 4V8a4 4 0 0 1 4 4h2a6 6 0 0 0-6-6zm0 12v-2a4 4 0 0 1-4-4H6a6 6 0 0 0 6 6v2l4-4-4-4z"/></svg></div><span>旋转</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M16 4l-1 5h5l-9 11 1-5H7l9-11z"/></svg></div><span>水平</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M4 4h16v3H4zm0 6h16v3H4zm0 6h16v3H4z"/></svg></div><span>垂直</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><circle cx="12" cy="12" r="4" fill="currentColor"/><path fill="currentColor" d="M3 12a9 9 0 0 1 18 0h-2a7 7 0 0 0-14 0H3z"/></svg></div><span>亮度</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><circle cx="12" cy="12" r="5" fill="none" stroke="currentColor" stroke-width="2"/><circle cx="12" cy="12" r="2" fill="currentColor"/></svg></div><span>对比</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="currentColor" d="M12 2a10 10 0 1 0 0 20 6 6 0 0 1 0-12 6 6 0 0 0 0-8z"/></svg></div><span>饱和</span></div>
            <div class="tool"><div class="tb"><svg viewBox="0 0 24 24" width="18" height="18"><path fill="none" stroke="currentColor" stroke-width="2" d="M3 12a9 9 0 1 0 18 0 9 9 0 0 0-18 0zm9-7v14M5 12h14"/></svg></div><span>比例</span></div>
          </div>
          <div class="slider-row">
            <div class="label-row"><span class="lbl">亮度</span><span class="val">+10</span></div>
            <div class="slider"></div>
          </div>
          <div class="slider-row">
            <div class="label-row"><span class="lbl">对比度</span><span class="val">+5</span></div>
            <div class="slider"></div>
          </div>
          <div class="slider-row">
            <div class="label-row"><span class="lbl">饱和度</span><span class="val">+15</span></div>
            <div class="slider"></div>
          </div>
          <div style="display:flex;justify-content:space-between;padding:18px">
            <button class="btn btn-secondary btn-sm">
              <svg viewBox="0 0 24 24" width="14" height="14"><path fill="none" stroke="currentColor" stroke-width="2" d="M9 14L4 9l5-5M4 9h11a5 5 0 0 1 0 10h-3"/></svg>
              撤销
            </button>
            <button class="btn btn-primary btn-sm">下一步</button>
          </div>
        </div>
      </div>`
  },

  {
    id:'params', group:'M2 创作', name:'参数配置', route:'/create/params',
    desc:'选择总颗数、底板规格、难度、风格。系统会基于图片智能推荐参数。',
    elements:['图片缩略预览','总颗数档位 7 档','底板 3 种规格','难度 + 风格 2 选','AI 智能推荐标签'],
    design:['智能推荐置于顶部，用主色高亮','档位卡片化，选中态用渐变填充','参数组横向 Chip 流式布局'],
    html:`
      <div class="page">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">参数配置</div>
          <button class="btn-ghost">跳过</button>
        </div>
        <div class="page-body">
          <div style="padding:0 16px;margin-top:14px">
            <div class="recommend-pill">智能推荐 · 这是一张人物肖像</div>
            <div style="display:flex;gap:10px;align-items:center">
              <div class="preview-thumb" style="height:90px;flex:0 0 90px;border-radius:14px"></div>
              <div style="flex:1">
                <div style="font-size:14px;font-weight:600">建议：50×50 板 / 5000 颗 / 进阶</div>
                <div style="font-size:11px;color:var(--ink-3);margin-top:2px">预计 8-10 小时完成 · 适合熟手</div>
              </div>
            </div>
          </div>
          <div class="params">
            <div class="params-group">
              <div class="gh">总颗数 <span class="hint">颗数越多越精细</span></div>
              <div class="option-row">
                <div class="option"><div>500</div><div class="o-sub">钥匙扣</div></div>
                <div class="option"><div>1,000</div><div class="o-sub">小卡</div></div>
                <div class="option active"><div>5,000</div><div class="o-sub">推荐</div></div>
                <div class="option"><div>10,000</div><div class="o-sub">大作</div></div>
              </div>
            </div>
            <div class="params-group">
              <div class="gh">底板规格 <span class="hint">29 / 50 / 58</span></div>
              <div class="option-row">
                <div class="option"><div>29×29</div><div class="o-sub">7.5cm</div></div>
                <div class="option active"><div>50×50</div><div class="o-sub">13cm</div></div>
                <div class="option"><div>58×58</div><div class="o-sub">15cm</div></div>
              </div>
            </div>
            <div class="params-group">
              <div class="gh">难度 <span class="hint">影响 AI 处理的色数</span></div>
              <div class="option-row">
                <div class="option"><div>简单</div><div class="o-sub">8-15 色</div></div>
                <div class="option active"><div>进阶</div><div class="o-sub">16-30 色</div></div>
                <div class="option"><div>写实</div><div class="o-sub">30-80 色</div></div>
              </div>
            </div>
            <div class="params-group">
              <div class="gh">风格 <span class="hint">MVP 仅 2 种</span></div>
              <div class="option-row">
                <div class="option active"><div>写实</div><div class="o-sub">还原度高</div></div>
                <div class="option"><div>卡通</div><div class="o-sub">色块鲜明</div></div>
              </div>
            </div>
          </div>
          <div style="padding:18px 16px 24px">
            <button class="btn btn-primary full">开始 AI 生成</button>
          </div>
        </div>
      </div>`
  },

  {
    id:'generating', group:'M2 创作', name:'AI 生成中', route:'/create/generating',
    desc:'WebSocket 实时推送生成进度。≤5000 颗同步返回，>5000 颗可后台完成。',
    elements:['可视化进度艺术','当前阶段文字 + 百分比','5 步进度阶段','可切换到后台 / 取消'],
    design:['大图渐变 + 网格扫描动画','底部阶段步骤，已完成绿色，当前橙色主色','背景进入其他页面时仍可继续'],
    html:`
      <div class="page generating">
        <div class="gen-art">
          <div class="gen-icon">
            <svg viewBox="0 0 24 24" width="48" height="48"><path fill="none" stroke="#fff" stroke-width="2" d="M12 2v4M12 18v4M2 12h4M18 12h4M5 5l3 3M16 16l3 3M5 19l3-3M16 8l3-3"/><circle cx="12" cy="12" r="4" fill="#fff"/></svg>
          </div>
          <div class="gen-percent">62%</div>
        </div>
        <div class="gen-stage">正在映射色号</div>
        <div class="gen-stage-sub">智能识别每一颗拼豆的色号<br/>约还需要 12 秒</div>
        <div class="gen-progress">
          <div class="bar"><div class="fill"></div></div>
          <div class="row"><span>已完成 3 / 5</span><span>剩余 12s</span></div>
        </div>
        <div class="gen-stages">
          <div class="gen-stage-item done"><div class="dot"><svg viewBox="0 0 16 16" width="10" height="10"><path fill="none" stroke="#fff" stroke-width="2.5" d="M3 8l3 3 7-7"/></svg></div><span>正在分析图像</span></div>
          <div class="gen-stage-item done"><div class="dot"><svg viewBox="0 0 16 16" width="10" height="10"><path fill="none" stroke="#fff" stroke-width="2.5" d="M3 8l3 3 7-7"/></svg></div><span>正在智能裁剪</span></div>
          <div class="gen-stage-item active"><div class="dot"></div><span>正在映射色号 (62%)</span></div>
          <div class="gen-stage-item"><div class="dot"></div><span>正在生成图纸</span></div>
          <div class="gen-stage-item"><div class="dot"></div><span>即将完成</span></div>
        </div>
        <div style="position:absolute;bottom:24px;left:24px;right:24px">
          <button class="btn btn-secondary full" style="background:rgba(255,255,255,.7);backdrop-filter:blur(10px)">后台运行，完成后通知我</button>
        </div>
      </div>`
  },

  {
    id:'result', group:'M2 创作', name:'结果预览', route:'/create/result/:id',
    desc:'生成完成的拼豆图预览。支持双指缩放、格线开关、色号标签、长按查看详情。',
    elements:['拼豆图大图预览','格线 / 色号编号 / 长按查看 开关','对比原图','重新生成 / 保存 / 分享 / 导出'],
    design:['深色背景突出彩色拼豆图','底部双 CTA：主操作"查看色号表" + 次要"保存草稿"','历史版本 5 个，横向滚动切换'],
    html:`
      <div class="page result">
        <div class="appbar" style="background:rgba(0,0,0,.4);backdrop-filter:blur(10px);color:#fff">
          <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title" style="color:#fff">小咪 · 5,000 颗</div>
          <button class="icon-btn" style="color:#fff"><svg viewBox="0 0 24 24" width="20" height="20"><circle cx="5" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="19" cy="12" r="2" fill="currentColor"/></svg></button>
        </div>
        <div class="result-image">
          <div style="position:absolute;left:14px;top:14px;background:rgba(0,0,0,.5);color:#fff;padding:6px 12px;border-radius:999px;font-size:11px;backdrop-filter:blur(6px);z-index:3">50 × 50 · 进阶</div>
          <div style="position:absolute;right:14px;top:14px;background:rgba(0,0,0,.5);color:#fff;padding:6px 12px;border-radius:999px;font-size:11px;backdrop-filter:blur(6px);z-index:3">28 色</div>
        </div>
        <div class="result-toolbar">
          <div class="tb active">
            <div class="ib"><svg viewBox="0 0 24 24" width="20" height="20" fill="#fff"><path d="M3 3h7v7H3zm11 0h7v7h-7zM3 14h7v7H3zm11 0h7v7h-7z"/></svg></div>
            <span>格线</span>
          </div>
          <div class="tb">
            <div class="ib"><svg viewBox="0 0 24 24" width="20" height="20"><text x="3" y="16" font-size="11" fill="#fff" stroke="none">M01</text><circle cx="12" cy="12" r="9" fill="none" stroke="#fff" stroke-width="2"/></svg></div>
            <span>色号</span>
          </div>
          <div class="tb">
            <div class="ib"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="#fff" stroke-width="2" d="M3 12a9 9 0 1 0 18 0 9 9 0 0 0-18 0zm9-7v14M5 12h14"/></svg></div>
            <span>对比</span>
          </div>
          <div class="tb">
            <div class="ib"><svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="#fff" stroke-width="2"><path d="M9 3l-1 5H3l4 3-1 5 4-3 4 3-1-5 4-3h-5l-1-5z"/></svg></div>
            <span>缩放</span>
          </div>
        </div>
        <div class="result-foot">
          <button class="btn">
            <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="2" d="M4 4h12l4 4v12H4V4zM4 14h16M9 9h6"/></svg>
            重新生成
          </button>
          <button class="btn btn-primary">
            查看色号表
            <svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="#fff" stroke-width="2" d="M9 6l6 6-6 6"/></svg>
          </button>
        </div>
      </div>`
  }
];

// 导出加载函数
if (typeof module !== 'undefined') module.exports = SCREENS;
