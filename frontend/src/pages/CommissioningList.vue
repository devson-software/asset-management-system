<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Commissioning Master</div>
          <div class="text-subtitle1 text-grey-7">Technical verification based on CIBSE & ASHRAE standards</div>
        </div>
        <div class="q-gutter-sm">
          <q-btn color="green-7" icon="fas fa-file-excel" label="Export XLSX" @click="exportToExcel" class="shadow-2" />
          <q-btn-dropdown color="primary" label="New Commissioning Report" icon="fas fa-plus" class="shadow-2">
            <q-list style="min-width: 200px">
              <q-item clickable v-close-popup @click="startComm('Pump')">
                <q-item-section avatar><q-icon name="fas fa-faucet-drip" color="blue" /></q-item-section>
                <q-item-section><q-item-label class="text-weight-bold">Pump / Hydraulic</q-item-label></q-item-section>
              </q-item>
              <q-item clickable v-close-popup @click="startComm('Fan')">
                <q-item-section avatar><q-icon name="fas fa-wind" color="teal" /></q-item-section>
                <q-item-section><q-item-label class="text-weight-bold">Fan / Airflow</q-item-label></q-item-section>
              </q-item>
              <q-item clickable v-close-popup @click="startComm('DX Split')">
                <q-item-section avatar><q-icon name="fas fa-snowflake" color="orange" /></q-item-section>
                <q-item-section><q-item-label class="text-weight-bold">DX Split Unit</q-item-label></q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="filteredRows"
            :columns="columns"
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
                    <!-- Sort Icon on the Left -->
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
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Records...">
                <template v-slot:append><q-icon name="fas fa-magnifying-glass" size="xs" /></template>
              </q-input>
            </template>

            <template v-slot:body-cell-id="props">
              <q-td :props="props">
                <div class="text-weight-bold text-primary">{{ props.row.id }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip 
                  :color="props.row.status === 'Completed' ? 'green-1' : 'orange-1'" 
                  :text-color="props.row.status === 'Completed' ? 'green-9' : 'orange-9'"
                  :label="props.row.status"
                  dense
                  class="text-weight-bold"
                />
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable @click="viewRecord(props.row)">
                        <q-item-section avatar><q-icon name="fas fa-file-lines" color="primary" size="sm" /></q-item-section>
                        <q-item-section>View Report</q-item-section>
                      </q-item>
                      <q-item clickable @click="deleteRecord(props.row)" class="text-negative">
                        <q-item-section avatar><q-icon name="fas fa-trash-can" color="negative" size="sm" /></q-item-section>
                        <q-item-section>Delete</q-item-section>
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
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import * as XLSX from 'xlsx'

export default defineComponent({
  name: 'CommissioningList',
  setup () {
    const router = useRouter()
    const $q = useQuasar()
    const filter = ref('')
    const columnFilters = reactive({
      id: '',
      date: '',
      unitRef: '',
      type: '',
      customer: '',
      status: ''
    })

    const columns = [
      { name: 'id', label: 'ID', align: 'left', field: 'id', sortable: true },
      { name: 'date', label: 'Date', align: 'left', field: 'date', sortable: true },
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'type', label: 'Type', align: 'left', field: 'type', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer', sortable: true },
      { name: 'status', label: 'Status', align: 'center', field: 'status', sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    const filteredRows = computed(() => {
      return store.commissioningRecords.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const startComm = (type) => {
      router.push(`/commissioning/new/${type}`)
    }

    const viewRecord = (record) => {
      router.push(`/commissioning/view/${record.id}`)
    }

    const deleteRecord = (record) => {
      const index = store.commissioningRecords.findIndex(r => r.id === record.id)
      if (index !== -1) {
        store.commissioningRecords.splice(index, 1)
      }
    }

    const exportToExcel = () => {
      $q.notify({ message: 'Exporting commissioning records to Excel...', color: 'green-7', icon: 'file_download' })
      
      const exportData = filteredRows.value.map(r => ({
        'ID': r.id,
        'Date': r.date,
        'Unit Ref': r.unitRef,
        'Type': r.type,
        'Customer': r.customer,
        'Status': r.status
      }))

      const worksheet = XLSX.utils.json_to_sheet(exportData)
      const workbook = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(workbook, worksheet, 'Commissioning Records')

      XLSX.writeFile(workbook, `commissioning_records_${new Date().toISOString().split('T')[0]}.xlsx`)
    }

    return { 
      store, 
      filter, 
      columnFilters, 
      columns, 
      filteredRows, 
      startComm, 
      viewRecord, 
      deleteRecord,
      exportToExcel
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
// This page is used to list all the commissioning records