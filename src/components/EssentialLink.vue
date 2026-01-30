<template>
  <q-drawer
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    show-if-above
    bordered
    class="bg-white q-pa-sm"
    :width="280"
  >
    <q-list class="q-gutter-y-xs">
      <q-item
        clickable
        to="/dashboard"
        exact
        class="navigation-item"
        active-class="navigation-item-active"
      >
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

        <q-item
          clickable
          to="/admin/users"
          class="navigation-item"
          active-class="navigation-item-active"
        >
          <q-item-section avatar>
            <q-icon name="fas fa-users-gear" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">User Management</q-item-label>
            <q-item-label caption>Roles & Access</q-item-label>
          </q-item-section>
        </q-item>

        <q-item
          clickable
          to="/admin/teams"
          class="navigation-item"
          active-class="navigation-item-active"
        >
          <q-item-section avatar>
            <q-icon name="fas fa-users-rectangle" size="14px" />
          </q-item-section>
          <q-item-section>
            <q-item-label class="text-weight-bold">Team Management</q-item-label>
            <q-item-label caption>Technician Teams</q-item-label>
          </q-item-section>
        </q-item>
      </template>

      <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-md q-pb-md">
        ASSETS & CUSTOMERS
      </q-item-label>

      <q-item
        clickable
        to="/customers"
        class="navigation-item"
        active-class="navigation-item-active"
      >
        <q-item-section avatar>
          <q-icon name="fas fa-users" size="14px" />
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-weight-bold">Customers</q-item-label>
          <q-item-label caption>Manage Clients</q-item-label>
        </q-item-section>
      </q-item>

      <q-item
        clickable
        to="/projects"
        class="navigation-item"
        active-class="navigation-item-active"
      >
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

      <q-item
        clickable
        to="/quotations"
        class="navigation-item"
        active-class="navigation-item-active"
      >
        <q-item-section avatar>
          <q-icon name="fas fa-file-invoice-dollar" size="14px" />
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-weight-bold">Quotation</q-item-label>
          <q-item-label caption>Cost Estimations</q-item-label>
        </q-item-section>
      </q-item>

      <q-item-label header class="text-overline text-weight-bold text-grey-8 q-pt-lg q-pb-md">
        MAINTENANCE
      </q-item-label>

      <q-item
        clickable
        to="/service-calendar"
        class="navigation-item"
        active-class="navigation-item-active"
      >
        <q-item-section avatar>
          <q-icon name="fas fa-calendar-days" size="14px" />
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-weight-bold">Service Schedule</q-item-label>
          <q-item-label caption>Maintenance Planning</q-item-label>
        </q-item-section>
      </q-item>

      <q-item
        clickable
        to="/job-cards"
        class="navigation-item"
        active-class="navigation-item-active"
      >
        <q-item-section avatar>
          <q-icon name="fas fa-file-contract" size="14px" />
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-weight-bold">Job Card History</q-item-label>
          <q-item-label caption>Digital Records</q-item-label>
        </q-item-section>
      </q-item>

      <q-item
        clickable
        to="/technical-data"
        class="navigation-item"
        active-class="navigation-item-active"
      >
        <q-item-section avatar>
          <q-icon name="fas fa-chart-line" size="14px" />
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-weight-bold">Technical Data Sheet</q-item-label>
          <q-item-label caption>Unit Specifications</q-item-label>
        </q-item-section>
      </q-item>

      <q-item
        clickable
        to="/commissioning"
        class="navigation-item"
        active-class="navigation-item-active"
      >
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
</template>

<script setup>
import { defineOptions } from 'vue'

defineOptions({
  name: 'NavigationDrawer',
})

defineProps({
  modelValue: {
    type: Boolean,
    default: false,
  },
  isAdmin: {
    type: Boolean,
    default: false,
  },
})

defineEmits(['update:modelValue'])
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
  color: #1976d2 !important;
}

.text-overline {
  font-size: 0.65rem;
  letter-spacing: 1px;
}
</style>
