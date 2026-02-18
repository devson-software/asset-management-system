<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div class="row items-center no-wrap">
          <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
          <div>
            <div class="text-h5 text-weight-bold text-primary">Job Cards</div>
            <div class="text-subtitle2 text-grey-7">View job history and add new cards</div>
          </div>
        </div>
        <q-btn
          color="primary"
          icon="fas fa-plus"
          label="Add Job Card"
          class="shadow-1"
          @click="goToAdd"
        />
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders q-pa-md">
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-sm-4">
              <q-select
                v-model="buildingFilter"
                :options="buildingOptions"
                label="Filter by Building"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              />
            </div>
            <div class="col-12 col-sm-4">
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
            <div class="col-12 col-sm-4">
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
            :rows="filteredRows"
            :columns="columns"
            row-key="id"
            flat
            class="master-table"
          >
            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip
                  dense
                  :color="props.row.faultFound ? 'negative' : 'positive'"
                  text-color="white"
                  size="sm"
                  class="text-weight-bold"
                >
                  {{ props.row.faultFound ? 'Fault Found' : 'No Fault' }}
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
import { useRouter, useRoute } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianJobCardList',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const buildingFilter = ref(null)
    const areaFilter = ref(null)
    const locationFilter = ref(null)

    const assetIndex = computed(() => {
      const map = new Map()
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          project.assets.forEach((asset) => {
            map.set(asset.unitRef, {
              customerName: customer.fullName || customer.name,
              building: project.name,
              area: asset.vendorArea || '',
              location: asset.vendorLocation || project.siteAddress || '',
            })
          })
        })
      })
      return map
    })

    const rows = computed(() => {
      return store.jobCards.map((job) => {
        const assetMeta = assetIndex.value.get(job.unitRef) || {}
        return {
          ...job,
          building: assetMeta.building || job.project || '',
          area: assetMeta.area || '',
          location: assetMeta.location || '',
        }
      })
    })

    const buildingOptions = computed(() => {
      const set = new Set(rows.value.map((r) => r.building).filter(Boolean))
      return Array.from(set).map((val) => ({ label: val, value: val }))
    })

    const areaOptions = computed(() => {
      const set = new Set(rows.value.map((r) => r.area).filter(Boolean))
      return Array.from(set).map((val) => ({ label: val, value: val }))
    })

    const locationOptions = computed(() => {
      const set = new Set(rows.value.map((r) => r.location).filter(Boolean))
      return Array.from(set).map((val) => ({ label: val, value: val }))
    })

    const filteredRows = computed(() => {
      return rows.value.filter((row) => {
        if (buildingFilter.value && row.building !== buildingFilter.value) return false
        if (areaFilter.value && row.area !== areaFilter.value) return false
        if (locationFilter.value && row.location !== locationFilter.value) return false
        return true
      })
    })

    const columns = [
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef' },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer' },
      { name: 'building', label: 'Building', align: 'left', field: 'building' },
      { name: 'area', label: 'Area', align: 'left', field: 'area' },
      { name: 'location', label: 'Location', align: 'left', field: 'location' },
      { name: 'date', label: 'Date', align: 'left', field: 'date' },
      { name: 'tech', label: 'Tech', align: 'left', field: 'tech' },
      { name: 'status', label: 'Status', align: 'left' },
    ]

    const goToProjectActions = () => {
      const projectId = route.query.projectId || ''
      const customerId = route.query.customerId || ''
      if (projectId) {
        router.push({ path: `/field/projects/${projectId}/actions`, query: { customerId, projectId } })
        return
      }
      router.push('/field/projects')
    }

    const goToAdd = () => {
      router.push('/field/job-cards/add')
    }

    return {
      buildingFilter,
      areaFilter,
      locationFilter,
      buildingOptions,
      areaOptions,
      locationOptions,
      filteredRows,
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

.job-card-item {
  background: #fff;
}
</style>
