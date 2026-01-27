<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Assets Register</div>
          <div class="text-subtitle1 text-grey-7" v-if="activeProjectName">
            Managing equipment for: <span class="text-weight-bold text-primary">{{ activeProjectName }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Complete repository of all installed HVAC units
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn v-if="$route.query.projectId" flat color="primary" icon="fas fa-arrow-left" label="Show All Assets" :to="{ path: '/assets', query: {} }" />
          <q-btn color="primary" icon="fas fa-plus" label="Register New Asset" @click="goToAddAsset" class="shadow-2" />
          <q-btn color="green-7" icon="fas fa-file-excel" label="Export XLSX" @click="exportToExcel" class="shadow-2" />
          <q-btn color="indigo-7" icon="fas fa-qrcode" label="Generate QRs" @click="generateQRs" class="shadow-2" />
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
          class="master-table"
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
            <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Assets...">
              <template v-slot:append>
                <q-icon name="fas fa-magnifying-glass" size="xs" />
              </template>
            </q-input>
          </template>

          <template v-slot:body-cell-customerName="props">
            <q-td :props="props">
              <div class="text-weight-bold text-primary">{{ props.value }}</div>
            </q-td>
          </template>

          <template v-slot:body-cell-projectName="props">
            <q-td :props="props">
              <div class="text-grey-8">{{ props.value }}</div>
            </q-td>
          </template>

          <template v-slot:body-cell-serviceCount="props">
            <q-td :props="props">
              <q-chip 
                dense 
                :color="props.value > 0 ? 'blue-1' : 'grey-2'" 
                :text-color="props.value > 0 ? 'primary' : 'grey-7'"
                class="text-weight-bold"
              >
                {{ props.value }}
              </q-chip>
            </q-td>
          </template>

          <template v-slot:body-cell-siteAddress="props">
            <q-td :props="props">
              <div class="text-caption text-grey-8" style="max-width: 200px; white-space: normal;">
                {{ props.value }}
              </div>
            </q-td>
          </template>

          <template v-slot:body-cell-vendorArea="props">
            <q-td :props="props">
              <div class="text-weight-medium text-grey-9">{{ props.value }}</div>
            </q-td>
          </template>

          <template v-slot:body-cell-vendorLocation="props">
            <q-td :props="props">
              <q-chip dense color="blue-grey-1" text-color="blue-grey-9" size="sm">
                {{ props.value }}
              </q-chip>
            </q-td>
          </template>

          <template v-slot:body-cell-outdoorModel="props">
            <q-td :props="props">
              <div class="text-caption text-grey-7">{{ props.value || 'N/A' }}</div>
            </q-td>
          </template>

          <template v-slot:body-cell-unitRef="props">
            <q-td :props="props">
              <div class="row items-center no-wrap">
                <q-avatar color="blue-1" text-color="primary" icon="fas fa-snowflake" size="32px" class="q-mr-md" />
                <div>
                  <div class="text-weight-bold text-primary">{{ props.row.unitRef }}</div>
                  <div class="text-caption text-grey-7">{{ props.row.manufacturer }}</div>
                </div>
              </div>
            </q-td>
          </template>

          <template v-slot:body-cell-unitType="props">
            <q-td :props="props">
              <div class="row items-center no-wrap">
                <q-avatar size="32px" class="q-mr-md bg-grey-1">
                  <q-icon 
                    :name="props.row.unitType?.includes('Cassette') ? 'fas fa-square-poll-vertical' : (props.row.unitType?.includes('Split') ? 'fas fa-window-minimize' : 'fas fa-box')" 
                    size="xs" 
                    color="grey-7" 
                  />
                </q-avatar>
                <div class="text-weight-medium">{{ props.row.unitType || 'N/A' }}</div>
              </div>
            </q-td>
          </template>

          <template v-slot:body-cell-status="props">
            <q-td :props="props">
              <q-badge :color="props.row.status === 'Registered' ? 'positive' : 'orange'" rounded class="q-px-sm">
                {{ props.row.status }}
              </q-badge>
            </q-td>
          </template>

          <template v-slot:body-cell-actions="props">
            <q-td :props="props">
              <q-btn flat round color="grey-7" icon="more_vert">
                <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                  <q-list style="min-width: 180px">
                    <q-item clickable :to="'/assets/' + props.row.id">
                      <q-item-section avatar><q-icon name="visibility" color="primary" size="sm" /></q-item-section>
                      <q-item-section>Quick View</q-item-section>
                    </q-item>
                    <q-item clickable :to="'/assets/' + props.row.id + '/history'">
                      <q-item-section avatar><q-icon name="fas fa-history" color="indigo" size="sm" /></q-item-section>
                      <q-item-section>Service History</q-item-section>
                    </q-item>
                    <q-item clickable :to="'/customers/' + props.row.customerId + '/projects/' + props.row.projectId + '/assets/' + props.row.id + '/edit'">
                      <q-item-section avatar><q-icon name="edit" color="grey-7" size="sm" /></q-item-section>
                      <q-item-section>Edit Asset</q-item-section>
                    </q-item>
                    <q-separator />
                        <q-item clickable @click="printQR(props.row)">
                          <q-item-section avatar><q-icon name="print" color="orange-8" size="sm" /></q-item-section>
                          <q-item-section>Print QR Label</q-item-section>
                        </q-item>
                        <q-item clickable :to="'/technical-data/' + props.row.id">
                          <q-item-section avatar><q-icon name="analytics" color="secondary" size="sm" /></q-item-section>
                          <q-item-section>Tech Data Sheet</q-item-section>
                        </q-item>
                        <q-item clickable @click="$router.push('/service-entry')">
                      <q-item-section avatar><q-icon name="handyman" color="deep-orange" size="sm" /></q-item-section>
                      <q-item-section>New Service Record</q-item-section>
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
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../store'
import * as XLSX from 'xlsx'

export default defineComponent({
  name: 'AssetsPage',
  setup () {
    const $q = useQuasar()
    const route = useRoute()
    const router = useRouter()
    const filter = ref('')
    const columnFilters = reactive({
      unitRef: '',
      manufacturer: '',
      unitType: '',
      indoorModel: '',
      outdoorModel: '',
      vendorArea: '',
      vendorLocation: '',
      serviceCount: '',
      status: ''
    })

    const columns = [
      { name: 'unitRef', label: 'Unit Ref #', align: 'left', field: 'unitRef', sortable: true },
      { name: 'manufacturer', label: 'Manufacturer', align: 'left', field: 'manufacturer', sortable: true },
      { name: 'unitType', label: 'Type of Unit', align: 'left', field: 'unitType', sortable: true },
      { name: 'indoorModel', label: 'Indoor Model', align: 'left', field: 'indoorModel', sortable: true },
      { name: 'outdoorModel', label: 'Outdoor Model', align: 'left', field: 'outdoorModel', sortable: true },
      { name: 'vendorArea', label: 'Area', align: 'left', field: 'vendorArea', sortable: true },
      { name: 'vendorLocation', label: 'Specific Location', align: 'left', field: 'vendorLocation', sortable: true },
      { name: 'serviceCount', label: 'Call Outs', align: 'center', field: 'serviceCount', sortable: true },
      { name: 'status', label: 'Status', align: 'center', field: row => row.status, sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    const rows = computed(() => {
      const allAssets = []
      const projectIdQuery = route.query.projectId

      store.customers.forEach(customer => {
        customer.projects.forEach(project => {
          // Filter by projectId if present in query
          if (projectIdQuery && project.id !== projectIdQuery) return

          project.assets.forEach(asset => {
            const serviceCount = store.services.filter(s => 
              s.unitRef === asset.unitRef && s.customer === customer.name
            ).length

            allAssets.push({
              ...asset,
              customerName: customer.name,
              projectName: project.name,
              siteAddress: project.siteAddress || 'N/A',
              vendorArea: asset.vendorArea || 'N/A',
              vendorLocation: asset.vendorLocation || project.vendorLocation || 'N/A',
              customerId: customer.id,
              projectId: project.id,
              serviceCount
            })
          })
        })
      })
      return allAssets
    })

    const filteredRows = computed(() => {
      return rows.value.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const activeProjectName = computed(() => {
      const projectId = route.query.projectId
      if (!projectId) return null
      let foundName = null
      store.customers.forEach(c => {
        const p = c.projects.find(proj => proj.id === projectId)
        if (p) foundName = p.name
      })
      return foundName
    })

    const goToAddAsset = () => {
      const projectId = route.query.projectId
      
      if (!projectId) {
        router.push('/assets/add')
        return
      }

      let customerId = null
      store.customers.forEach(c => {
        if (c.projects.some(p => p.id === projectId)) {
          customerId = c.id
        }
      })

      if (customerId && projectId) {
        router.push(`/customers/${customerId}/projects/${projectId}/add-asset`)
      }
    }

    const generateQRs = () => {
      $q.notify({ message: 'Preparing high-resolution QR labels...', color: 'secondary', icon: 'qr_code' })
    }

    const exportToExcel = () => {
      $q.notify({ message: 'Exporting assets register to Excel...', color: 'green-7', icon: 'file_download' })
      
      // Define the columns to export
      const exportCols = [
        { label: 'Customer', field: 'customerName' },
        { label: 'Project', field: 'projectName' },
        { label: 'Unit Ref #', field: 'unitRef' },
        { label: 'Manufacturer', field: 'manufacturer' },
        { label: 'Type of Unit', field: 'unitType' },
        { label: 'Indoor Model', field: 'indoorModel' },
        { label: 'Indoor Serial', field: 'serialNumber' },
        { label: 'Outdoor Model', field: 'outdoorModel' },
        { label: 'Outdoor Serial', field: 'outdoorSerial' },
        { label: 'Area', field: 'vendorArea' },
        { label: 'Specific Location', field: 'vendorLocation' },
        { label: 'Refrigerant', field: 'refrigerantType' },
        { label: 'Charge (kg)', field: 'refrigerantKg' },
        { label: 'Service Schedule', field: 'serviceSchedule' },
        { label: 'Service Duration', field: 'serviceTime' },
        { label: 'Call Outs', field: 'serviceCount' },
        { label: 'Status', field: 'status' }
      ]

      // Prepare the data for XLSX
      const data = rows.value.map(r => {
        const rowData = {}
        exportCols.forEach(c => {
          rowData[c.label] = r[c.field] || 'N/A'
        })
        return rowData
      })

      // Create workbook and worksheet
      const worksheet = XLSX.utils.json_to_sheet(data)
      const workbook = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(workbook, worksheet, 'Assets Register')

      // Export the file
      XLSX.writeFile(workbook, `assets_register_${new Date().toISOString().split('T')[0]}.xlsx`)
    }

    const printQR = (row) => {
      $q.notify({ message: `Sent QR sticker for ${row.unitRef} to label printer.`, color: 'orange-8', icon: 'print' })
    }

    return {
      filter,
      columnFilters,
      columns,
      rows,
      filteredRows,
      activeProjectName,
      goToAddAsset,
      generateQRs,
      exportToExcel,
      printQR
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.master-table {
  background: white;
}
</style>
