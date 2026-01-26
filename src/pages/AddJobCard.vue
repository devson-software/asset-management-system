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
              <!-- Section: Job Context & Timing -->
              <div class="row q-col-gutter-md">
                <div class="col-12 col-sm-4">
                  <q-input v-model="form.date" label="Service Date" type="date" outlined dense stack-label required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-calendar" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-sm-4">
                  <q-input v-model="form.checkInTime" label="Check-In Time" type="time" outlined dense stack-label bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-clock" size="xs" color="positive" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-sm-4">
                  <q-input v-model="form.checkOutTime" label="Check-Out Time" type="time" outlined dense stack-label bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-clock-rotate-left" size="xs" color="negative" /></template>
                  </q-input>
                </div>

                <div class="col-12 col-sm-6">
                  <q-select v-model="form.techId" :options="technicians" label="Assign Technician" outlined dense emit-value map-options required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-user-gear" size="xs" color="secondary" /></template>
                  </q-select>
                </div>
                <div class="col-12 col-sm-6">
                  <q-select v-model="form.workType" :options="workTypeOptions" label="Job Type" outlined dense required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-toolbox" size="xs" color="primary" /></template>
                  </q-select>
                </div>

                <div class="col-12">
                  <q-select v-model="form.assetId" :options="allAssets" label="Select Target Asset" outlined dense emit-value map-options required use-input @filter="filterAssets" bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-snowflake" size="xs" color="primary" /></template>
                  </q-select>
                </div>
              </div>

              <q-separator />

              <!-- Section: Fault Analysis -->
              <div class="text-subtitle1 text-weight-bold row items-center">
                <q-icon name="fas fa-magnifying-glass-chart" color="orange-9" class="q-mr-sm" />
                Fault Analysis & Diagnostics
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12">
                  <div class="q-pa-md bg-orange-1 rounded-borders border-dashed">
                    <q-toggle v-model="form.faultFound" label="Was a fault discovered during this site visit?" color="negative" class="text-weight-bold" />
                  </div>
                </div>

                <template v-if="form.faultFound">
                  <div class="col-12 col-md-4">
                    <q-input v-model="form.faultReported" label="Fault Reported" outlined dense bg-color="white" placeholder="Client's complaint..." />
                  </div>
                  <div class="col-12 col-md-4">
                    <q-input v-model="form.rootCause" label="Root Cause Found" outlined dense bg-color="white" placeholder="What failed and why?" />
                  </div>
                  <div class="col-12 col-md-4">
                    <q-input v-model="form.remedy" label="Remedy / Action Taken" outlined dense bg-color="white" placeholder="What did you do to fix it?" />
                  </div>
                </template>
              </div>

              <q-separator />

              <!-- Section: Technical Readings -->
              <div class="text-subtitle1 text-weight-bold row items-center">
                <q-icon name="fas fa-gauge-high" color="blue-9" class="q-mr-sm" />
                Technical Performance Readings
              </div>
              <div class="row q-col-gutter-sm bg-blue-50 q-pa-md rounded-borders">
                <div class="col-6 col-sm-2">
                  <q-input v-model="form.readings.suctionPressure" label="Suction (kPa)" outlined dense bg-color="white" type="number" />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input v-model="form.readings.dischargePressure" label="Disch. (kPa)" outlined dense bg-color="white" type="number" />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input v-model="form.readings.supplyTemp" label="Supply Temp (°C)" outlined dense bg-color="white" type="number" />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input v-model="form.readings.returnTemp" label="Return Temp (°C)" outlined dense bg-color="white" type="number" />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input v-model="form.readings.amps" label="Current (Amps)" outlined dense bg-color="white" type="number" />
                </div>
              </div>

              <q-separator />

              <!-- Section: Materials & Parts -->
              <div class="text-subtitle1 text-weight-bold row items-center">
                <q-icon name="fas fa-box-open" color="brown-9" class="q-mr-sm" />
                Materials & Parts Consumed
              </div>
              <div class="bg-grey-2 q-pa-md rounded-borders">
                <div class="row q-col-gutter-sm items-center q-mb-md">
                  <div class="col-8">
                    <q-input v-model="newPart.description" label="Part Description" outlined dense bg-color="white" @keyup.enter="addPart" />
                  </div>
                  <div class="col-2">
                    <q-input v-model.number="newPart.quantity" label="Qty" type="number" outlined dense bg-color="white" />
                  </div>
                  <div class="col-2">
                    <q-btn color="brown-7" icon="fas fa-plus" @click="addPart" class="full-width" />
                  </div>
                </div>

                <q-list separator bordered class="bg-white rounded-borders" v-if="form.partsUsed.length > 0">
                  <q-item v-for="(part, index) in form.partsUsed" :key="index">
                    <q-item-section avatar>
                      <q-avatar color="brown-1" text-color="brown" icon="fas fa-gears" size="30px" />
                    </q-item-section>
                    <q-item-section>
                      <q-item-label>{{ part.description }}</q-item-label>
                      <q-item-label caption>Quantity: {{ part.quantity }}</q-item-label>
                    </q-item-section>
                    <q-item-section side>
                      <q-btn flat round color="red-5" icon="fas fa-trash-can" size="sm" @click="removePart(index)" />
                    </q-item-section>
                  </q-item>
                </q-list>
                <div v-else class="text-center text-grey-6 q-pa-sm italic">No parts recorded yet</div>
              </div>

              <!-- Section: Final Comments -->
              <div class="col-12">
                <q-input v-model="form.comments" label="General Work Performed & Technician Observations" type="textarea" outlined dense placeholder="Provide any additional comments or future recommendations..." rows="4" bg-color="white">
                  <template v-slot:prepend><q-icon name="fas fa-comment-dots" size="xs" /></template>
                </q-input>
              </div>

              <div class="row justify-between q-mt-xl">
                <q-btn label="Discard & Return" flat color="grey-7" @click="$router.back()" />
                <q-btn :label="isEdit ? 'Update Job Card' : 'Confirm & Finalize Job Card'" type="submit" color="secondary" unelevated class="q-px-lg" icon="fas fa-cloud-arrow-up" />
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
      checkInTime: '',
      checkOutTime: '',
      assetId: null,
      techId: null,
      workType: 'Maintenance',
      faultFound: false,
      faultReported: '',
      rootCause: '',
      remedy: '',
      comments: '',
      partsUsed: [],
      readings: {
        suctionPressure: '',
        dischargePressure: '',
        supplyTemp: '',
        returnTemp: '',
        amps: ''
      }
    })

    const workTypeOptions = ['Maintenance', 'Repair', 'Installation', 'Emergency Callout', 'Warranty']
    const newPart = reactive({ description: '', quantity: 1 })

    const addPart = () => {
      if (newPart.description) {
        form.partsUsed.push({ ...newPart })
        newPart.description = ''
        newPart.quantity = 1
      }
    }

    const removePart = (index) => {
      form.partsUsed.splice(index, 1)
    }

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
        form.checkInTime = existingJob.checkInTime || ''
        form.checkOutTime = existingJob.checkOutTime || ''
        form.workType = existingJob.workType || 'Maintenance'
        form.faultFound = existingJob.faultFound
        form.faultReported = existingJob.faultReported || ''
        form.rootCause = existingJob.rootCause || ''
        form.remedy = existingJob.remedy || ''
        form.comments = existingJob.comments || ''
        form.partsUsed = JSON.parse(JSON.stringify(existingJob.partsUsed || []))
        form.readings = JSON.parse(JSON.stringify(existingJob.readings || {
          suctionPressure: '',
          dischargePressure: '',
          supplyTemp: '',
          returnTemp: '',
          amps: ''
        }))
        
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
        checkInTime: form.checkInTime,
        checkOutTime: form.checkOutTime,
        unitRef: selectedAsset.unitRef,
        customer: selectedAsset.customer,
        tech: selectedTech.label,
        workType: form.workType,
        faultFound: form.faultFound,
        faultReported: form.faultReported,
        rootCause: form.rootCause,
        remedy: form.remedy,
        comments: form.comments,
        partsUsed: [...form.partsUsed],
        readings: { ...form.readings }
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
          message: 'New job card finalized and saved',
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
      onSubmit,
      workTypeOptions,
      newPart,
      addPart,
      removePart
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}
</style>
