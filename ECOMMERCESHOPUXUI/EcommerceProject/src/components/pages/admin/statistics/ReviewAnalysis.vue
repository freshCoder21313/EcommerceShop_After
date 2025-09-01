<template>
  <div class="row mb-4">
    <div class="col-12">

      <div v-if="isLoading" class="text-center my-4">
        <LoadingSpinner />
      </div>

      <div v-else-if="!data || Object.keys(data).length === 0" class="text-center my-4">
        <NoDataMessage />
      </div>

      <div v-else>
        <div class="position-relative mb-4">
          <Overlay
            :is-visible="!data.averageRating"
            overlay-content="Không có dữ liệu đánh giá để thống kê."
          />
          <div class="chart-container p-2 border rounded bg-white">
            <canvas ref="reviewRatingChartCanvas"></canvas>
          </div>
        </div>

        <div class="text-center mt-3">
          <p class="mb-1 text-black">Đánh giá trung bình</p>
          <h4 class="text-primary-gradient mb-0">
            <i class="icon-star mr-2"></i>
            {{ data.averageRating ? data.averageRating.toFixed(1) : 0 }}
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
  data: { default: () => ({}) },
  isLoading: { type: Boolean, default: false },
});

const reviewChart = ref(null);
const reviewRatingChartCanvas = ref(null);

const renderChart = () => {
  const canvas = reviewRatingChartCanvas.value;
  if (!canvas) return;
  const context = canvas.getContext('2d');
  if (!context) return;

  if (!props.data || (!props.data.highestRatedProduct && !props.data.lowestRatedProduct && !props.data.mostReviewedProduct)) {
    if (reviewChart.value) {
      reviewChart.value.destroy();
      reviewChart.value = null;
    }
    return;
  }

  const entries = [
    {
      label: 'Cao nhất',
      product: props.data.highestRatedProduct,
      color: 'rgba(75, 192, 192, 0.7)',
    },
    {
      label: 'Thấp nhất',
      product: props.data.lowestRatedProduct,
      color: 'rgba(255, 99, 132, 0.7)',
    },
    {
      label: 'Nhiều nhất',
      product: props.data.mostReviewedProduct,
      color: 'rgba(255, 206, 86, 0.7)',
    },
  ].filter(e => e.product && typeof e.product.averageRating === 'number');

  const labels = entries.map(e => `${e.label}: ${e.product.productName}`);
  const dataValues = entries.map(e => e.product.averageRating);
  const backgroundColors = entries.map(e => e.color);

  const chartData = {
    labels,
    datasets: [
      {
        label: 'Điểm trung bình',
        data: dataValues,
        backgroundColor: backgroundColors,
        borderColor: backgroundColors.map(c => c.replace('0.7', '1')),
        borderWidth: 1,
      },
    ],
  };

  const chartOptions = {
    indexAxis: 'y',
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        min: 0,
        max: 5,
        beginAtZero: true,
        title: { display: true, text: 'Điểm trung bình (sao)' },
      },
      y: {
        title: { display: true, text: 'Sản phẩm' },
      },
    },
    plugins: {
      legend: { display: false },
      title: {
        display: true,
        text: 'So sánh điểm đánh giá sản phẩm nổi bật',
        font: { size: 16, weight: 'bold' },
      },
    },
  };

  if (reviewChart.value) {
    reviewChart.value.destroy();
  }

  reviewChart.value = new Chart(context, {
    type: 'bar',
    data: chartData,
    options: chartOptions,
  });
};

watch(() => props.isLoading, (newVal) => {
  if (!newVal) nextTick(renderChart);
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
  if (reviewChart.value) {
    reviewChart.value.destroy();
  }
});
</script>

<style scoped>
canvas {
  width: 100%;
  min-height: 20em;
  max-height: 30em;
}
</style>