<script setup lang="ts">
/** 通用分页 */
defineProps<{
  total: number
  page: number
  pageSize: number
  pageSizes?: number[]
}>()
const emit = defineEmits<{
  (e: 'update:page', v: number): void
  (e: 'update:pageSize', v: number): void
}>()

function changePage(v: number) { emit('update:page', v) }
function changeSize(v: number) { emit('update:pageSize', v); emit('update:page', 1) }
</script>

<template>
  <div class="pager">
    <div>共 <b style="color: var(--ink)">{{ total }}</b> 条 · 第 {{ (page - 1) * pageSize + 1 }}-{{ Math.min(page * pageSize, total) }} 条</div>
    <el-pagination
      background
      layout="prev, pager, next, sizes"
      :total="total"
      :page-size="pageSize"
      :current-page="page"
      :page-sizes="pageSizes || [20, 50, 100]"
      @current-change="changePage"
      @size-change="changeSize"
    />
  </div>
</template>

<style scoped>
.pager { display: flex; align-items: center; justify-content: space-between; }
:deep(.el-pagination) { padding: 0; }
:deep(.el-pagination .btn-prev),
:deep(.el-pagination .btn-next),
:deep(.el-pagination .el-pager li) {
  background: var(--surface) !important;
  color: var(--ink-2) !important;
  border: 1px solid var(--line) !important;
  font-size: 12px;
  min-width: 28px; height: 28px; line-height: 26px;
}
:deep(.el-pagination .el-pager li.is-active) {
  background: var(--ink) !important; color: #fff !important; border-color: var(--ink) !important;
}
</style>
