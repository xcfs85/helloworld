<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

const dialogVisible = ref(false)
const form = ref({ userId: '', memberType: 'month', durationDays: 30, reason: '' })

const handleSubmit = async () => {
  ElMessage.success('已开通')
  dialogVisible.value = false
}
</script>
<template>
  <div class="page-container">
    <div class="toolbar">
      <div class="right">
        <el-button type="primary" @click="dialogVisible = true">手动开通会员</el-button>
      </div>
    </div>
    <el-alert title="会员管理" type="info" description="用户会员订单请查阅" :closable="false" />
    <el-table :data="[]" border stripe style="margin-top: 20px">
      <el-table-column prop="userId" label="用户" />
      <el-table-column prop="memberType" label="会员类型" />
      <el-table-column prop="startTime" label="开始时间" />
      <el-table-column prop="expireTime" label="到期时间" />
    </el-table>

    <el-dialog v-model="dialogVisible" title="手动开通会员" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="用户ID">
          <el-input v-model="form.userId" placeholder="请输入用户ID" />
        </el-form-item>
        <el-form-item label="会员类型">
          <el-select v-model="form.memberType">
            <el-option label="月度" value="month" />
            <el-option label="季度" value="quarter" />
            <el-option label="年度" value="year" />
            <el-option label="终身" value="lifetime" />
          </el-select>
        </el-form-item>
        <el-form-item label="时长(天)">
          <el-input-number v-model="form.durationDays" :min="1" :max="36500" />
        </el-form-item>
        <el-form-item label="原因">
          <el-input v-model="form.reason" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确认</el-button>
      </template>
    </el-dialog>
  </div>
</template>
