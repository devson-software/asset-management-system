<template>
  <q-page padding>
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <div class="flex justify-between items-center q-mb-lg">
          <div>
            <div class="text-h4 text-weight-bold text-primary">{{ isEdit ? 'Edit Job Card' : 'Create Job Card' }}</div>
            <div class="text-subtitle1 text-grey-7">{{ isEdit ? 'Update job details and technician notes' : 'Record a manual service or repair event' }}</div>
          </div>
          <q-btn flat color="grey-7" icon="fas fa-arrow-left" label="Back to History" @click="$router.back()" />
        </div>

        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-md">
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
                  />
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
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-user-gear" color="secondary" />
                    </template>
                  </q-select>
                </div>

                <div class="col-12">
                  <q-select 
                    v-model="form.assetId" 
                    :options="allAssets" 
                    label="Select Asset" 
                    outlined 
                    dense 
                    emit-value
                    map-options
                    required
                    use-input
                    @filter="filterAssets"
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-snowflake" color="primary" />
                    </template>
                  </q-select>
                </div>

                <div class="col-12">
                  <q-toggle 
                    v-model="form.faultFound" 
                    label="Was a fault found during service?" 
                    color="negative" 
                    class="text-weight-bold"
                  />
                </div>

                <div class="col-12">
                  <q-input 
                    v-model="form.comments" 
                    label="Technician Notes / Work Performed" 
                    type="textarea" 
                    outlined 
                    dense 
                    placeholder="Enter details of the service or repair..."
                    rows="6"
                  />
                </div>
              </div>

              <div class="row justify-end q-mt-xl q-gutter-sm">
                <q-btn label="Discard" flat color="grey-7" @click="$router.back()" />
                <q-btn :label="isEdit ? 'Update Job Card' : 'Save & Store Job Card'" type="submit" color="secondary" unelevated class="q-px-xl" icon="fas fa-cloud-arrow-up" />
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
