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
          <q-btn
            color="primary"
            icon="fas fa-pen-to-square"
            label="Manual Add"
            class="shadow-1"
            @click="goToAdd('manual')"
          />
          <q-btn
            color="secondary"
            icon="fas fa-qrcode"
            label="QR Scan Add"
            class="shadow-1"
            @click="goToAdd('qr')"
          />
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

          <div v-if="filteredAssets.length" class="column q-gutter-md">
            <q-card
              v-for="asset in filteredAssets"
              :key="asset.id"
              flat
              bordered
              class="rounded-borders asset-card"
            >
              <q-card-section>
                <div class="row items-center justify-between">
                  <div class="text-subtitle1 text-weight-bold">{{ asset.unitRef || 'Unit' }}</div>
                  <q-chip dense color="blue-1" text-color="primary" size="sm">
                    {{ asset.status || 'Registered' }}
                  </q-chip>
                </div>
                <div class="row q-gutter-x-md q-mt-sm">
                  <div class="text-caption text-grey-7 row items-center">
                    <q-icon name="fas fa-layer-group" size="12px" class="q-mr-xs" />
                    {{ asset.unitType || 'Type not set' }}
                  </div>
                  <div class="text-caption text-grey-7 row items-center">
                    <q-icon name="fas fa-building" size="12px" class="q-mr-xs" />
                    {{ asset.manufacturer || 'Manufacturer' }}
                  </div>
                </div>
                <div class="text-caption text-grey-7 q-mt-xs">
                  <q-icon name="fas fa-location-dot" size="12px" class="q-mr-xs" />
                  {{ asset.vendorLocation || asset.siteAddress || 'Location not set' }}
                </div>
                <div v-if="asset.vendorArea" class="text-caption text-grey-7 q-mt-xs">
                  <q-icon name="fas fa-map" size="12px" class="q-mr-xs" />
                  {{ asset.vendorArea }}
                </div>
              </q-card-section>
            </q-card>
          </div>

          <q-card v-else flat bordered class="rounded-borders">
            <q-card-section class="text-center text-grey-7">
              No assets found for this project yet.
            </q-card-section>
          </q-card>
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

.asset-card {
  background: #fff;
}
</style>
