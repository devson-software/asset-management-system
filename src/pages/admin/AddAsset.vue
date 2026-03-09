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

        <q-card-section class="q-pa-none">
          <q-tabs v-model="entryTab" dense class="text-grey" active-color="primary" indicator-color="primary" align="justify" narrow-indicator>
            <q-tab name="manual" label="Manual Entry" icon="fas fa-keyboard" />
            <q-tab name="library" label="Import from Library" icon="fas fa-book" />
          </q-tabs>
          <q-separator />
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-tab-panels v-model="entryTab" animated>
            <q-tab-panel name="library" class="q-pa-none">
              <div class="q-mb-md">
                <div class="text-subtitle2 text-grey-7 q-mb-sm">Select equipment template from global library:</div>
                <q-select
                  v-model="selectedLibraryItem"
                  :options="store.unitLibrary"
                  label="Search Library (e.g. Samsung AM036...)"
                  outlined
                  dense
                  use-input
                  @filter="filterLibrary"
                  bg-color="blue-0"
                >
                  <template v-slot:option="scope">
                    <q-item v-bind="scope.itemProps" @click="importFromLibrary(scope.opt)">
                      <q-item-section avatar>
                        <q-icon name="fas fa-snowflake" color="primary" />
                      </q-item-section>
                      <q-item-section>
                        <q-item-label>{{ scope.opt.manufacturer }} - {{ scope.opt.unitType }}</q-item-label>
                        <q-item-label caption>{{ scope.opt.indoorModel }} / {{ scope.opt.outdoorModel }}</q-item-label>
                      </q-item-section>
                    </q-item>
                  </template>
                </q-select>
              </div>
              <q-banner rounded class="bg-blue-1 text-primary q-mb-lg">
                <template v-slot:avatar><q-icon name="fas fa-info-circle" /></template>
                Importing from library will pre-fill manufacturer, models, refrigerant, and service times. You'll only need to enter serial numbers and specific location.
              </q-banner>
            </q-tab-panel>
          </q-tab-panels>

          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <!-- Project Context -->
            <div v-if="project && !isEdit" class="bg-green-1 q-pa-md rounded-borders flex items-center border-green">
              <q-avatar color="green-2" text-color="positive" icon="fas fa-location-dot" size="sm" class="q-mr-sm" />
              <div>
                <span class="text-grey-7">Adding to project:</span>
                <span class="text-weight-bold text-positive q-ml-xs">{{ projectName }}</span>
              </div>
            </div>

            <!-- Manual Project Selection if not provided -->
            <div v-if="!project && !isEdit" class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-select
                  v-model="manualSelection.customerId"
                  :options="customerOptions"
                  label="Select Customer"
                  outlined
                  dense
                  required
                  bg-color="white"
                  emit-value
                  map-options
                >
                  <template v-slot:prepend><q-icon name="fas fa-business-time" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-select
                  v-model="manualSelection.projectId"
                  :options="projectOptions"
                  label="Select Project"
                  outlined
                  dense
                  required
                  :disable="!manualSelection.customerId"
                  bg-color="white"
                  emit-value
                  map-options
                >
                  <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
                </q-select>
              </div>
            </div>

            <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center">
              <q-icon name="fas fa-list-check" size="xs" class="q-mr-sm" />
              Unit Identification
            </div>

            <div class="row q-col-gutter-md q-mt-xs">
              <div class="col-12 col-md-6">
                <q-btn
                  color="primary"
                  icon="fas fa-qrcode"
                  label="Scan & Auto-fill"
                  class="full-width"
                  @click="startAutoFillScan"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-file
                  v-model="asset.nameplatePhoto"
                  label="Nameplate Photo"
                  outlined
                  dense
                  accept="image/*"
                  capture="environment"
                  bg-color="white"
                  @update:model-value="onNameplateSelected"
                >
                  <template v-slot:prepend><q-icon name="fas fa-camera" size="xs" /></template>
                </q-file>
                <q-img
                  v-if="nameplatePreview"
                  :src="nameplatePreview"
                  class="q-mt-sm rounded-borders"
                  ratio="16/9"
                />
              </div>
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="asset.plantCategory" 
                  :options="Object.keys(store.plantHierachy)" 
                  label="Plant Category" 
                  outlined 
                  dense
                  required
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-industry" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="asset.unitType" 
                  :options="unitTypeOptions" 
                  label="Type of Unit" 
                  outlined 
                  dense
                  required
                  :disable="!asset.plantCategory"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-layer-group" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.manufacturer" 
                  label="Manufacturer" 
                  outlined 
                  dense 
                  required 
                  placeholder="e.g. Samsung, Daikin, LG"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
                </q-input>
              </div>
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
                  v-model="asset.installationDate"
                  label="Installation Date"
                  type="date"
                  stack-label
                  outlined
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-calendar-day" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <q-separator />

            <div class="row q-col-gutter-md">
              <div
                v-if="showIndoorUnit"
                :class="showOutdoorUnit ? 'col-12 col-md-6' : 'col-12'"
              >
                <q-card flat bordered class="bg-grey-1">
                  <q-card-section class="q-pa-sm text-overline">{{ asset.plantCategory || 'Indoor Unit' }}</q-card-section>
                  <q-card-section class="q-pt-none q-gutter-y-sm">
                    <q-input v-model="asset.indoorModel" label="Model Number" outlined dense required bg-color="white" />
                    <q-input v-model="asset.serialNumber" label="Serial Number" outlined dense required bg-color="white" />
                  </q-card-section>
                </q-card>
              </div>
              <div
                v-if="showOutdoorUnit && asset.unitType && asset.plantCategory"
                :class="showIndoorUnit ? 'col-12 col-md-6' : 'col-12'"
              >
                <q-card flat bordered class="bg-grey-1">
                  <q-card-section class="q-pa-sm text-overline">{{ (asset.unitType === 'Heat recovery box' || asset.unitType === 'Hydronic control unit') ? asset.unitType : (asset.plantCategory || 'Outdoor Unit') }}</q-card-section>
                  <q-card-section class="q-pt-none q-gutter-y-sm">
                    <q-input v-model="asset.outdoorModel" label="Model Number" outlined dense bg-color="white" />
                    <q-input v-model="asset.outdoorSerial" label="Serial Number" outlined dense bg-color="white" />
                  </q-card-section>
                </q-card>
              </div>
            </div>

            <div v-if="showRefrigerant" class="row q-col-gutter-md q-mt-sm">
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="asset.refrigerantType" 
                  :options="['R410A', 'R32', 'R22', 'R404A', 'R134a', 'R407C']" 
                  label="Refrigerant Type" 
                  outlined 
                  dense
                  use-input
                  hide-selected
                  fill-input
                  input-debounce="0"
                  @new-value="createValue"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-gas-pump" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.refrigerantKg" 
                  label="Refrigerant QTY (kg)" 
                  type="number" 
                  step="0.01"
                  outlined 
                  dense
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-weight-hanging" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center q-mt-md">
              <q-icon name="fas fa-calendar-check" size="xs" class="q-mr-sm" />
              Service & Maintenance Plan
              <q-space />
              <span class="text-caption text-primary bg-blue-1 q-px-sm q-py-xs rounded-borders">
                <q-icon name="fas fa-robot" size="10px" class="q-mr-xs" />
                Should we automatically create a service Schedule?
                <q-toggle v-model="asset.autoSchedule" size="sm" dense class="q-ml-sm" />
              </span>
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="asset.serviceSchedule" 
                  :options="['Monthly', 'Quarterly', 'Bi-annual', 'Annual']" 
                  label="Service Schedule" 
                  outlined 
                  dense
                  required
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-repeat" size="xs" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.serviceTime" 
                  label="Service Duration" 
                  outlined 
                  dense
                  bg-color="white"
                  placeholder="e.g. 2 hours"
                  hint="Specify time or use default from library"
                >
                  <template v-slot:prepend><q-icon name="fas fa-clock" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <!-- DX Split Maintenance Checklist configuration (per asset) -->
            <div class="q-mt-md q-pa-md bg-green-50 rounded-borders border-green-3">
              <div class="text-subtitle1 text-weight-bold row items-center q-mb-sm">
                <q-icon name="fas fa-list-check" color="green-9" class="q-mr-sm" />
                Cassette DX – Service Checklist (per asset)
              </div>
              <div class="text-caption text-grey-8 q-mb-sm">
                Define which Cassette DX checklist tasks apply to this unit and their default
                frequency. Technicians will follow this when servicing the asset.
              </div>

              <div class="q-mt-sm">
                <q-tabs
                  v-model="dxChecklistTabAsset"
                  dense
                  class="bg-grey-2 rounded-borders q-mb-sm"
                  active-color="green-8"
                  indicator-color="green-8"
                  inline-label
                >
                  <q-tab
                    name="indoor"
                    icon="fas fa-fan"
                    label="INDOOR UNIT – AIRSIDE / HYGIENE TASKS"
                  />
                  <q-tab
                    name="outdoor"
                    icon="fas fa-snowflake"
                    label="OUTDOOR UNIT – REFRIGERATION / ELECTRICAL TASKS"
                  />
                </q-tabs>

                <div v-if="dxChecklistTabAsset === 'indoor'">
                  <q-list bordered class="bg-white rounded-borders">
                    <q-item
                      v-for="task in dxChecklistIndoor"
                      :key="task.id"
                      dense
                      tag="label"
                      clickable
                    >
                      <q-item-section avatar top>
                        <q-checkbox v-model="task.enabled" color="green-7" />
                      </q-item-section>
                      <q-item-section>
                        <q-item-label>{{ task.label }}</q-item-label>
                        <q-item-label caption>
                          <q-select
                            v-model="task.frequency"
                            :options="dxFrequencyOptions"
                            dense
                            borderless
                            emit-value
                            map-options
                            options-dense
                            style="max-width: 140px"
                          />
                        </q-item-label>
                      </q-item-section>
                    </q-item>
                  </q-list>
                </div>

                <div v-else>
                  <q-list bordered class="bg-white rounded-borders">
                    <q-item
                      v-for="task in dxChecklistOutdoor"
                      :key="task.id"
                      dense
                      tag="label"
                      clickable
                    >
                      <q-item-section avatar top>
                        <q-checkbox v-model="task.enabled" color="green-7" />
                      </q-item-section>
                      <q-item-section>
                        <q-item-label>{{ task.label }}</q-item-label>
                        <q-item-label caption>
                          <q-select
                            v-model="task.frequency"
                            :options="dxFrequencyOptions"
                            dense
                            borderless
                            emit-value
                            map-options
                            options-dense
                            style="max-width: 140px"
                          />
                        </q-item-label>
                      </q-item-section>
                    </q-item>
                  </q-list>
                </div>
              </div>
            </div>

            <!-- Performance Verification (ASHRAE 180) configuration (per asset) -->
            <div class="q-mt-md q-pa-sm bg-amber-1 rounded-borders">
              <div class="text-caption text-weight-bold q-mb-xs">
                Performance Verification (ASHRAE 180)
              </div>
              <div class="row q-col-gutter-sm">
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.recordedVoltage"
                    label="Recorded voltage"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.returnAirTemp"
                    label="Return air temperature (°C)"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.supplyAirTemp"
                    label="Supply air temperature (°C)"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.deltaT"
                    label="Air ΔT across coil (°C)"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.compressorCurrent"
                    label="Compressor running current (A)"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="asset.dxPerformance.unitOperation"
                    label="Unit operation / alarms"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
              </div>
            </div>

            <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center q-mt-md">
              <q-icon name="fas fa-location-crosshairs" size="xs" class="q-mr-sm" />
              Specific Location
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.vendorArea" 
                  label="Area" 
                  outlined 
                  dense
                  bg-color="white"
                  placeholder="e.g. North Wing, Ground Floor"
                >
                  <template v-slot:prepend><q-icon name="fas fa-map" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="asset.vendorLocation" 
                  label="Specific Location" 
                  outlined 
                  dense
                  bg-color="white"
                  placeholder="e.g. Office 12, Server Room A"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
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
import { defineComponent, reactive, computed, ref, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { store } from '../../store'
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

    const entryTab = ref('manual')
    const selectedLibraryItem = ref(null)

    const manualSelection = reactive({
      customerId: customerId || null,
      projectId: projectId || null
    })

    const asset = reactive({ 
      unitRef: '', 
      plantCategory: '',
      unitType: '',
      manufacturer: '',
      indoorModel: '', 
      serialNumber: '', 
      outdoorModel: '',
      outdoorSerial: '',
      installationDate: '',
      refrigerantType: 'R410A', 
      refrigerantKg: '',
      serviceSchedule: 'Monthly',
      serviceTime: '',
      autoSchedule: true,
      vendorLocation: '',
      vendorArea: '',
      vendorAddress: '',
      nameplatePhoto: null,
      dxPerformance: {
        recordedVoltage: '',
        returnAirTemp: '',
        supplyAirTemp: '',
        deltaT: '',
        compressorCurrent: '',
        unitOperation: '',
      },
    })

    const dxFrequencyOptions = [
      { label: 'Monthly', value: 'Monthly' },
      { label: 'Quarterly', value: 'Quarterly' },
      { label: 'Bi-annual', value: 'Bi-annual' },
      { label: 'Annual', value: 'Annual' },
    ]

    const dxChecklistTabAsset = ref('indoor')

    const dxChecklistIndoor = ref([
      { id: 'indoor_inspect_air_filters', label: 'Inspect air filters', enabled: true, frequency: 'Monthly' },
      { id: 'indoor_clean_replace_filters', label: 'Clean / replace filters', enabled: true, frequency: 'Quarterly' },
      { id: 'indoor_inspect_evaporator_coil', label: 'Inspect evaporator coil', enabled: true, frequency: 'Quarterly' },
      { id: 'indoor_clean_evaporator_coil', label: 'Clean evaporator coil', enabled: true, frequency: 'Annually' },
      { id: 'indoor_inspect_condensate_tray', label: 'Inspect condensate tray', enabled: true, frequency: 'Quarterly' },
      { id: 'indoor_flush_condensate_drain', label: 'Flush condensate drain', enabled: true, frequency: 'Quarterly' },
      { id: 'indoor_inspect_fan_wheel', label: 'Inspect indoor fan & wheel', enabled: true, frequency: 'Annually' },
      { id: 'indoor_check_controller', label: 'Check controller operation', enabled: true, frequency: 'Annually' },
    ])

    const dxChecklistOutdoor = ref([
      { id: 'outdoor_inspect_condenser_coil', label: 'Inspect condenser coil', enabled: true, frequency: 'Quarterly' },
      { id: 'outdoor_clean_condenser_coil', label: 'Clean condenser coil', enabled: true, frequency: 'Annually' },
      { id: 'outdoor_check_compressor_op', label: 'Check compressor operation', enabled: true, frequency: 'Quarterly' },
      { id: 'outdoor_record_running_amps', label: 'Record compressor running amps', enabled: true, frequency: 'Quarterly' },
      { id: 'outdoor_check_fan_operation', label: 'Check condenser fan operation', enabled: true, frequency: 'Quarterly' },
      { id: 'outdoor_inspect_refrigerant_pipework', label: 'Inspect refrigerant pipework', enabled: true, frequency: 'Quarterly' },
      { id: 'outdoor_leak_inspection', label: 'Leak inspection', enabled: true, frequency: 'Annually' },
      { id: 'outdoor_electrical_terminals', label: 'Electrical terminals', enabled: true, frequency: 'Annually' },
    ])

    const customerOptions = computed(() => {
      return store.customers.map(c => ({ label: c.name, value: c.id }))
    })

    const projectOptions = computed(() => {
      if (!manualSelection.customerId) return []
      const c = store.customers.find(cust => cust.id === manualSelection.customerId)
      return c ? c.projects.map(p => ({ label: p.name, value: p.id })) : []
    })

    const unitTypeOptions = computed(() => {
      if (!asset.plantCategory) return []
      return store.plantHierachy[asset.plantCategory] || []
    })

    const showIndoorUnit = computed(() => {
      // return [
      //   'Direct expansion split units',
      //   'VRF Indoor units',
      //   'Fan coil units',
      //   'Air handling units',
      // ].includes(asset.plantCategory)
      return false
    })

    const showOutdoorUnit = computed(() => {
      // return ['VRF condensing units', 'Package plant', 'Chiller'].includes(asset.plantCategory)
      return true
    })

    const showRefrigerant = computed(() => {
      return [
        'Direct expansion split units',
        'VRF condensing units',
        'Package plant',
        'Chiller',
      ].includes(asset.plantCategory)
    })

    watch(() => manualSelection.customerId, () => {
      if (!isEdit) {
        manualSelection.projectId = null
      }
    })

    watch(
      () => asset.plantCategory,
      (next, prev) => {
        if (next !== prev) {
          asset.unitType = ''
        }
      },
    )

    const filterLibrary = (val, update) => {
      // Library searching logic if needed
      update()
    }

    const importFromLibrary = (item) => {
      asset.manufacturer = item.manufacturer
      asset.unitType = item.unitType
      asset.indoorModel = item.indoorModel
      asset.outdoorModel = item.outdoorModel
      asset.refrigerantType = item.refrigerantType
      asset.refrigerantKg = item.refrigerantCharge
      asset.serviceTime = item.serviceDuration
      
      // Try to auto-set plant category based on unit type
      for (const [cat, types] of Object.entries(store.plantHierachy)) {
        if (types.includes(item.unitType)) {
          asset.plantCategory = cat
          break
        }
      }

      entryTab.value = 'manual'
      $q.notify({
        color: 'info',
        message: `Template for ${item.manufacturer} ${item.unitType} imported!`,
        icon: 'fas fa-file-import'
      })
    }

    const createValue = (val, done) => {
      if (val.length > 0) {
        done(val, 'add-unique')
      }
    }

    const nameplatePreview = ref('')

    const onNameplateSelected = (file) => {
      if (!file) {
        nameplatePreview.value = ''
        return
      }
      nameplatePreview.value = URL.createObjectURL(file)
    }

    const startAutoFillScan = () => {
      if (store.unitLibrary?.length) {
        const template = store.unitLibrary[0]
        importFromLibrary(template)
        $q.notify({
          color: 'positive',
          message: 'Scan captured. Unit data auto-filled from template.',
          icon: 'fas fa-qrcode',
        })
      } else {
        $q.notify({
          color: 'warning',
          message: 'No templates available for auto-fill.',
          icon: 'fas fa-triangle-exclamation',
        })
      }
    }

    if (isEdit && project) {
      const existingAsset = project.assets.find(a => a.id === assetId)
      if (existingAsset) {
        Object.assign(asset, existingAsset)
      }
    }

    const onSubmit = () => {
      const finalCustomerId = manualSelection.customerId
      const finalProjectId = manualSelection.projectId

      if (!finalCustomerId || !finalProjectId) {
        $q.notify({ color: 'negative', message: 'Please select a customer and project' })
        return
      }

      if (isEdit) {
        store.updateAsset(finalCustomerId, finalProjectId, assetId, { ...asset })
        $q.notify({ color: 'positive', message: 'Asset updated successfully' })
      } else {
        store.addAsset(finalCustomerId, finalProjectId, { ...asset })
        $q.notify({ color: 'positive', message: 'Asset registered successfully' })
      }

      // Clear form
      Object.keys(asset).forEach(key => asset[key] = '')

      router.push({ path: '/assets', query: { projectId: finalProjectId } })
    }

    return { 
      asset, 
      projectName, 
      onSubmit, 
      isEdit, 
      store, 
      entryTab, 
      selectedLibraryItem, 
      unitTypeOptions,
      showIndoorUnit,
      showOutdoorUnit,
      showRefrigerant,
      manualSelection,
      customerOptions,
      projectOptions,
      project,
      filterLibrary,
      importFromLibrary,
      createValue,
      nameplatePreview,
      onNameplateSelected,
      startAutoFillScan,
      dxChecklistTabAsset,
      dxChecklistIndoor,
      dxChecklistOutdoor,
      dxFrequencyOptions
    }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 800px;
  margin: 0 auto;
}
</style>

