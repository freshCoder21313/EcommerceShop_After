<template>
  <div v-if="combos.length" class="table-responsive">
    <table id="comboDatatable" class="table table-hover"></table>
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
import StarRating from '@/components/common/StarRating.vue';
import { createApp } from 'vue';
import DetailCard from '@/components/common/DetailCard.vue';

export default {
  name: 'ComboTable',
  components: { NoDataMessage },
  props: {
    combos: {
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
      const dataSet = this.combos.map((combo) => ({
        comboId: combo.comboId,
        comboName: combo.comboName,
        salesCount: combo.salesCount,
        revenue: combo.revenue,
        starCount: combo.starCount
      }))

      // Khởi tạo DataTable
      const table = $('#comboDatatable').DataTable({
        data: dataSet,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'comboId', title: 'Mã combo', className: 'text-center' },
          { data: 'comboName', title: 'Tên combo' },
          {
            data: 'starCount',
            title: 'Đánh giá',
            className: 'text-center',
            render: (data, type, /* row */) => { // eslint-disable-line no-unused-vars
              if (type === 'display') {
                const container = document.createElement('div');
                const app = createApp(StarRating, {
                  rating: data,
                  readOnly: true,
                  showRating: true,
                  starSize: 20
                });
                app.mount(container);
                return container.outerHTML;
              }
              return data;
            }
          },
          { data: 'salesCount', title: 'Số lượng bán', className: 'text-center' },
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
        ],
        language: configsDt.defaultLanguageDatatable, // Sử dụng ngôn ngữ từ configs
        initComplete: () => {
          configsDt.attachDetailsControl(`#comboDatatable`, this.formatDetails.bind(this))
        },
      })
      configsDt.attachSearchDebounce('#comboDatatable', table)
    },
    formatDetails(rowData) {
      const combo = this.combos.find((x) => x.comboId == rowData.comboId);
      const container = document.createElement('div');
      container.className = 'container-fluid p-3';

      const title = document.createElement('h6');
      title.className = 'mb-3 text-primary';
      title.textContent = `Chi tiết combo: ${combo.comboName}`;
      container.appendChild(title);

      const row = document.createElement('div');
      row.className = 'row g-3';
      container.appendChild(row);

      if (combo.detailTopCombos && combo.detailTopCombos.length > 0) {
        combo.detailTopCombos.forEach(dCbo => {
          const col = document.createElement('div');
          col.className = 'col-sm-12 col-md-6 col-lg-4';
          
          const cardContainer = document.createElement('div');
          col.appendChild(cardContainer);
          row.appendChild(col);

          const app = createApp(DetailCard, {
            title: `Sản phẩm: ${dCbo.tenSanPham}`,
            subtitle: `Mã sản phẩm: ${dCbo.comboId}`,
            imageSrc: pathReplaceImg(undefined, 'HinhAnh/Products', dCbo.hinhAnh),
            imageAlt: 'Hình ảnh sản phẩm',
            imageClass: 'rounded me-3',
            details: [
              { label: 'Số lượng', value: dCbo.soLuong, valueClass: 'text-info' },
              { label: 'Đơn giá', value: formatCurrency(dCbo.donGia), valueClass: 'text-danger' }
            ]
          });
          app.mount(cardContainer);
        });
      } else {
        const noData = document.createElement('div');
        noData.className = 'col-12';
        noData.innerHTML = '<p class="text-center text-muted">Không có chi tiết sản phẩm trong combo này để hiển thị.</p>';
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
