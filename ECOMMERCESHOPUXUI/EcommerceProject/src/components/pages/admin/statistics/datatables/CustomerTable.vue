<template>
  <div v-if="customers.length" class="table-responsive">
    <table id="customerDatatable" class="table table-hover"></table>
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
  name: 'CustomerTable',
  components: {
    NoDataMessage,
  },
  props: {
    customers: {
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
      const dataSet = this.customers.map((customer) => ({
        customerId: customer.customerId,
        customerName: customer.customerName,
        revenue: customer.revenue, // Keep as a number
        location: customer.location,
        ageGroup: customer.ageGroup,
      }))

      // Khởi tạo DataTable
      const table = $('#customerDatatable').DataTable({
        data: dataSet,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'customerId', title: 'Mã khách hàng', className: 'text-center' },
          { data: 'customerName', title: 'Tên khách hàng' },
          {
            data: 'revenue',
            title: 'Doanh thu',
            className: 'text-right',
            render: function (data, type, /* row */) { // eslint-disable-line no-unused-vars
              if (type === 'display') {
                return formatCurrency(data)
              }
              return data
            },
          },
          {
            data: 'location',
            title: 'Địa điểm',
            render: function (data, type, /* row */) { // eslint-disable-line no-unused-vars
              if (type === 'sort') {
                const match = data.match(/^(\d+)/)
                return match ? parseInt(match[1], 10) : 0
              }
              return data
            },
          },
          { data: 'ageGroup', title: 'Nhóm tuổi' },
        ],
        language: configsDt.defaultLanguageDatatable, // Sử dụng ngôn ngữ từ configs
        initComplete: () => {
          configsDt.attachDetailsControl(`#customerDatatable`, this.formatDetails.bind(this))
        },
      })
      configsDt.attachSearchDebounce('#customerDatatable', table)
    },
    formatDetails(rowData) {
      const customer = this.customers.find((x) => x.customerId == rowData.customerId);
      const container = document.createElement('div');
      container.className = 'container-fluid p-3';

      const title = document.createElement('h6');
      title.className = 'mb-3 text-primary';
      title.textContent = `Chi tiết đơn hàng gần đây của ${customer.customerName}`;
      container.appendChild(title);

      const row = document.createElement('div');
      row.className = 'row g-3';
      container.appendChild(row);

      if (customer.orderRecents && customer.orderRecents.length > 0) {
        customer.orderRecents.forEach(order => {
          const col = document.createElement('div');
          col.className = 'col-sm-12 col-md-6 col-lg-4';
          
          const cardContainer = document.createElement('div');
          col.appendChild(cardContainer);
          row.appendChild(col);

          const app = createApp(DetailCard, {
            title: `Mã hóa đơn: ${order.maHd}`,
            subtitle: order.hoTen,
            imageSrc: pathReplaceImg(undefined, 'HinhAnh/Avatar', order.avatar),
            imageAlt: 'Khách hàng',
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
