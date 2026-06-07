<script setup lang="ts">
import { ref, onMounted } from 'vue'
const overview = ref<any>({})
onMounted(() => {})
</script>
<template>
  <div class="page-container">
    <el-alert title="内容分析" type="info" :closable="false" />
  </div>
</template>
