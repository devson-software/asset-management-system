<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Projects Explorer</div>
          <div class="text-subtitle1 text-grey-7" v-if="activeCustomerName">
            Managing sites for: <span class="text-weight-bold text-primary">{{ activeCustomerName }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Complete directory of all managed customer sites
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn v-if="$route.query.customerId" flat color="primary" icon="fas fa-arrow-left" label="Show All Customers" :to="{ path: '/projects', query: {} }" />
          <q-btn 
            color="primary" 
            icon="fas fa-plus" 
            label="Add New Project" 
            :to="$route.query.customerId ? '/customers/' + $route.query.customerId + '/add-project' : '/projects/add'" 
            class="shadow-2" 
          />
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders shadow-1">
          <q-table
            :rows="filteredRows"
            :columns="projectColumns"
            row-key="id"
            flat
            :filter="filter"
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
            <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Projects...">
              <template v-slot:append>
                <q-icon name="fas fa-magnifying-glass" size="xs" />
              </template>
            </q-input>
          </template>

          <!-- Custom Project Name / Customer Cell -->
          <template v-slot:body-cell-name="props">
            <q-td :props="props">
              <div class="row items-center no-wrap">
                <q-avatar color="blue-1" text-color="primary" size="32px" class="q-mr-md shadow-1">
                  <img v-if="props.row.pictureUrl" :src="props.row.pictureUrl">
                  <q-icon v-else name="fas fa-map-location-dot" size="16px" />
                </q-avatar>
                <div>
                  <div class="text-weight-bold text-primary">{{ props.row.name }}</div>
                  <div class="text-caption text-grey-7">Client: {{ props.row.customerName }}</div>
                </div>
              </div>
            </q-td>
          </template>

            <!-- Custom Site Address Cell -->
            <template v-slot:body-cell-siteAddress="props">
              <q-td :props="props">
                <div class="text-grey-9">{{ props.row.siteAddress }}</div>
                <div class="text-caption text-grey-6">{{ props.row.vendorLocation }}</div>
              </q-td>
            </template>

            <!-- Time Allocation Cell -->
            <template v-slot:body-cell-timeAllocation="props">
              <q-td :props="props">
                <q-chip dense color="orange-1" text-color="orange-9" icon="fas fa-hourglass-half" size="sm" v-if="props.value">
                  {{ props.value }}
                </q-chip>
                <span v-else class="text-grey-4 italic">Not set</span>
              </q-td>
            </template>

            <!-- Assets Count Badge Cell -->
            <template v-slot:body-cell-assetsCount="props">
              <q-td :props="props" align="center">
                <q-chip 
                  clickable 
                  @click="$router.push({ path: '/assets', query: { projectId: props.row.id } })"
                  color="blue-1" 
                  text-color="blue-9" 
                  icon="ac_unit" 
                  size="sm"
                >
                  {{ props.row.assets.length }} Units
                  <q-tooltip>View Assets for this Project</q-tooltip>
                </q-chip>
              </q-td>
            </template>

            <!-- Actions Cell -->
            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="more_vert">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable :to="{ path: '/assets', query: { projectId: props.row.id } }">
                        <q-item-section avatar>
                          <q-icon name="ac_unit" color="primary" size="sm" />
                        </q-item-section>
                        <q-item-section>View Assets</q-item-section>
                      </q-item>
                      <q-separator />
                      <q-item clickable :to="'/customers/' + props.row.customerId + '/projects/' + props.row.id + '/edit'">
                        <q-item-section avatar>
                          <q-icon name="edit" color="grey-7" size="sm" />
                        </q-item-section>
                        <q-item-section>Edit Project</q-item-section>
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
import { defineComponent, ref, computed, reactive } from 'vue'
import { store } from '../store'
import { useRoute } from 'vue-router'

export default defineComponent({
  name: 'ProjectsPage',
  setup () {
    const route = useRoute()
    const filter = ref('')
    const columnFilters = reactive({
      name: '',
      siteAddress: '',
      timeAllocation: '',
      assetsCount: ''
    })

    const allProjects = computed(() => {
      const projects = []
      const customerIdQuery = route.query.customerId

      store.customers.forEach(customer => {
        if (customerIdQuery && customer.id !== customerIdQuery) return

        customer.projects.forEach(project => {
          projects.push({
            ...project,
            customerName: customer.name,
            customerId: customer.id
          })
        })
      })
      return projects
    })

    const filteredRows = computed(() => {
      return allProjects.value.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = key === 'assetsCount'
            ? row.assets.length.toString()
            : row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const activeCustomerName = computed(() => {
      const customerId = route.query.customerId
      if (!customerId) return null
      const customer = store.customers.find(c => c.id === customerId)
      return customer ? customer.name : null
    })

    const projectColumns = [
      { name: 'name', label: 'Project Name', align: 'left', field: 'name', sortable: true },
      { name: 'siteAddress', label: 'Location', align: 'left', field: 'siteAddress', sortable: true },
      { name: 'timeAllocation', label: 'Time Allocation', align: 'left', field: 'timeAllocation', sortable: true },
      { name: 'assetsCount', label: 'Assets', align: 'center', field: row => row.assets.length, sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    return {
      store,
      filter,
      columnFilters,
      allProjects,
      filteredRows,
      activeCustomerName,
      projectColumns
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
