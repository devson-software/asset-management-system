<template>
  <q-layout view="hHh Lpr lFf" class="bg-grey-1 field-layout">
    <q-header elevated class="bg-primary text-white field-header">
      <q-toolbar class="field-toolbar">
        <q-btn
          v-if="showBack"
          flat
          icon="fas fa-house"
          label="Home"
          aria-label="Home"
          @click="goToCustomers()"
        />
        <q-toolbar-title class="text-weight-bold cursor-pointer" @click="goToCustomers">
         <!-- Customers -->
        </q-toolbar-title>
        <q-btn flat round class="q-ml-xs" aria-label="Profile menu">
          <q-avatar size="34px" class="shadow-1">
            <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
          </q-avatar>
          <q-menu auto-close class="rounded-borders shadow-2">
            <q-list style="min-width: 160px">

              <q-item clickable @click="logout">
                <q-item-section avatar>
                  <q-icon name="fas fa-right-from-bracket" color="negative" size="sm" />
                </q-item-section>
                <q-item-section>Logout</q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container class="field-page-container">
      <router-view />
    </q-page-container>

  </q-layout>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

export default defineComponent({
  name: 'FieldLayout',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const goToCustomers = () => router.push('/field/customers')
    const logout = () => router.push('/login')
    const showBack = computed(() => route.path !== '/field/customers')
    return { goToCustomers, logout, showBack }
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
  padding: 12px;
}

@media (max-width: 360px) {
  .field-toolbar {
    min-height: 52px;
  }

  .field-page-container {
    padding: 10px;
  }
}
</style>
