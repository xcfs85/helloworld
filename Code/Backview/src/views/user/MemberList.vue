<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listMembers, openMember, getMemberLevelStats, getMemberStats } from '@/api/user'
import type { Member } from '@/types'

const loading = ref(false)
const list = ref<Member[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)

// 侧边栏统计（从接口获取）
const sideLevels = ref([
  { label: '全部', value: 'all', count: 0 },
  { label: 'VIP1', value: 'VIP1', count: 0 },
  { label: 'VIP2', value: 'VIP2', count: 0 },
  { label: 'VIP3', value: 'VIP3', count: 0 },
  { label: 'SVIP', value: 'SVIP', count: 0 }
])

// 加载会员统计
async function loadStats() {
  try {
    // 专门统计接口：拉取各等级会员数量
    const levelRes: any = await getMemberLevelStats()
    const levelData = levelRes.data || levelRes
    const levelMap: Record<string, number> = {}
    if (levelData.level_counts) {
      for (const item of levelData.level_counts) {
        levelMap[item.level] = item.count
      }
    }
    sideLevels.value[0].count = levelData.total || 0
    sideLevels.value[1].count = levelMap['VIP1'] || 0
    sideLevels.value[2].count = levelMap['VIP2'] || 0
    sideLevels.value[3].count = levelMap['VIP3'] || 0
    sideLevels.value[4].count = levelMap['SVIP'] || 0

    // 综合统计：拉取到期状态相关数量
    const res: any = await getMemberStats()
    const stats = res.data || res
    expiringSoonCount.value = stats.expiring_soon_count || 0
    expiring30dCount.value = stats.expiring_30d_count || 0
    longTermCount.value = stats.long_term_count || 0
    expiredCount.value = stats.expired_count || 0

    // 支付渠道统计
    if (stats.channel_counts) {
      for (const item of stats.channel_counts) {
        if (item.channel in channelCounts) {
          channelCounts[item.channel as keyof typeof channelCounts] = item.count
        }
      }
    }
  } catch (error) {
    console.error('加载会员统计失败:', error)
  }
}

// 到期状态各分类数量
const expiringSoonCount = ref(0)
const expiring30dCount = ref(0)
const longTermCount = ref(0)
const expiredCount = ref(0)

// 支付方式各分类数量
const channelCounts = reactive<Record<string, number>>({ wechat: 0, alipay: 0, appstore: 0, backend: 0 })

const filter = reactive({ keyword: '', level: 'all', expire: 'all', pay_channel: 'all' })

// 将后端 UserListDto 转换为前端 Member 类型
function transformToMember(item: any): Member {
  return {
    id: item.id || '',
    user_id: item.id || '',
    user_nickname: item.nickname || '',
    user_avatar: item.avatar,
    level: (item.member_level as Member['level']) || 'VIP1',
    expire_time: item.member_expire_time ? formatDate(item.member_expire_time) : '',
    auto_renew: item.auto_renew || false,
    total_paid: item.total_paid || 0,
    pay_channel: (item.pay_channel as Member['pay_channel']) || 'backend',
    create_time: item.create_time ? formatDate(item.create_time) : '',
    // 兼容字段
    nickname: item.nickname,
    is_member: item.is_member,
    member_expire_time: item.member_expire_time ? formatDate(item.member_expire_time) : '',
    member_level: item.member_level
  }
}

function formatDate(date: string | Date): string {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

async function load() {
  loading.value = true
  try {
    const res: any = await listMembers({ ...filter, page: page.value, page_size: pageSize.value })
    // 转换后端数据为前端格式
    list.value = (res.list || []).map(transformToMember)
    total.value = res.total || 0
  } catch (error) {
    console.error('加载会员列表失败:', error)
  } finally {
    loading.value = false
  }
}
function search() { page.value = 1; load() }
function reset() { filter.keyword = ''; filter.level = 'all'; filter.expire = 'all'; filter.pay_channel = 'all'; search() }

function remainDays(expire: string) {
  if (!expire) return 0
  return Math.ceil((new Date(expire).getTime() - Date.now()) / 86400000)
}

// 会员等级映射
const levelMap: Record<string, string> = {
  'VIP1': 'VIP1', 'VIP2': 'VIP2', 'VIP3': 'VIP3', 'SVIP': 'SVIP'
}

async function onOpen() {
  ElMessage.success('手动开通会员功能开发中')
}

onMounted(() => { load(); loadStats() })
</script>

<template>
  <div class="page-view">
    <div class="app-with-aside">
      <aside class="aside">
        <div class="aside-title">会员等级</div>
        <div v-for="l in sideLevels" :key="l.value" class="aside-item" :class="{ active: filter.level === l.value }" @click="filter.level = l.value; search()">
          {{ l.label }}
          <span class="badge">{{ l.count }}</span>
        </div>
        <div class="aside-title" style="margin-top: 14px">到期状态</div>
        <div class="aside-item" :class="{ active: filter.expire === 'all' }" @click="filter.expire = 'all'; search()">全部</div>
        <div class="aside-item" :class="{ active: filter.expire === 'expired' }" @click="filter.expire = 'expired'; search()">已过期 <span class="badge" style="background: var(--danger-soft, #ffe5e5); color: var(--danger, #d33)">{{ expiredCount }}</span></div>
        <div class="aside-item" :class="{ active: filter.expire === '7d' }" @click="filter.expire = '7d'; search()">7 天内到期 <span class="badge" style="background: var(--warn-soft); color: var(--warn)">{{ expiringSoonCount }}</span></div>
        <div class="aside-item" :class="{ active: filter.expire === '30d' }" @click="filter.expire = '30d'; search()">30 天内到期 <span class="badge">{{ expiring30dCount }}</span></div>
        <div class="aside-item" :class="{ active: filter.expire === 'long' }" @click="filter.expire = 'long'; search()">长期有效 <span class="badge">{{ longTermCount }}</span></div>
        <div class="aside-title" style="margin-top: 14px">支付方式</div>
        <div class="aside-item" :class="{ active: filter.pay_channel === 'all' }" @click="filter.pay_channel = 'all'; search()">全部</div>
        <div class="aside-item" :class="{ active: filter.pay_channel === 'wechat' }" @click="filter.pay_channel = 'wechat'; search()">微信支付 <span class="badge">{{ channelCounts.wechat }}</span></div>
        <div class="aside-item" :class="{ active: filter.pay_channel === 'alipay' }" @click="filter.pay_channel = 'alipay'; search()">支付宝 <span class="badge">{{ channelCounts.alipay }}</span></div>
        <div class="aside-item" :class="{ active: filter.pay_channel === 'appstore' }" @click="filter.pay_channel = 'appstore'; search()">App Store <span class="badge">{{ channelCounts.appstore }}</span></div>
        <div class="aside-item" :class="{ active: filter.pay_channel === 'backend' }" @click="filter.pay_channel = 'backend'; search()">后台开通 <span class="badge">{{ channelCounts.backend }}</span></div>
      </aside>

      <div class="main">
        <PageHead
          :crumbs="[{ label: '用户管理' }, { label: '会员管理' }]"
          title="会员列表"
          sub="共 312 个会员 · 月收入 ¥ 12,567"
        >
          <template #actions>
            <button class="btn btn-secondary">会员权益配置</button>
            <button class="btn btn-primary" @click="onOpen">+ 手动开通</button>
          </template>
        </PageHead>

        <div class="toolbar">
          <div class="search-input">
            <el-icon><Search /></el-icon>
            <input v-model="filter.keyword" placeholder="用户 ID / 昵称 / 手机号" @keydown.enter="search" />
          </div>
          <div class="f-select"><select v-model="filter.level" @change="search">
            <option value="all">等级：全部</option><option>VIP1</option><option>VIP2</option><option>VIP3</option><option>SVIP</option>
          </select></div>
          <div class="f-select"><select v-model="filter.expire" @change="search">
            <option value="all">到期：全部</option><option value="7d">7 天内</option><option value="30d">30 天内</option><option value="expired">已过期</option>
          </select></div>
          <div class="f-select"><select v-model="filter.pay_channel" @change="search">
            <option value="all">支付：全部</option><option value="wechat">微信</option><option value="alipay">支付宝</option><option value="appstore">App Store</option>
          </select></div>
          <button class="btn btn-sm btn-secondary" @click="reset">重置</button>
          <button class="btn btn-sm btn-primary" @click="search">搜索</button>
          <div class="f-spacer"></div>
          <button class="btn btn-sm btn-secondary">导出</button>
        </div>

        <div class="tbl-wrap">
          <table class="tbl">
            <thead>
              <tr>
                <th style="width: 32px"><span class="ck"></span></th>
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
              <tr v-for="m in list" :key="m.id">
                <td><span class="ck"></span></td>
                <td>
                  <div class="user-cell">
                    <div class="av" :class="'c' + ((parseInt(m.user_id.slice(2)) % 6) + 1)">{{ m.user_nickname[0] }}</div>
                    <div class="meta"><div class="nm">{{ m.user_nickname }}</div><div class="id">{{ m.user_id }}</div></div>
                  </div>
                </td>
                <td>
                  <StatusTag v-if="m.level === 'SVIP'" variant="primary" style="background: var(--accent); color: var(--ink)">SVIP</StatusTag>
                  <StatusTag v-else variant="primary">{{ m.level }}</StatusTag>
                </td>
                <td>{{ m.expire_time }}</td>
                <td>
                  <span v-if="remainDays(m.expire_time) <= 7" class="tag danger">{{ remainDays(m.expire_time) }} 天</span>
                  <span v-else-if="remainDays(m.expire_time) <= 30" class="tag warn">{{ remainDays(m.expire_time) }} 天</span>
                  <span v-else class="muted">{{ remainDays(m.expire_time) }} 天</span>
                </td>
                <td><span class="switch" :class="{ on: m.auto_renew }"></span></td>
                <td>¥ {{ m.total_paid }}</td>
                <td>{{ { wechat: '微信支付', alipay: '支付宝', appstore: 'App Store', backend: '后台开通' }[m.pay_channel] }}</td>
                <td class="col-actions">
                  <button class="btn btn-xs btn-ghost">续费记录</button>
                  <button class="btn btn-xs btn-ghost">调整</button>
                  <button v-if="remainDays(m.expire_time) <= 14" class="btn btn-xs btn-warn">提醒续费</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <Pager :total="total" :page="page" :page-size="pageSize"
          @update:page="(v) => { page = v; load() }"
          @update:page-size="(v) => { pageSize = v; load() }" />
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
.tbl-wrap { flex: 1; overflow: auto; }
</style>
