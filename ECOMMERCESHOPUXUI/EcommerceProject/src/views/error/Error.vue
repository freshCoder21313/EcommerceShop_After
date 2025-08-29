<template>
  <div>
    <div class="xp-authenticate-bg"></div>
    <!-- Start XP Container -->
    <div id="xp-container" class="xp-container">
      <!-- Start Container -->
      <div class="container">
        <!-- Start XP Row -->
        <div class="row vh-100 align-items-center">
          <!-- Start XP Col -->
          <div class="col-lg-12">
            <!-- Start XP Auth Box -->
            <div class="xp-auth-box">
              <div class="card">
                <div class="card-body">
                  <div class="xp-error-box text-center">
                    <h1 class="xp-error-title mb-3">
                      <span class="text-black">{{ statusMessage }}</span>
                    </h1>
                    <h4 class="xp-error-subtitle text-black m-b-30">
                      <i class="mdi mdi-emoticon-sad text-danger font-32"></i>{{ errorSubtitle }}
                    </h4>
                    <p class="text-muted m-b-30">{{ errorMessage }}</p>
                    <RouterLink :to="redirectLink" class="btn btn-primary"
                      >Quay lại trang</RouterLink
                    >
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Auth Box -->
          </div>
          <!-- End XP Col -->
        </div>
        <!-- End XP Row -->
      </div>
      <!-- End Container -->
    </div>
    <!-- End XP Container -->
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { RouterLink, useRoute } from 'vue-router';

const status = ref('');
const statusMessage = ref('');
const errorSubtitle = ref('');
const errorMessage = ref('');
const redirectLink = ref('/');

const setErrorDetails = () => {
  switch (status.value) {
    case '401':
      statusMessage.value = status.value;
      errorSubtitle.value = 'Trang không tồn tại hoặc phiên đăng nhập của bạn đã hết!';
      errorMessage.value = 'Vui lòng quay lại trang bạn có thể truy cập.';
      redirectLink.value = '/';
      break;
    case '404':
      statusMessage.value = status.value;
      errorSubtitle.value = 'Đã xảy ra lỗi!';
      errorMessage.value = 'Trang bạn đang tìm kiếm không tồn tại.';
      redirectLink.value = '/';
      break;
    case '500':
      statusMessage.value = status.value;
      errorSubtitle.value = 'Oops! Đã có sự cố.';
      errorMessage.value = 'Đã xảy ra lỗi trên máy chủ.';
      redirectLink.value = '/login';
      break;
    default:
      statusMessage.value = status.value;
      errorSubtitle.value = 'Vui lòng thử lại sau.';
      errorMessage.value = 'Chúng tôi không thể xử lý yêu cầu của bạn.';
      redirectLink.value = '/';
      break;
  }
};

onMounted(() => {
  const route = useRoute();
  status.value = route.params.status;
  setErrorDetails();
});
</script>

<style scoped>
/* Thêm các kiểu CSS nếu cần */
</style>
