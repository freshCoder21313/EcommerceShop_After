<template>
  <div v-if="employees.length" class="table-responsive">
    <table id="employeeDatatable" class="table table-hover"></table>
  </div>
  <NoDataMessage v-else />
</template>

<script>
import * as configsDt from '@/utils/configsDatatable.js'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import { formatCurrency } from '@/constants/formatCurrency'
import pathReplaceImg from '@/utils/processPathImg'
import NoDataMessage from '@/components/common/NoDataMessage.vue'
import { createApp } from 'vue';
import DetailCard from '@/components/common/DetailCard.vue';

export default {
  name: 'EmployeeTable',
  components: {
    NoDataMessage,
  },
  props: {
    employees: {
      type: Array,
      required: true,
    },
  },
  mounted() {
    this.initDataTable()
  },
  methods: {
    async initDataTable() {
      await this.$nextTick()
      const dataSet = this.employees.map((employee) => ({
        employeeId: employee.employeeId,
        employeeName: employee.employeeName,
        performanceScore: employee.performanceScore,
        positionName: employee.positionName,
        salesAmount: formatCurrency(employee.salesAmount), // Định dạng doanh số
      }))

      // Khởi tạo DataTable
      const table = $('#employeeDatatable').DataTable({
        data: dataSet,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'employeeId', title: 'Mã nhân viên', className: 'text-center' },
          { data: 'employeeName', title: 'Tên nhân viên' },
          // { data: 'performanceScore', title: 'Điểm hiệu suất', className: 'text-center' },
          { data: 'positionName', title: 'Chức vụ' },
          // ! { data: 'salesAmount', title: 'Doanh số', className: 'text-right' },
        ],
        language: configsDt.defaultLanguageDatatable, // Sử dụng ngôn ngữ từ configs
        initComplete: () => {
          configsDt.attachDetailsControl(`#employeeDatatable`, this.formatDetails.bind(this))
        },
      })
      configsDt.attachSearchDebounce('#employeeDatatable', table)
    },
    formatDetails(rowData) {
      const employee = this.employees.find((x) => x.employeeId == rowData.employeeId);
      const container = document.createElement('div');
      container.className = 'container-fluid p-3';

      const title = document.createElement('h6');
      title.className = 'mb-3 text-primary';
      title.textContent = `Chi tiết đơn hàng gần đây của ${employee.employeeName}`;
      container.appendChild(title);

      const row = document.createElement('div');
      row.className = 'row g-3';
      container.appendChild(row);

      if (employee.orderRecents && employee.orderRecents.length > 0) {
        employee.orderRecents.forEach(order => {
          const col = document.createElement('div');
          col.className = 'col-sm-12 col-md-6 col-lg-4';
          
          const cardContainer = document.createElement('div');
          col.appendChild(cardContainer);
          row.appendChild(col);

          const app = createApp(DetailCard, {
            title: `Mã hóa đơn: ${order.maHd}`,
            subtitle: order.hoTen,
            imageSrc: pathReplaceImg(undefined, 'HinhAnh/Avatar/', order.avatar),
            imageAlt: 'Nhân viên',
            imageClass: 'rounded-circle me-3',
            details: [
              { label: 'Ngày tạo', value: order.ngayTao ? new Date(order.ngayTao).toLocaleDateString() : '-' },
              { label: 'Trạng thái', value: order.tinhTrang, valueClass: `badge ${order.isActive ? 'bg-success' : 'bg-danger'}` },
              { label: 'Địa chỉ nhận', value: order.diaChiNhanHang }
            ]
          });
          app.mount(cardContainer);
        });
      } else {
        const noData = document.createElement('div');
        noData.className = 'col-12';
        noData.innerHTML = '<p class="text-center text-muted">Không có đơn hàng nào để hiển thị.</p>';
        row.appendChild(noData);
      }

      return container;
    },
  },
}
</script>

<style scoped>
.table th,
.table td {
  vertical-align: middle;
}
</style>
