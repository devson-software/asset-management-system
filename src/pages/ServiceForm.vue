<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="service-card shadow-2">
        <q-card-section :class="headerColor + ' text-white'">
          <div class="row items-center no-wrap">
            <q-icon :name="headerIcon" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5 text-weight-bold">Maintenance Service Entry</div>
              <div class="text-subtitle2" v-if="targetAsset">
                Unit: <span class="text-weight-bold text-yellow-6">{{ targetAsset.unitRef }}</span> | 
                Client: <span class="text-weight-bold">{{ targetAsset.customerName }}</span>
              </div>
              <div class="text-subtitle2" v-else>
                Scheduled Service Report
              </div>
            </div>
          </div>
        </q-card-section>

        <!-- Relevant Asset Info Banner -->
        <q-card-section v-if="targetAsset" class="bg-grey-1 q-py-sm">
          <div class="row q-col-gutter-md text-caption text-grey-8">
            <div class="col-6 col-md-3">
              <span class="text-weight-bold">Project:</span> {{ targetAsset.projectName }}
            </div>
            <div class="col-6 col-md-3">
              <span class="text-weight-bold">Model:</span> {{ targetAsset.indoorModel }}
            </div>
            <div class="col-6 col-md-3">
              <span class="text-weight-bold">Serial:</span> {{ targetAsset.serialNumber }}
            </div>
            <div class="col-6 col-md-3">
              <span class="text-weight-bold">Refrigerant:</span> {{ targetAsset.refrigerantType }} ({{ targetAsset.refrigerantKg }}kg)
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <!-- Service Header Info -->
            <div class="section-container bg-white">
              <div class="text-subtitle1 text-primary q-mb-md row items-center">
                <q-icon name="info" class="q-mr-sm" /> Schedule Information
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-input v-model="service.lastServiceDate" label="Date of Last Service" outlined dense readonly bg-color="grey-1" />
                </div>
                <div class="col-12 col-md-4">
                  <q-select 
                    v-model="service.frequency" 
                    :options="frequencyOptions" 
                    label="Service Frequency" 
                    outlined 
                    dense 
                    @update:model-value="calculateNextService"
                  />
                </div>
                <div class="col-12 col-md-4">
                  <q-select 
                    v-model="service.checklistType" 
                    :options="['Standard HVAC', 'Cooling Tower', 'Chiller', 'Fan / FCU', 'Electrical Panel']" 
                    label="Checklist Template" 
                    outlined 
                    dense 
                  />
                </div>
                <div class="col-12 col-md-4">
                  <q-input v-model="service.nextScheduleDate" label="Planned Next Service" outlined dense readonly bg-color="blue-1" />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="service.currentServiceDate" label="Today's Service Date" outlined dense type="date" stack-label />
                </div>
                <div class="col-12 col-md-6">
                  <q-input v-model="service.timeArrived" label="Site Arrival Time" outlined dense readonly bg-color="grey-1">
                    <template v-slot:prepend><q-icon name="login" /></template>
                  </q-input>
                </div>
              </div>
            </div>

            <!-- Checklist -->
            <div class="section-container">
              <div class="text-subtitle1 text-primary q-mb-md row items-center">
                <q-icon name="checklist" class="q-mr-sm" /> Maintenance Checklist ({{ service.frequency }})
              </div>
              <div class="checklist-container overflow-hidden rounded-borders border-grey">
                <q-list separator>
                  <q-item v-for="(task, index) in checklist" :key="index" tag="label" v-ripple class="q-py-md">
                    <q-item-section side top>
                      <q-checkbox v-model="task.status" true-value="completed" false-value="not_completed" color="positive" size="lg" />
                    </q-item-section>
                    <q-item-section>
                      <q-item-label class="text-weight-bold text-grey-9">{{ task.label }}</q-item-label>
                      <q-item-label caption class="text-grey-7">{{ task.desc }}</q-item-label>
                    </q-item-section>
                    <q-item-section side class="gt-xs">
                      <q-badge :color="task.status === 'completed' ? 'positive' : 'grey-4'" rounded>
                        {{ task.status === 'completed' ? 'Done' : 'Pending' }}
                      </q-badge>
                    </q-item-section>
                  </q-item>
                </q-list>
              </div>
            </div>

            <!-- Technical Measurements -->
            <div class="section-container bg-white">
              <div class="text-subtitle1 text-primary q-mb-md row items-center">
                <q-icon name="thermostat" class="q-mr-sm" /> Performance Verification (ASHRAE 180)
              </div>
              
              <!-- Dynamic Fields Based on Equipment Type -->
              <div class="row q-col-gutter-md">
                <!-- Standard / VRF / FCU -->
                <template v-if="service.checklistType === 'Standard HVAC' || service.checklistType === 'Fan / FCU'">
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.onCoilTemp" label="Return Air Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.offCoilTemp" label="Supply Air Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.suctionPressure" label="Suction Pressure (kPa)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.dischargePressure" label="Discharge Pressure (kPa)" outlined dense />
                  </div>
                </template>

                <!-- Cooling Tower -->
                <template v-if="service.checklistType === 'Cooling Tower'">
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.hotWaterIn" label="Hot Water Inlet Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.coldWaterOut" label="Cold Water Outlet Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.fanCurrent" label="Fan Motor Current (A)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.waterLevel" label="Water Level Stability" outlined dense />
                  </div>
                </template>

                <!-- Chiller -->
                <template v-if="service.checklistType === 'Chiller'">
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.chilledWaterIn" label="Chilled Water Return Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.chilledWaterOut" label="Chilled Water Supply Temp (°C)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.compCurrent" label="Compressor Current (A)" outlined dense />
                  </div>
                  <div class="col-12 col-sm-6">
                    <q-input v-model="service.approachTemp" label="Condenser Approach (°C)" outlined dense />
                  </div>
                </template>
              </div>
            </div>

            <!-- Faults and Comments -->
            <div class="section-container">
              <div class="text-subtitle1 text-primary q-mb-sm">Status & Repairs</div>
              <q-input
                v-model="service.generalComments"
                label="Technician's general comments"
                type="textarea"
                outlined
                dense
                placeholder="Describe the overall condition of the unit..."
              />

              <div class="row items-center q-mt-md">
                <q-toggle v-model="service.faultFound" label="Any Faults Discovered?" color="negative" />
              </div>
              
              <transition appear enter-active-class="animated slideInDown">
                <div v-if="service.faultFound" class="q-mt-md q-gutter-y-sm bg-red-1 q-pa-md rounded-borders">
                  <q-input
                    v-model="service.faultDetails"
                    label="Specify the fault details"
                    type="textarea"
                    outlined
                    dense
                    bg-color="white"
                  />
                  <q-file
                    v-model="service.faultPictures"
                    label="Capture / Upload Evidence"
                    outlined
                    dense
                    multiple
                    accept="image/*"
                    use-chips
                    bg-color="white"
                  >
                    <template v-slot:prepend><q-icon name="camera_alt" /></template>
                  </q-file>
                </div>
              </transition>
            </div>

            <!-- Service Completion -->
            <div class="section-container bg-blue-1">
              <div class="text-subtitle1 text-blue-9 q-mb-md row items-center">
                <q-icon name="qr_code_scanner" class="q-mr-sm" /> Service Validation
              </div>
              <div class="row items-center justify-between">
                <div>
                  <q-btn 
                    :color="service.timeEnded ? 'positive' : 'primary'" 
                    :label="service.timeEnded ? 'Exit QR Scanned' : 'Scan Exit QR'" 
                    icon="qr_code_scanner"
                    @click="showScanner = true"
                    class="q-px-lg"
                    :disable="!!service.timeEnded"
                  />
                  <div v-if="service.timeEnded" class="text-caption text-positive text-weight-bold q-mt-xs">
                    Time Recorded: {{ service.timeEnded }}
                  </div>
                </div>

                <div class="column items-center">
                   <div class="text-caption text-grey-8 q-mb-xs">Client Digital Sign-off</div>
                   <div class="signature-box flex flex-center">
                      <q-checkbox v-model="service.signed" label="Verify Signature" color="positive" size="lg" />
                   </div>
                </div>
              </div>

              <!-- QR Scanner Dialog -->
              <q-dialog v-model="showScanner">
                <q-card style="min-width: 350px">
                  <q-card-section class="row items-center q-pb-none">
                    <div class="text-h6">Scan Exit QR Code</div>
                    <q-space />
                    <q-btn icon="close" flat round dense v-close-popup />
                  </q-card-section>

                  <q-card-section class="q-pa-md">
                    <div class="scanner-wrapper overflow-hidden rounded-borders border-grey">
                      <qrcode-stream
                        v-if="showScanner"
                        @decode="onDecode"
                        @error="onError"
                        @init="onInit"
                        class="full-width"
                      />
                    </div>
                    <div class="text-caption text-grey-7 q-mt-md text-center">
                      Align the QR code within the camera frame
                    </div>
                  </q-card-section>
                </q-card>
              </q-dialog>
            </div>

            <div class="row q-gutter-md justify-end q-mt-xl">
              <q-btn label="Discard" flat color="grey-7" @click="$router.back()" />
              <q-btn 
                label="Generate & Store Job Card" 
                type="submit" 
                color="secondary" 
                icon="cloud_done" 
                class="q-px-lg"
                :disable="!service.timeEnded || !service.signed" 
              />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'
import { QrcodeStream } from 'qrcode-reader-vue3'

export default defineComponent({
  name: 'ServiceForm',
  components: {
    QrcodeStream
  },
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const assetId = route.params.assetId
    const showScanner = ref(false)

    const targetAsset = computed(() => {
      if (!assetId) return null
      let found = null
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          const a = p.assets.find(item => item.id === assetId)
          if (a) found = { ...a, customerName: c.name, projectName: p.name }
        })
      })
      return found
    })

    const headerColor = computed(() => {
      const type = service.checklistType
      if (type === 'Cooling Tower') return 'bg-cyan-9'
      if (type === 'Chiller') return 'bg-blue-10'
      if (type === 'Fan / FCU') return 'bg-teal-8'
      if (type === 'Electrical Panel') return 'bg-amber-9'
      return 'bg-secondary'
    })

    const headerIcon = computed(() => {
      const type = service.checklistType
      if (type === 'Cooling Tower') return 'water'
      if (type === 'Chiller') return 'ac_unit'
      if (type === 'Fan / FCU') return 'air'
      if (type === 'Electrical Panel') return 'bolt'
      return 'handyman'
    })

    const service = reactive({
      lastServiceDate: '2025-12-14',
      frequency: 'Monthly',
      checklistType: 'Standard HVAC',
      nextScheduleDate: '2026-01-14',
      currentServiceDate: new Date().toISOString().substr(0, 10),
      timeArrived: new Date().toLocaleTimeString(),
      timeEnded: null,
      travelDistance: '',
      onCoilTemp: '',
      offCoilTemp: '',
      suctionPressure: '',
      dischargePressure: '',
      hotWaterIn: '',
      coldWaterOut: '',
      fanCurrent: '',
      waterLevel: '',
      chilledWaterIn: '',
      chilledWaterOut: '',
      compCurrent: '',
      approachTemp: '',
      faultFound: false,
      faultDetails: '',
      faultPictures: null,
      generalComments: '',
      signed: false
    })

    const frequencyOptions = [
      'Monthly',
      'Quarterly',
      'Semi-Annual',
      'Annual',
      'Every 3-5 Years'
    ]

    const specializedChecklists = {
      'Cooling Tower': [
        { label: 'Inspect casing & structure', desc: 'No structural damage', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Inspect fan & guards', desc: 'Check blades, balance, guards secure', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Check basin cleanliness', desc: 'Clean; no bio growth', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Check drift eliminators', desc: 'Correctly seated; intact', freq: ['Quarterly'], status: 'not_completed' },
        { label: 'Inspect fill pack', desc: 'Clean; not collapsed', freq: ['Annual'], status: 'not_completed' }
      ],
      'Chiller': [
        { label: 'Inspect refrigerant piping', desc: 'No leaks; insulation intact', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Check compressor condition', desc: 'Normal operation; no noise', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Clean condenser coils', desc: 'Coils clean; no damage', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Check oil level / condition', desc: 'Within OEM range; clean', freq: ['Quarterly'], status: 'not_completed' }
      ],
      'Fan / FCU': [
        { label: 'Clean / replace filters', desc: 'Filter clean and intact', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Inspect & clean indoor coil', desc: 'No significant fouling', freq: ['Quarterly'], status: 'not_completed' },
        { label: 'Inspect fan motor & wheel', desc: 'Clean; smooth operation', freq: ['Quarterly'], status: 'not_completed' },
        { label: 'Check electrical terminals', desc: 'Within limits; secure', freq: ['Annual'], status: 'not_completed' }
      ],
      'Electrical Panel': [
        { label: 'Check for abnormal noise/smell', desc: 'None observed', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Visual inspection for dust/insects', desc: 'Clean and dry', freq: ['Monthly'], status: 'not_completed' },
        { label: 'Torque check on busbars', desc: 'As per manufacturer torque', freq: ['Quarterly'], status: 'not_completed' },
        { label: 'Thermal Scan', desc: 'No hot spots', freq: ['Annual'], status: 'not_completed' }
      ]
    }

    const allTasks = [
      { label: 'Visual Inspection', desc: 'Check unit for abnormal noise, vibration, or leaks.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Check Controller Operation', desc: 'Ensure remote control or thermostat functions properly.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Check Drainage', desc: 'Verify condensate is draining properly, no blockages or leaks.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Clean / Inspect Air Filters', desc: 'Remove, wash, or replace filters as needed.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Indoor Coil', desc: 'Check for dust buildup, corrosion, or mold. Clean if necessary.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Outdoor Coil', desc: 'Check for debris, corrosion, or fin damage. Clean with low-pressure water.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Condensate Tray & Drain', desc: 'Clean and flush condensate tray and drain lines.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Electrical Connections', desc: 'Check for loose or burnt terminals, tighten as required.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Refrigerant Lines', desc: 'Look for insulation damage, oil stains, or leaks.', freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Test System Operation', desc: 'Run system through heating/cooling cycle; verify temperature differential.', freq: ['Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Check Refrigerant Pressure', desc: 'Connect gauges, verify within manufacturer’s specifications.', freq: ['Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Mountings & Brackets', desc: 'Check indoor & outdoor units are securely mounted.', freq: ['Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Check Electrical Current Draw', desc: 'Measure current draw vs. nameplate values.', freq: ['Quarterly', 'Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Deep Clean Coils', desc: 'Apply approved coil cleaner and rinse.', freq: ['Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Fan & Blower', desc: 'Check for dirt, balance, and bearing condition.', freq: ['Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Test Thermostat Calibration', desc: 'Check accuracy of temperature sensing.', freq: ['Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Inspect Insulation', desc: 'Check refrigerant line insulation and repair any deterioration.', freq: ['Semi-Annual', 'Annual'], status: 'not_completed' },
      { label: 'Verify Refrigerant Charge', desc: 'Confirm correct refrigerant quantity and top up if required.', freq: ['Annual'], status: 'not_completed' },
      { label: 'Check Electrical Components', desc: 'Inspect relays, contactors, capacitors, and terminals for wear.', freq: ['Annual'], status: 'not_completed' },
      { label: 'Clean Evaporator & Condenser Fans', desc: 'Remove dust buildup to improve efficiency and airflow.', freq: ['Annual'], status: 'not_completed' },
      { label: 'Calibrate System Controls', desc: 'Recalibrate controller, sensors, and time schedules.', freq: ['Annual'], status: 'not_completed' },
      { label: 'Full Performance Test', desc: 'Record temperatures, pressures, current draw, and airflow readings.', freq: ['Annual'], status: 'not_completed' },
      { label: 'Replace Electrical Components', desc: 'Replace aging contactors, capacitors, etc. (as needed)', freq: ['Every 3-5 Years'], status: 'not_completed' },
      { label: 'Overhaul Fan Motors', desc: 'Re-grease or replace bearings, if applicable.', freq: ['Every 3-5 Years'], status: 'not_completed' }
    ]

    const checklist = computed(() => {
      const tasks = service.checklistType === 'Cooling Tower' 
        ? specializedChecklists['Cooling Tower'] 
        : allTasks
      return tasks.filter(task => task.freq.includes(service.frequency))
    })

    const calculateNextService = (val) => {
      const date = new Date(service.currentServiceDate)
      if (val === 'Monthly') date.setMonth(date.getMonth() + 1)
      else if (val === 'Quarterly') date.setMonth(date.getMonth() + 3)
      else if (val === 'Semi-Annual') date.setMonth(date.getMonth() + 6)
      else if (val === 'Annual') date.setFullYear(date.getFullYear() + 1)
      else if (val === 'Every 3-5 Years') date.setFullYear(date.getFullYear() + 3)
      
      service.nextScheduleDate = date.toISOString().substr(0, 10)
    }

    const onDecode = (text) => {
      if (text) {
        showScanner.value = false
        service.timeEnded = new Date().toLocaleTimeString()
        $q.notify({ 
          color: 'positive', 
          message: 'Exit QR Scanned Successfully! Work completed.', 
          icon: 'check_circle' 
        })
      }
    }

    const onError = (err) => {
      console.error(err)
      $q.notify({ 
        color: 'negative', 
        message: 'Camera error or permission denied.', 
        icon: 'error' 
      })
    }

    const onInit = async (promise) => {
      try {
        await promise
      } catch (error) {
        if (error.name === 'NotAllowedError') {
          $q.notify({ color: 'negative', message: 'ERROR: you need to grant camera access permission' })
        } else if (error.name === 'NotFoundError') {
          $q.notify({ color: 'negative', message: 'ERROR: no camera on this device' })
        } else if (error.name === 'NotSupportedError') {
          $q.notify({ color: 'negative', message: 'ERROR: secure context required (HTTPS, localhost)' })
        } else if (error.name === 'NotReadableError') {
          $q.notify({ color: 'negative', message: 'ERROR: is the camera already in use?' })
        } else if (error.name === 'OverconstrainedError') {
          $q.notify({ color: 'negative', message: 'ERROR: installed cameras are not suitable' })
        } else if (error.name === 'StreamApiNotSupportedError') {
          $q.notify({ color: 'negative', message: 'ERROR: Stream API is not supported in this browser' })
        }
      }
    }

    const onSubmit = () => {
      const newJobCard = {
        id: `JOB-${new Date().getFullYear()}-${Math.floor(Math.random() * 1000)}`,
        date: service.currentServiceDate,
        unitRef: targetAsset.value ? targetAsset.value.unitRef : 'Unknown',
        customer: targetAsset.value ? targetAsset.value.customerName : 'Unknown',
        tech: 'Jeram HVAC',
        faultFound: service.faultFound
      }
      store.jobCards.unshift(newJobCard)
      
      $q.notify({
        color: 'positive',
        message: 'Job card successfully uploaded to cloud storage',
        icon: 'cloud_done',
        timeout: 3000
      })
      router.push('/job-cards')
    }

    return {
      targetAsset,
      headerColor,
      headerIcon,
      service,
      frequencyOptions,
      checklist,
      showScanner,
      onDecode,
      onError,
      onInit,
      calculateNextService,
      onSubmit
    }
  }
})
</script>

<style scoped>
.service-card {
  max-width: 900px;
  margin: 0 auto;
}
.section-container {
  border: 1px solid #e0e0e0;
  padding: 20px;
  border-radius: 12px;
  background: #fdfdfd;
}
.checklist-container {
  border: 1px solid #e0e0e0;
}
.signature-box {
  border: 2px dashed #bbdefb;
  border-radius: 8px;
  padding: 10px;
  background: white;
}
.border-grey {
  border: 1px solid #e0e0e0;
}
.scanner-wrapper {
  min-height: 300px;
  background: black;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>

<style scoped>
.service-card {
  max-width: 800px;
  margin: 0 auto;
}
.checklist-container {
  background: white;
  border-radius: 4px;
}
.section-container {
  border: 1px solid #e0e0e0;
  padding: 16px;
  border-radius: 8px;
}
.signature-pad {
  border: 2px dashed #ccc;
  border-radius: 8px;
  background: #fafafa;
  min-height: 120px;
}
</style>

