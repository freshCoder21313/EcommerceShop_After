<template>
  <div class="row mb-4">
    <div class="col-12">
      <div v-if="isLoading" class="text-center my-4">
        <LoadingSpinner />
      </div>

      <div v-else-if="!data || Object.keys(data).length === 0" class="text-center my-4">
        <NoDataMessage />
      </div>

      <div v-else class="position-relative bg-white border rounded p-3">
        <Overlay
          :is-visible="data.totalCategories === 0"
          overlay-content="Không có dữ liệu danh mục để thống kê."
        />
        <canvas ref="categoryRevenueChartCanvas" class="w-100" style="min-height: 22em;"></canvas>

        <div class="text-center mt-4">
          <p class="text-black mb-1">Tổng số danh mục</p>
          <h4 class="text-primary-gradient">
            <i class="icon-wallet mr-2"></i>{{ data.totalCategories }}
          </h4>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount, nextTick } from 'vue';
import Overlay from '@/components/common/Overlay.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
import NoDataMessage from '@/components/common/NoDataMessage.vue';
import { Chart, registerables } from 'chart.js';


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

const categoryChart = ref(null);
const categoryRevenueChartCanvas = ref(null);

const renderChart = () => {
  const canvas = categoryRevenueChartCanvas.value;
  if (!canvas || !props.data?.topCategories?.length) {
    if (categoryChart.value) {
      categoryChart.value.destroy();
      categoryChart.value = null;
    }
    return;
  }

  const context = canvas.getContext('2d');
  if (!context) return;

  const chartData = {
    labels: props.data.topCategories.map((cat) => cat.categoryName),
    datasets: [
      {
        label: 'Doanh thu',
        data: props.data.topCategories.map((cat) => cat.totalRevenue),
        backgroundColor: 'rgba(54, 162, 235, 0.6)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      y: {
        beginAtZero: true,
        title: {
          display: true,
          text: 'Doanh thu (VNĐ)',
        },
      },
      x: {
        title: {
          display: true,
          text: 'Danh mục',
        },
      },
    },
    plugins: {
      title: {
        display: true,
        text: 'Top danh mục có doanh thu cao nhất',
        font: {
          size: 16,
          weight: 'bold',
        },
      },
      legend: {
        display: false,
      },
    },
  };

  if (categoryChart.value) {
    categoryChart.value.destroy();
  }
  categoryChart.value = new Chart(context, {
    type: 'bar',
    data: chartData,
    options: chartOptions,
  });
};

watch(() => props.isLoading, (newVal) => {
  if (!newVal) {
    nextTick(renderChart);
  }
});

watch(() => props.data, () => {
  nextTick(renderChart);
}, { deep: true });

onMounted(() => {
  if (!props.isLoading) {
    nextTick(renderChart);
  }
});

onBeforeUnmount(() => {
  if (categoryChart.value) {
    categoryChart.value.destroy();
  }
});
</script>

<style scoped>
.icon-wallet {
  margin-right: 5px;
}
canvas {
  width: 100%;
  min-height: 20em;
  max-height: 30em;
}
</style>