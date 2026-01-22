<template>
  <q-page padding>
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="bg-secondary text-white">
            <div class="row items-center no-wrap">
              <q-icon name="fas fa-file-invoice" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">{{ isEdit ? 'Update Job Card' : 'Create New Job Card' }}</div>
                <div class="text-subtitle2">{{ isEdit ? 'Refining job details and technician notes' : 'Capture a manual service or repair event for the history' }}</div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-sm-6">
                  <q-input 
                    v-model="form.date" 
                    label="Service Date" 
                    type="date" 
                    outlined 
                    dense 
                    stack-label 
                    required 
                    bg-color="white"
                  >
                    <template v-slot:prepend><q-icon name="fas fa-calendar" size="xs" /></template>
                  </q-input>
                </div>
                
                <div class="col-12 col-sm-6">
                  <q-select 
                    v-model="form.techId" 
                    :options="technicians" 
                    label="Assign Technician" 
                    outlined 
                    dense 
                    emit-value
                    map-options
                    required
                    bg-color="white"
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-user-gear" size="xs" color="secondary" />
                    </template>
                  </q-select>
                </div>

                <div class="col-12">
                  <q-select 
                    v-model="form.assetId" 
                    :options="allAssets" 
                    label="Select Target Asset" 
                    outlined 
                    dense 
                    emit-value
                    map-options
                    required
                    use-input
                    @filter="filterAssets"
                    bg-color="white"
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-snowflake" size="xs" color="primary" />
                    </template>
                  </q-select>
                </div>

                <div class="col-12">
                  <div class="q-pa-md bg-grey-1 rounded-borders border-dashed">
                    <q-toggle 
                      v-model="form.faultFound" 
                      label="Was a fault discovered during this site visit?" 
                      color="negative" 
                      class="text-weight-bold"
                    />
                  </div>
                </div>

                <div class="col-12">
                  <q-input 
                    v-model="form.comments" 
                    label="Work Performed & Technician Observations" 
                    type="textarea" 
                    outlined 
                    dense 
                    placeholder="Provide a detailed description of the service, repair or findings..."
                    rows="6"
                    bg-color="white"
                  >
                    <template v-slot:prepend><q-icon name="fas fa-comment-dots" size="xs" /></template>
                  </q-input>
                </div>
              </div>

              <div class="row justify-between q-mt-xl">
                <q-btn label="Discard & Return" flat color="grey-7" @click="$router.back()" />
                <q-btn :label="isEdit ? 'Update Job Card' : 'Confirm & Save Job Card'" type="submit" color="secondary" unelevated class="q-px-lg" icon="fas fa-cloud-arrow-up" />
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'AddJobCard',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()

    const jobId = route.params.jobId
    const isEdit = !!jobId

    const form = reactive({
      date: new Date().toISOString().substr(0, 10),
      assetId: null,
      techId: null,
      faultFound: false,
      comments: ''
    })

    const allAssetsOptions = computed(() => {
      const assets = []
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          p.assets.forEach(a => {
            assets.push({ 
              label: `${a.unitRef} - ${c.name} (${p.name})`,
              value: a.id,
              unitRef: a.unitRef,
              customer: c.name
            })
          })
        })
      })
      return assets
    })

    const technicians = computed(() => {
      return store.users
        .filter(u => u.role === 'technician' && u.active)
        .map(u => ({ label: u.fullName, value: u.id }))
    })

    // Pre-populate if in edit mode
    if (isEdit) {
      const existingJob = store.jobCards.find(j => j.id === jobId)
      if (existingJob) {
        form.date = existingJob.date.replace(/\//g, '-')
        form.faultFound = existingJob.faultFound
        form.comments = existingJob.comments || ''
        
        // Find asset ID by matching unitRef and customer
        const asset = allAssetsOptions.value.find(a => 
          a.unitRef === existingJob.unitRef && a.customer === existingJob.customer
        )
        if (asset) form.assetId = asset.value

        // Find tech ID by matching fullName
        const tech = store.users.find(u => u.fullName === existingJob.tech)
        if (tech) form.techId = tech.id
      }
    }

    const filteredAssets = ref(allAssetsOptions.value)

    const filterAssets = (val, update) => {
      if (val === '') {
        update(() => {
          filteredAssets.value = allAssetsOptions.value
        })
        return
      }

      update(() => {
        const needle = val.toLowerCase()
        filteredAssets.value = allAssetsOptions.value.filter(
          v => v.label.toLowerCase().indexOf(needle) > -1
        )
      })
    }

    const onSubmit = () => {
      const selectedAsset = allAssetsOptions.value.find(a => a.value === form.assetId)
      const selectedTech = technicians.value.find(t => t.value === form.techId)

      if (!selectedAsset || !selectedTech) {
        $q.notify({ color: 'negative', message: 'Please select an asset and a technician' })
        return
      }

      const jobData = {
        date: form.date.replace(/-/g, '/'),
        unitRef: selectedAsset.unitRef,
        customer: selectedAsset.customer,
        tech: selectedTech.label,
        faultFound: form.faultFound,
        comments: form.comments
      }

      if (isEdit) {
        store.updateJobCard(jobId, jobData)
        $q.notify({
          color: 'positive',
          message: 'Job card updated successfully',
          icon: 'fas fa-check-circle',
          position: 'top'
        })
      } else {
        store.addJobCard(jobData)
        $q.notify({
          color: 'positive',
          message: 'New job card created successfully',
          icon: 'fas fa-check-circle',
          position: 'top'
        })
      }

      // Clear form
      Object.assign(form, {
        date: new Date().toISOString().substr(0, 10),
        assetId: null,
        techId: null,
        faultFound: false,
        comments: ''
      })

      router.push('/job-cards')
    }

    return {
      form,
      allAssets: filteredAssets,
      technicians,
      isEdit,
      filterAssets,
      onSubmit
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}
</style>
