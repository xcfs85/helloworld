<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { authApi, type LoginRequest } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()

const formRef = ref()
const loading = ref(false)
const captchaKey = ref('')
const captchaImage = ref('')

const form = ref<LoginRequest>({
  username: 'admin',
  password: 'admin123',
  captcha: '',
  captchaKey: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
  captcha: [{ required: false, message: '请输入验证码', trigger: 'blur' }]
}

const loadCaptcha = async () => {
  try {
    const data = await authApi.captcha()
    captchaKey.value = data.captchaKey
    form.value.captchaKey = data.captchaKey
    captchaImage.value = data.captchaImage
  } catch {}
}

const handleLogin = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid: boolean) => {
    if (!valid) return
    loading.value = true
    try {
      const resp = await authApi.login(form.value)
      auth.setToken(resp.token, resp.refreshToken)
      auth.setUser(resp.user)
      ElMessage.success('登录成功')
      router.push('/dashboard')
    } catch (e) {
      // refresh captcha on error
      loadCaptcha()
    } finally {
      loading.value = false
    }
  })
}

loadCaptcha()
</script>

<template>
  <div class="login-container">
    <div class="login-box">
      <div class="login-header">
        <h1>拼豆后台管理系统</h1>
        <p>AI 照片转拼豆图纸工具</p>
      </div>
      <el-form ref="formRef" :model="form" :rules="rules" @keyup.enter="handleLogin">
        <el-form-item prop="username">
          <el-input v-model="form.username" placeholder="用户名" size="large" prefix-icon="User" />
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="form.password"
            type="password"
            placeholder="密码"
            size="large"
            prefix-icon="Lock"
            show-password
          />
        </el-form-item>
        <el-form-item prop="captcha">
          <el-input v-model="form.captcha" placeholder="验证码" size="large" prefix-icon="Picture">
            <template #append>
              <el-button @click="loadCaptcha" text type="primary">刷新</el-button>
            </template>
          </el-input>
        </el-form-item>
        <el-button type="primary" size="large" :loading="loading" style="width: 100%" @click="handleLogin">
          登 录
        </el-button>
      </el-form>
      <div class="login-footer">
        <p>默认账号: admin / admin123</p>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.login-container {
  height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex; align-items: center; justify-content: center;
}
.login-box {
  width: 420px; padding: 40px; background: #fff; border-radius: 8px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
}
.login-header { text-align: center; margin-bottom: 30px;
  h1 { font-size: 24px; color: #303133; }
  p { color: #909399; margin-top: 8px; }
}
.login-footer { margin-top: 16px; text-align: center; color: #909399; font-size: 12px; }
</style>
