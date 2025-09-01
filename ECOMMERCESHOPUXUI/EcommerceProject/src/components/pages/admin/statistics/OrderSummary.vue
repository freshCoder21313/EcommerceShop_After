<template>
  <div class="row mb-4">
    <div class="col-12">
      <div class="card m-b-30">
        <div class="card-header bg-white">
          <h5 class="card-title text-black mb-0">Doanh thu theo thời gian</h5>
        </div>
        <div class="card-body">
          <div v-if="isLoading" class="text-center my-4">
            <LoadingSpinner />
          </div>
          <div v-else-if="!data || Object.keys(data).length === 0" class="text-center my-4">
            <NoDataMessage />
          </div>
          <div v-else>
            <div class="text-center my-4">
              <select v-model="selectedTimePeriod" @change="updateCharts" class="form-select">
                <option value="date">Theo ngày</option>
                <option value="month">Theo tháng</option>
                <option value="year">Theo năm</option>
              </select>
            </div>
            <div class="row g-1">
              <div class="col-12 col-md-8 border-end position-relative">
                <Overlay
                  :is-visible="!hasRevenueChartData"
                  overlay-content="Không có dữ liệu doanh thu để thống kê biểu đồ."
                />
                <canvas
                  id="revenueChartByDay"
                  class="m-3"
                  v-if="selectedTimePeriod === 'date'"
                  width="700"
                  height="350"
                ></canvas>
                <canvas
                  id="revenueChartByMonth"
                  class="m-3"
                  v-if="selectedTimePeriod === 'month'"
                  width="700"
                  height="350"
                ></canvas>
                <canvas
                  id="revenueChartByYear"
                  class="m-3"
                  v-if="selectedTimePeriod === 'year'"
                  width="700"
                  height="350"
                ></canvas>
              </div>
              <div class="col-12 col-md-4 border-end position-relative">
                <Overlay
                  :is-visible="!hasOrderStatusChartData"
                  overlay-content="Không có dữ liệu doanh thu để thống kê biểu đồ."
                />
                <canvas
                  id="orderStatusChartByDay"
                  v-if="selectedTimePeriod === 'date'"
                  width="300"
                  height="350"
                  class="m-3"
                ></canvas>
                <canvas
                  id="orderStatusChartByMonth"
                  v-if="selectedTimePeriod === 'month'"
                  width="300"
                  height="350"
                  class="m-3"
                ></canvas>
                <canvas
                  id="orderStatusChartByYear"
                  v-if="selectedTimePeriod === 'year'"
                  width="300"
                  height="350"
                  class="m-3"
                ></canvas>
              </div>
            </div>
          </div>
        </div>
        <div class="card-footer" v-if="!isLoading && data && Object.keys(data).length > 0">
          <div class="xp-chart-label">
            <ul class="list-inline text-center">
              <li class="list-inline-item mx-3">
                <p class="text-black">Tổng số đơn hàng</p>
                <h4 class="text-primary-gradient mb-3">
                  <i class="icon-wallet mr-2"></i>{{ totalOrders ?? 0 }}
                </h4>
              </li>
              <li class="list-inline-item mx-3">
                <p class="text-black">Tổng doanh thu</p>
                <h4 class="text-success-gradient mb-3">
                  <i class="icon-wallet mr-2"></i>{{ formatCurrency(totalRevenue ?? 0) }}
                </h4>
              </li>
              <li class="list-inline-item mx-3">
                <p class="text-black">Giá trị đơn hàng trung bình</p>
                <h4 class="text-info-gradient mb-3">
                  <i class="icon-wallet mr-2"></i>{{ formatCurrency(averageOrderValue ?? 0) }}
                </h4>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, nextTick } from 'vue';
import Overlay from '@/components/common/Overlay.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
import NoDataMessage from '@/components/common/NoDataMessage.vue';
import { Chart, registerables } from 'chart.js';
import { formatCurrency } from '@/constants/formatCurrency';

Chart.register(...registerables);

const props = defineProps({
  data: {
    default: () => ({}),
  },
  isLoading: {
    type: Boolean,
    default: false,
  },
});

const selectedTimePeriod = ref('date');
const revenueChartByTime = ref(null);
const orderStatusChart = ref(null);
const totalOrders = ref(0);
const totalRevenue = ref(0);
const averageOrderValue = ref(0);
const hasRevenueChartData = ref(true);
const hasOrderStatusChartData = ref(true);

const updateCharts = () => {
  calculateOverviewData();
  checkChartData();
  renderCharts();
};

const calculateOverviewData = () => {
  let statusDataByTime;
  switch (selectedTimePeriod.value) {
    case 'date': {
      statusDataByTime = props.data.revenueByTime?.date ?? [];
      break;
    }
    case 'month': {
      statusDataByTime = props.data.revenueByTime?.month ?? [];
      break;
    }
    case 'year': {
      statusDataByTime = props.data.revenueByTime?.year ?? [];
      break;
    }
  }
  totalOrders.value = statusDataByTime.reduce((acc, item) => acc + item.count, 0);
  totalRevenue.value = statusDataByTime.reduce((acc, item) => acc + item.revenue, 0);
  averageOrderValue.value = totalOrders.value > 0 ? totalRevenue.value / totalOrders.value : 0;
};

const checkChartData = () => {
  let revenueData = [];
  if (selectedTimePeriod.value === 'date') {
    revenueData = props.data?.revenueByTime?.['date'] ?? [];
  } else if (selectedTimePeriod.value === 'month') {
    revenueData = props.data?.revenueByTime?.['month'] ?? [];
  } else {
    revenueData = props.data?.revenueByTime?.['year'] ?? [];
  }
  hasRevenueChartData.value =
    revenueData &&
    revenueData.length > 0 &&
    revenueData.some((item) => item.revenue > 0 || item.count > 0);

  let statusData = [];
  if (selectedTimePeriod.value === 'date') {
    statusData = props.data?.orderStatusStatistics?.['date'] ?? [];
  } else if (selectedTimePeriod.value === 'month') {
    statusData = props.data?.orderStatusStatistics?.['month'] ?? [];
  } else {
    statusData = props.data?.orderStatusStatistics?.['year'] ?? [];
  }
  hasOrderStatusChartData.value =
    statusData && statusData.length > 0 && statusData.some((item) => item.count > 0);
};

const renderCharts = () => {
  if (selectedTimePeriod.value === 'date') {
    renderRevenueChart('revenueChartByDay');
    renderOrderStatusChart('orderStatusChartByDay');
  } else if (selectedTimePeriod.value === 'month') {
    renderRevenueChart('revenueChartByMonth');
    renderOrderStatusChart('orderStatusChartByMonth');
  } else if (selectedTimePeriod.value === 'year') {
    renderRevenueChart('revenueChartByYear');
    renderOrderStatusChart('orderStatusChartByYear');
  }
};

const renderRevenueChart = async (chartId) => {
  await nextTick();
  const ctx = document.getElementById(chartId);

  const context = ctx.getContext('2d');
  if (revenueChartByTime.value) {
    revenueChartByTime.value.destroy();
  }

  let revenueData = [],
    countData = [],
    labels = [];
  if (selectedTimePeriod.value === 'date' && props.data?.revenueByTime?.['date']) {
    revenueData = props.data.revenueByTime['date'].map((item) => item.revenue);
    countData = props.data.revenueByTime['date'].map((item) => item.count);
    labels = props.data.revenueByTime['date'].map((item) => item.date.split('T')[0]);
  } else if (selectedTimePeriod.value === 'month' && props.data?.revenueByTime?.['month']) {
    revenueData = props.data.revenueByTime['month'].map((item) => item.revenue);
    countData = props.data.revenueByTime['month'].map((item) => item.count);
    labels = props.data.revenueByTime['month'].map((item) => `${item.month}/${item.year}`);
  } else if (selectedTimePeriod.value === 'year' && props.data?.revenueByTime?.['year']) {
    revenueData = props.data.revenueByTime['year'].map((item) => item.revenue);
    countData = props.data.revenueByTime['year'].map((item) => item.count);
    labels = props.data.revenueByTime['year'].map((item) => item.year);
  }

  revenueChartByTime.value = new Chart(context, {
    type: 'bar',
    data: {
      labels: labels,
      datasets: [
        {
          label: 'Doanh thu',
          data: revenueData,
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderColor: 'rgba(75, 192, 192, 1)',
          borderWidth: 1,
          type: 'line',
          yAxisID: 'y-revenue',
        },
        {
          label: 'Số lượng đơn hàng',
          data: countData,
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1,
          type: 'bar',
          yAxisID: 'y-count',
        },
      ],
    },
    options: {
      responsive: true,
      scales: {
        'y-revenue': {
          beginAtZero: true,
          title: {
            display: true,
            text: 'Doanh thu',
          },
        },
        'y-count': {
          beginAtZero: true,
          position: 'right',
          title: {
            display: true,
            text: 'Số lượng đơn hàng',
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
          text: 'Doanh thu và số lượng đơn hàng',
          font: {
            size: 16,
          },
        },
      },
    },
  });
};

const renderOrderStatusChart = async (chartId) => {
  await nextTick();
  const ctx = document.getElementById(chartId);

  const context = ctx.getContext('2d');
  if (orderStatusChart.value) {
    orderStatusChart.value.destroy();
  }

  let statusDataByTime;
  switch (selectedTimePeriod.value) {
    case 'date': {
      statusDataByTime = props.data.orderStatusStatistics.date;
      break;
    }
    case 'month': {
      statusDataByTime = props.data.orderStatusStatistics.month;
      break;
    }
    case 'year': {
      statusDataByTime = props.data.orderStatusStatistics.year;
      break;
    }
  }

  const statusData = statusDataByTime.map((item) => item.count);
  const statusLabels = statusDataByTime.map((item) => item.status);

  orderStatusChart.value = new Chart(context, {
    type: 'pie',
    data: {
      labels: statusLabels,
      datasets: [
        {
          label: 'Số lượng đơn hàng theo trạng thái',
          data: statusData,
          backgroundColor: [
            'rgba(255, 99, 132, 0.2)',
            'rgba(54, 162, 235, 0.2)',
            'rgba(255, 206, 86, 0.2)',
            'rgba(75, 192, 192, 0.2)',
            'rgba(153, 102, 255, 0.2)',
            'rgba(255, 159, 64, 0.2)',
          ],
          borderColor: [
            'rgba(255, 99, 132, 1)',
            'rgba(54, 162, 235, 1)',
            'rgba(255, 206, 86, 1)',
            'rgba(75, 192, 192, 1)',
            'rgba(153, 102, 255, 1)',
            'rgba(255, 159, 64, 1)',
          ],
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      plugins: {
        tooltip: {
          callbacks: {
            label: function (tooltipItem) {
              const label = tooltipItem.label || '';
              const value = tooltipItem.raw || 0;
              return `${label}: ${value}`;
            },
          },
        },
        legend: {
          position: 'right',
          labels: {
            font: {
              size: 14,
            },
          },
        },
        title: {
          display: true,
          text: 'Tỉ lệ trạng thái đơn hàng',
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
      updateCharts();
    });
  }
});

onMounted(() => {
  if (!props.isLoading) {
    updateCharts();
  }
});
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
}

canvas {
  width: 100%;
  min-height: 20em;
  max-height: 30em;
}
</style>