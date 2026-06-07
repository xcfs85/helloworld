<script setup lang="ts">
import { Search, Refresh } from '@element-plus/icons-vue'

defineProps<{
  showSearch?: boolean
  showRefresh?: boolean
}>()

const emit = defineEmits<{
  (e: 'search'): void
  (e: 'refresh'): void
}>()
</script>

<template>
  <div class="toolbar">
    <div class="left">
      <slot name="left" />
    </div>
    <div class="right">
      <slot name="right" />
      <el-button v-if="showSearch" :icon="Search" @click="emit('search')">搜索</el-button>
      <el-button v-if="showRefresh" :icon="Refresh" @click="emit('refresh')">刷新</el-button>
    </div>
  </div>
</template>
