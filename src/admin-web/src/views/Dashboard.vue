<script setup lang="ts">
import { ref, onMounted } from 'vue'

const stats = ref({
  totalUsers: 0,
  todayActive: 0,
  todayNew: 0,
  todayDiagrams: 0,
  todayPosts: 0,
  todayOrders: 0
})

onMounted(() => {
  // 加载统计数据
})
</script>

<template>
  <div class="dashboard">
    <h2>欢迎使用拼豆后台管理系统</h2>

    <el-row :gutter="20" class="stat-row">
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">总用户数</div>
            <div class="value">{{ stats.totalUsers }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">今日活跃</div>
            <div class="value">{{ stats.todayActive }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">今日新增</div>
            <div class="value">{{ stats.todayNew }}</div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card>
          <div class="stat-card">
            <div class="label">今日生成</div>
            <div class="value">{{ stats.todayDiagrams }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px">
      <el-col :span="12">
        <el-card title="趋势图">
          <template #header>用户增长趋势</template>
          <div class="chart-placeholder">图表占位</div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card>
          <template #header>核心指标</template>
          <div class="chart-placeholder">图表占位</div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style lang="scss" scoped>
.dashboard { h2 { margin-bottom: 20px; } }
.stat-row { margin-bottom: 0; }
.stat-card {
  display: flex; flex-direction: column; gap: 8px;
  text-align: center;
  .label { color: #909399; font-size: 14px; }
  .value { color: #303133; font-size: 28px; font-weight: 600; }
}
.chart-placeholder {
  height: 300px;
  display: flex; align-items: center; justify-content: center;
  background: #f5f7fa;
  color: #c0c4cc;
  border-radius: 4px;
}
</style>
