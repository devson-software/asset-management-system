<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Customer Directory</div>
          <div class="text-subtitle1 text-grey-7">Manage your global client base and contact records</div>
        </div>
        <q-btn color="primary" icon="fas fa-plus" label="Register New Customer" to="/customers/add" class="shadow-2" />
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="filteredRows"
            :columns="columns"
            row-key="id"
            flat
            :filter="filter"
            class="customers-table"
          >
            <template v-slot:header="props">
              <q-tr :props="props">
                <q-th
                  v-for="col in props.cols"
                  :key="col.name"
                  :props="props"
                  :class="'text-' + col.align"
                >
                  <div class="row items-center no-wrap" :class="col.align === 'center' ? 'justify-center' : (col.align === 'right' ? 'justify-end' : '')">
                    <!-- Modern Sort Icon on the Left -->
                    <q-icon
                      v-if="col.sortable"
                      :name="props.pagination && props.pagination.sortBy === col.name ? (props.pagination.descending ? 'fas fa-arrow-down-long' : 'fas fa-arrow-up-long') : 'fas fa-arrow-up-long'"
                      size="12px"
                      class="q-mr-xs cursor-pointer sort-icon"
                      :class="{ 'active': props.pagination && props.pagination.sortBy === col.name }"
                      @click="props.sort(col)"
                    />
                    <span class="cursor-pointer" @click="col.sortable && props.sort(col)">{{ col.label }}</span>
                    <q-btn
                      v-if="col.name !== 'actions'"
                      flat
                      round
                      dense
                      size="xs"
                      icon="fas fa-filter"
                      class="q-ml-xs filter-btn"
                      :class="{ 'active': columnFilters[col.name] }"
                      :color="columnFilters[col.name] ? 'primary' : 'grey-5'"
                    >
                      <q-menu cover anchor="top middle">
                        <q-list style="min-width: 200px">
                          <q-item>
                            <q-input
                              v-model="columnFilters[col.name]"
                              :label="'Filter ' + col.label"
                              outlined
                              dense
                              autofocus
                              clearable
                            >
                              <template v-slot:append>
                                <q-icon name="fas fa-magnifying-glass" size="xs" />
                              </template>
                            </q-input>
                          </q-item>
                        </q-list>
                      </q-menu>
                    </q-btn>
                  </div>
                </q-th>
              </q-tr>
            </template>
            <template v-slot:top-right>
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Customers...">
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-name="props">
              <q-td :props="props">
                <div class="row items-center no-wrap">
                  <q-avatar color="blue-1" text-color="primary" icon="fas fa-building" size="32px" class="q-mr-md" />
                  <div>
                    <div class="text-weight-bold">{{ props.row.name }}</div>
                    <div class="text-caption text-grey-7">ID: {{ props.row.id }}</div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-contact="props">
              <q-td :props="props">
                <div class="text-weight-medium text-grey-9">{{ props.row.contactName }}</div>
                <div class="text-caption text-grey-6">{{ props.row.email }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-mobile="props">
              <q-td :props="props">
                <q-chip outline color="grey-7" size="sm" icon="fas fa-phone" dense>
                  {{ props.row.mobile }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-projectsCount="props">
              <q-td :props="props" align="center">
                <q-chip 
                  clickable 
                  @click="$router.push({ path: '/projects', query: { customerId: props.row.id } })"
                  color="blue-1" 
                  text-color="primary" 
                  icon="fas fa-building-circle-check" 
                  size="sm"
                  class="text-weight-bold"
                >
                  {{ props.row.projects?.length || 0 }} Projects
                  <q-tooltip>View Projects for this Client</q-tooltip>
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable :to="{ path: '/projects', query: { customerId: props.row.id } }">
                        <q-item-section avatar>
                          <q-icon name="fas fa-eye" color="primary" size="sm" />
                        </q-item-section>
                        <q-item-section>View Projects</q-item-section>
                      </q-item>
                      <q-separator />
                      <q-item clickable :to="'/customers/edit/' + props.row.id">
                        <q-item-section avatar>
                          <q-icon name="fas fa-edit" color="grey-7" size="sm" />
                        </q-item-section>
                        <q-item-section>Edit Info</q-item-section>
                      </q-item>
                      <q-item clickable class="text-negative">
                        <q-item-section avatar>
                          <q-icon name="fas fa-trash-can" color="negative" size="sm" />
                        </q-item-section>
                        <q-item-section>Remove Client</q-item-section>
                      </q-item>
                    </q-list>
                  </q-menu>
                </q-btn>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, computed } from 'vue'
import { store } from '../store'

export default defineComponent({
  name: 'CustomerRegistration',
  setup () {
    const filter = ref('')
    const columnFilters = reactive({
      name: '',
      contact: '',
      mobile: '',
      projectsCount: '',
      vat: ''
    })

    const columns = [
      { name: 'name', label: 'Company Name', align: 'left', field: 'name', sortable: true },
      { name: 'contact', label: 'Primary Contact', align: 'left', field: 'contactName', sortable: true },
      { name: 'mobile', label: 'Mobile', align: 'left', field: 'mobile', sortable: true },
      { name: 'projectsCount', label: 'Projects', align: 'center', field: row => row.projects?.length || 0, sortable: true },
      { name: 'vat', label: 'VAT Number', align: 'left', field: 'vatNumber', sortable: true },
      { name: 'actions', label: '', align: 'right', field: 'id' }
    ]

    const filteredRows = computed(() => {
      return store.customers.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = key === 'projectsCount' 
            ? (row.projects?.length || 0).toString()
            : key === 'contact'
              ? row.contactName
              : row[key] || ''
          
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    return {
      store,
      filter,
      columnFilters,
      columns,
      filteredRows
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.customers-table {
  background: transparent;
}
</style>
