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

    <q-page-container>
      <router-view />
    </q-page-container>

    <q-footer elevated class="bg-white">
      <q-bottom-navigation
        v-model="activeTab"
        active-color="primary"
        indicator-color="primary"
        class="text-grey-7"
      >
        <q-btn
          label="Customers"
          icon="fas fa-users"
          @click="goTo('/field/customers')"
        />
        <q-btn
          label="Projects"
          icon="fas fa-building"
          @click="goTo('/field/projects')"
        />
        <q-btn
          label="Schedule"
          icon="fas fa-calendar-check"
          @click="goTo('/field/service-schedule')"
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
      if (route.path.includes('/field/projects')) activeTab.value = 'projects'
      else if (route.path.includes('/field/service-schedule')) activeTab.value = 'schedule'
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
.q-bottom-navigation {
  height: 64px;
}
</style>
