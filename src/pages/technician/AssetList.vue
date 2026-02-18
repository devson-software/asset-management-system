<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12">
        <div class="row items-center q-gutter-sm">
          <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
          <div>
            <div class="text-h5 text-weight-bold text-primary">Assets</div>
            <div class="text-subtitle2 text-grey-7">Register assets by manual entry or QR scan</div>
            <div v-if="projectName" class="text-subtitle2 text-primary text-weight-medium">
              {{ projectName }}
            </div>
          </div>
          <q-space />
          <q-btn-dropdown color="primary" icon="fas fa-plus" label="Add Asset" class="shadow-1">
            <q-list style="min-width: 200px">
              <q-item clickable v-close-popup @click="goToAdd('manual')">
                <q-item-section avatar>
                  <q-icon name="fas fa-pen-to-square" color="primary" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">Manual Add</q-item-label>
                  <q-item-label caption>Enter details manually</q-item-label>
                </q-item-section>
              </q-item>
              <q-item clickable v-close-popup @click="goToAdd('qr')">
                <q-item-section avatar>
                  <q-icon name="fas fa-qrcode" color="secondary" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">QR Scan Add</q-item-label>
                  <q-item-label caption>Scan asset QR code</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders q-pa-md">
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-sm-6">
              <q-select
                v-model="areaFilter"
                :options="areaOptions"
                label="Filter by Area"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              />
            </div>
            <div class="col-12 col-sm-6">
              <q-select
                v-model="locationFilter"
                :options="locationOptions"
                label="Filter by Location"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              />
            </div>
          </div>

          <q-table
            :rows="filteredAssets"
            :columns="columns"
            row-key="id"
            flat
            class="master-table"
          >
            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip dense color="blue-1" text-color="primary" size="sm">
                  {{ props.row.status || 'Registered' }}
                </q-chip>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianAssetList',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const customerId = computed(() => route.query.customerId || '')
    const projectId = computed(() => route.query.projectId || '')
    const areaFilter = ref(null)
    const locationFilter = ref(null)

    const projectName = computed(() => {
      if (!projectId.value) return ''
      const customer = store.customers.find((c) => c.id === customerId.value)
      const project = customer?.projects.find((p) => p.id === projectId.value)
      return project?.name || ''
    })

    const goToProjectActions = () => {
      if (projectId.value) {
        router.push({
          path: `/field/projects/${projectId.value}/actions`,
          query: { customerId: customerId.value, projectId: projectId.value },
        })
        return
      }
      router.push('/field/projects')
    }

    const assetRows = computed(() => {
      const rows = []
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          if (customerId.value && customer.id !== customerId.value) return
          if (projectId.value && project.id !== projectId.value) return
          project.assets.forEach((asset) => {
            rows.push({
              ...asset,
              projectName: project.name,
              siteAddress: project.siteAddress,
            })
          })
        })
      })
      return rows
    })

    const areaOptions = computed(() => {
      const set = new Set(assetRows.value.map((r) => r.vendorArea).filter(Boolean))
      return Array.from(set).map((val) => ({ label: val, value: val }))
    })

    const locationOptions = computed(() => {
      const set = new Set(
        assetRows.value.map((r) => r.vendorLocation || r.siteAddress).filter(Boolean),
      )
      return Array.from(set).map((val) => ({ label: val, value: val }))
    })

    const filteredAssets = computed(() => {
      return assetRows.value.filter((asset) => {
        if (areaFilter.value && asset.vendorArea !== areaFilter.value) return false
        const locationValue = asset.vendorLocation || asset.siteAddress
        if (locationFilter.value && locationValue !== locationFilter.value) return false
        return true
      })
    })

    const columns = [
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef' },
      { name: 'unitType', label: 'Type', align: 'left', field: 'unitType' },
      { name: 'manufacturer', label: 'Manufacturer', align: 'left', field: 'manufacturer' },
      { name: 'vendorArea', label: 'Area', align: 'left', field: 'vendorArea' },
      { name: 'vendorLocation', label: 'Location', align: 'left', field: 'vendorLocation' },
      { name: 'status', label: 'Status', align: 'left' },
    ]

    const goToAdd = (mode) => {
      router.push({
        path: '/field/assets/add',
        query: {
          customerId: customerId.value,
          projectId: projectId.value,
          mode,
        },
      })
    }

    return {
      projectName,
      areaFilter,
      locationFilter,
      areaOptions,
      locationOptions,
      filteredAssets,
      columns,
      goToAdd,
      goToProjectActions,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
