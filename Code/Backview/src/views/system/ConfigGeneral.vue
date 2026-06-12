<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import PageHead from '@/components/PageHead.vue'
import { getGeneralConfig, batchSetConfig, listKits, type ConfigItem, type SetConfigPayload } from '@/api/system'

const activeTab = ref('general')
const loading = ref(false)

// 当前展示用的扁平 key-value 表
const config = reactive<Record<string, any>>({})
const aiConfig = reactive<Record<string, any>>({})
const kits = ref<any[]>([])

// 用于 diff：保存原始值，提交时只提交变化项
const originalConfig = ref<Record<string, string>>({})
const originalAiConfig = ref<Record<string, string>>({})

const tabs = [
  { label: '通用配置', value: 'general', icon: 'Setting' },
  { label: 'AI 配置', value: 'ai', icon: 'Cpu' },
  { label: '色板配置', value: 'color', icon: 'BrushFilled' },
  { label: '套装配置', value: 'kit', icon: 'Box' },
  { label: '客服配置', value: 'cs', icon: 'Headset' },
  { label: '协议配置', value: 'agreement', icon: 'DocumentChecked' }
]

// 后端返回扁平列表 [{config_key, config_value, ...}]，按 key 转为对象
function toMap(list: ConfigItem[] | undefined): Record<string, any> {
  const map: Record<string, any> = {}
  if (!Array.isArray(list)) return map
  for (const item of list) {
    if (!item || !item.config_key) continue
    const v = item.config_value
    // boolean 字符串还原为 boolean（方便 el-switch 等使用）
    if (item.config_type === 'boolean') {
      map[item.config_key] = v === 'true' || v === '1'
    } else if (item.config_type === 'number') {
      map[item.config_key] = v == null || v === '' ? null : Number(v)
    } else if (typeof v === 'string' && (v === 'true' || v === 'false')) {
      // 类型缺失但值是布尔字符串，按布尔处理
      map[item.config_key] = v === 'true'
    } else {
      map[item.config_key] = v ?? ''
    }
  }
  return map
}

function snapshot(map: Record<string, any>): Record<string, string> {
  const out: Record<string, string> = {}
  for (const k in map) out[k] = map[k] == null ? '' : String(map[k])
  return out
}

async function loadAll() {
  loading.value = true
  try {
    const list: ConfigItem[] = (await getGeneralConfig()) || []
    const map = toMap(list)

    // 通用配置 key
    const generalKeys = ['app_name', 'app_version', 'copyright', 'comment_enabled', 'sensitive_word_enabled', 'guest_mode_enabled', 'register_enabled']
    for (const k of generalKeys) {
      config[k] = map[k] ?? defaultGeneral[k]
    }
    originalConfig.value = snapshot(config)

    // AI 配置 key
    const aiKeys = ['default_bead_count', 'default_difficulty', 'free_daily_quota', 'timeout', 'retry_count', 'fallback_enabled', 'queue_threshold']
    for (const k of aiKeys) {
      aiConfig[k] = map[k] ?? defaultAi[k]
    }
    originalAiConfig.value = snapshot(aiConfig)

    kits.value = (await listKits()) || []
  } catch (e) {
    console.error('加载系统配置失败', e)
  } finally {
    loading.value = false
  }
}

const defaultGeneral: Record<string, any> = {
  app_name: '拼豆',
  app_version: '1.0.0',
  copyright: '© 2026 拼豆团队',
  comment_enabled: true,
  sensitive_word_enabled: true,
  guest_mode_enabled: false,
  register_enabled: true
}

const defaultAi: Record<string, any> = {
  default_bead_count: 3000,
  default_difficulty: 'beginner',
  free_daily_quota: 3,
  timeout: 120,
  retry_count: 2,
  fallback_enabled: true,
  queue_threshold: 50
}

function buildPayload(current: Record<string, any>, original: Record<string, string>, typeOf: Record<string, 'boolean' | 'number' | 'string'>): SetConfigPayload[] {
  const payloads: SetConfigPayload[] = []
  for (const key in current) {
    const v = current[key]
    const orig = original[key] ?? ''
    const vStr = v == null ? '' : String(v)
    if (vStr === orig) continue // 未变化，跳过
    const type = typeOf[key] || 'string'
    payloads.push({ key, value: type === 'boolean' ? String(v === true || v === 'true') : vStr, type })
  }
  return payloads
}

async function onSave() {
  const generalTypes: Record<string, 'boolean' | 'number' | 'string'> = {
    app_name: 'string',
    app_version: 'string',
    copyright: 'string',
    comment_enabled: 'boolean',
    sensitive_word_enabled: 'boolean',
    guest_mode_enabled: 'boolean',
    register_enabled: 'boolean'
  }
  const aiTypes: Record<string, 'boolean' | 'number' | 'string'> = {
    default_bead_count: 'number',
    default_difficulty: 'string',
    free_daily_quota: 'number',
    timeout: 'number',
    retry_count: 'number',
    fallback_enabled: 'boolean',
    queue_threshold: 'number'
  }

  const generalPayload = buildPayload(config, originalConfig.value, generalTypes)
  const aiPayload = buildPayload(aiConfig, originalAiConfig.value, aiTypes)
  const payload = [...generalPayload, ...aiPayload]

  if (payload.length === 0) {
    ElMessage.info('没有需要保存的修改')
    return
  }

  loading.value = true
  try {
    // 后端批量保存接口：POST /api/admin/v1/config/batch
    await batchSetConfig(payload)
    ElMessage.success(`已保存 ${payload.length} 项配置`)
    // 重新加载以更新原始值
    await loadAll()
  } catch (e: any) {
    ElMessage.error(e?.message || '保存失败')
  } finally {
    loading.value = false
  }
}

onMounted(loadAll)
</script>

<template>
  <div class="page-view">
    <PageHead
      :crumbs="[{ label: '系统管理' }, { label: '系统配置' }]"
      title="系统配置"
      sub="通用 · AI · 色板 · 套装 · 客服 · 协议"
    >
      <template #actions>
        <button class="btn btn-secondary" :disabled="loading" @click="loadAll">恢复默认</button>
        <button class="btn btn-primary" :disabled="loading" @click="onSave">保存</button>
      </template>
    </PageHead>

    <div class="cfg-layout">
      <aside class="cfg-nav">
        <div
          v-for="t in tabs"
          :key="t.value"
          class="cfg-nav-item"
          :class="{ active: activeTab === t.value }"
          @click="activeTab = t.value"
        >
          <el-icon><component :is="t.icon" /></el-icon>
          {{ t.label }}
        </div>
      </aside>
      <main class="cfg-content">
        <div v-if="activeTab === 'general'">
          <div class="panel">
            <div class="panel-head"><div class="ph-title">基础信息</div></div>
            <div class="panel-body">
              <div class="form-row"><div class="lbl">应用名称</div><div class="input-line"><input v-model="config.app_name" /></div></div>
              <div class="form-row"><div class="lbl">版本号</div><div class="input-line"><input v-model="config.app_version" /></div></div>
              <div class="form-row"><div class="lbl">版权信息</div><div class="input-line"><input v-model="config.copyright" /></div></div>
            </div>
          </div>
          <div class="panel">
            <div class="panel-head"><div class="ph-title">功能开关</div></div>
            <div class="panel-body">
              <div class="form-row">
                <div class="lbl">评论功能</div>
                <div>
                  <span class="switch" :class="{ on: config.comment_enabled }" @click="config.comment_enabled = !config.comment_enabled"></span>
                  <span class="muted small">关闭后用户无法评论</span>
                </div>
              </div>
              <div class="form-row">
                <div class="lbl">敏感词过滤</div>
                <div>
                  <span class="switch" :class="{ on: config.sensitive_word_enabled }" @click="config.sensitive_word_enabled = !config.sensitive_word_enabled"></span>
                  <span class="muted small">生效中</span>
                </div>
              </div>
              <div class="form-row">
                <div class="lbl">游客模式</div>
                <div>
                  <span class="switch" :class="{ on: config.guest_mode_enabled }" @click="config.guest_mode_enabled = !config.guest_mode_enabled"></span>
                  <span class="muted small">允许游客浏览</span>
                </div>
              </div>
              <div class="form-row">
                <div class="lbl">新用户注册</div>
                <div>
                  <span class="switch" :class="{ on: config.register_enabled }" @click="config.register_enabled = !config.register_enabled"></span>
                  <span class="muted small">开放注册</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div v-if="activeTab === 'ai'">
          <div class="panel">
            <div class="panel-head"><div class="ph-title">AI 生成参数</div></div>
            <div class="panel-body">
              <div class="form-row"><div class="lbl">默认颗粒数</div><div class="input-line"><input type="number" v-model.number="aiConfig.default_bead_count" /></div></div>
              <div class="form-row">
                <div class="lbl">默认难度</div>
                <div class="f-select" style="width: 100%">
                  <select v-model="aiConfig.default_difficulty">
                    <option value="beginner">入门</option>
                    <option value="intermediate">进阶</option>
                    <option value="advanced">高阶</option>
                  </select>
                </div>
              </div>
              <div class="form-row"><div class="lbl">免费用户每日配额</div><div class="input-line"><input type="number" v-model.number="aiConfig.free_daily_quota" /></div></div>
              <div class="form-row"><div class="lbl">超时时间 (秒)</div><div class="input-line"><input type="number" v-model.number="aiConfig.timeout" /></div></div>
              <div class="form-row"><div class="lbl">重试次数</div><div class="input-line"><input type="number" v-model.number="aiConfig.retry_count" /></div></div>
            </div>
          </div>
          <div class="panel">
            <div class="panel-head"><div class="ph-title">降级策略</div></div>
            <div class="panel-body">
              <div class="form-row">
                <div class="lbl">启用降级</div>
                <div>
                  <span class="switch" :class="{ on: aiConfig.fallback_enabled }" @click="aiConfig.fallback_enabled = !aiConfig.fallback_enabled"></span>
                  <span class="muted small">AI 服务异常时切换备用节点</span>
                </div>
              </div>
              <div class="form-row"><div class="lbl">队列阈值</div><div class="input-line"><input type="number" v-model.number="aiConfig.queue_threshold" /></div></div>
            </div>
          </div>
        </div>

        <div v-if="activeTab === 'color'">
          <div class="panel">
            <div class="panel-head"><div class="ph-title">色板列表</div><button class="btn btn-xs btn-ghost">+ 导入色号</button></div>
            <div class="panel-body">
              <div class="color-grid">
                <div v-for="i in 16" :key="i" class="color-cell">
                  <div class="color-swatch" :style="`background: ${['#FF7A5A','#F5C45E','#4FBB8A','#6FA8D4','#9A7FCC','#E07777','#FFD2B0','#1F1A16','#FAF7F1','#E59A3A','#FFE5B4','#1F7A4B','#246E9C','#6849A6','#A8331F','#B83B1B'][i - 1]}`"></div>
                  <div class="color-code">H{{ i.toString().padStart(3, '0') }}</div>
                  <div class="muted small">{{ ['拼豆橙','琥珀黄','薄荷绿','天空蓝','紫罗兰','玫瑰红','奶茶','深咖','米白','焦糖','奶黄','森林绿','湖蓝','葡萄紫','砖红','酒红'][i - 1] }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div v-if="activeTab === 'kit'">
          <div class="panel">
            <div class="panel-head"><div class="ph-title">物理套装</div><button class="btn btn-xs btn-ghost">+ 新建套装</button></div>
            <div class="panel-body">
              <div class="kit-grid">
                <div v-for="k in kits" :key="k.id" class="kit-card">
                  <div class="kit-cover"></div>
                  <div style="font-weight: 700; margin-top: 8px">{{ k.kit_name || k.name }}</div>
                  <div class="muted small">{{ k.description || k.desc || (k.brand ? k.brand + ' / ' + k.color_count + '色' : '') }}</div>
                  <div class="row" style="margin-top: 8px; justify-content: space-between">
                    <span style="font-size: 18px; font-weight: 700; color: var(--primary)">¥ {{ k.price }}</span>
                    <span class="muted small">{{ k.color_count }} 色</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div v-if="activeTab === 'cs'" v-show="false"></div>
        <div v-if="activeTab === 'agreement'" v-show="false"></div>
      </main>
    </div>
  </div>
</template>

<style scoped>
.page-view { height: 100%; display: flex; flex-direction: column; overflow: hidden; }
.cfg-layout { display: grid; grid-template-columns: 200px 1fr; gap: 16px; padding: 16px 22px; flex: 1; overflow: hidden; background: var(--bg-2); }
.cfg-nav { background: var(--surface); border: 1px solid var(--line); border-radius: 10px; padding: 8px; display: flex; flex-direction: column; gap: 2px; height: fit-content; }
.cfg-nav-item { padding: 8px 12px; border-radius: 6px; font-size: 12.5px; color: var(--ink-2); cursor: pointer; display: flex; align-items: center; gap: 8px; }
.cfg-nav-item:hover { background: var(--bg); }
.cfg-nav-item.active { background: var(--ink); color: #fff; font-weight: 600; }
.cfg-content { display: flex; flex-direction: column; gap: 12px; overflow-y: auto; }
.color-grid { display: grid; grid-template-columns: repeat(8, 1fr); gap: 8px; }
.color-cell { background: var(--surface); border: 1px solid var(--line); border-radius: 8px; padding: 8px; text-align: center; }
.color-swatch { aspect-ratio: 1; border-radius: 5px; margin-bottom: 6px; }
.color-code { font-family: var(--mono); font-size: 11px; font-weight: 700; }
.kit-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 12px; }
.kit-card { background: var(--surface); border: 1px solid var(--line); border-radius: 8px; padding: 12px; }
.kit-cover { aspect-ratio: 1; background: linear-gradient(135deg, #FF8A5A, #F5C45E); border-radius: 6px; }
</style>
