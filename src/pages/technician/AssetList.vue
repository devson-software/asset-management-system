<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12">
        <div class="row items-center q-gutter-sm">
          <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
          <div>
            <div class="text-h5 text-weight-bold text-primary">Assets</div>
            <div class="text-subtitle2 text-grey-7">Register assets by manual entry or QR scan</div>
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
        <div v-if="assetRows.length" class="column q-gutter-md">
          <q-card
            v-for="asset in assetRows"
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
              <div class="text-caption text-grey-7">{{ asset.projectName }} · {{ asset.customerName }}</div>
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
            </q-card-section>
          </q-card>
        </div>

        <q-card v-else flat bordered class="rounded-borders">
          <q-card-section class="text-center text-grey-7">
            No assets found for this project yet.
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianAssetList',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const customerId = computed(() => route.query.customerId || '')
    const projectId = computed(() => route.query.projectId || '')

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
              customerName: customer.fullName,
              projectName: project.name,
              siteAddress: project.siteAddress,
            })
          })
        })
      })
      return rows
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

    return { assetRows, goToAdd, goToProjectActions }
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
