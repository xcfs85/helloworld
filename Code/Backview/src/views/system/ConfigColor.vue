<script setup lang="ts">
import { useRouter } from 'vue-router'
import { onMounted } from 'vue'
const router = useRouter()
onMounted(() => {
  // 色板配置复用系统配置页
  router.replace({ name: 'config-general' })
})
</script>
<template><div></div></template>
