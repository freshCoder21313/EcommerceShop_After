<template>
  <div v-if="products.length" class="table-responsive">
    <table id="productDatatable" class="table table-hover"></table>
  </div>
  <NoDataMessage v-else />
</template>

<script>
import * as configsDt from '@/utils/configsDatatable.js'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import { formatCurrency } from '@/constants/formatCurrency'
import pathReplaceImg from '@/utils/processPathImg' // eslint-disable-line no-unused-vars
import NoDataMessage from '@/components/common/NoDataMessage.vue'
import StarRating from '@/components/common/StarRating.vue';
import { createApp } from 'vue';
import DetailCard from '@/components/common/DetailCard.vue';

export default {
  name: 'ProductTable',
  components: {
    NoDataMessage,
    StarRating // eslint-disable-line vue/no-unused-components
  },
  props: {
    products: {
      type: Array,
      required: true,
    },
  },
  mounted() {
    this.initDataTable()
  },
  methods: {
    formatCurrency, // Make it available in the template if needed
    async initDataTable() {
      await this.$nextTick()
      const productMap = new Map(this.products.map((p) => [p.productId, p]))
      const dataSet = this.products.map((product) => ({
        productId: product.productId,
        productName: product.productName,
        categoryName: product.categoryName,
        revenue: product.revenue,
        count: product.count,
        averageRating: product.averageRating, // Use the direct value from the API
      }));

      const table = $('#productDatatable').DataTable({
        data: dataSet,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'productId', title: 'Mã sản phẩm', className: 'text-center' },
          { data: 'productName', title: 'Tên sản phẩm' },
          {
            data: 'averageRating',
            title: 'Đánh giá TB',
            className: 'text-center',
            render: (data, type, /* row */) => { // eslint-disable-line no-unused-vars
              if (type === 'display') {
                const container = document.createElement('div');
                const app = createApp(StarRating, {
                  rating: data, // data is now the correct averageRating
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
          { data: 'categoryName', title: 'Tên danh mục' },
          {
            data: 'revenue',
            title: 'Doanh thu',
            className: 'text-right',
            render: (data, type, /* row */) => { // eslint-disable-line no-unused-vars
              if (type === 'display') {
                return formatCurrency(data)
              }
              return data
            },
          },
          { data: 'count', title: 'Số lượng bán', className: 'text-center' },
        ],
        language: configsDt.defaultLanguageDatatable,
        initComplete: () => {
          configsDt.attachDetailsControl(
            `#productDatatable`,
            this.formatDetails.bind(this, productMap),
          )
        },
      })
      configsDt.attachSearchDebounce('#productDatatable', table)
    },
    formatDetails(productMap, rowData) {
      const detailProduct = productMap.get(rowData.productId);
      const container = document.createElement('div');
      container.className = 'container-fluid p-3';

      const title = document.createElement('h6');
      title.className = 'mb-3 text-primary';
      title.textContent = `Chi tiết các biến thể: ${detailProduct.productName}`;
      container.appendChild(title);

      const row = document.createElement('div');
      row.className = 'row g-3';
      container.appendChild(row);

      if (detailProduct.detailTopProducts && detailProduct.detailTopProducts.length > 0) {
        detailProduct.detailTopProducts.forEach(detail => {
          const col = document.createElement('div');
          col.className = 'col-sm-12 col-md-6 col-lg-4';
          
          const cardContainer = document.createElement('div');
          col.appendChild(cardContainer);
          row.appendChild(col);

          const app = createApp(DetailCard, {
            title: `Mã CTSP: ${detail.maCtsp}`,
            details: [
              { label: 'Màu sắc', value: detail.mauSac || '-' },
              { label: 'Kích thước', value: detail.kichThuoc || '-' },
              { label: 'Đơn giá', value: this.formatCurrency(detail.donGia || 0) },
              { label: 'Đánh giá', value: '', valueClass: 'd-none' } // Hide label, star rating will be mounted below
            ]
          });
          const mountedApp = app.mount(cardContainer);

          const ratingContainer = document.createElement('div');
          mountedApp.$el.querySelector('.card-body').appendChild(ratingContainer);

          const ratingApp = createApp(StarRating, {
            rating: detail.soSao,
            readOnly: true,
            showRating: true,
            starSize: 16
          });
          ratingApp.mount(ratingContainer);
        });
      } else {
        const noData = document.createElement('div');
        noData.className = 'col-12';
        noData.innerHTML = '<p class="text-center text-muted">Không có biến thể nào để hiển thị.</p>';
        row.appendChild(noData);
      }

      return container;
    },
  },
}
</script>

<style scoped></style>
