<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import Pager from '@/components/Pager.vue'
import StatusTag from '@/components/StatusTag.vue'
import { listMembers, openMember } from '@/api/user'
import type { Member } from '@/types'

const loading = ref(false)
const list = ref<Member[]>([])
const total = ref(0)
const page = ref(1)
const pageSize = ref(20)

const filter = reactive({ keyword: '', level: 'all', expire: 'all', pay_channel: 'all' })

const sideLevels = [
  { label: '全部', value: 'all', count: 312 },
  { label: 'VIP1', value: 'VIP1', count: 182 },
  { label: 'VIP2', value: 'VIP2', count: 86 },
  { label: 'VIP3', value: 'VIP3', count: 38 },
  { label: 'SVIP', value: 'SVIP', count: 6 }
]

async function load() {
  loading.value = true
  try {
    const res: any = await listMembers({ ...filter, page: page.value, page_size: pageSize.value })
    list.value = res.list
    total.value = res.total
  } finally { loading.value = false }
}
function search() { page.value = 1; load() }
function reset() { filter.keyword = ''; filter.level = 'all'; filter.expire = 'all'; filter.pay_channel = 'all'; search() }

function remainDays(expire: string) {
  return Math.ceil((new Date(expire).getTime() - Date.now()) / 86400000)
}

async function onOpen() {
  ElMessage.success('手动开通会员已记录')
}

onMounted(load)
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
        <div class="aside-item" @click="filter.expire = 'expired'; search()">已过期</div>
        <div class="aside-item" @click="filter.expire = '7d'; search()">7 天内到期 <span class="badge" style="background: var(--warn-soft); color: var(--warn)">28</span></div>
        <div class="aside-item" @click="filter.expire = '30d'; search()">30 天内到期</div>
        <div class="aside-item" @click="filter.expire = 'long'; search()">长期有效</div>
        <div class="aside-title" style="margin-top: 14px">支付方式</div>
        <div class="aside-item">微信支付</div>
        <div class="aside-item">支付宝</div>
        <div class="aside-item">App Store</div>
        <div class="aside-item">后台开通</div>
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
