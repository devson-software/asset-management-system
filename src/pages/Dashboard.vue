<template>
  <q-page padding>
    <div class="row q-col-gutter-md">
      <!-- Welcome Header -->
      <div class="col-12">
        <div class="text-h4 text-weight-bold text-primary">System Overview</div>
        <div class="text-subtitle1 text-grey-7">Welcome back! Here's what's happening today.</div>
      </div>

      <!-- Quick Stats - 6 in a row -->
      <div class="col-6 col-md-2">
        <q-card class="hvac-gradient text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Total Customers</div>
            <div class="text-h4 text-weight-bolder">{{ store.customers.length }}</div>
            <q-icon name="fas fa-users-viewfinder" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-6 col-md-2">
        <q-card class="cool-gradient text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Visits</div>
            <div class="text-h4 text-weight-bolder">{{ store.services.length }}</div>
            <q-icon name="fas fa-user-gear" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-6 col-md-2">
        <q-card class="bg-orange-8 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Projects</div>
            <div class="text-h4 text-weight-bolder">{{ totalProjects }}</div>
            <q-icon name="fas fa-building" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-6 col-md-2">
        <q-card class="bg-red-9 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Faults</div>
            <div class="text-h4 text-weight-bolder">1</div>
            <q-icon name="fas fa-triangle-exclamation" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-6 col-md-2">
        <q-card class="bg-green-8 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Reports</div>
            <div class="text-h4 text-weight-bolder">{{ store.jobCards.length }}</div>
            <q-icon name="fas fa-file-invoice" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <div class="col-6 col-md-2">
        <q-card class="bg-indigo-8 text-white shadow-5 hover-lift">
          <q-card-section>
            <div class="text-subtitle2 opacity-80">Commission</div>
            <div class="text-h4 text-weight-bolder">{{ store.commissioningRecords.length }}</div>
            <q-icon name="fas fa-clipboard-check" size="md" class="absolute-right q-ma-sm opacity-40" />
          </q-card-section>
        </q-card>
      </div>

      <!-- Main Actions side by side -->
      <div class="col-12 col-md-8">
        <q-card flat bordered class="full-height">
          <q-card-section class="row items-center">
            <div class="text-h6">Recent Assets</div>
            <q-space />
            <q-btn flat color="primary" label="View All" to="/assets" />
          </q-card-section>
          <q-separator />
          <q-list>
            <q-item v-for="asset in allAssets.slice(0, 5)" :key="asset.id" clickable v-ripple>
              <q-item-section avatar>
                <q-avatar color="blue-1" text-color="primary" icon="fas fa-snowflake" />
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-weight-bold text-primary">{{ asset.unitRef }}</q-item-label>
                <q-item-label caption>
                  <span class="text-weight-medium text-grey-9">{{ asset.projectName }}</span>
                  <span class="q-mx-xs">|</span>
                  <span>{{ asset.customerName }}</span>
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                <div class="column items-center">
                  <q-badge color="indigo" class="q-mb-xs">
                    {{ asset.serviceCount }} Service{{ asset.serviceCount !== 1 ? 's' : '' }}
                  </q-badge>
                  <q-item-label caption class="text-uppercase text-weight-bold" style="font-size: 10px">
                    Call Outs
                  </q-item-label>
                </div>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card>
      </div>

      <div class="col-12 col-md-4">
        <q-card flat bordered class="full-height">
          <q-card-section>
            <div class="text-h6">Quick Actions</div>
          </q-card-section>
          <q-card-section class="q-gutter-y-sm">
            <q-btn color="primary" icon="fas fa-user-plus" label="New Customer" class="full-width" to="/customers/add" />
            <q-btn color="secondary" icon="fas fa-calendar-day" label="Service Schedule" class="full-width" to="/service-calendar" />
            <q-btn color="indigo-8" icon="fas fa-clipboard-check" label="Commissioning Master" class="full-width" to="/commissioning" />
            <q-btn outline color="primary" icon="fas fa-file-export" label="Export Register" class="full-width" to="/assets" />
            <q-btn color="orange-8" icon="fas fa-building" label="Manage Projects" class="full-width" to="/projects" />
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
    const isAdmin = computed(() => store.currentUser?.role === 'administrator')
    const allAssets = computed(() => {
      const assets = []
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          p.assets.forEach(a => {
            const serviceCount = store.services.filter(s => s.unitRef === a.unitRef).length
            assets.push({ 
              ...a, 
              customerName: c.name, 
              projectName: p.name,
              serviceCount: serviceCount
            })
          })
        })
      })
      return assets
    })

    const totalProjects = computed(() => {
      let count = 0
      store.customers.forEach(c => {
        count += c.projects.length
      })
      return count
    })

    return { store, allAssets, totalProjects, isAdmin }
  }
})
</script>

<style scoped>
.opacity-40 { opacity: 0.4; }
.full-width { width: 100%; }
</style>

