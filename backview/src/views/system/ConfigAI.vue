<script setup lang="ts">
import { useRouter } from 'vue-router'
import { onMounted } from 'vue'
const router = useRouter()
onMounted(() => {
  // AI 配置复用系统配置页
  router.replace({ name: 'config-general' })
})
</script>
<template><div></div></template>
