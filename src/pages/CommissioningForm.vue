<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card shadow-2">
        <q-card-section :class="headerClass + ' text-white'">
          <div class="row items-center no-wrap">
            <q-icon :name="headerIcon" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5 text-weight-bold">{{ type }} - Commissioning Record</div>
              <div class="text-subtitle2">CIBSE & ASHRAE ALIGNED PERFORMANCE LOG</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            
            <!-- SECTION 1: ASSET / PROJECT INFORMATION -->
            <div class="section-container">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-circle-info" size="xs" class="q-mr-sm" /> Asset / Project Information
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input v-model="form.project" label="Project Name" outlined dense required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-diagram-project" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.customer" label="Client" outlined dense required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.site" label="Site / Location" outlined dense required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.assetId" label="Asset ID" outlined dense required bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-tag" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.manufacturer" label="Manufacturer" outlined dense bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-industry" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.model" label="Model" outlined dense bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-box" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.serialNo" label="Serial No" outlined dense bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-barcode" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.date" label="Date" type="date" outlined dense stack-label bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-calendar-day" size="xs" /></template>
                  </q-input>
                </div>
              </div>
            </div>

            <!-- SECTION 2: ELECTRICAL MEASUREMENTS -->
            <div class="section-container">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-bolt" size="xs" class="q-mr-sm" /> Electrical Measurements
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-input v-model="form.elec.l1l2" label="L1-L2 (V)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-4">
                  <q-input v-model="form.elec.l2l3" label="L2-L3 (V)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-4">
                  <q-input v-model="form.elec.l1l3" label="L1-L3 (V)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-4">
                  <q-input v-model="form.elec.ratedAmps" label="Rated Amps" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-4">
                  <q-input v-model="form.elec.measuredAmps" label="Measured Amps" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-4">
                  <q-select v-model="form.elec.pass" :options="['Pass', 'Fail']" label="Result" outlined dense bg-color="white" />
                </div>
              </div>
            </div>

            <!-- SECTION 3: PERFORMANCE MEASUREMENTS (Dynamic based on Type) -->
            <div class="section-container" v-if="type === 'Pump'">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-faucet-drip" size="xs" class="q-mr-sm" /> Hydraulic / Performance Measurements
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.suction" label="Suction Pressure (kPa / bar)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.discharge" label="Discharge Pressure (kPa / bar)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.flowRate" label="Calculated Flow Rate (L/s)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.waterTemp" label="Water Temperature (°C)" outlined dense bg-color="white" />
                </div>
              </div>
            </div>

            <div class="section-container" v-if="type === 'Fan'">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-wind" size="xs" class="q-mr-sm" /> Air Performance Measurements
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.airflow" label="Measured Airflow (L/s)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.staticPressure" label="External Static Pressure (Pa)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.fanSpeed" label="Fan Speed (RPM or %)" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.vibration" label="Vibration Level (mm/s)" outlined dense bg-color="white" />
                </div>
              </div>
            </div>

            <div class="section-container" v-if="type === 'DX Split'">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-snowflake" size="xs" class="q-mr-sm" /> Refrigeration / Airside Performance
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.returnAir" label="Return Air Temp (Off Coil) °C" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.supplyAir" label="Supply Air Temp (On Coil) °C" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.suctionLine" label="Suction Line Temp °C" outlined dense bg-color="white" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.perf.liquidLine" label="Liquid Line Temp °C" outlined dense bg-color="white" />
                </div>
              </div>
            </div>

            <!-- SECTION 4: FUNCTIONAL CHECKS -->
            <div class="section-container">
              <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                <q-icon name="fas fa-clipboard-check" size="xs" class="q-mr-sm" /> Functional Checks
              </div>
              <q-list separator class="border-grey rounded-borders bg-white">
                <q-item v-for="(check, index) in functionalChecks" :key="index" tag="label" v-ripple>
                  <q-item-section side top>
                    <q-checkbox v-model="check.value" color="positive" />
                  </q-item-section>
                  <q-item-section>
                    <q-item-label class="text-grey-9">{{ check.label }}</q-item-label>
                  </q-item-section>
                  <q-item-section side>
                    <q-badge :color="check.value ? 'positive' : 'grey-4'" class="q-px-sm">{{ check.value ? 'YES' : 'NO' }}</q-badge>
                  </q-item-section>
                </q-item>
              </q-list>
            </div>

            <!-- SIGN-OFF -->
            <div class="section-container bg-grey-1">
              <div class="text-subtitle1 text-weight-bold text-grey-9 q-mb-md row items-center">
                <q-icon name="fas fa-signature" size="xs" class="q-mr-sm" /> Summary & Sign-off
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input v-model="form.engineer" label="Commissioned By (Engineer)" outlined dense bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-user-gear" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="form.witness" label="Witness" outlined dense bg-color="white">
                    <template v-slot:prepend><q-icon name="fas fa-user-check" size="xs" /></template>
                  </q-input>
                </div>
                <div class="col-12">
                  <q-input v-model="form.comments" label="Outstanding Issues / Comments" type="textarea" outlined dense bg-color="white" />
                </div>
              </div>
            </div>

            <div class="row q-gutter-md justify-between q-mt-xl">
              <q-btn label="Discard Report" flat color="grey-7" @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Report' : 'Finalize & Submit Report'" color="primary" type="submit" icon="fas fa-cloud-arrow-up" class="q-px-lg" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'CommissioningForm',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const typeFromRoute = route.params.type
    const recordId = route.params.recordId
    const isEdit = !!recordId

    const form = reactive({
      project: '',
      customer: '',
      site: '',
      assetId: '',
      manufacturer: '',
      model: '',
      serialNo: '',
      date: new Date().toISOString().substr(0, 10),
      elec: { l1l2: '', l2l3: '', l1l3: '', ratedAmps: '', measuredAmps: '', pass: 'Pass' },
      perf: {},
      engineer: 'Jeram HVAC',
      witness: '',
      comments: ''
    })

    const functionalChecks = reactive([
      { label: 'Correct rotation / airflow direction', value: false },
      { label: 'No abnormal noise or vibration', value: false },
      { label: 'Isolation valves open and tagged', value: false },
      { label: 'Safety interlocks proven', value: false },
      { label: 'Controller / remote operation verified', value: false },
      { label: 'Condensate drainage free and clear', value: false }
    ])

    let type = ref(typeFromRoute)

    if (isEdit) {
      const record = store.commissioningRecords.find(r => r.id === recordId)
      if (record) {
        type.value = record.type
        Object.assign(form, record.formData || {})
        // Re-map functional checks if they were stored
        if (record.functionalChecks) {
          functionalChecks.forEach((check, index) => {
            if (record.functionalChecks[index]) {
              check.value = record.functionalChecks[index].value
            }
          })
        }
        
        // Basic fields from list if formData didn't have them
        form.project = record.project
        form.customer = record.customer
        form.assetId = record.unitRef
        form.date = record.date
      }
    }

    const headerClass = computed(() => {
      if (type.value === 'Pump') return 'bg-blue-8'
      if (type.value === 'Fan') return 'bg-teal-8'
      return 'bg-orange-8'
    })

    const headerIcon = computed(() => {
      if (type.value === 'Pump') return 'water'
      if (type.value === 'Fan') return 'air'
      return 'ac_unit'
    })

    const onSubmit = () => {
      const recordData = {
        date: form.date,
        unitRef: form.assetId,
        type: type.value,
        customer: form.customer,
        project: form.project,
        status: 'Completed',
        formData: { ...form },
        functionalChecks: [...functionalChecks]
      }

      if (isEdit) {
        store.updateCommissioningRecord(recordId, recordData)
        $q.notify({ color: 'positive', message: 'Commissioning report updated', icon: 'check_circle' })
      } else {
        store.addCommissioningRecord(recordData)
        $q.notify({ color: 'positive', message: 'Commissioning report finalized and uploaded', icon: 'check_circle' })
      }
      router.push('/commissioning')
    }

    return { type, form, headerClass, headerIcon, functionalChecks, onSubmit, isEdit }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 900px;
  margin: 0 auto;
}
.section-container {
  border: 1px solid #e0e0e0;
  padding: 20px;
  border-radius: 12px;
  background: #fdfdfd;
}
.border-grey {
  border: 1px solid #e0e0e0;
}
</style>
