<script setup lang="ts">
/** 通用页面头部：面包屑 + 标题 + 右侧操作 */
defineProps<{
  crumbs: { label: string; to?: string }[]
  title: string
  sub?: string
}>()
</script>

<template>
  <div class="page-head">
    <div>
      <div class="crumbs">
        <template v-for="(c, i) in crumbs" :key="i">
          <span v-if="i > 0" class="sep">/</span>
          <router-link v-if="c.to" :to="c.to" class="crumb-link">{{ c.label }}</router-link>
          <span v-else :class="{ current: i === crumbs.length - 1 }">{{ c.label }}</span>
        </template>
      </div>
      <div class="page-title">
        {{ title }}
        <span v-if="sub" class="sub">{{ sub }}</span>
      </div>
    </div>
    <div class="head-actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<style scoped>
.crumbs { font-size: 11px; color: var(--ink-3); display: flex; align-items: center; gap: 6px; margin-bottom: 4px; }
.crumbs .sep { color: var(--ink-4); }
.crumbs .current { color: var(--ink-2); }
.crumb-link { color: var(--ink-3); }
.crumb-link:hover { color: var(--primary-ink); }
.page-title { font-size: 18px; font-weight: 700; letter-spacing: .2px; display: flex; align-items: center; gap: 10px; }
.page-title .sub { font-size: 12px; color: var(--ink-3); font-weight: 500; }
.head-actions { display: flex; gap: 8px; align-items: center; }
</style>
