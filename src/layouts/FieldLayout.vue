<template>
  <q-layout view="hHh Lpr lFf" class="bg-grey-1 field-layout">
    <q-header elevated class="bg-primary text-white field-header">
      <q-toolbar class="field-toolbar">
        <q-btn
          flat
          round
          icon="fas fa-arrow-left"
          aria-label="Back"
          @click="$router.back()"
        />
        <q-toolbar-title class="text-weight-bold">Field Service</q-toolbar-title>
        <q-avatar size="34px" class="shadow-1">
          <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
        </q-avatar>
      </q-toolbar>
    </q-header>

    <q-page-container class="field-page-container">
      <router-view />
    </q-page-container>

    <q-footer v-if="showFooter" elevated class="bg-white field-footer">
      <q-bottom-navigation
        v-model="activeTab"
        active-color="primary"
        indicator-color="primary"
        class="text-grey-7 field-bottom-nav"
      >
        <q-btn
          label="Customers"
          icon="fas fa-users"
          @click="goTo('/field/customers')"
        />
        <q-btn
          label="Search"
          icon="fas fa-magnifying-glass"
          @click="goTo('/field/projects')"
        />
        <q-btn
          label="Profile"
          icon="fas fa-user"
          @click="goTo('/field/profile')"
        />
      </q-bottom-navigation>
    </q-footer>
  </q-layout>
</template>

<script>
import { defineComponent, ref, watch, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

export default defineComponent({
  name: 'FieldLayout',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const activeTab = ref('customers')
    const showFooter = computed(() => !route.path.includes('/field/service-schedule'))

    const updateTab = () => {
      if (route.path.includes('/field/profile')) activeTab.value = 'profile'
      else if (route.path.includes('/field/projects')) activeTab.value = 'search'
      else if (route.path.includes('/field/service-schedule')) activeTab.value = 'search'
      else activeTab.value = 'customers'
    }

    const goTo = (path) => {
      router.push(path)
    }

    watch(
      () => route.path,
      () => updateTab(),
      { immediate: true },
    )

    return { activeTab, goTo, showFooter }
  },
})
</script>

<style scoped>
.field-layout {
  min-height: 100vh;
}

.field-header {
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.12);
}

.field-toolbar {
  min-height: 56px;
  padding: 6px 12px;
}

.field-page-container {
  padding: 12px 12px 96px;
}

.field-footer {
  box-shadow: 0 -8px 20px rgba(0, 0, 0, 0.08);
  backdrop-filter: blur(10px);
}

.field-bottom-nav {
  height: 72px;
  padding: 6px 8px;
  width: 100%;
  display: flex;
  justify-content: space-between;
}

.field-bottom-nav .q-btn {
  flex: 0 0 33.3333%;
  min-width: 0;
  padding: 6px 0;
  font-weight: 600;
}

.field-bottom-nav .q-icon {
  font-size: 19px;
}

.field-bottom-nav .q-btn__content {
  font-size: 11px;
  line-height: 1.1;
  letter-spacing: 0.2px;
}

.field-bottom-nav .q-btn--active {
  background: rgba(25, 118, 210, 0.12);
  border-radius: 12px;
  margin: 0 8px;
}

@media (max-width: 360px) {
  .field-toolbar {
    min-height: 52px;
  }

  .field-page-container {
    padding: 10px 10px 90px;
  }

  .field-bottom-nav {
    height: 66px;
  }

  .field-bottom-nav .q-icon {
    font-size: 18px;
  }

  .field-bottom-nav .q-btn__content {
    font-size: 10px;
  }
}
</style>
