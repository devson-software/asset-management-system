<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-lg">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Assets Register</div>
          <div class="text-subtitle1 text-grey-7" v-if="activeProjectName">
            Managing units for: <span class="text-weight-bold text-primary">{{ activeProjectName }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Complete repository of all installed HVAC equipment
          </div>
        </div>
        <q-space />
        <div class="q-gutter-sm">
          <q-btn v-if="$route.query.projectId" flat color="primary" icon="arrow_back" label="Show All Assets" :to="{ path: '/assets', query: {} }" />
          <q-btn color="secondary" icon="add" label="Register New Asset" @click="goToAddAsset" class="shadow-1" />
          <q-btn color="green-7" icon="file_download" label="Export CSV" @click="exportToExcel" class="shadow-1" />
          <q-btn color="secondary" icon="qr_code" label="Generate QR" @click="generateQRs" class="shadow-1" />
        </div>
      </div>

      <q-card flat bordered class="rounded-borders">
        <q-table
          :rows="rows"
          :columns="columns"
          row-key="id"
          flat
          :filter="filter"
          class="master-table"
        >
          <template v-slot:top-right>
            <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Assets...">
              <template v-slot:append><q-icon name="search" /></template>
            </q-input>
          </template>

          <template v-slot:body-cell-customer="props">
            <q-td :props="props">
              <div class="text-weight-bold">{{ props.row.customerName }}</div>
              <div class="text-caption text-grey-7">{{ props.row.projectName }}</div>
            </q-td>
          </template>

          <template v-slot:body-cell-unitRef="props">
            <q-td :props="props">
              <q-chip dense color="blue-1" text-color="primary" class="text-weight-bold">
                {{ props.row.unitRef }}
              </q-chip>
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
  </q-page>
</template>

<script>
import { defineComponent, ref, computed } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../store'

export default defineComponent({
  name: 'AssetsPage',
  setup () {
    const $q = useQuasar()
    const route = useRoute()
    const router = useRouter()
    const filter = ref('')

    const columns = [
      { name: 'customer', label: 'Client / Project', align: 'left', field: row => row.customerName, sortable: true },
      { name: 'unitRef', label: 'Unit Ref #', align: 'left', field: 'unitRef', sortable: true },
      { name: 'indoorModel', label: 'Indoor Model', align: 'left', field: 'indoorModel', sortable: true },
      { name: 'serialNumber', label: 'Serial Number', align: 'left', field: 'serialNumber', sortable: true },
      { name: 'status', label: 'Status', align: 'center', field: row => row.status },
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
            allAssets.push({
              ...asset,
              customerName: customer.name,
              projectName: project.name,
              customerId: customer.id,
              projectId: project.id
            })
          })
        })
      })
      return allAssets
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
        $q.notify({
          message: 'Please select a project first by viewing its assets',
          color: 'warning',
          icon: 'warning',
          actions: [
            { label: 'Go to Projects', color: 'white', handler: () => router.push('/projects') }
          ]
        })
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
      $q.notify({ message: 'Exporting assets register to CSV...', color: 'green-7', icon: 'file_download' })
      const header = columns.map(c => c.label).join(',')
      const data = rows.value.map(r => [
        r.customerName + ' / ' + r.projectName, r.unitRef, r.indoorModel, r.serialNumber, r.status
      ].join(',')).join('\n')
      
      const blob = new Blob([header + '\n' + data], { type: 'text/csv' })
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.setAttribute('hidden', '')
      a.setAttribute('href', url)
      a.setAttribute('download', 'assets_register.csv')
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
    }

    const printQR = (row) => {
      $q.notify({ message: `Sent QR sticker for ${row.unitRef} to label printer.`, color: 'orange-8', icon: 'print' })
    }

    return {
      filter,
      columns,
      rows,
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
