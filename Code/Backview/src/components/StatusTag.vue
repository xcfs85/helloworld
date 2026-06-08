<script setup lang="ts">
import { computed } from 'vue'
const props = defineProps<{
  status: string
  variant?: 'ok' | 'warn' | 'danger' | 'info' | 'purple' | 'neutral' | 'primary'
}>()

const variantClass = computed(() => props.variant || 'neutral')
</script>

<template>
  <span class="tag" :class="variantClass"><slot /></span>
</template>
