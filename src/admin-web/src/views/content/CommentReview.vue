<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { commentApi } from '@/api'
import { ElMessage } from 'element-plus'
const list = ref<any[]>([])
const load = async () => {
  list.value = []
  ElMessage.info('评论审核占位')
}
onMounted(load)
</script>
<template>
  <div class="page-container">
    <el-alert title="评论审核模块" type="info" description="详见 帖子审核页 / 评论tab" :closable="false" />
  </div>
</template>
