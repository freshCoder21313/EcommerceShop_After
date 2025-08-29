<template>
  <div class="card" style="height: 100%">
    <div class="card-header bg-white d-flex justify-content-between align-items-center">
      <h5 class="card-title text-black mb-0">Sản phẩm thu theo thời gian</h5>
      <div class="gap-2 d-flex gap-5 align-items-center" style="position: relative">
        <div class="text-center my-4">
          <select v-model="selectedTimePeriod" @change="updateSalesChart" class="form-select">
            <option value="date">Theo ngày</option>
            <option value="month">Theo tháng</option>
            <option value="year">Theo năm</option>
          </select>
        </div>
      </div>
    </div>
    <div class="card-body m-3">
      <div class="flex align-items-center">
        <div class="chart-container flex align-items-center position-relative">
          <Overlay
            :is-visible="!hasSalesChartData && !isLoading"
            overlay-content="Không có dữ liệu để hiển thị biểu đồ."
          />
          <div v-if="isLoading" class="text-center my-4">
            <LoadingSpinner />
          </div>
          <div class="flex align-items-center" v-else>
            <div class="" v-show="selectedTimePeriod === 'date'">
              <canvas id="salesQuantityChartByDay"></canvas>
            </div>
            <div class="" v-show="selectedTimePeriod === 'month'">
              <canvas id="salesQuantityChartByMonth"></canvas>
            </div>
            <div class="" v-show="selectedTimePeriod === 'year'">
              <canvas id="salesQuantityChartByYear"></canvas>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="card-footer" v-if="!isLoading && data && Object.keys(data).length > 0">
      <div class="xp-chart-label">
        <ul class="list-inline text-center">
          <li class="list-inline-item mx-3" v-for="item in summaryList" :key="item.label">
            <p class="text-black">{{ item.label }}</p>
            <h4 class="text-primary-gradient mb-3">
              <i class="icon-wallet mr-2"></i>{{ item.value }}
            </h4>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, nextTick, defineProps } from 'vue';
import { Chart, registerables } from 'chart.js';
import { formatCurrency } from '@/constants/formatCurrency';
import Overlay from '@/components/common/Overlay.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';

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

const productChart = ref(null);
const salesQuantityChart = ref(null);
const selectedTimePeriod = ref('date');
const hasSalesChartData = ref(true);

const summaryList = computed(() => {
  return [
    { label: 'Tổng doanh thu', value: formatCurrency(props.data?.totalRevenue ?? 0) },
    { label: 'Tổng giảm giá', value: formatCurrency(props.data?.totalDiscount ?? 0) },
    { label: 'Giá trung bình', value: formatCurrency(props.data?.averagePrice ?? 0) },
  ];
});

const updateSalesChart = () => {
  let canvasId = '';
  if (selectedTimePeriod.value === 'date') {
    canvasId = 'salesQuantityChartByDay';
  } else if (selectedTimePeriod.value === 'month') {
    canvasId = 'salesQuantityChartByMonth';
  } else if (selectedTimePeriod.value === 'year') {
    canvasId = 'salesQuantityChartByYear';
  }

  const canvas = document.getElementById(canvasId);
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  if (salesQuantityChart.value) {
    salesQuantityChart.value.destroy();
  }

  let salesData = [];
  let labels = [];
  let revenueData = [];
  let quantityData = [];

  if (selectedTimePeriod.value === 'date') {
    salesData = props.data.salesByTimes?.date || [];
    labels = salesData.map((item) => item.date.split('T')[0]);
    revenueData = salesData.map((item) => item.revenue);
    quantityData = salesData.map((item) => item.count);
  } else if (selectedTimePeriod.value === 'month') {
    salesData = props.data.salesByTimes?.month || [];
    labels = salesData.map((item) => `${item.month}/${item.year}`);
    revenueData = salesData.map((item) => item.revenue);
    quantityData = salesData.map((item) => item.count);
  } else if (selectedTimePeriod.value === 'year') {
    salesData = props.data.salesByTimes?.year || [];
    labels = salesData.map((item) => item.year);
    revenueData = salesData.map((item) => item.revenue);
    quantityData = salesData.map((item) => item.count);
  }

  hasSalesChartData.value =
    salesData &&
    salesData.length > 0 &&
    (revenueData.some((v) => v > 0) || quantityData.some((v) => v > 0));

  if (!hasSalesChartData.value) {
    labels = [''];
    revenueData = [0];
    quantityData = [0];
  }

  salesQuantityChart.value = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: labels,
      datasets: [
        {
          label: 'Doanh thu',
          data: revenueData,
          type: 'line',
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderWidth: 2,
          fill: true,
          yAxisID: 'y',
        },
        {
          label: 'Số lượng bán',
          data: quantityData,
          type: 'bar',
          backgroundColor: 'rgba(255, 206, 86, 0.7)',
          borderColor: 'rgba(255, 206, 86, 1)',
          borderWidth: 1,
          yAxisID: 'y1',
        },
      ],
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true,
          position: 'left',
          title: {
            display: true,
            text: 'Doanh thu',
          },
        },
        y1: {
          beginAtZero: true,
          position: 'right',
          grid: {
            drawOnChartArea: false,
          },
          title: {
            display: true,
            text: 'Số lượng bán',
          },
        },
      },
      plugins: {
        tooltip: {
          mode: 'index',
          intersect: false,
          callbacks: {
            label: function (context) {
              let label = context.dataset.label || '';
              if (label) {
                label += ': ';
              }
              if (context.parsed.y !== null) {
                label += context.parsed.y;
              }
              return label;
            },
          },
        },
        legend: {
          display: true,
        },
        title: {
          display: true,
          text: 'Doanh thu và số lượng bán theo thời gian',
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
    nextTick(() => {
      updateSalesChart();
    });
  }
});

watch(() => props.data, () => {
  if (!props.isLoading) {
    nextTick(() => {
      updateSalesChart();
    });
  }
}, { deep: true });

onMounted(() => {
  if (!props.isLoading) {
    updateSalesChart();
  }
});
</script>

<style scoped>
canvas {
  width: 100%;
  height: 35rem;
}
</style>
