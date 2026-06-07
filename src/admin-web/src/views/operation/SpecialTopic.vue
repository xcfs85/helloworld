<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
const list = ref<any[]>([])
onMounted(() => { })
</script>
<template>
  <div class="page-container">
    <el-alert title="专题管理" type="info" :closable="false" />
  </div>
</template>
