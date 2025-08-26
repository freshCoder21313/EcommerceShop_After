<template>
  <div style="margin-top: 5rem" class="xp-contentbar">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
          <li class="breadcrumb-item active h5"><strong>Thống kê</strong></li>
        </ol>
      </nav>
      <div>
        <button class="btn btn-primary" @click="reloadData" :disabled="isLoading">
          <i class="bi bi-arrow-clockwise"></i> Tải lại
        </button>
        <div class="btn-group ms-2">
          <button
            type="button"
            class="btn btn-info dropdown-toggle"
            data-bs-toggle="dropdown"
            aria-expanded="false"
            :disabled="isLoading"
          >
            <i class="bi bi-file-earmark-arrow-down"></i> Xuất Báo Cáo
          </button>
          <ul class="dropdown-menu">
            <li>
              <a class="dropdown-item" href="#" @click.prevent="exportToWord"
                >Xuất ra Word (.docx)</a
              >
            </li>
            <li>
              <a class="dropdown-item" href="#" @click.prevent="exportToExcel"
                >Xuất ra Excel (.xlsx)</a
              >
            </li>
          </ul>
        </div>
      </div>
    </div>
    <hr />

    <RevenueStatistic
      :data="revenueStatisticData"
      :is-loading="revenueIsLoading"
    ></RevenueStatistic>

    <div class="row align-items-stretch">
      <div class="col-md-12 col-lg-12 col-xl-7 m-b-30">
        <ProductStatistic
          :data="productStatisticData"
          :is-loading="productIsLoading"
        ></ProductStatistic>
      </div>

      <div class="col-md-12 col-lg-12 col-xl-5">
        <div class="flex-grow-1">
          <EmployeeStatistic
            :data="employeeStatisticsData"
            :is-loading="employeeIsLoading"
          ></EmployeeStatistic>
        </div>
        <div class="flex-grow-1">
          <CustomerStatistic
            :data="customerStatisticsData"
            :is-loading="customerIsLoading"
          ></CustomerStatistic>
        </div>
      </div>
    </div>

    <OrderSummary :data="orderSummaryData" :is-loading="orderSummaryIsLoading"></OrderSummary>

    <DatatableStatistic
      :data="datatableStatisticsResponse"
      :is-loading="datatableIsLoading"
      :category-data="categoryStatisticData"
      :category-loading="categoryIsLoading"
      :inventory-loading="inventoryIsLoading"
      :review-data="reviewAnalysisData"
      :review-loading="reviewIsLoading"
    ></DatatableStatistic>
  </div>
</template>

<script>
import ConfigsRequest from '@/models/ConfigsRequest'
import * as axiosConfig from '@/utils/axiosClient'
import Cookies from 'js-cookie'
import Swal from 'sweetalert2'
import * as XLSX from 'xlsx'
import { Document, Packer, Paragraph, HeadingLevel, Table, TableCell, TableRow, WidthType, ImageRun } from 'docx'
import { saveAs } from 'file-saver'
import Chart from 'chart.js/auto';

import OrderSummary from '@/components/pages/admin/statistics/OrderSummary.vue'
import ProductStatistic from '@/components/pages/admin/statistics/ProductStatistic.vue'
import CustomerStatistic from '@/components/pages/admin/statistics/CustomerStatistic.vue'
import EmployeeStatistic from '@/components/pages/admin/statistics/EmployeeStatistic.vue'
import RevenueStatistic from '@/components/pages/admin/statistics/RevenueStatistic.vue'
import DatatableStatistic from '@/components/pages/admin/statistics/DatatableStatistic.vue'

export default {
  name: 'StatisticsView',
  components: {
    OrderSummary,
    ProductStatistic,
    CustomerStatistic,
    EmployeeStatistic,
    RevenueStatistic,
    DatatableStatistic,
  },
  props: {},
  data() {
    return {
      orderSummaryData: {},
      productStatisticData: {},
      customerStatisticsData: {},
      employeeStatisticsData: {},
      revenueStatisticData: {},
      datatableStatisticsResponse: {},
      categoryStatisticData: {},
      reviewAnalysisData: {},
      isLoading: false,
      revenueIsLoading: true,
      productIsLoading: true,
      customerIsLoading: true,
      employeeIsLoading: true,
      orderSummaryIsLoading: true,
      datatableIsLoading: true,
      categoryIsLoading: true,
      inventoryIsLoading: true,
      reviewIsLoading: true,
    }
  },
  computed: {},
  watch: {},
  async mounted() {
    this.waitForAuthAndLoad()
  },
  methods: {
    waitForAuthAndLoad(retries = 50) {
      if (Cookies.get('accessToken') && axiosConfig.isEndpointAvailable()) {
        this.reloadData()
      } else if (retries > 0) {
        setTimeout(() => this.waitForAuthAndLoad(retries - 1), 100)
      } else {
        let errorMessage = 'Không thể tải dữ liệu thống kê.'
        if (!Cookies.get('accessToken')) {
          errorMessage = 'Không tìm thấy thông tin đăng nhập. Vui lòng thử đăng nhập lại.'
        } else if (!axiosConfig.isEndpointAvailable()) {
          errorMessage = 'Không thể kết nối đến máy chủ API. Vui lòng kiểm tra lại kết nối.'
        }
        Swal.fire('Lỗi', errorMessage, 'error')
        this.isLoading = false
      }
    },
    async reloadData() {
      this.isLoading = true
      const tasks = [
        this.loadOrderSummaryData(),
        this.loadProductStatisticsData(),
        this.loadCustomerStatisticsData(),
        this.loadEmployeeStatisticsData(),
        this.loadRevenueStatisticsData(),
        this.loadDatatableData(),
        this.loadCategoryStatisticsData(),
        this.loadReviewAnalysisData(),
      ]
      await Promise.allSettled(tasks)
      this.isLoading = false
    },

    async exportToWord() {
      const children = [
        new Paragraph({
          text: 'Báo Cáo Thống Kê',
          heading: HeadingLevel.TITLE,
          alignment: 'center',
        }),
        new Paragraph({
          text: `Ngày: ${new Date().toLocaleDateString('vi-VN')}`,
          alignment: 'center',
        }),
        new Paragraph({ text: '', spacing: { after: 200 } }),
      ];

      // Section 1: Revenue
      children.push(new Paragraph({ text: '1. Thống Kê Doanh Thu', heading: HeadingLevel.HEADING_1 }));
      children.push(this._createTableFromObject(this.revenueStatisticData, {
        totalRevenue: 'Tổng doanh thu',
        averageDailyRevenue: 'Doanh thu trung bình ngày',
        averageMonthlyRevenue: 'Doanh thu trung bình tháng',
      }));
      if (this.productStatisticData.salesByTimes && this.productStatisticData.salesByTimes.month.length > 0) {
        const revenueChart = await this._createBarChart('Doanh thu theo tháng', this.productStatisticData.salesByTimes.month.map(d => `Tháng ${d.month}`), this.productStatisticData.salesByTimes.month.map(d => d.revenue));
        children.push(new Paragraph({ children: [revenueChart]}));
      }

      // Section 2: Order Summary
      children.push(new Paragraph({ text: '2. Tóm Tắt Đơn Hàng', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      const orderStatusData = this.orderSummaryData.orderStatusStatistics?.month || [];
      children.push(this._createTableFromArray(orderStatusData, ['Trạng thái', 'Số lượng'], ['status', 'count']));
      if (orderStatusData.length > 0) {
        const orderStatusChart = await this._createPieChart('Tỷ lệ trạng thái đơn hàng', orderStatusData.map(d => d.status), orderStatusData.map(d => d.count));
        children.push(new Paragraph({ children: [orderStatusChart]}));
      }

      // Section 3: Product Statistics
      children.push(new Paragraph({ text: '3. Thống Kê Sản Phẩm', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      children.push(this._createTableFromObject(this.productStatisticData, {
        totalProducts: 'Tổng số sản phẩm',
        totalActiveProducts: 'Sản phẩm đang hoạt động',
        productsSoldCount: 'Số sản phẩm đã bán',
      }));
      const topProducts = this.datatableStatisticsResponse.topProducts || [];
      children.push(new Paragraph({ text: 'Top Sản Phẩm', heading: HeadingLevel.HEADING_2 }));
      children.push(this._createTableFromArray(topProducts, ['Tên sản phẩm', 'Doanh thu', 'Số lượng bán'], ['productName', 'revenue', 'count']));
      if (topProducts.length > 0) {
        const topProductsChart = await this._createBarChart('Top sản phẩm theo doanh thu', topProducts.map(p => p.productName), topProducts.map(p => p.revenue), 'y');
        children.push(new Paragraph({ children: [topProductsChart]}));
      }

      // Section 4: Category Statistics
      children.push(new Paragraph({ text: '4. Thống Kê Danh Mục', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      const topCategories = this.categoryStatisticData.topCategories || [];
      children.push(this._createTableFromArray(topCategories, ['Tên danh mục', 'Số lượng bán', 'Doanh thu'], ['categoryName', 'productsSoldCount', 'totalRevenue']));
      if (topCategories.length > 0) {
        const categoryChart = await this._createBarChart('Top danh mục theo doanh thu', topCategories.map(c => c.categoryName), topCategories.map(c => c.totalRevenue), 'y');
        children.push(new Paragraph({ children: [categoryChart]}));
      }

      // Section 5: Customer Statistics
      children.push(new Paragraph({ text: '5. Thống Kê Khách Hàng', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      children.push(this._createTableFromObject(this.customerStatisticsData, {
        totalCustomers: 'Tổng khách hàng',
        totalActiveCustomers: 'Khách hàng đang hoạt động',
        averagePurchaseAmount: 'Chi tiêu trung bình',
      }));

      // Section 6: Employee Statistics
      children.push(new Paragraph({ text: '6. Thống Kê Nhân Viên', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      children.push(this._createTableFromObject(this.employeeStatisticsData, {
        totalEmployees: 'Tổng số nhân viên',
        totalActiveEmployees: 'Nhân viên đang hoạt động',
      }));

      // Section 7: Review Analysis
      children.push(new Paragraph({ text: '7. Phân Tích Đánh Giá', heading: HeadingLevel.HEADING_1, pageBreakBefore: true }));
      children.push(this._createTableFromObject(this.reviewAnalysisData, { averageRating: 'Đánh giá trung bình' }));
      if(this.reviewAnalysisData.mostReviewedProduct) {
        children.push(new Paragraph({ text: 'Sản phẩm được đánh giá nhiều nhất', heading: HeadingLevel.HEADING_2 }));
        children.push(this._createTableFromObject(this.reviewAnalysisData.mostReviewedProduct, {
            productName: 'Tên sản phẩm',
            averageRating: 'Đánh giá trung bình',
            reviewCount: 'Số lượng đánh giá',
        }));
      }

      const doc = new Document({ sections: [{ children }] });

      Packer.toBlob(doc).then((blob) => {
        saveAs(blob, 'BaoCaoThongKe.docx')
      })
    },

    async _createBarChart(title, labels, data, axis = 'x') {
      if (!data || data.length === 0) return null;
      const canvas = document.createElement('canvas');
      new Chart(canvas.getContext('2d'), {
        type: 'bar',
        data: { labels, datasets: [{ label: title, data, backgroundColor: 'rgba(54, 162, 235, 0.6)' }] },
        options: { indexAxis: axis, responsive: false, animation: { duration: 0 } }
      });
      await new Promise(resolve => setTimeout(resolve, 500));
      const dataUrl = canvas.toDataURL('image/png');
      const imageBuffer = Uint8Array.from(atob(dataUrl.split(',')[1]), c => c.charCodeAt(0));
      return new ImageRun({ data: imageBuffer, transformation: { width: 600, height: axis === 'y' ? data.length * 40 + 120 : 400 } });
    },

    async _createPieChart(title, labels, data) {
      if (!data || data.length === 0) return null;
      const canvas = document.createElement('canvas');
      new Chart(canvas.getContext('2d'), {
        type: 'pie',
        data: { labels, datasets: [{ label: title, data }] },
        options: { responsive: false, animation: { duration: 0 } }
      });
      await new Promise(resolve => setTimeout(resolve, 500));
      const dataUrl = canvas.toDataURL('image/png');
      const imageBuffer = Uint8Array.from(atob(dataUrl.split(',')[1]), c => c.charCodeAt(0));
      return new ImageRun({ data: imageBuffer, transformation: { width: 500, height: 500 } });
    },

    _createTableFromObject(data, labels) {
      if (!data) return new Paragraph('Không có dữ liệu.');
      const rows = Object.keys(labels).map((key) => {
        return new TableRow({
          children: [
            new TableCell({ children: [new Paragraph(labels[key])], width: { size: 4500, type: WidthType.DXA } }),
            new TableCell({ children: [new Paragraph(String(data[key] || 'N/A'))], width: { size: 4500, type: WidthType.DXA } }),
          ],
        });
      });
      return new Table({ rows, width: { size: 9000, type: WidthType.DXA } });
    },

    _createTableFromArray(data, headers, keys) {
      if (!data || data.length === 0) return new Paragraph('Không có dữ liệu.');
      const columnWidth = Math.floor(9000 / headers.length);
      const headerRow = new TableRow({ children: headers.map((header) => new TableCell({ children: [new Paragraph({ text: header, bold: true })], width: { size: columnWidth, type: WidthType.DXA } })) });
      const dataRows = data.map((item) => new TableRow({ children: keys.map((key) => new TableCell({ children: [new Paragraph(String(item[key] || ''))], width: { size: columnWidth, type: WidthType.DXA } })) }));
      return new Table({ rows: [headerRow, ...dataRows], width: { size: 9000, type: WidthType.DXA } });
    },

    async exportToExcel() {
      const wb = XLSX.utils.book_new();
      if (this.revenueStatisticData) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet([this.revenueStatisticData]), 'Doanh Thu');
      if (this.orderSummaryData && this.orderSummaryData.orderStatusStatistics) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(this.orderSummaryData.orderStatusStatistics.month), 'Trạng Thái Đơn Hàng');
      if (this.productStatisticData) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet([this.productStatisticData]), 'Thống Kê Sản Phẩm');
      if (this.customerStatisticsData) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet([this.customerStatisticsData]), 'Khách Hàng');
      if (this.employeeStatisticsData) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet([this.employeeStatisticsData]), 'Nhân Viên');
      if (this.datatableStatisticsResponse && this.datatableStatisticsResponse.topProducts) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(this.datatableStatisticsResponse.topProducts), 'Top Sản Phẩm');
      if (this.categoryStatisticData && this.categoryStatisticData.topCategories) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet(this.categoryStatisticData.topCategories), 'Top Danh Mục');
      if (this.reviewAnalysisData) XLSX.utils.book_append_sheet(wb, XLSX.utils.json_to_sheet([this.reviewAnalysisData]), 'Phân Tích Đánh Giá');
      XLSX.writeFile(wb, 'BaoCaoThongKe.xlsx');
    },
    async loadOrderSummaryData() {
      this.orderSummaryIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetOrderSummary', ConfigsRequest.takeAuth())
      this.orderSummaryData = response.data || {}
      await this.$nextTick()
      this.orderSummaryIsLoading = false
    },
    async loadProductStatisticsData() {
      this.productIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetProductStatistics', ConfigsRequest.takeAuth())
      this.productStatisticData = response.data || {}
      await this.$nextTick()
      this.productIsLoading = false
    },
    async loadCustomerStatisticsData() {
      this.customerIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetCustomerStatistics', ConfigsRequest.takeAuth())
      this.customerStatisticsData = response.data || {}
      await this.$nextTick()
      this.customerIsLoading = false
    },
    async loadEmployeeStatisticsData() {
      this.employeeIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetEmployeeStatistics', ConfigsRequest.takeAuth())
      this.employeeStatisticsData = response.data || {}
      await this.$nextTick()
      this.employeeIsLoading = false
    },
    async loadRevenueStatisticsData() {
      this.revenueIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetRevenueStatistics', ConfigsRequest.takeAuth())
      this.revenueStatisticData = response.data || {}
      await this.$nextTick()
      this.revenueIsLoading = false
    },
    async loadDatatableData() {
      this.datatableIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetDatatableStatistics', ConfigsRequest.takeAuth())
      this.datatableStatisticsResponse = response.data || {}
      await this.$nextTick()
      this.datatableIsLoading = false
    },
    async loadCategoryStatisticsData() {
      this.categoryIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetCategoryStatistics', ConfigsRequest.takeAuth())
      this.categoryStatisticData = response.data || {}
      await this.$nextTick()
      this.categoryIsLoading = false
    },
    async loadReviewAnalysisData() {
      this.reviewIsLoading = true
      const response = await axiosConfig.getFromApi('/Statistics/GetReviewAnalysis', ConfigsRequest.takeAuth())
      this.reviewAnalysisData = response.data || {}
      await this.$nextTick()
      this.reviewIsLoading = false
    },
    resetLoadingState() {
      this.isLoading = true
      this.revenueIsLoading = true
      this.productIsLoading = true
      this.customerIsLoading = true
      this.employeeIsLoading = true
      this.orderSummaryIsLoading = true
      this.datatableIsLoading = true
      this.categoryIsLoading = true
      this.inventoryIsLoading = true
      this.reviewIsLoading = true
    },
  },
  activated() {
    this.reloadData()
  },
  deactivated() {
    this.resetLoadingState()
  },
  unmounted() {
    this.resetLoadingState()
  },
}
</script>
<style scoped></style>