<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="row items-center no-wrap">
            <q-btn flat round color="primary" icon="fas fa-arrow-left" @click="$router.back()" class="q-mr-md" />
            <div>
              <div class="text-h4 text-weight-bold text-primary">Service Record History</div>
              <div class="text-subtitle1 text-grey-7">
                Unit: <span class="text-weight-bold text-primary">{{ asset?.unitRef }}</span> 
                <span v-if="asset"> — {{ asset.indoorModel }}</span>
              </div>
            </div>
          </div>
        </div>
        <q-btn 
          color="primary" 
          icon="fas fa-plus" 
          label="New Service Record" 
          :to="'/service-entry/' + assetId" 
          class="shadow-2" 
        />
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="history"
            :columns="columns"
            row-key="id"
            flat
            :filter="filter"
            class="history-table"
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
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search History...">
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-type="props">
              <q-td :props="props">
                <q-chip 
                  dense 
                  :color="props.value.includes('Breakdown') || props.value.includes('Emergency') ? 'red-1' : 'blue-1'" 
                  :text-color="props.value.includes('Breakdown') || props.value.includes('Emergency') ? 'red-9' : 'blue-9'"
                  class="text-weight-bold"
                >
                  {{ props.value.toUpperCase() }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable :to="'/service-entry/' + assetId">
                        <q-item-section avatar><q-icon name="fas fa-eye" color="primary" size="sm" /></q-item-section>
                        <q-item-section>View Details</q-item-section>
                      </q-item>
                      <q-item clickable @click="printRecord(props.row)">
                        <q-item-section avatar><q-icon name="fas fa-print" color="grey-7" size="sm" /></q-item-section>
                        <q-item-section>Print Report</q-item-section>
                      </q-item>
                    </q-list>
                  </q-menu>
                </q-btn>
              </q-td>
            </template>
          </q-table>

          <div v-if="history.length === 0" class="column items-center q-pa-xl text-grey-5 bg-white">
            <q-icon name="fas fa-folder-open" size="80px" color="grey-3" />
            <div class="text-h6 q-mt-md">No service records found for this unit.</div>
            <q-btn flat color="primary" label="Schedule a visit?" to="/service-calendar" class="q-mt-sm" />
          </div>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed, reactive } from 'vue'
import { useRoute } from 'vue-router'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AssetServiceHistory',
  setup () {
    const $q = useQuasar()
    const route = useRoute()
    const assetId = route.params.assetId
    const filter = ref('')
    
    const columnFilters = reactive({
      date: '',
      type: '',
      customer: '',
      project: ''
    })

    const columns = [
      { name: 'date', label: 'Service Date', align: 'left', field: 'date', sortable: true },
      { name: 'type', label: 'Service Type', align: 'left', field: 'type', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer', sortable: true },
      { name: 'project', label: 'Project', align: 'left', field: 'project', sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    const asset = computed(() => {
      let found = null
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          const a = p.assets.find(as => as.id === assetId)
          if (a) found = a
        })
      })
      return found
    })

    const history = computed(() => {
      if (!asset.value) return []
      return store.services
        .filter(s => s.unitRef === asset.value.unitRef)
        .filter(row => {
          return Object.keys(columnFilters).every(key => {
            if (!columnFilters[key]) return true
            return String(row[key] || '').toLowerCase().includes(columnFilters[key].toLowerCase())
          })
        })
        .sort((a, b) => new Date(b.date) - new Date(a.date))
    })

    const printRecord = (record) => {
      $q.notify({
        message: 'Generating PDF report for service on ' + record.date,
        color: 'primary',
        icon: 'fas fa-file-pdf'
      })
    }

    return {
      assetId,
      asset,
      history,
      columns,
      filter,
      columnFilters,
      printRecord
    }
  }
})
</script>

<style scoped>
.history-table {
  background: white;
}
.rounded-borders {
  border-radius: 12px;
}
</style>
