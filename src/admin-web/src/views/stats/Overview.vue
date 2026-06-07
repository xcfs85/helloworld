<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { statsApi } from '@/api'

const overview = ref<any>({})
const daily = ref<any[]>([])

const load = async () => {
  overview.value = await statsApi.overview()
  daily.value = await statsApi.daily()
}

onMounted(load)
</script>
<template>
  <div class="page-container">
    <h2>数据概览</h2>
    <el-row :gutter="20" class="mt-20">
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">总用户数</div>
            <div class="value">{{ overview.totalUsers || 0 }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">总图纸数</div>
            <div class="value">{{ overview.totalDiagrams || 0 }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">总订单数</div>
            <div class="value">{{ overview.totalOrders || 0 }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">总收入</div>
            <div class="value">¥{{ overview.totalRevenue || 0 }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card class="mt-20" header="趋势图">
      <div class="chart-placeholder">趋势图占位 (使用 ECharts)</div>
    </el-card>
  </div>
</template>

<style lang="scss" scoped>
.stat-card { text-align: center;
  .label { color: #909399; font-size: 14px; }
  .value { color: #303133; font-size: 28px; font-weight: 600; margin-top: 8px; }
}
.chart-placeholder { height: 300px; display: flex; align-items: center; justify-content: center; background: #f5f7fa; color: #c0c4cc; border-radius: 4px; }
</style>
