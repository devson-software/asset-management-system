<template>
  <q-layout view="hHh Lpr lFf">
    <q-header elevated class="bg-white text-grey-9 shadow-1" height-hint="64">
      <q-toolbar class="q-py-sm">
        <q-btn
          flat
          dense
          round
        
          icon="menu"
          aria-label="Menu"
          @click="toggleLeftDrawer"
          class="q-mr-sm"
        />

        <q-toolbar-title class="text-weight-bold row items-center">
          <q-icon name="ac_unit" color="primary" size="32px" class="q-mr-sm" />
          <span class="gt-xs">Asset Management System</span>
          <span class="xs">AMS</span>
        </q-toolbar-title>

        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn round flat icon="search" color="grey-7" class="gt-sm">
            <q-tooltip>Search Anything</q-tooltip>
          </q-btn>
          <q-btn round flat icon="notifications" color="grey-7">
            <q-badge color="negative" floating>2</q-badge>
            <q-tooltip>Notifications</q-tooltip>
          </q-btn>
          
          <q-separator vertical inset class="q-mx-sm gt-xs" />

          <!-- Dummy Logged In User -->
          <div class="row items-center no-wrap cursor-pointer profile-section q-pa-xs rounded-borders">
            <div class="column items-end q-mr-md gt-xs">
              <div class="text-weight-bold text-subtitle2 line-height-1">Jeram HVAC</div>
              <div class="text-caption text-grey-6 line-height-1">Senior Technician</div>
            </div>
            <q-avatar size="40px" class="shadow-1">
              <img src="https://cdn.quasar.dev/img/avatar2.jpg">
              <q-badge color="positive" floating rounded />
            </q-avatar>
            
            <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
              <q-list style="min-width: 200px">
                <q-item class="q-py-md">
                  <q-item-section avatar>
                    <q-avatar size="48px">
                      <img src="https://cdn.quasar.dev/img/avatar2.jpg">
                    </q-avatar>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label class="text-weight-bold">Jeram HVAC</q-item-label>
                    <q-item-label caption>jeram@example.com</q-item-label>
                  </q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable>
                  <q-item-section avatar><q-icon name="person" color="primary" /></q-item-section>
                  <q-item-section>My Profile</q-item-section>
                </q-item>
                <q-item clickable>
                  <q-item-section avatar><q-icon name="settings" color="grey-7" /></q-item-section>
                  <q-item-section>Account Settings</q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable to="/login" class="text-negative">
                  <q-item-section avatar><q-icon name="logout" color="negative" /></q-item-section>
                  <q-item-section>Sign Out</q-item-section>
                </q-item>
              </q-list>
            </q-menu>
          </div>
        </div>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      show-if-above
      bordered
      class="bg-white q-pa-sm"
      :width="280"
    >
      <q-list class="q-gutter-y-xs">
        <q-item clickable to="/dashboard" exact class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="dashboard" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Dashboard</q-item-label>
          </q-item-section>
        </q-item>

        <q-separator q-my-md />

        <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-md q-pb-md">
          ASSETS & CUSTOMERS
        </q-item-label>

        <q-item clickable to="/customers" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="people" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Customers</q-item-label>
            <q-item-label caption>Manage Clients</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/projects" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="assignment" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Projects</q-item-label>
            <q-item-label caption>Manage Sites</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/assets" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="inventory" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Assets</q-item-label>
            <q-item-label caption>HVAC Inventory</q-item-label>
          </q-item-section>
        </q-item>

        <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-lg q-pb-md">
          MAINTENANCE
        </q-item-label>

        <q-item clickable to="/service-calendar" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="calendar_month" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Service Schedule</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/job-cards" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="cloud_done" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Job Card History</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/technical-data" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="analytics" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Technical Data Sheet</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/commissioning" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fact_check" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Commissioning</q-item-label>
            <q-item-label caption>CIBSE & ASHRAE Master</q-item-label>
          </q-item-section>
        </q-item>

        <q-separator q-my-lg />

        <q-item clickable to="/login" class="navigation-item text-grey-7">
          <q-item-section avatar>
            <q-icon name="logout" size="sm" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Sign Out</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
import { defineComponent, ref } from 'vue'

export default defineComponent({
  name: 'MainLayout',
  setup () {
    const leftDrawerOpen = ref(false)

    return {
      leftDrawerOpen,
      toggleLeftDrawer () {
        leftDrawerOpen.value = !leftDrawerOpen.value
      }
    }
  }
})
</script>

<style scoped>
.navigation-item {
  border-radius: 8px;
  margin: 2px 0;
  color: #5f6368;
}

.navigation-item-active {
  background-color: #e8f0fe !important;
  color: #1976D2 !important;
}

.text-overline {
  font-size: 0.65rem;
  letter-spacing: 1px;
}

.profile-section {
  transition: background 0.3s ease;
}

.profile-section:hover {
  background: #f1f3f4;
}

.line-height-1 {
  line-height: 1.2;
}

.rounded-borders {
  border-radius: 8px;
}
</style>
