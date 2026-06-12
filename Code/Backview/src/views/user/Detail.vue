<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import StatusTag from '@/components/StatusTag.vue'
import { getUser, disableUser, resetPassword, muteUser, type UserListItem } from '@/api/user'

const route = useRoute()
const router = useRouter()
const user = ref<UserListItem | null>(null)

const sideItems = [
  { label: '基本信息', anchor: '#basic' },
  { label: '会员信息', anchor: '#member' },
  { label: '数据统计', anchor: '#stats' },
  { label: '发布内容', anchor: '#posts' },
  { label: '订单记录', anchor: '#orders' },
  { label: '操作日志', anchor: '#logs' },
  { label: '登录设备', anchor: '#devices' }
]

onMounted(async () => {
  user.value = await getUser(route.params.id as string)
})

async function onResetPwd() {
  try {
    await ElMessageBox.confirm('确认重置该用户密码？新密码将生成后展示。', '重置密码', { type: 'warning' })
    const res: any = await resetPassword(user.value!.id)
    await ElMessageBox.alert(`新密码：${res?.newPassword || '(见响应)'}\n请通过站内信发送给用户`, '密码已重置', { type: 'success' })
  } catch {}
}

async function onMute() {
  try {
    const { value } = await ElMessageBox.prompt('禁言天数（1-30）', '禁言', { inputValue: '3' })
    await muteUser(user.value!.id, { days: parseInt(value), reason: '运营禁言' })
    ElMessage.success('已禁言')
  } catch {}
}

async function onDisable() {
  try {
    await ElMessageBox.confirm('确定禁用该账号？禁用后用户无法登录', '禁用账号', { type: 'warning' })
    await disableUser(user.value!.id, '违规')
    ElMessage.success('已禁用')
  } catch {}
}

const recentPosts = [
  { time: '06-07 09:30', title: '帖子「给闺蜜的生日礼物」', status: '已通过', desc: '9 张图 · 1,234 互动', tagVariant: 'ok' as const },
  { time: '06-05 18:21', title: '教程「圣诞老人入门教程」', status: '已通过', desc: '关联图纸 d_xxxxx', tagVariant: 'ok' as const },
  { time: '06-03 14:08', title: 'AI 生成「樱花树」', status: '未发布', desc: '50×50 · 32 色', tagVariant: 'neutral' as const },
  { time: '06-01 22:14', title: '评论于「星空物语」帖子', status: '被举报 1 次', desc: '已处理', tagVariant: 'warn' as const }
]

const orders = [
  { time: '05-24', title: 'VIP1 月卡', desc: '¥ 18 · 微信支付' },
  { time: '04-12', title: 'VIP1 月卡', desc: '¥ 18 · 微信支付' },
  { time: '01-01', title: 'VIP1 年卡', desc: '¥ 168 · 支付宝' }
]

const devices = [
  { name: 'iPhone 14 Pro', desc: 'iOS 17.4 · 当前' },
  { name: 'iPad Air', desc: 'iPadOS 17 · 7 天前' },
  { name: 'MacBook Pro', desc: 'macOS 14 · 30 天前' }
]
</script>

<template>
  <div class="page-view">
    <div class="app-with-aside">
      <aside class="aside">
        <div class="aside-title">返回</div>
        <div class="aside-item" @click="router.back()">‹ 用户列表</div>
        <div class="aside-title" style="margin-top: 14px">用户档案</div>
        <div v-for="(s, i) in sideItems" :key="s.anchor" class="aside-item" :class="{ active: i === 0 }">{{ s.label }}</div>
        <div class="aside-title" style="margin-top: 14px">快捷操作</div>
        <div class="aside-item">发送站内信</div>
        <div class="aside-item">调整会员</div>
        <div class="aside-item">手动赠送</div>
      </aside>

      <div class="main" v-if="user">
        <PageHead
          :crumbs="[{ label: '用户管理', to: '/user/list' }, { label: '用户列表', to: '/user/list' }, { label: user.id }]"
          :title="user.nickname"
          :sub="`${user.id} · 注册于 ${user.register_time}`"
        >
          <template #actions>
            <button class="btn btn-secondary">发送站内信</button>
            <button class="btn btn-secondary" @click="onResetPwd">重置密码</button>
            <button class="btn btn-secondary" @click="onMute">禁言</button>
            <button class="btn btn-danger" @click="onDisable">禁用账号</button>
          </template>
        </PageHead>

        <div class="detail">
          <div class="col-main">
            <div class="panel" id="basic">
              <div class="panel-head">
                <div class="ph-title">基本信息</div>
                <button class="btn btn-xs btn-ghost">编辑</button>
              </div>
              <div class="panel-body">
                <div class="form-row">
                  <div class="lbl">头像 / 昵称</div>
                  <div class="row" style="gap: 10px">
                    <div class="av lg" :class="'c' + ((parseInt(user.id.slice(2)) % 6) + 1)">{{ user.nickname[0] }}</div>
                    <div>
                      <div style="font-weight: 600">{{ user.nickname }}</div>
                      <div class="muted small">ID {{ user.id }} · {{ user.city || '—' }} · {{ { male: '男', female: '女', unknown: '未设置' }[user.gender || 'unknown'] }}</div>
                    </div>
                  </div>
                </div>
                <div class="form-row"><div class="lbl">手机号</div><div class="mono">{{ user.phone || '—' }}</div></div>
                <div class="form-row">
                  <div class="lbl">注册方式</div>
                  <div>
                    <StatusTag :variant="user.register_method === 'wechat' ? 'info' : user.register_method === 'apple' ? 'purple' : 'neutral'">
                      {{ ({ wechat: '微信', phone: '手机号', apple: 'Apple', guest: '游客' } as any)[user.register_method || ''] || '—' }}
                    </StatusTag>
                    <span class="muted small" v-if="user.register_method === 'wechat'"> · 微信昵称「{{ user.nickname }}」</span>
                  </div>
                </div>
                <div class="form-row"><div class="lbl">注册时间</div><div>{{ user.register_time || user.create_time }} <span v-if="user.register_time" class="muted small">已使用 {{ Math.ceil((Date.now() - new Date(user.register_time).getTime()) / 86400000) }} 天</span></div></div>
                <div class="form-row"><div class="lbl">最后登录</div><div>{{ user.last_login_time }} · IP 123.125.*.* · iPhone 14 Pro</div></div>
                <div class="form-row">
                  <div class="lbl">账号状态</div>
                  <div>
                    <StatusTag v-if="user.status === 'normal'" variant="ok">正常</StatusTag>
                    <StatusTag v-else-if="user.status === 'muted'" variant="warn">禁言</StatusTag>
                    <StatusTag v-else variant="danger">已禁用</StatusTag>
                    <span class="muted small">· 无未处理举报</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="panel" id="member">
              <div class="panel-head">
                <div class="ph-title">会员信息</div>
                <button class="btn btn-xs btn-ghost">调整</button>
              </div>
              <div class="panel-body">
                <div class="form-row">
                  <div class="lbl">会员状态</div>
                  <div>
                    <StatusTag variant="primary">有效</StatusTag>
                    <span v-if="user.is_member"> · {{ user.member_level }} · {{ user.member_expire_time }} 到期</span>
                    <span v-else class="muted"> · 非会员</span>
                  </div>
                </div>
                <div class="form-row"><div class="lbl">累计付费</div><div>¥ 168.00</div></div>
                <div class="form-row"><div class="lbl">开通渠道</div><div>App Store · 微信支付 · 支付宝</div></div>
                <div class="form-row">
                  <div class="lbl">自动续费</div>
                  <div><span class="switch on"></span> <span class="muted small">将于 2026-12-24 自动续费</span></div>
                </div>
                <div class="form-row">
                  <div class="lbl">本期权益使用</div>
                  <div>
                    <div class="row" style="gap: 10px">
                      <div style="flex: 1"><div class="muted small">AI 生成</div><div class="progress"><div class="fill" style="width: 34%"></div></div></div>
                      <div style="flex: 1"><div class="muted small">高级模板</div><div class="progress"><div class="fill mint" style="width: 62%"></div></div></div>
                      <div style="flex: 1"><div class="muted small">云存储</div><div class="progress"><div class="fill warn" style="width: 88%"></div></div></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="panel" id="posts">
              <div class="panel-head">
                <div class="ph-title">最近发布</div>
                <a class="btn btn-xs btn-ghost">查看全部 {{ user.post_count }}</a>
              </div>
              <div class="panel-body">
                <div class="timeline">
                  <div v-for="p in recentPosts" :key="p.time" class="tl-item">
                    <div class="tl-time">{{ p.time }}</div>
                    <div class="tl-body">
                      <div class="what">{{ p.title }}</div>
                      <div class="who"><StatusTag :variant="p.tagVariant">{{ p.status }}</StatusTag> · {{ p.desc }}</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="panel" id="stats">
              <div class="panel-head"><div class="ph-title">数据统计</div></div>
              <div class="panel-body">
                <div class="metric-strip">
                  <div class="ms"><div class="ml">作品数</div><div class="mv">{{ user.post_count }}</div></div>
                  <div class="ms"><div class="ml">教程数</div><div class="mv">5</div></div>
                  <div class="ms"><div class="ml">粉丝数</div><div class="mv">{{ user.follower_count }}</div></div>
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
                <div class="form-row"><div class="lbl">信用分</div><div><b style="color: var(--mint); font-size: 18px">92</b> <span class="muted small">/ 100</span></div></div>
                <div class="form-row"><div class="lbl">违规记录</div><div>0 次</div></div>
                <div class="form-row"><div class="lbl">被举报</div><div>1 次 · 已处理</div></div>
                <div class="form-row"><div class="lbl">敏感词命中</div><div>0 次</div></div>
              </div>
            </div>

            <div class="panel" id="orders">
              <div class="panel-head"><div class="ph-title">订单记录</div></div>
              <div class="panel-body">
                <div class="timeline">
                  <div v-for="o in orders" :key="o.time" class="tl-item">
                    <div class="tl-time">{{ o.time }}</div>
                    <div class="tl-body">
                      <div class="what">{{ o.title }}</div>
                      <div class="who muted small">{{ o.desc }}</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="panel" id="devices">
              <div class="panel-head"><div class="ph-title">登录设备</div></div>
              <div class="panel-body">
                <div v-for="d in devices" :key="d.name" class="form-row">
                  <div class="lbl">{{ d.name }}</div>
                  <div class="muted small">{{ d.desc }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.app-with-aside { display: grid; grid-template-columns: var(--sub-aside-w) 1fr; height: 100%; background: var(--bg-2); flex: 1; overflow: hidden; }
.aside { background: var(--surface); border-right: 1px solid var(--line); padding: 14px 10px; display: flex; flex-direction: column; gap: 2px; overflow-y: auto; }
.aside-title { font-size: 10px; font-weight: 700; color: var(--ink-3); letter-spacing: 1.2px; text-transform: uppercase; padding: 6px 10px 4px; }
.aside-item { display: flex; align-items: center; justify-content: space-between; gap: 6px; padding: 7px 10px; border-radius: 6px; font-size: 12.5px; color: var(--ink-2); cursor: pointer; transition: background .12s, color .12s; }
.aside-item:hover { background: var(--bg); }
.aside-item.active { background: var(--ink); color: #fff; font-weight: 600; }
.aside-item .badge { font-size: 10px; background: var(--primary-soft); color: var(--primary-ink); padding: 1px 6px; border-radius: 99px; font-weight: 600; }
.aside-item.active .badge { background: rgba(255,255,255,.2); color: #fff; }
.main { display: flex; flex-direction: column; overflow: hidden; min-width: 0; }
.detail { display: grid; grid-template-columns: 1fr 320px; gap: 14px; padding: 16px 22px; flex: 1; overflow: auto; background: var(--bg-2); }
.col-main, .col-side { display: flex; flex-direction: column; gap: 14px; min-width: 0; }
.metric-strip { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }
.metric-strip .ms { background: var(--surface); border: 1px solid var(--line); border-radius: 8px; padding: 10px 12px; }
.metric-strip .ml { font-size: 11px; color: var(--ink-3); font-weight: 500; }
.metric-strip .mv { font-size: 16px; font-weight: 700; margin-top: 2px; }
.timeline { display: flex; flex-direction: column; gap: 0; }
.tl-item { display: grid; grid-template-columns: 90px 1fr; gap: 14px; padding: 10px 0; border-bottom: 1px solid var(--line); font-size: 12.5px; }
.tl-item:last-child { border-bottom: none; }
.tl-time { color: var(--ink-3); font-size: 11.5px; }
.tl-body .what { font-weight: 600; color: var(--ink); margin-bottom: 3px; }
.tl-body .who { font-size: 11.5px; color: var(--ink-3); display: flex; align-items: center; gap: 6px; }
</style>
