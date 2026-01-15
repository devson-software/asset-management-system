<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card">
        <q-card-section>
          <div class="text-h5 text-positive">Register New Asset</div>
          <div class="text-subtitle2 text-grey-7">
            Project: <span class="text-weight-bold">{{ projectName }}</span>
          </div>
        </q-card-section>

        <q-card-section>
          <q-form @submit="onSubmit" class="q-gutter-md">
            <div class="text-subtitle1 text-primary">Unit Specifications</div>
            <div class="row q-col-gutter-sm">
              <div class="col-12 col-md-6">
                <q-input v-model="asset.unitRef" label="Unit Reference #" outlined dense required hint="e.g. Ac1.01" />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="asset.indoorModel" label="Indoor Model" outlined dense required />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="asset.serialNumber" label="Serial Number" outlined dense required />
              </div>
              <div class="col-12 col-md-6">
                <q-select v-model="asset.refrigerantType" :options="['R410A', 'R32', 'R22']" label="Refrigerant Type" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="asset.refrigerantKg" label="Charge (kg)" type="number" outlined dense />
              </div>
            </div>

            <div class="row q-gutter-sm justify-end q-mt-xl">
              <q-btn label="Cancel" flat @click="$router.back()" />
              <q-btn label="Complete Registration" color="positive" type="submit" icon="check" />
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
    const { customerId, projectId } = route.params
    
    const customer = store.customers.find(c => c.id === customerId)
    const project = customer ? customer.projects.find(p => p.id === projectId) : null
    const projectName = computed(() => project ? project.name : 'Unknown')

    const asset = reactive({ 
      unitRef: '', 
      indoorModel: '', 
      serialNumber: '', 
      refrigerantType: 'R410A', 
      refrigerantKg: '' 
    })

    const onSubmit = () => {
      store.addAsset(customerId, projectId, { ...asset })
      $q.notify({ color: 'positive', message: 'Asset registered successfully' })
      router.push({ path: '/assets', query: { projectId } })
    }

    return { asset, projectName, onSubmit }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 800px;
  margin: 0 auto;
}
</style>

