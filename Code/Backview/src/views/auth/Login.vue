<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { login, getCaptcha } from '@/api/auth'

const router = useRouter()
const tab = ref<'password' | 'sms' | 'sso'>('password')
const form = reactive({
  username: 'admin',
  password: 'admin123',
  captcha: '',
  captcha_key: '',
  remember: true
})
const loading = ref(false)
const captchaLoading = ref(false)
const captchaImage = ref('')
const showCaptcha = ref(false)

// 获取验证码
async function fetchCaptcha() {
  captchaLoading.value = true
  try {
    const res: any = await getCaptcha()
    form.captcha_key = res.captcha_key
    // 实际应该显示图片，后端返回 base64 或 URL
    // 这里先显示 key 的前6位模拟显示
    captchaImage.value = res.captcha_key.substring(0, 6).toUpperCase()
    showCaptcha.value = true
  } catch (e: any) {
    console.error('获取验证码失败', e)
  } finally {
    captchaLoading.value = false
  }
}

async function onSubmit() {
  if (!form.username || !form.password) {
    ElMessage.warning('请填写账号和密码')
    return
  }
  // 密码错误3次后需要验证码
  if (showCaptcha.value && !form.captcha) {
    ElMessage.warning('请填写验证码')
    return
  }
  loading.value = true
  try {
    const res: any = await login({
      username: form.username,
      password: form.password,
      captcha: form.captcha || undefined,
      captcha_key: form.captcha_key || undefined
    })
    localStorage.setItem('admin_token', res.token)
    localStorage.setItem('admin_refresh_token', res.refresh_token || '')
    localStorage.setItem('admin_user', JSON.stringify(res.user))
    localStorage.setItem('admin_nickname', res.user.nickname)
    ElMessage.success('登录成功')
    router.push('/dashboard')
  } catch (e: any) {
    // 密码错误3次后触发验证码
    if (e.code === 1001 || e.message?.includes('密码错误')) {
      if (!showCaptcha.value) {
        await fetchCaptcha()
      }
    }
    ElMessage.error(e.message || '登录失败')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  // 检查是否需要验证码
  const errorCount = parseInt(localStorage.getItem('login_error_count') || '0')
  if (errorCount >= 3) {
    fetchCaptcha()
  }
})

function refreshCaptcha() {
  fetchCaptcha()
}
</script>

<template>
  <div class="login-page">
    <!-- 左侧：登录表单 -->
    <div class="auth-left">
      <div class="auth-head">
        <div class="mark">
          <svg viewBox="0 0 32 32" width="20" height="20">
            <circle cx="8" cy="8" r="3" fill="#FF7A5A" />
            <circle cx="16" cy="8" r="3" fill="#F5C45E" />
            <circle cx="24" cy="8" r="3" fill="#FF7A5A" />
            <circle cx="8" cy="16" r="3" fill="#F5C45E" />
            <circle cx="16" cy="16" r="3" fill="#FF7A5A" />
            <circle cx="24" cy="16" r="3" fill="#F5C45E" />
            <circle cx="8" cy="24" r="3" fill="#FF7A5A" />
            <circle cx="16" cy="24" r="3" fill="#F5C45E" />
            <circle cx="24" cy="24" r="3" fill="#FF7A5A" />
          </svg>
        </div>
        <div class="nm">拼豆<small>PINDOU ADMIN</small></div>
      </div>
      <div class="auth-form">
        <h1>登录控制台</h1>
        <div class="sub">使用管理员账号登录 · 默认 7 天 Token 有效期</div>

        <div class="auth-tabs">
          <div class="auth-tab" :class="{ active: tab === 'password' }" @click="tab = 'password'">账号密码</div>
          <div class="auth-tab" :class="{ active: tab === 'sms' }" @click="tab = 'sms'">手机验证</div>
          <div class="auth-tab" :class="{ active: tab === 'sso' }" @click="tab = 'sso'">SSO</div>
        </div>

        <el-form @submit.prevent="onSubmit" v-if="tab === 'password'">
          <div class="auth-field">
            <label>账号</label>
            <div class="auth-input">
              <el-icon><User /></el-icon>
              <input v-model="form.username" placeholder="请输入账号" />
            </div>
          </div>
          <div class="auth-field">
            <label>密码</label>
            <div class="auth-input">
              <el-icon><Lock /></el-icon>
              <input v-model="form.password" type="password" placeholder="请输入密码" />
            </div>
          </div>
          <div class="auth-field" v-if="showCaptcha">
            <label>验证码 <span class="muted small">密码错误 3 次后必填</span></label>
            <div class="auth-row">
              <div class="auth-input">
                <el-icon><Key /></el-icon>
                <input v-model="form.captcha" placeholder="输入验证码" maxlength="6" />
              </div>
              <div class="auth-captcha" @click="refreshCaptcha" :class="{ 'captcha-loading': captchaLoading }">
                <span v-if="captchaLoading">加载中...</span>
                <span v-else>{{ captchaImage }}</span>
              </div>
            </div>
          </div>
          <div class="auth-meta">
            <label class="row" style="gap: 6px; font-size: 12px; color: var(--ink-2); cursor: pointer">
              <input type="checkbox" v-model="form.remember" class="ck" :checked="form.remember" />
              记住登录状态 7 天
            </label>
            <a>忘记密码？</a>
          </div>
          <button class="auth-btn" :disabled="loading" @click="onSubmit">
            <el-icon v-if="loading"><Loading /></el-icon>
            {{ loading ? '登录中...' : '登 录 控 制 台' }}
          </button>
          <div class="row" style="justify-content: center; gap: 6px; margin-top: 18px; font-size: 11.5px; color: var(--ink-3)">
            <el-icon><InfoFilled /></el-icon>
            登录即同意《管理员协议》与《操作日志规范》
          </div>
        </el-form>

        <el-form v-else-if="tab === 'sms'">
          <div class="auth-field">
            <label>手机号</label>
            <div class="auth-input">
              <el-icon><Iphone /></el-icon>
              <input placeholder="请输入手机号" />
            </div>
          </div>
          <div class="auth-field">
            <label>短信验证码</label>
            <div class="auth-row">
              <div class="auth-input">
                <el-icon><Message /></el-icon>
                <input placeholder="6 位验证码" maxlength="6" />
              </div>
              <button class="btn btn-secondary" type="button" style="height: 40px">获取验证码</button>
            </div>
          </div>
          <button class="auth-btn" @click="onSubmit">登 录</button>
        </el-form>

        <div v-else style="padding: 40px 0; text-align: center; color: var(--ink-3)">
          SSO 单点登录，请使用企业账号登录
        </div>
      </div>
    </div>

    <!-- 右侧：品牌 -->
    <div class="auth-right">
      <div class="auth-head" style="margin-bottom: 0">
        <div class="mark" style="background: rgba(255,255,255,.9)">
          <svg viewBox="0 0 32 32" width="20" height="20">
            <circle cx="8" cy="8" r="3" fill="#B83B1B" />
            <circle cx="16" cy="8" r="3" fill="#2A1F1A" />
            <circle cx="24" cy="8" r="3" fill="#B83B1B" />
            <circle cx="8" cy="16" r="3" fill="#2A1F1A" />
            <circle cx="16" cy="16" r="3" fill="#F5C45E" />
            <circle cx="24" cy="16" r="3" fill="#2A1F1A" />
            <circle cx="8" cy="24" r="3" fill="#B83B1B" />
            <circle cx="16" cy="24" r="3" fill="#2A1F1A" />
            <circle cx="24" cy="24" r="3" fill="#B83B1B" />
          </svg>
        </div>
        <div class="nm" style="color: var(--ink)">PINDOU ADMIN<small style="color: var(--ink-2)">v0.1</small></div>
      </div>
      <div class="auth-pitch">
        <h1>运营工作台</h1>
        <p>用户、内容、模板、运营配置、数据统计 · 7 大模块 · 25 个核心页面 · 为拼豆团队日常运营提供稳定高效的操作体验。</p>
        <div style="margin-top: 24px; display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; max-width: 380px">
          <div>
            <div class="kpi" style="font-size: 22px">12.5k</div>
            <div class="kpi-sub">日活用户</div>
          </div>
          <div>
            <div class="kpi" style="font-size: 22px">1.2k</div>
            <div class="kpi-sub">待审内容</div>
          </div>
          <div>
            <div class="kpi" style="font-size: 22px">5.6k</div>
            <div class="kpi-sub">在线模板</div>
          </div>
        </div>
      </div>
      <div class="auth-foot">
        © 2026 PINDOU · 拼豆团队 · 工单支持 ops@pindou.work
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  display: grid;
  grid-template-columns: 1.1fr 1fr;
  height: 100vh;
  background: var(--surface);
}
.auth-left {
  padding: 40px 44px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  max-width: 560px;
  width: 100%;
  margin: 0 auto;
}
.auth-head { display: flex; align-items: center; gap: 10px; margin-bottom: 32px; }
.auth-head .mark {
  width: 36px; height: 36px; border-radius: 9px;
  background: linear-gradient(135deg, #FF8A5A, #F5C45E);
  display: grid; place-items: center;
}
.auth-head .nm { font-size: 15px; font-weight: 700; }
.auth-head .nm small { font-size: 10px; color: var(--ink-3); letter-spacing: 1.5px; margin-left: 4px; }
.auth-form h1 { font-size: 24px; margin: 0 0 6px; }
.auth-form .sub { color: var(--ink-3); font-size: 13px; margin-bottom: 24px; }
.auth-tabs { display: flex; gap: 0; margin-bottom: 24px; border-bottom: 1px solid var(--line); }
.auth-tab {
  padding: 10px 14px; font-size: 13px; font-weight: 600;
  color: var(--ink-3); border-bottom: 2px solid transparent; cursor: pointer;
  transition: color .12s, border-color .12s;
}
.auth-tab:hover { color: var(--ink-2); }
.auth-tab.active { color: var(--ink); border-bottom-color: var(--primary); }
.auth-field { margin-bottom: 14px; }
.auth-field label { font-size: 12px; color: var(--ink-2); font-weight: 600; display: block; margin-bottom: 6px; }
.auth-field .muted { color: var(--ink-3); font-weight: 400; }
.auth-input {
  display: flex; align-items: center; gap: 8px; height: 40px; padding: 0 12px;
  background: var(--surface); border: 1px solid var(--line-2); border-radius: 7px;
  transition: border-color .12s, box-shadow .12s;
}
.auth-input:focus-within { border-color: var(--primary); box-shadow: 0 0 0 3px var(--primary-soft); }
.auth-input input { flex: 1; border: none; outline: none; background: transparent; font-size: 14px; }
.auth-input .el-icon { color: var(--ink-3); }
.auth-row { display: flex; gap: 8px; align-items: center; }
.auth-row .auth-input { flex: 1; }
.auth-captcha {
  width: 110px; height: 40px; border-radius: 7px;
  background: linear-gradient(135deg, #E6DFD2, #FAF7F1);
  display: grid; place-items: center;
  font-family: var(--mono); font-size: 16px; font-weight: 700; letter-spacing: 4px; color: var(--ink-2);
  cursor: pointer; user-select: none;
  background-image:
    linear-gradient(45deg, transparent 48%, var(--ink-4) 49%, var(--ink-4) 51%, transparent 52%),
    linear-gradient(-45deg, transparent 48%, var(--ink-4) 49%, var(--ink-4) 51%, transparent 52%);
  background-size: 10px 10px;
  flex-shrink: 0;
  transition: transform .12s;
}
.auth-captcha:hover { transform: scale(1.02); }
.captcha-loading { cursor: wait; opacity: 0.7; }
.auth-meta {
  display: flex; align-items: center; justify-content: space-between;
  font-size: 12px; color: var(--ink-3); margin: 4px 0 18px;
}
.auth-meta a { color: var(--primary-ink); font-weight: 600; cursor: pointer; }
.auth-meta a:hover { text-decoration: underline; }
.auth-btn {
  height: 40px; border-radius: 7px;
  background: var(--ink); color: #fff; font-weight: 600; font-size: 14px;
  display: flex; align-items: center; justify-content: center; gap: 6px;
  cursor: pointer; width: 100%; transition: background .12s;
}
.auth-btn:hover:not(:disabled) { background: #000; }
.auth-btn:disabled { opacity: 0.6; cursor: not-allowed; }

.auth-right {
  background: linear-gradient(160deg, #FFE6D4 0%, #FFD2B8 60%, #F5C45E 100%);
  display: flex; flex-direction: column; justify-content: space-between;
  padding: 40px; color: var(--ink);
  position: relative; overflow: hidden;
}
.auth-right::after {
  content: ""; position: absolute; right: -80px; bottom: -80px;
  width: 280px; height: 280px; border-radius: 50%; background: rgba(255, 255, 255, 0.12);
}
.auth-right::before {
  content: ""; position: absolute; left: -40px; top: -40px;
  width: 160px; height: 160px; border-radius: 50%; background: rgba(255, 255, 255, 0.1);
}
.auth-pitch { position: relative; z-index: 1; }
.auth-pitch h1 { font-size: 30px; line-height: 1.35; margin: 0 0 14px; letter-spacing: .2px; max-width: 380px; }
.auth-pitch p { font-size: 14px; line-height: 1.7; color: var(--ink-2); max-width: 380px; margin: 0; }
.auth-foot { position: relative; z-index: 1; font-size: 12px; color: var(--ink-2); line-height: 1.7; }
</style>
