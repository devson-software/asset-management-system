<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <!-- Welcome Header -->
      <div class="col-12">
        <div class="text-h4 text-weight-bold text-primary">System Overview</div>
        <div class="text-subtitle1 text-grey-7">Welcome back! Here's what's happening today.</div>
      </div>

      <!-- Quick Stats -->
      <div class="col-12 col-md-3">
        <q-card class="hvac-gradient text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Total Assets</div>
            <div class="text-h3 text-weight-bolder">{{ allAssets.length }}</div>
            <q-icon name="ac_unit" size="lg" class="absolute-right q-ma-md opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="cool-gradient text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Scheduled Visits</div>
            <div class="text-h3 text-weight-bolder">{{ store.services.length }}</div>
            <q-icon name="engineering" size="lg" class="absolute-right q-ma-md opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="bg-red-9 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Emergency Faults</div>
            <div class="text-h3 text-weight-bolder">1</div>
            <q-icon name="warning" size="lg" class="absolute-right q-ma-md opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="bg-green-8 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Reports Ready</div>
            <div class="text-h3 text-weight-bolder">{{ store.jobCards.length }}</div>
            <q-icon name="cloud_done" size="lg" class="absolute-right q-ma-md opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12 col-md-3">
        <q-card class="bg-indigo-8 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Commissioning</div>
            <div class="text-h3 text-weight-bolder">{{ store.commissioningRecords.length }}</div>
            <q-icon name="fact_check" size="lg" class="absolute-right q-ma-md opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <!-- Main Actions -->
      <div class="col-12 col-md-8">
        <q-card flat bordered>
          <q-card-section class="row items-center">
            <div class="text-h6">Recent Assets</div>
            <q-space />
            <q-btn flat color="primary" label="View All" to="/assets" />
          </q-card-section>
          <q-separator />
          <q-list>
            <q-item v-for="asset in allAssets.slice(0, 5)" :key="asset.id" clickable v-ripple>
              <q-item-section avatar>
                <q-avatar color="blue-1" text-color="primary" icon="ac_unit" />
              </q-item-section>
              <q-item-section>
                <q-item-label>{{ asset.unitRef }}</q-item-label>
                <q-item-label caption>{{ asset.indoorModel }} | {{ asset.customerName }}</q-item-label>
              </q-item-section>
              <q-item-section side>
                <q-badge color="positive" label="Active" />
              </q-item-section>
            </q-item>
          </q-list>
        </q-card>
      </div>

      <div class="col-12 col-md-4">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">Quick Actions</div>
          </q-card-section>
          <q-card-section class="q-gutter-y-sm">
            <q-btn color="primary" icon="person_add" label="New Customer" class="full-width" to="/customers/add" />
            <q-btn color="secondary" icon="calendar_today" label="Service Schedule" class="full-width" to="/service-calendar" />
            <q-btn color="indigo-8" icon="fact_check" label="Commissioning Master" class="full-width" to="/commissioning" />
            <q-btn outline color="primary" icon="file_download" label="Export Register" class="full-width" to="/assets" />
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { store } from '../store'

export default defineComponent({
  name: 'DashboardPage',
  setup () {
    const allAssets = computed(() => {
      const assets = []
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          p.assets.forEach(a => {
            assets.push({ ...a, customerName: c.name })
          })
        })
      })
      return assets
    })

    return { store, allAssets }
  }
})
</script>

<style scoped>
.opacity-40 { opacity: 0.4; }
.full-width { width: 100%; }
</style>

