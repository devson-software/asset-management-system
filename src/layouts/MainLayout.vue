<template>
  <q-layout view="hHh Lpr lFf">
    <q-header elevated class="bg-white text-grey-9 shadow-1" height-hint="64">
      <q-toolbar class="q-py-sm">
        <q-btn
          flat
          dense
          round
        
          icon="fas fa-bars"
          aria-label="Menu"
          @click="toggleLeftDrawer"
          class="q-mr-sm"
        />

        <q-toolbar-title class="text-weight-bold row items-center">
          <q-icon name="fas fa-snowflake" color="primary" size="32px" class="q-mr-sm" />
          <span class="gt-xs">HVAC Management Tool</span>
          <span class="xs">QR</span>
        </q-toolbar-title>

        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn round flat icon="fas fa-magnifying-glass" color="grey-7" class="gt-sm">
            <q-tooltip>Search Anything</q-tooltip>
          </q-btn>
          <q-btn round flat icon="fas fa-bell" color="grey-7">
            <q-badge color="negative" floating>2</q-badge>
            <q-tooltip>Notifications</q-tooltip>
          </q-btn>
          
          <q-separator vertical inset class="q-mx-sm gt-xs" />

          <!-- Dummy Logged In User -->
          <div class="row items-center no-wrap cursor-pointer profile-section q-pa-xs rounded-borders">
            <div class="column items-end q-mr-md gt-xs">
              <div class="text-weight-bold text-subtitle2 line-height-1">{{ store.currentUser.fullName || store.currentUser.username }}</div>
              <div class="text-caption text-grey-6 line-height-1 text-capitalize">{{ store.currentUser.role }}</div>
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
                    <q-item-label class="text-weight-bold">{{ store.currentUser.fullName || store.currentUser.username }}</q-item-label>
                    <q-item-label caption>{{ store.currentUser.email || 'user@example.com' }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable>
                  <q-item-section avatar><q-icon name="fas fa-user-gear" color="primary" /></q-item-section>
                  <q-item-section>My Profile</q-item-section>
                </q-item>
                <q-item clickable>
                  <q-item-section avatar><q-icon name="fas fa-gear" color="grey-7" /></q-item-section>
                  <q-item-section>Account Settings</q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable to="/login" class="text-negative">
                  <q-item-section avatar><q-icon name="fas fa-arrow-right-from-bracket" color="negative" /></q-item-section>
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
            <q-icon name="fas fa-table-columns" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Dashboard</q-item-label>
            <q-item-label caption>System Overview</q-item-label>
          </q-item-section>
        </q-item>

        
        <q-separator q-my-md />

        <!-- Separate Administration Menu -->
        <template v-if="isAdmin">
          <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-md q-pb-md">
            ADMINISTRATION
          </q-item-label>

          <q-item clickable to="/admin/users" class="navigation-item" active-class="navigation-item-active">
            <q-item-section avatar>
              <q-icon name="fas fa-users-gear" size="14px" />
            </q-item-section>
            <q-item-section>
              <q-item-label class="text-weight-bold">User Management</q-item-label>
              <q-item-label caption>Roles & Access</q-item-label>
            </q-item-section>
          </q-item>
        </template>


        <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-md q-pb-md">
          ASSETS & CUSTOMERS
        </q-item-label>

        <q-item clickable to="/customers" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-users" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Customers</q-item-label>
            <q-item-label caption>Manage Clients</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/projects" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-building" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Projects</q-item-label>
            <q-item-label caption>Manage Sites</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/assets" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-boxes-stacked" size="14px" />
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
            <q-icon name="fas fa-calendar-days" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Service Schedule</q-item-label>
            <q-item-label caption>Maintenance Planning</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/job-cards" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-file-contract" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Job Card History</q-item-label>
            <q-item-label caption>Digital Records</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/technical-data" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-chart-line" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Technical Data Sheet</q-item-label>
            <q-item-label caption>Unit Specifications</q-item-label>
          </q-item-section>
        </q-item>

        <q-item clickable to="/commissioning" class="navigation-item" active-class="navigation-item-active">
          <q-item-section avatar>
            <q-icon name="fas fa-clipboard-check" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Commissioning</q-item-label>
            <q-item-label caption>CIBSE & ASHRAE Master</q-item-label>
          </q-item-section>
        </q-item>

        <q-separator q-my-lg />

        <q-item clickable to="/login" class="navigation-item text-grey-7">
          <q-item-section avatar>
            <q-icon name="fas fa-arrow-right-from-bracket" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Sign Out</q-item-label>
            <q-item-label caption>Secure Logout</q-item-label>
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
import { defineComponent, ref, computed } from 'vue'
import { store } from '../store'

export default defineComponent({
  name: 'MainLayout',
  setup () {
    const leftDrawerOpen = ref(false)
    const isAdmin = computed(() => store.currentUser?.role === 'administrator')

    return {
      store,
      leftDrawerOpen,
      isAdmin,
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

.navigation-item .q-item__section--avatar {
  min-width: 32px !important;
  padding-right: 1rem !important;
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
