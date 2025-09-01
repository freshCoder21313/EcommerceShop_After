<template>
  <div v-if="isLoading" class="text-center my-4">
    <LoadingSpinner />
  </div>
  <div v-else-if="!data || Object.keys(data).length === 0" class="text-center my-4">
    <NoDataMessage />
  </div>
  <div v-else class="row align-items-stretch">
    <div class="col-md-12 col-lg-8 col-xl-8 align-self-center">
      <div class="card bg-white m-b-30">
        <div class="card-header px-3 bg-white d-flex justify-content-between align-items-center">
          <h5 class="card-title text-black mb-0 col-6">{{ tableTitle }}</h5>
          <div class="mb-3 col-auto">
            <select id="dataSelect" class="form-select" v-model="selectedTable">
              <option value="products" :disabled="!data.topProducts.length">
                Sản phẩm bán chạy nhất
              </option>
              <option value="customers" :disabled="!data.topCustomers.length">
                Khách hàng hàng đầu
              </option>
              <option value="employees" :disabled="!data.topEmployees.length">
                Nhân viên hàng đầu
              </option>
              <option value="combos" :disabled="!data.topCombos.length">Combo hàng đầu</option>
            </select>
          </div>
        </div>
        <div class="card-body">
          <div class="">
            <ProductTable v-if="selectedTable === 'products'" :products="data.topProducts" />
          </div>
          <div class="">
            <CustomerTable v-if="selectedTable === 'customers'" :customers="data.topCustomers" />
          </div>
          <div class="">
            <EmployeeTable v-if="selectedTable === 'employees'" :employees="data.topEmployees" />
          </div>
          <div class="">
            <ComboTable v-if="selectedTable === 'combos'" :combos="data.topCombos" />
          </div>
        </div>
      </div>
    </div>
    <div class="col-md-12 col-lg-4 col-xl-4 align-self-center">
      <div class="card bg-white m-b-30">
        <div class="card-header px-3 bg-white d-flex justify-content-between align-items-center">
          <h5 class="card-title text-black mb-0 col-6">Thống kê bổ sung</h5>
          <div class="mb-3 col-auto">
            <select id="statsSelect" class="form-select" v-model="selectedStats">
              <option :disabled="!categoryData" value="categories">Danh mục</option>
              <option :disabled="!reviewData" value="reviews">Đánh giá</option>
            </select>
          </div>
        </div>
        <div class="card-body" style="overflow-y: auto">
          <div v-if="selectedStats === 'coupons'">
          </div>
          <div v-if="selectedStats === 'categories'">
            <CategoryStatistic :data="categoryData" :is-loading="categoryLoading" />
          </div>
          <div v-if="selectedStats === 'reviews'">
            <ReviewAnalysis :data="reviewData" :is-loading="reviewLoading" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
import NoDataMessage from '@/components/common/NoDataMessage.vue';
import ProductTable from '@/components/pages/admin/statistics/datatables/ProductTable.vue';
import CustomerTable from '@/components/pages/admin/statistics/datatables/CustomerTable.vue';
import EmployeeTable from '@/components/pages/admin/statistics/datatables/EmployeeTable.vue';
import ComboTable from '@/components/pages/admin/statistics/datatables/ComboTable.vue';
import CategoryStatistic from '@/components/pages/admin/statistics/CategoryStatistic.vue';
import ReviewAnalysis from '@/components/pages/admin/statistics/ReviewAnalysis.vue';

const props = defineProps({
  data: {
    default: () => ({}),
  },
  isLoading: {
    type: Boolean,
    default: true,
  },
  couponLoading: {
    type: Boolean,
    default: true,
  },
  categoryData: {
    default: () => ({}),
  },
  categoryLoading: {
    type: Boolean,
    default: true,
  },
  inventoryData: {
    default: () => ({}),
  },
  reviewData: {
    default: () => ({}),
  },
  reviewLoading: {
    type: Boolean,
    default: true,
  },
});

const selectedTable = ref('products');
const selectedStats = ref('categories');

const hasCategoryData = computed(() => {
  return props.categoryData?.totalCategories > 0 &&
    Array.isArray(props.categoryData.topCategories) &&
    props.categoryData.topCategories.length > 0;
});

const hasReviewData = computed(() => {
  return typeof props.reviewData?.averageRating === 'number' &&
    props.reviewData.averageRating > 0 &&
    props.reviewData.reviewCountsByStar &&
    Object.keys(props.reviewData.reviewCountsByStar).length > 0;
});

const tableTitle = computed(() => {
  switch (selectedTable.value) {
    case 'customers':
      return 'Khách hàng hàng đầu';
    case 'employees':
      return 'Nhân viên hàng đầu';
    case 'combos':
      return 'Combo hàng đầu';
    default:
      return 'Sản phẩm bán chạy nhất';
  }
});



watch(() => props.isLoading, () => {});
watch(() => props.data, () => {}, { deep: true });

onMounted(() => {
  // setDefaultSelectedStats()
});
</script>

<style scoped>
.table th,
.table td {
  vertical-align: middle;
}
</style>