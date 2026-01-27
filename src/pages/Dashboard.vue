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
        <q-card flat bordered class="full-height shadow-1">
          <q-card-section class="row items-center bg-blue-grey-1">
            <div class="text-h6 text-grey-9">
              <q-icon name="fas fa-calendar-day" class="q-mr-sm" color="primary" />
              Today's Schedule
            </div>
            <q-space />
            <q-btn flat color="primary" label="View Calendar" to="/service-calendar" />
          </q-card-section>
          <q-separator />
          <q-list separator>
            <q-item v-for="service in todaysServices" :key="service.id" clickable v-ripple>
              <q-item-section avatar>
                <q-avatar :style="{ backgroundColor: getTeamColor(service.teamId) + '20', color: getTeamColor(service.teamId) }">
                  <q-icon name="fas fa-tools" size="xs" />
                </q-avatar>
              </q-item-section>
              <q-item-section>
                <q-item-label class="text-weight-bold text-primary">{{ service.unitRef }}</q-item-label>
                <q-item-label caption class="text-grey-9 text-weight-medium">
                  {{ service.type }}
                </q-item-label>
                <q-item-label caption>
                  {{ service.projectName }} | {{ service.customer }}
                </q-item-label>
              </q-item-section>
              <q-item-section side>
                <div class="column items-end">
                  <q-chip dense outline color="primary" icon="access_time" size="sm">
                    {{ service.time }}
                  </q-chip>
                  <div class="text-caption text-grey-7 q-mt-xs">{{ service.duration }}</div>
                </div>
              </q-item-section>
            </q-item>
            
            <div v-if="todaysServices.length === 0" class="column items-center q-pa-xl text-grey-5">
              <q-icon name="fas fa-calendar-check" size="64px" class="opacity-20" />
              <div class="text-subtitle1 q-mt-md">No service call outs scheduled for today.</div>
            </div>
          </q-list>
        </q-card>
      </div>

      <div class="col-12 col-md-4">
        <q-card flat bordered class="full-height shadow-1">
          <q-card-section class="bg-blue-grey-1">
            <div class="text-h6 text-grey-9">Add New</div>
          </q-card-section>
          <q-separator />
          <q-card-section class="q-gutter-y-md q-pa-lg">
            <q-btn 
              color="primary" 
              icon="fas fa-user-plus" 
              label="New Customer" 
              class="full-width action-btn" 
              to="/customers/add" 
              unelevated 
            />
            <q-btn 
              color="teal-6" 
              icon="fas fa-plus-circle" 
              label="Add New Project" 
              class="full-width action-btn" 
              to="/projects/add" 
              unelevated 
            >
              <template v-slot:prepend>
                <q-icon name="fas fa-building" class="q-mr-xs" />
              </template>
            </q-btn>
            <q-btn 
              color="indigo-6" 
              icon="fas fa-clipboard-check" 
              label="Add New Commissioning Data" 
              class="full-width action-btn" 
              to="/commissioning" 
              unelevated 
            />
            <q-btn 
              color="blue-6" 
              icon="fas fa-file-circle-plus" 
              label="Create New Job Card" 
              class="full-width action-btn" 
              to="/job-cards/add" 
              unelevated 
            />
            <q-btn 
              color="orange-8" 
              icon="fas fa-file-invoice-dollar" 
              label="Add Quotation" 
              class="full-width action-btn" 
              to="/dashboard" 
              unelevated 
            />
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

    const todaysServices = computed(() => {
      const today = new Date().toISOString().split('T')[0].replace(/-/g, '/')
      return store.services.filter(s => s.date === today)
    })

    const getTeamColor = (teamId) => {
      const team = store.teams.find(t => t.id === teamId)
      return team ? team.color : '#9e9e9e'
    }

    return { store, allAssets, totalProjects, isAdmin, todaysServices, getTeamColor }
  }
})
</script>

<style scoped>
.opacity-40 { opacity: 0.4; }
.opacity-20 { opacity: 0.2; }
.full-width { width: 100%; }
.action-btn {
  height: 56px;
  border-radius: 12px;
  font-weight: 600;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
}
.action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}
.hover-lift {
  transition: transform 0.2s ease;
}
.hover-lift:hover {
  transform: translateY(-4px);
}
.hvac-gradient {
  background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%);
}
.cool-gradient {
  background: linear-gradient(135deg, #0f766e 0%, #14b8a6 100%);
}
</style>

