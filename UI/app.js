// 拼豆 · 移动端原型 · 应用逻辑
(function(){
  // 合并所有屏幕数据
  const ALL = [].concat(
    (typeof SCREENS !== 'undefined' ? SCREENS : []),
    (typeof SCREENS_2 !== 'undefined' ? SCREENS_2 : []),
    (typeof SCREENS_3 !== 'undefined' ? SCREENS_3 : []),
    (typeof SCREENS_4 !== 'undefined' ? SCREENS_4 : [])
  );
  const APP_SCREENS = ALL;
  const GROUPS = {};
  APP_SCREENS.forEach(s => {
    if(!GROUPS[s.group]) GROUPS[s.group] = [];
    GROUPS[s.group].push(s);
  });

  // 渲染导航
  const navList = document.getElementById('navList');
  Object.keys(GROUPS).forEach(g => {
    const grp = document.createElement('div');
    grp.className = 'nav-group';
    grp.innerHTML = `<div class="nav-group-title">${g}</div>`;
    const ul = document.createElement('div');
    GROUPS[g].forEach(s => {
      const item = document.createElement('div');
      item.className = 'nav-item';
      item.dataset.id = s.id;
      item.innerHTML = `<span>${s.name}</span><span class="ni-route">${s.route}</span>`;
      item.addEventListener('click', () => goTo(s.id));
      ul.appendChild(item);
    });
    grp.appendChild(ul);
    navList.appendChild(grp);
  });

  // 当前屏幕
  let currentIndex = 0;
  const screen = document.getElementById('screen');
  const stageName = document.getElementById('stageName');
  const stageRoute = document.getElementById('stageRoute');
  const stageIndex = document.getElementById('stageIndex');
  const notesTitle = document.getElementById('notesTitle');
  const notesDesc = document.getElementById('notesDesc');
  const notesElements = document.getElementById('notesElements');
  const notesDesign = document.getElementById('notesDesign');

  function render(i){
    const s = APP_SCREENS[i];
    if(!s) return;
    currentIndex = i;
    // 渲染内容
    screen.innerHTML = s.html;
    // 标题
    stageName.textContent = s.name;
    stageRoute.textContent = s.route;
    stageIndex.textContent = `${i+1} / ${APP_SCREENS.length}`;
    // 说明
    notesTitle.textContent = s.name;
    notesDesc.textContent = s.desc;
    notesElements.innerHTML = (s.elements||[]).map(e=>`<li>${e}</li>`).join('');
    notesDesign.innerHTML = (s.design||[]).map(e=>`<li>${e}</li>`).join('');
    // 导航高亮
    document.querySelectorAll('.nav-item').forEach(el=>{
      el.classList.toggle('active', el.dataset.id === s.id);
    });
    // 滚动到可视
    const active = document.querySelector('.nav-item.active');
    if(active) active.scrollIntoView({block:'nearest',behavior:'smooth'});
  }

  function goTo(id){
    const i = SCREENS.findIndex(s => s.id === id);
    if(i >= 0) render(i);
  }

  // 上下页
  document.getElementById('prevScreen').addEventListener('click', () => {
    render((currentIndex - 1 + APP_SCREENS.length) % APP_SCREENS.length);
  });
  document.getElementById('nextScreen').addEventListener('click', () => {
    render((currentIndex + 1) % APP_SCREENS.length);
  });

  // 键盘
  document.addEventListener('keydown', e => {
    if(e.key === 'ArrowLeft') render((currentIndex - 1 + APP_SCREENS.length) % APP_SCREENS.length);
    if(e.key === 'ArrowRight') render((currentIndex + 1) % APP_SCREENS.length);
  });

  // 搜索
  const navSearch = document.getElementById('navSearch');
  navSearch.addEventListener('input', e => {
    const q = e.target.value.toLowerCase().trim();
    document.querySelectorAll('.nav-item').forEach(el => {
      const txt = el.textContent.toLowerCase();
      el.style.display = (q === '' || txt.includes(q)) ? '' : 'none';
    });
    document.querySelectorAll('.nav-group').forEach(grp => {
      const visible = Array.from(grp.querySelectorAll('.nav-item')).some(el => el.style.display !== 'none');
      grp.style.display = visible ? '' : 'none';
    });
  });

  // 启动
  render(0);
})();
