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
                <div class="text-subtitle2">
                  {{
                    isEdit
                      ? 'Refining job details and technician notes'
                      
                      : 'Capture a manual service or repair event for the history'
                  }}
                </div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <!-- Section: Job Context & Timing -->
              <div class="row q-col-gutter-md">
                <div class="col-12 col-sm-4">
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
                <div class="col-12 col-sm-4">
                  <q-input
                    v-model="form.checkInTime"
                    label="Check-In Time"
                    type="time"
                    outlined
                    dense
                    stack-label
                    bg-color="white"
                  >
                    <template v-slot:prepend
                      ><q-icon name="fas fa-clock" size="xs" color="positive"
                    /></template>
                  </q-input>
                </div>
                <div class="col-12 col-sm-4">
                  <q-input
                    v-model="form.checkOutTime"
                    label="Check-Out Time"
                    type="time"
                    outlined
                    dense
                    stack-label
                    bg-color="white"
                  >
                    <template v-slot:prepend
                      ><q-icon name="fas fa-clock-rotate-left" size="xs" color="negative"
                    /></template>
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
                    <template v-slot:prepend
                      ><q-icon name="fas fa-user-gear" size="xs" color="secondary"
                    /></template>
                  </q-select>
                </div>
                <div class="col-12 col-sm-6">
                  <q-select
                    v-model="form.workType"
                    :options="workTypeOptions"
                    label="Job Type"
                    outlined
                    dense
                    required
                    bg-color="white"
                  >
                    <template v-slot:prepend
                      ><q-icon name="fas fa-toolbox" size="xs" color="primary"
                    /></template>
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
                    <template v-slot:prepend
                      ><q-icon name="fas fa-snowflake" size="xs" color="primary"
                    /></template>
                  </q-select>
                </div>
                <div class="col-12">
                  <q-input
                    v-model="form.unitRef"
                    label="Traveling Distance (km)"
                    outlined
                    dense
                    bg-color="white"
                  />
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
                    <q-toggle
                      v-model="form.faultFound"
                      label="Was a fault discovered during this site visit?"
                      color="negative"
                      class="text-weight-bold"
                    />
                  </div>
                </div>

                <!-- Keep original top-level fault fields -->
                <template v-if="form.faultFound">
                  <div class="col-12 col-md-4">
                    <q-input
                      v-model="form.faultReported"
                      label="Fault Reported"
                      outlined
                      dense
                      bg-color="white"
                      placeholder="Client's complaint..."
                    />
                  </div>
                  <div class="col-12 col-md-4">
                    <q-input
                      v-model="form.rootCause"
                      label="Fault Found"
                      outlined
                      dense
                      bg-color="white"
                      placeholder="What failed and why?"
                    />
                  </div>
                  <div class="col-12 col-md-4">
                    <q-input
                      v-model="form.remedy"
                      label="Remedy / Action Taken"
                      outlined
                      dense
                      bg-color="white"
                      placeholder="What did you do to fix it?"
                    />
                  </div>
                </template>

                <!-- Service-entry style multiple faults list -->
                <transition appear enter-active-class="animated slideInDown">
                  <div
                    v-if="form.faultFound"
                    class="col-12 q-mt-md q-gutter-y-sm bg-red-1 q-pa-md rounded-borders"
                  >
                    <div
                      v-for="(fault, index) in form.faults"
                      :key="`fault-${index}`"
                      class="bg-white q-pa-sm rounded-borders"
                    >
                      <div class="row items-center justify-between q-mb-xs">
                        <div class="text-caption text-grey-7">Fault {{ index + 1 }}</div>
                        <q-btn
                          v-if="form.faults.length > 1"
                          flat
                          dense
                          icon="fas fa-trash-can"
                          color="negative"
                          @click="removeFault(index)"
                        />
                      </div>
                      <!-- <q-input
                        v-model="fault.details"
                        label="Specify the fault details"
                        type="textarea"
                        outlined
                        dense
                        bg-color="white"
                      /> -->
                      <q-file
                        v-model="fault.pictures"
                        label="Capture / Upload Evidence"
                        outlined
                        dense
                        multiple
                        accept="image/*"
                        use-chips
                        bg-color="white"
                        class="q-mt-sm"
                      >
                        <template v-slot:prepend><q-icon name="fas fa-camera" /></template>
                      </q-file>
                    </div>
                    <q-btn
                      class="full-width q-mt-sm"
                      color="negative"
                      unelevated
                      icon="fas fa-plus"
                      label="Add Another Fault"
                      @click="addFault"
                    />
                  </div>
                </transition>
              </div>

              <q-separator />

              <!-- Section: Technical Readings -->
               <!-- {{ form.workType }} -->
              <div class="text-subtitle1 text-weight-bold row items-center">
                <q-icon name="fas fa-gauge-high" color="blue-9" class="q-mr-sm" />
                Technical Performance Readings
              </div>
              <div v-if="form.workType != 'Repair'" class="row q-col-gutter-sm bg-blue-50 q-pa-md rounded-borders">
                <div class="col-6 col-sm-2">
                  <q-input
                    v-model="form.readings.suctionPressure"
                    label="Suction (kPa)"
                    outlined
                    dense
                    bg-color="white"
                    type=""
                  />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input
                    v-model="form.readings.dischargePressure"
                    label="Disch. (kPa)"
                    outlined
                    dense
                    bg-color="white"
                    type=""
                  />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input
                    v-model="form.readings.supplyTemp"
                    label="Supply Temp (°C)"
                    outlined
                    dense
                    bg-color="white"
                    type=""
                  />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input
                    v-model="form.readings.returnTemp"
                    label="Return Temp (°C)"
                    outlined
                    dense
                    bg-color="white"
                    type=""
                  />
                </div>
                <div class="col-6 col-sm-2">
                  <q-input
                    v-model="form.readings.amps"
                    label="Current (Amps)"
                    outlined
                    dense
                    bg-color="white"
                    type=""
                  />
                </div>
              </div>

              <!-- Performance verification (yellow items) - ASHRAE 180 -->
              <div
                v-if="isDxMaintenance"
                class="q-mt-md q-pa-sm bg-amber-1 rounded-borders"
              >
                <div class="text-caption text-weight-bold q-mb-xs">
                  Performance Verification (ASHRAE 180)
                </div>
                <div class="row q-col-gutter-sm">
                  <div
                    v-for="field in DX_INPUT_TEMPLATE"
                    :key="field.id"
                    class="col-12 col-sm-6"
                  >
                    <q-input
                      v-model="form.maintenanceInputs[field.id]"
                      :label="field.label"
                      outlined
                      dense
                      bg-color="white"
                    />
                  </div>
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
                    <q-input
                      v-model="newPart.description"
                      label="Part Description"
                      outlined
                      dense
                      bg-color="white"
                      @keyup.enter="addPart"
                    />
                  </div>
                  <div class="col-2">
                    <q-input
                      v-model.number="newPart.quantity"
                      label="Qty"
                      type="number"
                      outlined
                      dense
                      bg-color="white"
                    />
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
                      <q-btn
                        flat
                        round
                        color="red-5"
                        icon="fas fa-trash-can"
                        size="sm"
                        @click="removePart(index)"
                      />
                    </q-item-section>
                  </q-item>
                </q-list>
                <div v-else class="text-center text-grey-6 q-pa-sm italic">No parts recorded yet</div>
              </div>

              <!-- Section: Final Comments -->
              <div class="col-12">
                <q-input
                  v-model="form.comments"
                  label="General Work Performed & Technician Observations"
                  type="textarea"
                  outlined
                  dense
                  placeholder="Provide any additional comments or future recommendations..."
                  rows="4"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-comment-dots" size="xs" /></template>
                </q-input>
              </div>

              <!-- Section: Invoice & Quotation Status -->
              <div class="col-12 q-mt-md">
                <div class="text-subtitle1 text-weight-bold row items-center q-mb-sm">
                  <q-icon name="fas fa-file-invoice-dollar" color="orange-9" class="q-mr-sm" />
                  Quotation
                </div>
                <div class="row q-col-gutter-md">
                  <div class="col-12 col-sm-4">
                    <q-toggle
                      v-model="form.invoiced"
                      label="Quotation"
                      color="primary"
                      class="text-weight-bold"
                    />
                  </div>
                  <!-- <div class="col-12 col-sm-4">
                    <q-input
                      v-model="form.quotationNumber"
                      label="Quotation Number"
                      outlined
                      dense
                      bg-color="white"
                      placeholder="e.g. Q-2026-001"
                    />
                  </div>
                  <div class="col-12 col-sm-4">
                    <q-select
                      v-model="form.quotationStatus"
                      :options="['Not Required', 'Awaiting Approval', 'Approved', 'Rejected']"
                      label="Quotation Status"
                      outlined
                      dense
                      bg-color="white"
                    />
                  </div> -->
                </div>
              </div>

              <!-- Section: Signature -->
              <div class="col-12 q-mt-md">
                <div class="text-subtitle1 text-weight-bold row items-center q-mb-sm">
                  <q-icon name="fas fa-pen-fancy" color="primary" class="q-mr-sm" />
                  Signature
                </div>
                <div class="row q-col-gutter-sm">
                  <div class="col-6">
                    <q-btn
                      class="full-width"
                      label="Technician"
                      color="primary"
                      :outline="form.signedBy !== 'technician'"
                      :text-color="form.signedBy === 'technician' ? 'white' : 'primary'"
                      unelevated
                      @click="form.signedBy = 'technician'"
                    />
                  </div>
                  <div class="col-6">
                    <q-btn
                      class="full-width"
                      label="Customer"
                      color="secondary"
                      :outline="form.signedBy !== 'customer'"
                      :text-color="form.signedBy === 'customer' ? 'white' : 'secondary'"
                      unelevated
                      @click="form.signedBy = 'customer'"
                    />
                  </div>
                </div>
                <div class="signature-pad q-mt-md flex flex-center text-grey-6">
                  Sign here
                </div>
                <q-checkbox
                  v-model="form.signed"
                  label="Signature Captured"
                  color="positive"
                  size="xl"
                  class="full-width signature-checkbox q-mt-md"
                />
              </div>

              <div class="row justify-between q-mt-xl">
                <q-btn label="Discard & Return" flat color="grey-7" @click="$router.back()" />
                <q-btn
                  :label="isEdit ? 'Update Job Card' : 'Confirm & Finalize Job Card'"
                  type="submit"
                  color="secondary"
                  unelevated
                  class="q-px-lg"
                  icon="fas fa-cloud-arrow-up"
                  :disable="!form.signed"
                />
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed, ref, watch } from 'vue'
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

    const DX_CHECKLIST_BASE = [
      // Indoor unit – airside / hygiene
      { id: 'indoor_inspect_air_filters', label: 'Inspect air filters', section: 'indoor', defaultFrequency: 'Monthly' },
      { id: 'indoor_clean_replace_filters', label: 'Clean / replace filters', section: 'indoor', defaultFrequency: 'Quarterly' },
      { id: 'indoor_inspect_evaporator_coil', label: 'Inspect evaporator coil', section: 'indoor', defaultFrequency: 'Quarterly' },
      { id: 'indoor_clean_evaporator_coil', label: 'Clean evaporator coil', section: 'indoor', defaultFrequency: 'Annually' },
      { id: 'indoor_inspect_condensate_tray', label: 'Inspect condensate tray', section: 'indoor', defaultFrequency: 'Quarterly' },
      { id: 'indoor_flush_condensate_drain', label: 'Flush condensate drain', section: 'indoor', defaultFrequency: 'Quarterly' },
      { id: 'indoor_inspect_fan_wheel', label: 'Inspect indoor fan & wheel', section: 'indoor', defaultFrequency: 'Annually' },
      { id: 'indoor_check_controller', label: 'Check controller operation', section: 'indoor', defaultFrequency: 'Annually' },
      // Outdoor unit – refrigeration / electrical
      { id: 'outdoor_inspect_condenser_coil', label: 'Inspect condenser coil', section: 'outdoor', defaultFrequency: 'Quarterly' },
      { id: 'outdoor_clean_condenser_coil', label: 'Clean condenser coil', section: 'outdoor', defaultFrequency: 'Annually' },
      { id: 'outdoor_check_compressor_op', label: 'Check compressor operation', section: 'outdoor', defaultFrequency: 'Quarterly' },
      { id: 'outdoor_record_running_amps', label: 'Record compressor running amps', section: 'outdoor', defaultFrequency: 'Quarterly' },
      { id: 'outdoor_check_fan_operation', label: 'Check condenser fan operation', section: 'outdoor', defaultFrequency: 'Quarterly' },
      { id: 'outdoor_inspect_refrigerant_pipework', label: 'Inspect refrigerant pipework', section: 'outdoor', defaultFrequency: 'Quarterly' },
      { id: 'outdoor_leak_inspection', label: 'Leak inspection', section: 'outdoor', defaultFrequency: 'Annually' },
      { id: 'outdoor_electrical_terminals', label: 'Check electrical terminals', section: 'outdoor', defaultFrequency: 'Annually' },
    ]

    const DX_HIDEAWAY_EXTRAS = [
      { id: 'indoor_check_return_air_path', label: 'Check return air path and grilles (hide-away)', section: 'indoor', defaultFrequency: 'Quarterly' },
      { id: 'indoor_check_ceiling_plenum', label: 'Inspect ceiling plenum access & seals', section: 'indoor', defaultFrequency: 'Annually' },
    ]

    const DX_INPUT_TEMPLATE = [
      { id: 'recordedVoltage', label: 'Recorded voltage' },
      { id: 'returnAirTemp', label: 'Return air temperature (°C)' },
      { id: 'supplyAirTemp', label: 'Supply air temperature (°C)' },
      { id: 'deltaT', label: 'Air ΔT across coil (°C)' },
      { id: 'compressorCurrent', label: 'Compressor running current (A)' },
      { id: 'unitOperation', label: 'Unit operation / alarms' },
    ]

    const frequencyOptions = [
      { label: 'Monthly', value: 'Monthly' },
      { label: 'Quarterly', value: 'Quarterly' },
      { label: 'Bi-annual', value: 'Bi-annual' },
      { label: 'Annual', value: 'Annual' },
    ]

    const dxChecklistTab = ref('indoor')

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
      faults: [],
      maintenanceTasks: [],
      maintenanceInputs: {},
      invoiced: false,
      quotationNumber: '',
      quotationStatus: 'Not Required',
      signed: false,
      signedBy: 'technician',
      comments: '',
      partsUsed: [],
      readings: {
        suctionPressure: '',
        dischargePressure: '',
        supplyTemp: '',
        returnTemp: '',
        amps: '',
      },
    })

    const workTypeOptions = ['Maintenance/Service DX split unit', 'DX split unit', 'General Job card', 'Installation', 'Emergency Callout', 'Warranty']
    const newPart = reactive({ description: '', quantity: 1 })
    const faultFile = ref(null)

    const addFault = () => {
      form.faults.push({
        details: '',
        pictures: [],
      })
    }

    const removeFault = (index) => {
      form.faults.splice(index, 1)
    }

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
      store.customers.forEach((c) => {
        c.projects.forEach((p) => {
          p.assets.forEach((a) => {
            assets.push({
              label: `${a.unitRef} - ${c.name} (${p.name})`,
              value: a.id,
              unitRef: a.unitRef,
              customer: c.name,
            })
          })
        })
      })
      return assets
    })

    const technicians = computed(() => {
      return store.users
        .filter((u) => u.role === 'technician' && u.active)
        .map((u) => ({ label: u.fullName, value: u.id }))
    })

    const isDxMaintenance = computed(
      () => form.workType === 'Maintenance/Service DX split unit',
    )

    const indoorChecklist = computed(() =>
      form.maintenanceTasks.filter((t) => t.section === 'indoor'),
    )
    const outdoorChecklist = computed(() =>
      form.maintenanceTasks.filter((t) => t.section === 'outdoor' && !t.custom),
    )
    const clientTasks = computed(() =>
      form.maintenanceTasks.filter((t) => t.custom),
    )

    const checklistInputs = computed(() =>
      DX_INPUT_TEMPLATE.map((f) => ({
        id: f.id,
        label: f.label,
        value: form.maintenanceInputs[f.id] ?? '',
      })),
    )

    const initialiseDxTemplateForAsset = (asset) => {
      // If we already have tasks populated, don't overwrite
      if (form.maintenanceTasks && form.maintenanceTasks.length) return

      const base = [...DX_CHECKLIST_BASE]
      const unitType = (asset && asset.unitType) || ''
      const isHideaway = /hide[- ]?away/i.test(unitType)
      const combined = isHideaway ? base.concat(DX_HIDEAWAY_EXTRAS) : base

      form.maintenanceTasks = combined.map((t) => ({
        ...t,
        frequency: t.defaultFrequency,
        done: false,
        required: true,
        custom: false,
      }))

      const inputsState = {}
      DX_INPUT_TEMPLATE.forEach((f) => {
        inputsState[f.id] = ''
      })
      form.maintenanceInputs = inputsState
    }

    const tryLoadPreviousDxConfig = (unitRef) => {
      if (!unitRef) return false
      const previous = [...store.jobCards]
        .slice()
        .reverse()
        .find(
          (j) =>
            j.unitRef === unitRef &&
            j.workType === 'Maintenance/Service DX split unit' &&
            Array.isArray(j.maintenanceTasks) &&
            j.maintenanceTasks.length,
        )
      if (!previous) return false
      form.maintenanceTasks = JSON.parse(JSON.stringify(previous.maintenanceTasks || []))
      form.maintenanceInputs = JSON.parse(
        JSON.stringify(previous.maintenanceInputs || {}),
      )
      return true
    }

    const addClientTask = () => {
      form.maintenanceTasks.push({
        id: `client_${Date.now()}`,
        label: '',
        section: 'outdoor',
        frequency: 'Quarterly',
        done: false,
        required: true,
        custom: true,
      })
    }

    const removeClientTask = (id) => {
      form.maintenanceTasks = form.maintenanceTasks.filter((t) => t.id !== id)
    }

    // Pre-populate if in edit mode
    if (isEdit) {
      const existingJob = store.jobCards.find((j) => j.id === jobId)
      if (existingJob) {
        form.date = existingJob.date.replace(/\//g, '-')
        form.checkInTime = existingJob.checkInTime || ''
        form.checkOutTime = existingJob.checkOutTime || ''
        form.workType = existingJob.workType || 'Maintenance'
        form.faultFound = existingJob.faultFound
        form.faultReported = existingJob.faultReported || ''
        form.rootCause = existingJob.rootCause || ''
        form.remedy = existingJob.remedy || ''
        form.invoiced = !!existingJob.invoiced
        form.quotationNumber = existingJob.quotationNumber || ''
        form.quotationStatus = existingJob.quotationStatus || 'Not Required'
        form.comments = existingJob.comments || ''
        form.partsUsed = JSON.parse(JSON.stringify(existingJob.partsUsed || []))
        form.readings = JSON.parse(
          JSON.stringify(
            existingJob.readings || {
              suctionPressure: '',
              dischargePressure: '',
              supplyTemp: '',
              returnTemp: '',
              amps: '',
            },
          ),
        )

        // Find asset ID by matching unitRef and customer
        const asset = allAssetsOptions.value.find(
          (a) => a.unitRef === existingJob.unitRef && a.customer === existingJob.customer,
        )
        if (asset) form.assetId = asset.value

        // Find tech ID by matching fullName
        const tech = store.users.find((u) => u.fullName === existingJob.tech)
        if (tech) form.techId = tech.id
      }
    }

    const filteredAssets = ref(allAssetsOptions.value)

    const openFaultFilePicker = () => {
      if (faultFile.value) {
        faultFile.value.pickFiles()
      }
    }

    watch(
      () => form.faultFound,
      (val) => {
        if (val && form.faults.length === 0) {
          addFault()
        }
        if (!val) {
          form.faults = []
        }
      },
    )

    const resolveSelectedAsset = () => {
      if (!form.assetId) return { asset: null, unitRef: null }
      const selectedMeta = allAssetsOptions.value.find((a) => a.value === form.assetId)
      if (!selectedMeta) return { asset: null, unitRef: null }
      let found = null
      store.customers.forEach((c) => {
        c.projects.forEach((p) => {
          const hit = p.assets.find((as) => as.id === selectedMeta.value)
          if (hit) found = hit
        })
      })
      return { asset: found, unitRef: selectedMeta.unitRef }
    }

    const ensureDxChecklistLoaded = () => {
      if (!isDxMaintenance.value) return
      if (form.maintenanceTasks && form.maintenanceTasks.length) return

      const { asset, unitRef } = resolveSelectedAsset()
      const loaded = unitRef ? tryLoadPreviousDxConfig(unitRef) : false
      if (!loaded) {
        initialiseDxTemplateForAsset(asset)
      }
    }

    // When switching job type to DX maintenance, ensure checklist is present
    watch(
      () => form.workType,
      () => {
        ensureDxChecklistLoaded()
      },
    )

    // When selecting an asset while already in DX maintenance, ensure checklist is present
    watch(
      () => form.assetId,
      () => {
        ensureDxChecklistLoaded()
      },
    )

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
          (v) => v.label.toLowerCase().indexOf(needle) > -1,
        )
      })
    }

    const onSubmit = () => {
      const selectedAsset = allAssetsOptions.value.find((a) => a.value === form.assetId)
      const selectedTech = technicians.value.find((t) => t.value === form.techId)

      if (!selectedAsset || !selectedTech) {
        $q.notify({ color: 'negative', message: 'Please select an asset and a technician' })
        return
      }

      if (!form.signed) {
        $q.notify({ color: 'negative', message: 'Please capture a signature before saving.' })
        return
      }

      const primaryFault = form.faults[0] || {
        details: '',
        pictures: [],
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
        faultReported: primaryFault.details || form.faultReported,
        rootCause: form.rootCause,
        remedy: form.remedy,
        invoiced: form.invoiced,
        quotationNumber: form.quotationNumber,
        quotationStatus: form.quotationStatus,
        faults: form.faults.map((f) => ({
          details: f.details || '',
          pictures: f.pictures || [],
        })),
        maintenanceTasks: isDxMaintenance.value ? [...form.maintenanceTasks] : [],
        maintenanceInputs: isDxMaintenance.value ? { ...form.maintenanceInputs } : {},
        signed: form.signed,
        signedBy: form.signedBy,
        comments: form.comments,
        partsUsed: [...form.partsUsed],
        readings: { ...form.readings },
      }

      if (isEdit) {
        store.updateJobCard(jobId, jobData)
        $q.notify({
          color: 'positive',
          message: 'Job card updated successfully',
          icon: 'fas fa-check-circle',
          position: 'top',
        })
      } else {
        store.addJobCard(jobData)
        $q.notify({
          color: 'positive',
          message: 'New job card finalized and saved',
          icon: 'fas fa-check-circle',
          position: 'top',
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
      // DX maintenance helpers
      isDxMaintenance,
      dxChecklistTab,
      frequencyOptions,
      indoorChecklist,
      outdoorChecklist,
      clientTasks,
      checklistInputs,
      addClientTask,
      removeClientTask,
      newPart,
      addPart,
      removePart,
      faultFile,
      openFaultFilePicker,
      addFault,
      removeFault,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}

.signature-pad {
  border: 2px dashed #ccc;
  border-radius: 8px;
  background: #fafafa;
  min-height: 120px;
}
.signature-checkbox {
  padding: 10px 14px;
  min-height: 56px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background: #fff;
}
</style>
