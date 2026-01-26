<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card shadow-2">
        <q-card-section class="bg-positive text-white">
          <div class="row items-center no-wrap">
            <q-icon name="fas fa-snowflake" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5">{{ isEdit ? 'Update Asset' : 'Register New Asset' }}</div>
              <div class="text-subtitle2">{{ isEdit ? 'Editing unit specifications' : 'Adding a new unit to the site register' }}</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <!-- Project Context -->
            <div class="bg-green-1 q-pa-md rounded-borders flex items-center border-green">
              <q-avatar color="green-2" text-color="positive" icon="fas fa-location-dot" size="sm" class="q-mr-sm" />
              <div>
                <span class="text-grey-7">Adding to project:</span>
                <span class="text-weight-bold text-positive q-ml-xs">{{ projectName }}</span>
              </div>
            </div>

            <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center">
              <q-icon name="fas fa-list-check" size="xs" class="q-mr-sm" />
              Unit Specifications
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.unitRef" 
                  label="Unit Reference #" 
                  outlined 
                  dense 
                  required 
                  hint="e.g. Ac1.01"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-tag" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.indoorModel" 
                  label="Indoor Model" 
                  outlined 
                  dense 
                  required
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-box" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.serialNumber" 
                  label="Serial Number" 
                  outlined 
                  dense 
                  required
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-barcode" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="asset.refrigerantType" 
                  :options="['R410A', 'R32', 'R22', 'R404A', 'R134a']" 
                  label="Refrigerant Type" 
                  outlined 
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-gas-pump" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.refrigerantKg" 
                  label="Charge (kg)" 
                  type="number" 
                  step="0.01"
                  outlined 
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-weight-hanging" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.serviceTime" 
                  label="Service Time (Minutes)" 
                  type="number" 
                  outlined 
                  dense
                  bg-color="white"
                  hint="Time per asset for calculations"
                >
                  <template v-slot:prepend><q-icon name="fas fa-clock" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center q-mt-md">
              <q-icon name="fas fa-building" size="xs" class="q-mr-sm" />
              Base Vendor Details
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-4">
                <q-input 
                  v-model="asset.vendorLocation" 
                  label="Vendor Location" 
                  outlined 
                  dense
                  bg-color="white"
                  hint="e.g. TFG"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-4">
                <q-input 
                  v-model="asset.vendorArea" 
                  label="Vendor Area" 
                  outlined 
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-map-location-dot" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-4">
                <q-input 
                  v-model="asset.vendorAddress" 
                  label="Vendor Address" 
                  outlined 
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-map-pin" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <div class="row q-gutter-sm justify-between q-mt-xl">
              <q-btn label="Cancel" flat color="grey-7" @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Asset' : 'Complete Registration'" color="positive" type="submit" icon="fas fa-check-circle" class="q-px-md" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AddAsset',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const { customerId, projectId, assetId } = route.params
    const isEdit = !!assetId
    
    const customer = store.customers.find(c => c.id === customerId)
    const project = customer ? customer.projects.find(p => p.id === projectId) : null
    const projectName = computed(() => project ? project.name : 'Unknown')

    const asset = reactive({ 
      unitRef: '', 
      indoorModel: '', 
      serialNumber: '', 
      refrigerantType: 'R410A', 
      refrigerantKg: '',
      serviceTime: '',
      vendorLocation: '',
      vendorArea: '',
      vendorAddress: ''
    })

    if (isEdit && project) {
      const existingAsset = project.assets.find(a => a.id === assetId)
      if (existingAsset) {
        Object.assign(asset, existingAsset)
      }
    }

    const onSubmit = () => {
      if (isEdit) {
        store.updateAsset(customerId, projectId, assetId, { ...asset })
        $q.notify({ color: 'positive', message: 'Asset updated successfully' })
      } else {
        store.addAsset(customerId, projectId, { ...asset })
        $q.notify({ color: 'positive', message: 'Asset registered successfully' })
      }

      // Clear form
      Object.keys(asset).forEach(key => asset[key] = '')

      router.push({ path: '/assets', query: { projectId } })
    }

    return { asset, projectName, onSubmit, isEdit }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 800px;
  margin: 0 auto;
}
</style>

