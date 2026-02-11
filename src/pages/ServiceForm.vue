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
                Unit:
                <span class="text-weight-bold text-yellow-6">{{ targetAsset.unitRef }}</span> |
                Client: <span class="text-weight-bold">{{ targetAsset.customerName }}</span>
              </div>
              <div class="text-subtitle2" v-else>Scheduled Service Report</div>
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
              <span class="text-weight-bold">Refrigerant:</span>
              {{ targetAsset.refrigerantType }} ({{ targetAsset.refrigerantKg }}kg)
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <q-stepper
              v-model="currentStep"
              vertical
              animated
              header-nav
              @update:model-value="onStepperChange"
            >
              <q-step :name="1" title="Start Scan" icon="fas fa-qrcode" :done="!!service.timeArrived">
                <div class="section-container bg-white">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-qrcode" class="q-mr-sm" /> Start Scan
                  </div>
                  <div class="row items-center justify-between">
                    <div>
                      <q-btn
                        :color="service.timeArrived ? 'positive' : 'primary'"
                        :label="service.timeArrived ? 'Start QR Scanned' : 'Scan Start QR'"
                        icon="fas fa-qrcode"
                        @click="openScanner('start')"
                        class="q-px-lg"
                        :disable="!!service.timeArrived"
                      />
                      <div
                        v-if="service.timeArrived"
                        class="text-caption text-positive text-weight-bold q-mt-xs"
                      >
                        Time Recorded: {{ service.timeArrived }}
                      </div>
                    </div>
                    <q-btn
                      flat
                      color="primary"
                      label="Continue"
                      @click="goToStep(2)"
                      :disable="!service.timeArrived"
                    />
                  </div>
                  <div class="row q-mt-md">
                    <q-btn
                      outline
                      color="primary"
                      icon="fas fa-bolt"
                      label="Simulate Start Scan"
                      @click="simulateScan('start')"
                      :disable="!!service.timeArrived"
                    />
                  </div>
                </div>
              </q-step>

              <q-step :name="2" title="Unit J/C" icon="fas fa-file-invoice" :done="service.unitJobCardDone">
                <div class="section-container bg-white">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-file-invoice" class="q-mr-sm" /> Unit Job Card
                  </div>
                  <div class="row items-center justify-between">
                    <q-toggle
                      v-model="service.unitJobCardDone"
                      label="Unit job card checked"
                      color="primary"
                      class="text-weight-bold"
                    />
                    <q-btn
                      flat
                      color="primary"
                      label="Continue"
                      @click="goToStep(3)"
                      :disable="!service.unitJobCardDone"
                    />
                  </div>
                </div>
              </q-step>

              <q-step :name="3" title="Input Info" icon="fas fa-pen-to-square">
                <div class="section-container bg-white">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-info-circle" class="q-mr-sm" /> Schedule Information
                  </div>
                  <div class="row q-col-gutter-md">
                    <div class="col-12 col-md-4">
                      <q-input
                        v-model="service.lastServiceDate"
                        label="Date of Last Service"
                        outlined
                        dense
                        readonly
                        bg-color="grey-1"
                      />
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
                        :options="[
                          'Standard HVAC',
                          'Cooling Tower',
                          'Chiller',
                          'Fan / FCU',
                          'Electrical Panel',
                        ]"
                        label="Checklist Template"
                        outlined
                        dense
                      />
                    </div>
                    <div class="col-12 col-md-4">
                      <q-input
                        v-model="service.nextScheduleDate"
                        label="Planned Next Service"
                        outlined
                        dense
                        readonly
                        bg-color="blue-1"
                      />
                    </div>
                    <div class="col-12 col-md-6">
                      <q-input
                        v-model="service.currentServiceDate"
                        label="Today's Service Date"
                        outlined
                        dense
                        type="date"
                        stack-label
                      />
                    </div>
                    <div class="col-12 col-md-6">
                      <q-input
                        v-model="service.timeArrived"
                        label="Site Arrival Time"
                        outlined
                        dense
                        readonly
                        bg-color="grey-1"
                      >
                        <template v-slot:prepend><q-icon name="fas fa-sign-in-alt" /></template>
                      </q-input>
                    </div>
                  </div>
                </div>

                <div class="section-container">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-check-square" class="q-mr-sm" /> Maintenance Checklist ({{
                      service.frequency
                    }})
                  </div>
                  <div class="checklist-container overflow-hidden rounded-borders border-grey">
                    <q-list separator>
                      <q-item
                        v-for="(task, index) in checklist"
                        :key="index"
                        tag="label"
                        v-ripple
                        class="q-py-md"
                      >
                        <q-item-section side top>
                          <q-checkbox
                            v-model="task.status"
                            true-value="completed"
                            false-value="not_completed"
                            color="positive"
                            size="lg"
                          />
                        </q-item-section>
                        <q-item-section>
                          <q-item-label class="text-weight-bold text-grey-9">{{
                            task.label
                          }}</q-item-label>
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

                <div class="section-container bg-white">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-thermometer-half" class="q-mr-sm" /> Performance Verification
                    (ASHRAE 180)
                  </div>

                  <div class="row q-col-gutter-md">
                    <template
                      v-if="
                        service.checklistType === 'Standard HVAC' ||
                        service.checklistType === 'Fan / FCU'
                      "
                    >
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.onCoilTemp"
                          label="Return Air Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.offCoilTemp"
                          label="Supply Air Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.suctionPressure"
                          label="Suction Pressure (kPa)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.dischargePressure"
                          label="Discharge Pressure (kPa)"
                          outlined
                          dense
                        />
                      </div>
                    </template>

                    <template v-if="service.checklistType === 'Cooling Tower'">
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.hotWaterIn"
                          label="Hot Water Inlet Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.coldWaterOut"
                          label="Cold Water Outlet Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.fanCurrent"
                          label="Fan Motor Current (A)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.waterLevel"
                          label="Water Level Stability"
                          outlined
                          dense
                        />
                      </div>
                    </template>

                    <template v-if="service.checklistType === 'Chiller'">
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.chilledWaterIn"
                          label="Chilled Water Return Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.chilledWaterOut"
                          label="Chilled Water Supply Temp (°C)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.compCurrent"
                          label="Compressor Current (A)"
                          outlined
                          dense
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-input
                          v-model="service.approachTemp"
                          label="Condenser Approach (°C)"
                          outlined
                          dense
                        />
                      </div>
                    </template>
                  </div>
                </div>

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
                    <q-toggle
                      v-model="service.faultFound"
                      label="Any Faults Discovered?"
                      color="negative"
                    />
                  </div>

                  <transition appear enter-active-class="animated slideInDown">
                    <div
                      v-if="service.faultFound"
                      class="q-mt-md q-gutter-y-sm bg-red-1 q-pa-md rounded-borders"
                    >
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
                        <template v-slot:prepend><q-icon name="fas fa-camera" /></template>
                      </q-file>
                    </div>
                  </transition>
                </div>

                <div class="row justify-end">
                  <q-btn flat color="primary" label="Continue" @click="goToStep(4)" />
                </div>
              </q-step>

              <q-step :name="4" title="Closing Scan" icon="fas fa-qrcode" :done="!!service.timeEnded">
                <div class="section-container bg-blue-1">
                  <div class="text-subtitle1 text-blue-9 q-mb-md row items-center">
                    <q-icon name="fas fa-qrcode" class="q-mr-sm" /> Closing Scan
                  </div>
                  <div class="row items-center justify-between">
                    <div>
                      <q-btn
                        :color="service.timeEnded ? 'positive' : 'primary'"
                        :label="service.timeEnded ? 'Exit QR Scanned' : 'Scan Exit QR'"
                        icon="fas fa-qrcode"
                        @click="openScanner('end')"
                        class="q-px-lg"
                        :disable="!!service.timeEnded"
                      />
                      <div
                        v-if="service.timeEnded"
                        class="text-caption text-positive text-weight-bold q-mt-xs"
                      >
                        Time Recorded: {{ service.timeEnded }}
                      </div>
                    </div>
                    <q-btn
                      flat
                      color="primary"
                      label="Continue"
                      @click="goToStep(5)"
                      :disable="!service.timeEnded"
                    />
                  </div>
                  <div class="row q-mt-md">
                    <q-btn
                      outline
                      color="primary"
                      icon="fas fa-bolt"
                      label="Simulate Closing Scan"
                      @click="simulateScan('end')"
                      :disable="!!service.timeEnded"
                    />
                  </div>
                </div>
              </q-step>

              <q-step :name="5" title="Signature" icon="fas fa-pen-fancy" :done="service.signed">
                <div class="section-container bg-white">
                  <div class="text-subtitle1 text-primary q-mb-md row items-center">
                    <q-icon name="fas fa-pen-fancy" class="q-mr-sm" /> Signature (Client / Tech)
                  </div>
                  <div class="row items-center justify-between">
                    <q-btn-toggle
                      v-model="service.signedBy"
                      :options="signatureOptions"
                      color="primary"
                      toggle-color="primary"
                      unelevated
                      size="sm"
                    />
                    <q-checkbox
                      v-model="service.signed"
                      label="Signature Captured"
                      color="positive"
                      size="lg"
                    />
                    <q-btn
                      type="submit"
                      color="secondary"
                      icon="fas fa-cloud-check"
                      label="Complete & Save"
                      :disable="!canSubmit"
                    />
                  </div>
                </div>
              </q-step>
            </q-stepper>

            <q-dialog v-model="showScanner">
              <q-card style="min-width: 350px">
                <q-card-section class="row items-center q-pb-none">
                  <div class="text-h6">
                    {{ scannerMode === 'start' ? 'Scan Start QR Code' : 'Scan Exit QR Code' }}
                  </div>
                  <q-space />
                  <q-btn icon="fas fa-times" flat round dense v-close-popup />
                </q-card-section>

                <q-card-section class="q-pa-md">
                  <div class="scanner-wrapper overflow-hidden rounded-borders border-grey">
                    <div class="absolute-full flex flex-center text-grey-4">
                      <div class="column items-center">
                        <q-icon name="fas fa-qrcode" size="64px" class="q-mb-sm" />
                        <div class="text-subtitle2">Dummy Scan Mode</div>
                      </div>
                    </div>
                  </div>
                  <div class="text-caption text-grey-7 q-mt-md text-center">
                    Tap "Simulate Scan" to proceed.
                  </div>
                  <div class="row justify-center q-mt-sm">
                    <q-btn
                      color="primary"
                      icon="fas fa-bolt"
                      label="Simulate Scan"
                      @click="simulateScan(scannerMode)"
                    />
                  </div>
                </q-card-section>
              </q-card>
            </q-dialog>

            <div class="row q-gutter-md justify-between q-mt-xl">
              <q-btn label="Pause & Continue Later" flat color="primary" @click="pauseProgress" />
              <q-btn label="Back to Schedule" flat color="grey-7" @click="goBackToSchedule" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'ServiceForm',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const assetId = route.params.assetId
    const showScanner = ref(false)
    const scannerMode = ref('start')
    const currentStep = ref(1)
    const maxStepReached = ref(1)
    const serviceId = route.query.serviceId ? String(route.query.serviceId) : ''

    const targetAsset = computed(() => {
      if (!assetId) return null
      let found = null
      store.customers.forEach((c) => {
        c.projects.forEach((p) => {
          const a = p.assets.find((item) => item.id === assetId)
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
      if (type === 'Cooling Tower') return 'fas fa-water'
      if (type === 'Chiller') return 'fas fa-snowflake'
      if (type === 'Fan / FCU') return 'fas fa-wind'
      if (type === 'Electrical Panel') return 'fas fa-bolt'
      return 'fas fa-tools'
    })

    const service = reactive({
      lastServiceDate: '2025-12-14',
      frequency: 'Monthly',
      checklistType: 'Standard HVAC',
      nextScheduleDate: '2026-01-14',
      currentServiceDate: new Date().toISOString().substr(0, 10),
      timeArrived: null,
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
      signed: false,
      signedBy: 'client',
      unitJobCardDone: false,
    })

    const frequencyOptions = ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual', 'Every 3-5 Years']

    const specializedChecklists = {
      'Cooling Tower': [
        {
          label: 'Inspect casing & structure',
          desc: 'No structural damage',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Inspect fan & guards',
          desc: 'Check blades, balance, guards secure',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Check basin cleanliness',
          desc: 'Clean; no bio growth',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Check drift eliminators',
          desc: 'Correctly seated; intact',
          freq: ['Quarterly'],
          status: 'not_completed',
        },
        {
          label: 'Inspect fill pack',
          desc: 'Clean; not collapsed',
          freq: ['Annual'],
          status: 'not_completed',
        },
      ],
      Chiller: [
        {
          label: 'Inspect refrigerant piping',
          desc: 'No leaks; insulation intact',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Check compressor condition',
          desc: 'Normal operation; no noise',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Clean condenser coils',
          desc: 'Coils clean; no damage',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Check oil level / condition',
          desc: 'Within OEM range; clean',
          freq: ['Quarterly'],
          status: 'not_completed',
        },
      ],
      'Fan / FCU': [
        {
          label: 'Clean / replace filters',
          desc: 'Filter clean and intact',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Inspect & clean indoor coil',
          desc: 'No significant fouling',
          freq: ['Quarterly'],
          status: 'not_completed',
        },
        {
          label: 'Inspect fan motor & wheel',
          desc: 'Clean; smooth operation',
          freq: ['Quarterly'],
          status: 'not_completed',
        },
        {
          label: 'Check electrical terminals',
          desc: 'Within limits; secure',
          freq: ['Annual'],
          status: 'not_completed',
        },
      ],
      'Electrical Panel': [
        {
          label: 'Check for abnormal noise/smell',
          desc: 'None observed',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Visual inspection for dust/insects',
          desc: 'Clean and dry',
          freq: ['Monthly'],
          status: 'not_completed',
        },
        {
          label: 'Torque check on busbars',
          desc: 'As per manufacturer torque',
          freq: ['Quarterly'],
          status: 'not_completed',
        },
        { label: 'Thermal Scan', desc: 'No hot spots', freq: ['Annual'], status: 'not_completed' },
      ],
    }

    const allTasks = [
      {
        label: 'Visual Inspection',
        desc: 'Check unit for abnormal noise, vibration, or leaks.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Check Controller Operation',
        desc: 'Ensure remote control or thermostat functions properly.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Check Drainage',
        desc: 'Verify condensate is draining properly, no blockages or leaks.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Clean / Inspect Air Filters',
        desc: 'Remove, wash, or replace filters as needed.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Indoor Coil',
        desc: 'Check for dust buildup, corrosion, or mold. Clean if necessary.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Outdoor Coil',
        desc: 'Check for debris, corrosion, or fin damage. Clean with low-pressure water.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Condensate Tray & Drain',
        desc: 'Clean and flush condensate tray and drain lines.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Electrical Connections',
        desc: 'Check for loose or burnt terminals, tighten as required.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Refrigerant Lines',
        desc: 'Look for insulation damage, oil stains, or leaks.',
        freq: ['Monthly', 'Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Test System Operation',
        desc: 'Run system through heating/cooling cycle; verify temperature differential.',
        freq: ['Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Check Refrigerant Pressure',
        desc: 'Connect gauges, verify within manufacturer’s specifications.',
        freq: ['Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Mountings & Brackets',
        desc: 'Check indoor & outdoor units are securely mounted.',
        freq: ['Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Check Electrical Current Draw',
        desc: 'Measure current draw vs. nameplate values.',
        freq: ['Quarterly', 'Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Deep Clean Coils',
        desc: 'Apply approved coil cleaner and rinse.',
        freq: ['Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Fan & Blower',
        desc: 'Check for dirt, balance, and bearing condition.',
        freq: ['Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Test Thermostat Calibration',
        desc: 'Check accuracy of temperature sensing.',
        freq: ['Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Inspect Insulation',
        desc: 'Check refrigerant line insulation and repair any deterioration.',
        freq: ['Semi-Annual', 'Annual'],
        status: 'not_completed',
      },
      {
        label: 'Verify Refrigerant Charge',
        desc: 'Confirm correct refrigerant quantity and top up if required.',
        freq: ['Annual'],
        status: 'not_completed',
      },
      {
        label: 'Check Electrical Components',
        desc: 'Inspect relays, contactors, capacitors, and terminals for wear.',
        freq: ['Annual'],
        status: 'not_completed',
      },
      {
        label: 'Clean Evaporator & Condenser Fans',
        desc: 'Remove dust buildup to improve efficiency and airflow.',
        freq: ['Annual'],
        status: 'not_completed',
      },
      {
        label: 'Calibrate System Controls',
        desc: 'Recalibrate controller, sensors, and time schedules.',
        freq: ['Annual'],
        status: 'not_completed',
      },
      {
        label: 'Full Performance Test',
        desc: 'Record temperatures, pressures, current draw, and airflow readings.',
        freq: ['Annual'],
        status: 'not_completed',
      },
      {
        label: 'Replace Electrical Components',
        desc: 'Replace aging contactors, capacitors, etc. (as needed)',
        freq: ['Every 3-5 Years'],
        status: 'not_completed',
      },
      {
        label: 'Overhaul Fan Motors',
        desc: 'Re-grease or replace bearings, if applicable.',
        freq: ['Every 3-5 Years'],
        status: 'not_completed',
      },
    ]

    const checklist = computed(() => {
      const tasks =
        service.checklistType === 'Cooling Tower'
          ? specializedChecklists['Cooling Tower']
          : allTasks
      return tasks.filter((task) => task.freq.includes(service.frequency))
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

    const simulateScan = (mode) => {
      showScanner.value = false
      if (mode === 'start') {
        service.timeArrived = new Date().toLocaleTimeString()
        $q.notify({
          color: 'positive',
          message: 'Start scan simulated.',
          icon: 'fas fa-check-circle',
        })
      } else {
        service.timeEnded = new Date().toLocaleTimeString()
        $q.notify({
          color: 'positive',
          message: 'Closing scan simulated.',
          icon: 'fas fa-check-circle',
        })
      }
    }

    const onSubmit = () => {
      if (!canSubmit.value) {
        $q.notify({ color: 'negative', message: 'Complete all steps before saving.' })
        return
      }

      const newJobCard = {
        id: `JOB-${new Date().getFullYear()}-${Math.floor(Math.random() * 1000)}`,
        date: service.currentServiceDate,
        unitRef: targetAsset.value ? targetAsset.value.unitRef : 'Unknown',
        customer: targetAsset.value ? targetAsset.value.customerName : 'Unknown',
        tech: 'Jeram HVAC',
        faultFound: service.faultFound,
      }
      store.jobCards.unshift(newJobCard)

      $q.notify({
        color: 'positive',
        message: 'Job card successfully uploaded to cloud storage',
        icon: 'cloud_done',
        timeout: 3000,
      })
      clearProgress()
      goBackToSchedule()
    }

    const openScanner = (mode) => {
      scannerMode.value = mode
      showScanner.value = true
    }

    const goToStep = (step) => {
      currentStep.value = step
      if (step > maxStepReached.value) {
        maxStepReached.value = step
      }
    }

    const onStepperChange = (nextStep) => {
      if (nextStep <= maxStepReached.value) {
        currentStep.value = nextStep
      } else {
        currentStep.value = maxStepReached.value
      }
    }

    const signatureOptions = [
      { label: 'Client', value: 'client' },
      { label: 'Technician', value: 'technician' },
    ]

    const progressKey = computed(() => {
      const id = serviceId || assetId || 'manual'
      return `service-progress:${id}`
    })

    const saveProgress = () => {
      const payload = {
        currentStep: currentStep.value,
        service: {
          lastServiceDate: service.lastServiceDate,
          frequency: service.frequency,
          checklistType: service.checklistType,
          nextScheduleDate: service.nextScheduleDate,
          currentServiceDate: service.currentServiceDate,
          timeArrived: service.timeArrived,
          timeEnded: service.timeEnded,
          travelDistance: service.travelDistance,
          onCoilTemp: service.onCoilTemp,
          offCoilTemp: service.offCoilTemp,
          suctionPressure: service.suctionPressure,
          dischargePressure: service.dischargePressure,
          hotWaterIn: service.hotWaterIn,
          coldWaterOut: service.coldWaterOut,
          fanCurrent: service.fanCurrent,
          waterLevel: service.waterLevel,
          chilledWaterIn: service.chilledWaterIn,
          chilledWaterOut: service.chilledWaterOut,
          compCurrent: service.compCurrent,
          approachTemp: service.approachTemp,
          faultFound: service.faultFound,
          faultDetails: service.faultDetails,
          generalComments: service.generalComments,
          signed: service.signed,
          signedBy: service.signedBy,
          unitJobCardDone: service.unitJobCardDone,
        },
      }
      localStorage.setItem(progressKey.value, JSON.stringify(payload))
    }

    const loadProgress = () => {
      const raw = localStorage.getItem(progressKey.value)
      if (!raw) return
      try {
        const parsed = JSON.parse(raw)
        if (parsed?.service) {
          delete parsed.service.captureScanData
          delete parsed.service.qrGenerated
          Object.assign(service, parsed.service)
        }
        if (parsed?.currentStep) {
          currentStep.value = parsed.currentStep
          maxStepReached.value = Math.max(maxStepReached.value, parsed.currentStep)
        }
      } catch (err) {
        console.log(err)
        localStorage.removeItem(progressKey.value)
      }
    }

    const clearProgress = () => {
      localStorage.removeItem(progressKey.value)
    }

    const pauseProgress = () => {
      saveProgress()
      $q.notify({ color: 'info', message: 'Progress saved. You can continue later.' })
    }

    const goBackToSchedule = () => {
      const basePath = route.path.startsWith('/field') ? '/field/service-schedule' : '/service-calendar'
      router.push(basePath)
    }

    const canSubmit = computed(() => {
      return (
        !!service.timeArrived &&
        !!service.unitJobCardDone &&
        !!service.timeEnded &&
        !!service.signed
      )
    })

    onMounted(() => {
      loadProgress()
    })

    watch(
      () => ({ ...service, currentStep: currentStep.value }),
      () => saveProgress(),
      { deep: true },
    )

    return {
      targetAsset,
      headerColor,
      headerIcon,
      service,
      frequencyOptions,
      checklist,
      showScanner,
      scannerMode,
      currentStep,
      maxStepReached,
      signatureOptions,
      canSubmit,
      openScanner,
      goToStep,
      onStepperChange,
      pauseProgress,
      goBackToSchedule,
      simulateScan,
      calculateNextService,
      onSubmit,
    }
  },
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
