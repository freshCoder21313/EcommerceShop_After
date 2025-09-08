<script setup>
import { onMounted } from 'vue'
import HeaderComponent from '../../components/Header.vue'
import FooterComponent from '../../components/Footer.vue'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap'
import '@fortawesome/fontawesome-free/css/all.min.css'
import 'jquery-ui-dist/jquery-ui.css'
import 'magnific-popup/dist/magnific-popup.css'
import 'slicknav/dist/slicknav.min.css'

import 'jquery-ui-dist/jquery-ui'
import 'magnific-popup'
import 'mixitup/dist/mixitup.min.js'
import 'jquery-countdown/dist/jquery.countdown.min.js'
import 'slicknav/dist/jquery.slicknav.min.js'
import 'jquery.nicescroll/dist/jquery.nicescroll.min.js'
import initTemplate from '@/plugins/template.js'

import Chatbot from '@/components/chatbot/Chatbot.vue'
import CompareProduct from '@/components/specicals/CompareProduct.vue'

import { useModalVisibility } from '@/composables/useModalVisibility'
const { isModalVisible } = useModalVisibility({
  exclude: ["Profile", "Login"]
})

onMounted(() => {
  initTemplate();
});
</script>
<template>
  <div style="background-color:#fffcfa">
    <HeaderComponent />
    <router-view :key="$route.fullPath" />
    <Chatbot />
    <compare-product v-if="isModalVisible"/>
    <FooterComponent />
  </div>
</template>

<style scoped>
@import '../../assets/Customer/css/elegant-icons.css';
@import '../../assets/Customer/css/style.css';
</style>
