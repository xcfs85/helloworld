<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { userApi } from '@/api'

const list = ref<any[]>([])
const load = async () => {
  const data = await userApi.orders({ page: 1, size: 20 })
  list.value = data.list
}
onMounted(load)
</script>
<template>
  <div class="page-container">
    <el-table :data="list" border stripe>
      <el-table-column prop="orderNo" label="订单号" width="200" />
      <el-table-column prop="userId" label="用户" width="120" />
      <el-table-column prop="productName" label="商品" />
      <el-table-column prop="amount" label="金额" width="100" />
      <el-table-column label="状态" width="100">
        <template #default="{ row }">
          <el-tag v-if="row.status === 'paid'" type="success">已支付</el-tag>
          <el-tag v-else-if="row.status === 'pending'" type="warning">待支付</el-tag>
          <el-tag v-else type="info">{{ row.status }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="170" />
      <el-table-column prop="payTime" label="支付时间" width="170" />
    </el-table>
  </div>
</template>
