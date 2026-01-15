<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Projects Explorer</div>
          <div class="text-subtitle1 text-grey-7" v-if="activeCustomerName">
            Projects for: <span class="text-weight-bold text-primary">{{ activeCustomerName }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Manage sites across all customers
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn v-if="$route.query.customerId" flat color="primary" icon="arrow_back" label="Show All Customers" :to="{ path: '/projects', query: {} }" />
          <q-btn v-if="$route.query.customerId" color="secondary" icon="add" label="Add Project" :to="'/customers/' + $route.query.customerId + '/add-project'" class="shadow-1" />
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders shadow-1">
          <q-table
            :rows="allProjects"
            :columns="projectColumns"
            row-key="id"
            flat
            :filter="filter"
          >
            <template v-slot:top-right>
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Projects...">
                <template v-slot:append><q-icon name="search" /></template>
              </q-input>
            </template>

            <!-- Custom Project Name / Customer Cell -->
            <template v-slot:body-cell-name="props">
              <q-td :props="props">
                <div class="text-weight-bold text-primary">{{ props.row.name }}</div>
                <div class="text-caption text-grey-7">Client: {{ props.row.customerName }}</div>
              </q-td>
            </template>

            <!-- Custom Site Address Cell -->
            <template v-slot:body-cell-siteAddress="props">
              <q-td :props="props">
                <div class="text-grey-9">{{ props.row.siteAddress }}</div>
                <div class="text-caption text-grey-6">{{ props.row.vendorLocation }}</div>
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
import { defineComponent, ref, computed } from 'vue'
import { store } from '../store'
import { useRoute } from 'vue-router'

export default defineComponent({
  name: 'ProjectsPage',
  setup () {
    const route = useRoute()
    const filter = ref('')

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

    const activeCustomerName = computed(() => {
      const customerId = route.query.customerId
      if (!customerId) return null
      const customer = store.customers.find(c => c.id === customerId)
      return customer ? customer.name : null
    })

    const projectColumns = [
      { name: 'name', label: 'Project Name', align: 'left', field: 'name', sortable: true },
      { name: 'siteAddress', label: 'Location', align: 'left', field: 'siteAddress', sortable: true },
      { name: 'assetsCount', label: 'Assets', align: 'center', field: row => row.assets.length, sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    return {
      store,
      filter,
      allProjects,
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
