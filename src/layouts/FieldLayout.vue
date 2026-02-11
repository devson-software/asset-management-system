<template>
  <q-layout view="hHh Lpr lFf" class="bg-grey-1">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="fas fa-arrow-left"
          aria-label="Back"
          @click="$router.back()"
        />
        <q-toolbar-title class="text-weight-bold">Field Service</q-toolbar-title>
        <q-avatar size="32px">
          <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
        </q-avatar>
      </q-toolbar>
    </q-header>

    <q-page-container class="field-page-container">
      <router-view />
    </q-page-container>

    <q-footer elevated class="bg-white field-footer">
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
import { defineComponent, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

export default defineComponent({
  name: 'FieldLayout',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const activeTab = ref('customers')

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

    return { activeTab, goTo }
  },
})
</script>

<style scoped>
.field-footer {
  box-shadow: 0 -6px 18px rgba(0, 0, 0, 0.06);
}

.field-bottom-nav {
  height: 84px;
  padding: 4px 0;
  width: 100%;
  display: flex;
  justify-content: space-between;
}

.field-bottom-nav .q-btn {
  flex: 0 0 33.3333%;
  min-width: 0;
  padding: 8px 0;
  font-weight: 600;
}

.field-bottom-nav .q-icon {
  font-size: 22px;
}

.field-bottom-nav .q-btn__content {
  font-size: 12px;
  line-height: 1.1;
  letter-spacing: 0.2px;
}

.field-bottom-nav .q-btn--active {
  background: rgba(25, 118, 210, 0.08);
  border-radius: 12px;
  margin: 0 6px;
}

.field-page-container {
  padding-top: 10px;
}
</style>
