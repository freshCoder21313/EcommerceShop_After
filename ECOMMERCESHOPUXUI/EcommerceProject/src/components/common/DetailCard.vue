<template>
  <div class="card h-100 shadow-sm border-0">
    <div class="card-body d-flex flex-column">
      <div class="d-flex align-items-center mb-3">
        <img v-if="imageSrc" :src="imageSrc" class="rounded me-3" :class="imageClass" style="object-fit: cover;" :alt="imageAlt">
        <div>
          <h5 class="card-title mb-0">{{ title }}</h5>
          <p v-if="subtitle" class="card-subtitle text-muted">{{ subtitle }}</p>
        </div>
      </div>
      <div v-for="(item, index) in details" :key="index" class="mb-1">
        <strong>{{ item.label }}:</strong>
        <span :class="item.valueClass">{{ item.value }}</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DetailCard',
  props: {
    title: {
      type: String,
      required: true,
    },
    subtitle: {
      type: String,
      default: '',
    },
    imageSrc: {
      type: String,
      default: '',
    },
    imageAlt: {
      type: String,
      default: 'Image',
    },
    imageClass: {
      type: String,
      default: 'rounded',
    },
    details: {
      type: Array,
      required: true,
    },
  },
};
</script>
