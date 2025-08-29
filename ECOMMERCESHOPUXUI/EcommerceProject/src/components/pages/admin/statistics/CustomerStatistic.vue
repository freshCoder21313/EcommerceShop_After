<template>
  <div class="col-12">
    <!-- Khung dữ liệu tóm tắt và biểu đồ -->
    <div class="card m-b-30">
      <div class="card-header bg-white">
        <h5 class="card-title text-black mb-0">Tổng quan khách hàng</h5>
      </div>
      <div class="card-body">
        <div v-if="isLoading" class="text-center my-4">
          <LoadingSpinner />
        </div>
        <div v-else-if="!data || Object.keys(data).length === 0" class="text-center my-4">
          <NoDataMessage />
        </div>
        <div v-else class="row align-items-center g-3">
          <div class="col-md-6">
            <!-- Khung dữ liệu tóm tắt -->
            <div class="xp-chart-label">
              <ul class="list-inline text-center">
                <li class="list-inline-item mx-3">
                  <p class="text-black">Giá trị mua hàng TB</p>
                  <h4 class="text-primary-gradient mb-3">
                    <i class="icon-wallet mr-2"></i>{{ formatCurrency(data?.averagePurchaseAmount) }}
                  </h4>
                </li>
                <li class="list-inline-item mx-3">
                  <p class="text-black">Tổng giá trị mua hàng</p>
                  <h4 class="text-success-gradient mb-3">
                    <i class="icon-wallet mr-2"></i>{{ formatCurrency(data?.totalPurchaseAmount) }}
                  </h4>
                </li>
              </ul>
            </div>
          </div>

          <div class="col-md-6">
            <!-- Khung biểu đồ -->
            <div class="chart-container">
              <canvas ref="customerChartCanvas" v-show="!isLoading"></canvas>
              <div v-if="isLoading" class="text-center my-4">
                <LoadingSpinner />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, nextTick, defineProps } from 'vue';
import { Chart, registerables } from 'chart.js';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
import NoDataMessage from '@/components/common/NoDataMessage.vue';
import { formatCurrency } from '@/constants/formatCurrency';

Chart.register(...registerables);

const props = defineProps({
  data: {
    default: () => ({}),
  },
  isLoading: {
    type: Boolean,
    default: true,
  },
});

const customerChart = ref(null);
const customerChartCanvas = ref(null);

const renderCustomerChart = () => {
  const canvas = customerChartCanvas.value;
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  if (customerChart.value) {
    customerChart.value.destroy();
  }
  const labels = ['Hoạt động', 'Không hoạt động'];
  const dataValues = [
    props.data?.totalActiveCustomers ?? 0,
    props.data?.totalInactiveCustomers ?? 0,
  ];
  customerChart.value = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: labels,
      datasets: [
        {
          label: 'Số lượng khách hàng',
          data: dataValues,
          backgroundColor: ['rgba(75, 192, 192, 0.7)', 'rgba(255, 99, 132, 0.7)'],
          borderColor: ['rgba(75, 192, 192, 1)', 'rgba(255, 99, 132, 1)'],
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            font: {
              size: 14,
            },
          },
        },
        tooltip: {
          callbacks: {
            label: function (context) {
              let label = context.label || '';
              if (label) {
                label += ': ';
              }
              if (context.parsed !== null) {
                label += context.parsed;
              }
              return label;
            },
          },
        },
        title: {
          display: true,
          text: 'Số lượng khách hàng hoạt động/không hoạt động',
          font: {
            size: 16,
          },
        },
      },
    },
  });
};

watch(() => props.isLoading, (newVal) => {
  if (!newVal) {
    nextTick(() => renderCustomerChart());
  }
});

watch(() => props.data, () => {
  if (!props.isLoading) {
    nextTick(() => renderCustomerChart());
  }
}, { deep: true });

onMounted(() => {
  if (!props.isLoading) {
    renderCustomerChart();
  }
});
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
  height: 100%; /* Ensure chart container takes full height */
}

canvas {
  min-height: 10em;
  max-height: 15em;
}
</style>