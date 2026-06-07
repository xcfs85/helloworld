// 拼豆 · 移动端原型数据 (第二部分)
window.PINDOU = window.PINDOU || {};
PINDOU.screens2 = [
  {
    id:'colors', group:'M5 色号', name:'色号表详情', route:'/create/colors/:id',
    desc:'M5 主页面。展示拼豆图所用全部色号，支持排序、筛选、难度调整、导出。',
    elements:['色号 / 颗数汇总','占比柱状预览','4 种排序 + 4 种筛选','色号表 6 列','导出 PNG / Excel / PDF'],
    design:['顶部概览卡片，色块拼成柱状图','表格行紧凑，色块前置，颗数右对齐','底部固定"导出 + 耗材推荐"双 CTA'],
    html:`
      <div class="page colors">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">色号表</div>
          <button class="btn-ghost">导出</button>
        </div>
        <div class="page-body" style="padding-bottom:80px">
          <div class="colors-summary">
            <div class="top">
              <div>
                <h2>共 28 <span class="u">色</span></h2>
                <div class="total">5,000 颗 · 50×50 板</div>
              </div>
              <div style="text-align:right">
                <div style="font-size:11px;color:var(--ink-3)">完成时间</div>
                <div style="font-size:14px;font-weight:600;margin-top:2px">约 8-10h</div>
              </div>
            </div>
            <div class="color-bar">
              <span style="background:#FF7A5A;flex:4.7"></span>
              <span style="background:#2A1F1A;flex:3.8"></span>
              <span style="background:#F5C45E;flex:3.2"></span>
              <span style="background:#6BC7A1;flex:2.5"></span>
              <span style="background:#9DC8E5;flex:2.1"></span>
              <span style="background:#B49DD8;flex:1.8"></span>
              <span style="background:#8B5A3C;flex:1.5"></span>
              <span style="background:#F2A6A6;flex:1.2"></span>
              <span style="background:var(--bg-2);flex:79.2"></span>
            </div>
            <div class="stats">
              <div class="st"><div class="n">5</div><div class="l">主色 (>5%)</div></div>
              <div class="st"><div class="n">12</div><div class="l">辅色 (1-5%)</div></div>
              <div class="st"><div class="n">11</div><div class="l">配色 (<1%)</div></div>
            </div>
          </div>
          <div class="colors-filter">
            <div class="left">
              <div class="chip active">全部 28</div>
              <div class="chip">主色</div>
              <div class="chip">辅色</div>
              <div class="chip">配色</div>
            </div>
            <div class="chip">
              <svg viewBox="0 0 24 24" width="12" height="12"><path fill="currentColor" d="M3 6h18v2H3zm3 5h12v2H6zm4 5h4v2h-4z"/></svg>
              颗数↓
            </div>
          </div>
          <div class="colors-table">
            <table>
              <thead><tr><th>#</th><th>色号</th><th>颜色</th><th style="text-align:right">颗数</th><th style="text-align:right">占比</th><th>位置</th></tr></thead>
              <tbody>
                <tr><td class="idx">01</td><td class="code">M01</td><td><span class="swatch" style="background:#FF7A5A"></span>珊瑚红</td><td class="beads">234</td><td class="pct">4.7%</td><td><div class="dots"><span class="b"></span><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">02</td><td class="code">M15</td><td><span class="swatch" style="background:#2A1F1A"></span>深棕黑</td><td class="beads">189</td><td class="pct">3.8%</td><td><div class="dots"><span class="b"></span><span class="b"></span><span class="b"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">03</td><td class="code">H08</td><td><span class="swatch" style="background:#F5C45E"></span>奶油黄</td><td class="beads">156</td><td class="pct">3.1%</td><td><div class="dots"><span class="b"></span><span class="b"></span><span class="b"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">04</td><td class="code">G05</td><td><span class="swatch" style="background:#6BC7A1"></span>薄荷绿</td><td class="beads">128</td><td class="pct">2.6%</td><td><div class="dots"><span class="b"></span><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">05</td><td class="code">B12</td><td><span class="swatch" style="background:#9DC8E5"></span>天空蓝</td><td class="beads">112</td><td class="pct">2.2%</td><td><div class="dots"><span class="b"></span><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">06</td><td class="code">V03</td><td><span class="swatch" style="background:#B49DD8"></span>淡紫</td><td class="beads">98</td><td class="pct">2.0%</td><td><div class="dots"><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">07</td><td class="code">N07</td><td><span class="swatch" style="background:#8B5A3C"></span>巧克力</td><td class="beads">76</td><td class="pct">1.5%</td><td><div class="dots"><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
                <tr><td class="idx">08</td><td class="code">P04</td><td><span class="swatch" style="background:#F2A6A6"></span>樱花粉</td><td class="beads">62</td><td class="pct">1.2%</td><td><div class="dots"><span class="b"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span><span class="b gray"></span></div></td></tr>
              </tbody>
            </table>
          </div>
        </div>
        <div style="position:absolute;left:0;right:0;bottom:0;background:rgba(255,255,255,.95);backdrop-filter:blur(10px);border-top:1px solid var(--line);padding:10px 16px 24px;display:flex;gap:10px">
          <button class="btn btn-secondary"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 3v12M6 9l6 6 6-6M5 21h14"/></svg> 导出</button>
          <button class="btn btn-primary"><svg viewBox="0 0 24 24" width="16" height="16"><path fill="#fff" d="M3 6h18l-2 12H5L3 6zm5 0V3h8v3"/></svg> 耗材推荐 · MARD 168 色 ¥89</button>
        </div>
      </div>`
  },

  {
    id:'difficulty', group:'M5 色号', name:'难度调整', route:'/create/colors/:id/adjust',
    desc:'M5 难度调整：极简 8-12 / 简单 13-20 / 标准 21-35 / 精细 36-60 / 极致 60-100。',
    elements:['5 档滑杆','当前色数 / 目标色数','预计减少 / 增加 色数','一键应用 / 取消'],
    design:['滑杆 5 段式视觉，激活段主色','对比展示调整前后的效果','色块减少采用智能合并算法'],
    html:`
      <div class="page" style="background:var(--bg)">
        <div class="appbar solid">
          <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div class="appbar-title">难度调整</div>
          <button class="btn-ghost">取消</button>
        </div>
        <div class="page-body">
          <div style="padding:18px 16px;background:#fff;margin-bottom:10px">
            <div style="font-size:13px;color:var(--ink-3);margin-bottom:4px">当前色数</div>
            <div style="display:flex;align-items:baseline;gap:8px">
              <div style="font-size:32px;font-weight:700">28</div>
              <div style="font-size:14px;color:var(--ink-3)">色 · 5,000 颗</div>
            </div>
          </div>
          <div style="padding:18px 16px;background:#fff;margin-bottom:10px">
            <div style="font-size:13px;font-weight:600;margin-bottom:14px">目标难度</div>
            <div style="display:flex;height:8px;border-radius:4px;overflow:hidden;margin-bottom:10px">
              <div style="flex:1;background:var(--primary)"></div>
              <div style="flex:1;background:var(--primary)"></div>
              <div style="flex:1;background:var(--bg-2)"></div>
              <div style="flex:1;background:var(--bg-2)"></div>
              <div style="flex:1;background:var(--bg-2)"></div>
            </div>
            <div style="display:flex;justify-content:space-between;font-size:10px">
              <div class="col" style="align-items:center;gap:2px"><span style="color:var(--primary-ink);font-weight:600">极简</span><span style="color:var(--ink-3)">8-12</span></div>
              <div class="col" style="align-items:center;gap:2px"><span style="color:var(--primary-ink);font-weight:600">简单</span><span style="color:var(--ink-3)">13-20</span></div>
              <div class="col" style="align-items:center;gap:2px"><span>标准</span><span style="color:var(--ink-3)">21-35</span></div>
              <div class="col" style="align-items:center;gap:2px"><span>精细</span><span style="color:var(--ink-3)">36-60</span></div>
              <div class="col" style="align-items:center;gap:2px"><span>极致</span><span style="color:var(--ink-3)">60-100</span></div>
            </div>
            <div style="margin-top:18px;padding:14px;background:var(--bg-2);border-radius:12px">
              <div style="font-size:13px;font-weight:600;margin-bottom:4px">预计调整</div>
              <div style="font-size:12px;color:var(--ink-2);line-height:1.6">当前 28 色 → <b>18 色</b><br/>减少 10 色 · 智能合并相近色<br/>预计用时从 10h → <b>7h</b></div>
            </div>
          </div>
          <div style="padding:18px">
            <button class="btn btn-primary full">应用调整</button>
            <div style="text-align:center;margin-top:10px;font-size:11px;color:var(--ink-3)">调整不会损失原图，可在历史版本回退</div>
          </div>
        </div>
      </div>`
  },

  {
    id:'forum', group:'M3 论坛', name:'论坛首页', route:'/forum',
    desc:'M3 信息流。关注 / 推荐 / 话题 三种流。支持双击点赞，长按保存，沉浸式滑动。',
    elements:['三 Tab 信息流','搜索 + 发布按钮','帖子卡片：作品 / 求图 / 教程 / 讨论','互动：点赞 / 评论 / 收藏 / 分享'],
    design:['Tab 切换有指示条 + 颜色变化','卡片用 1dp 浅色描边而非大阴影','图片区支持双击点赞 + 粒子动画'],
    html:`
      <div class="page home">
        <div class="appbar solid">
          <div class="left"><div style="font-size:17px;font-weight:700">社区</div></div>
          <div class="right">
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M11 4a7 7 0 1 1 0 14 7 7 0 0 1 0-14zm5 12l4 4"/></svg></button>
            <button class="icon-btn"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M12 4v16M4 12h16"/></svg></button>
          </div>
        </div>
        <div class="forum-tabs">
          <div class="forum-tab active">推荐</div>
          <div class="forum-tab">关注</div>
          <div class="forum-tab">话题</div>
        </div>
        <div class="page-body" style="background:var(--bg)">
          <div class="post-card">
            <div class="post-head">
              <div class="avatar"></div>
              <div><div class="name">小美</div><div class="time">2 小时前</div></div>
              <button class="follow">+ 关注</button>
            </div>
            <div class="body">
              <div class="pt">给闺蜜的生日礼物🎁</div>
              <div class="pd">50×50 板 / 28 色 / 拼了 8 小时终于完成！闺蜜看到哭了一小时 😭</div>
            </div>
            <div class="imgs"><div></div><div></div><div></div></div>
            <div class="tags">
              <span class="tag warn">#生日礼物</span>
              <span class="tag">#闺蜜</span>
              <span class="tag">#拼豆日常</span>
            </div>
            <div class="actions">
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg> 1.2k</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg> 89</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg> 234</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M18 16l-4-4 4-4M6 8l4 4-4 4"/></svg></span>
            </div>
          </div>
          <div class="post-card">
            <div class="post-head">
              <div class="avatar" style="background:linear-gradient(135deg,#9DC8E5,#6BC7A1)"></div>
              <div><div class="name">Lily 老师</div><div class="time">5 小时前 · 认证创作者</div></div>
              <button class="follow" style="background:transparent;color:var(--ink-3)">已关注</button>
            </div>
            <div class="body">
              <div class="pt">【教程】0 基础必看！拼豆入门 8 问 8 答</div>
              <div class="pd">从工具选购到第一颗拼豆，手把手带你避坑。视频教程在第 3 楼 ⬇️</div>
            </div>
            <div class="imgs" style="grid-template-columns:1fr">
              <div style="aspect-ratio:1.6/1;background:linear-gradient(135deg,#FFE2D3,#F5C45E);position:relative">
                <div style="position:absolute;left:14px;bottom:14px;background:rgba(0,0,0,.5);color:#fff;padding:4px 10px;border-radius:999px;font-size:11px;backdrop-filter:blur(4px)">📹 8:23</div>
                <div style="position:absolute;inset:0;display:grid;place-items:center"><div style="width:54px;height:54px;border-radius:50%;background:rgba(255,255,255,.9);display:grid;place-items:center"><svg viewBox="0 0 24 24" width="22" height="22"><path fill="#FF7A5A" d="M8 5l12 7-12 7V5z"/></svg></div></div>
              </div>
            </div>
            <div class="tags">
              <span class="tag ok">#教程</span>
              <span class="tag">#新手入门</span>
            </div>
            <div class="actions">
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg> 567</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg> 23</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg> 178</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M18 16l-4-4 4-4M6 8l4 4-4 4"/></svg></span>
            </div>
          </div>
          <div class="post-card">
            <div class="post-head">
              <div class="avatar" style="background:linear-gradient(135deg,#B49DD8,#F2A6A6)"></div>
              <div><div class="name">求图达人</div><div class="time">昨天</div></div>
            </div>
            <div class="body">
              <div class="pt">求一张宫崎骏《千与千寻》千寻的拼豆图纸 🙏</div>
              <div class="pd">想拼给女朋友当周年礼物，最好 50×50 进阶难度，能有大大分享吗？</div>
            </div>
            <div class="imgs" style="grid-template-columns:1fr">
              <div style="aspect-ratio:1.4/1;background:linear-gradient(135deg,#9DC8E5,#B49DD8);position:relative">
                <div style="position:absolute;left:14px;bottom:14px;background:rgba(0,0,0,.5);color:#fff;padding:4px 10px;border-radius:999px;font-size:11px;backdrop-filter:blur(4px)">参考图</div>
              </div>
            </div>
            <div class="tags">
              <span class="tag pink">#求图</span>
              <span class="tag">#千与千寻</span>
            </div>
            <div class="actions">
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg> 12</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg> 8</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg> 0</span>
              <span><svg viewBox="0 0 24 24"><path fill="currentColor" d="M18 16l-4-4 4-4M6 8l4 4-4 4"/></svg></span>
            </div>
          </div>
        </div>
        <div class="tabbar">
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 3l9 8h-3v9h-4v-6H10v6H6v-9H3l9-8z"/></svg><div>首页</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M3 5h18v2H3zm0 6h18v2H3zm0 6h12v2H3z"/></svg><div>模板</div></div>
          <div class="tab-fab"><svg viewBox="0 0 24 24"><path fill="none" stroke="#fff" stroke-width="2.5" d="M12 5v14M5 12h14"/></svg></div>
          <div class="tab active"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg><div>社区</div></div>
          <div class="tab"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-9 1.5-9 4.5V21h18v-2.5c0-3-6-4.5-9-4.5z"/></svg><div>我的</div></div>
        </div>
      </div>`
  },

  {
    id:'post-detail', group:'M3 论坛', name:'帖子详情', route:'/post/:id',
    desc:'帖子详情：作品展示、文字描述、拼豆参数、评论互动。',
    elements:['顶部大图 cover','作者信息卡','作品参数徽章','图集 + 描述 + 标签','底部评论输入栏 + 互动栏'],
    design:['图片底部淡入白色，使文字浮在主图之上','长按图片可保存到相册（带 Toast 提示）','双击图片=点赞+粒子动画'],
    html:`
      <div class="page post-detail">
        <div class="appbar" style="position:absolute;top:0;left:0;right:0;z-index:5;color:#fff">
          <button class="icon-btn" style="color:#fff;background:rgba(0,0,0,.3);backdrop-filter:blur(6px)"><svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M15 6l-6 6 6 6"/></svg></button>
          <div></div>
          <button class="icon-btn" style="color:#fff;background:rgba(0,0,0,.3);backdrop-filter:blur(6px)"><svg viewBox="0 0 24 24" width="20" height="20"><circle cx="5" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="19" cy="12" r="2" fill="currentColor"/></svg></button>
        </div>
        <div class="cover">
          <div style="position:absolute;left:18px;bottom:36px;color:#fff;z-index:2">
            <div style="font-size:11px;opacity:.85;margin-bottom:4px">🎁 给闺蜜的礼物</div>
            <div style="font-size:22px;font-weight:700;line-height:1.3">50×50 板<br/>28 色 / 拼了 8 小时</div>
          </div>
        </div>
        <div class="content">
          <div class="author">
            <div class="avatar"></div>
            <div class="info">
              <div class="name">小美 ✨</div>
              <div class="bio">拼豆玩家 · 作品 23 · 粉丝 89</div>
            </div>
            <button class="btn btn-primary btn-sm">+ 关注</button>
          </div>
          <div class="text">闺蜜下个月生日🎂 她超喜欢我们家小咪，于是决定把小咪拼成拼豆送给她！虽然第 5 行拼错了一次又拆了重来，但最后还是完成啦～ 看到成品的那一刻眼泪都流出来了 😭</div>
          <div class="gallery"><div></div><div></div><div></div></div>
          <div class="tags">
            <span class="tag warn">#生日礼物</span>
            <span class="tag">#闺蜜</span>
            <span class="tag">#猫咪</span>
            <span class="tag">#拼豆日常</span>
          </div>
          <div style="margin-top:18px">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:12px">
              <div style="font-size:14px;font-weight:600">热门评论 89</div>
              <div style="font-size:11px;color:var(--ink-3)">按热度 ▾</div>
            </div>
            <div style="display:flex;gap:10px;margin-bottom:14px">
              <div class="avatar" style="width:32px;height:32px;background:linear-gradient(135deg,#9DC8E5,#6BC7A1);border-radius:50%"></div>
              <div style="flex:1">
                <div style="font-size:12px;font-weight:600">Lily 老师 <span style="color:var(--ink-3);font-weight:400;margin-left:6px">认证创作者</span></div>
                <div style="font-size:13px;color:var(--ink-2);line-height:1.6;margin-top:2px">太厉害啦！第 5 行能看出是米色拼错了色，色号是 H08 哦～</div>
                <div style="font-size:11px;color:var(--ink-3);margin-top:4px">2 小时前 · ♡ 23</div>
              </div>
            </div>
            <div style="display:flex;gap:10px">
              <div class="avatar" style="width:32px;height:32px;background:linear-gradient(135deg,#B49DD8,#F2A6A6);border-radius:50%"></div>
              <div style="flex:1">
                <div style="font-size:12px;font-weight:600">求图达人</div>
                <div style="font-size:13px;color:var(--ink-2);line-height:1.6;margin-top:2px">求这个图纸！可以分享吗？</div>
                <div style="font-size:11px;color:var(--ink-3);margin-top:4px">1 小时前 · ♡ 5</div>
              </div>
            </div>
          </div>
        </div>
        <div class="bottom-bar">
          <div class="inp">说点什么…</div>
          <div class="icon-row">
            <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M12 21s-7-4.5-9-9.5C1.5 7 4 4 7 4c2 0 3.5 1 5 3 1.5-2 3-3 5-3 3 0 5.5 3 4 7.5-2 5-9 9.5-9 9.5z"/></svg>
            <svg viewBox="0 0 24 24" width="20" height="20"><path fill="none" stroke="currentColor" stroke-width="2" d="M21 6h-2v9H6v2c0 .5.5 1 1 1h11l4 4V7c0-.5-.5-1-1-1z"/></svg>
            <svg viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M12 2l3 6 6 1-4.5 4 1 6L12 16l-5.5 3 1-6L3 9l6-1 3-6z"/></svg>
          </div>
        </div>
      </div>`
  }
];
