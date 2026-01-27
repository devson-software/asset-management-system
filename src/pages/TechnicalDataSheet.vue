<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-primary text-weight-bold">
            {{ form.unitType ? form.unitType : 'Technical Data Sheet' }}
          </div>
          <div class="text-subtitle1 text-grey-7" v-if="targetAsset">
            Configuring specifications for: <span class="text-weight-bold text-primary">{{ targetAsset.unitRef }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Complete technical library of all installed equipment specifications
          </div>
        </div>
        <q-btn v-if="!showForm" color="primary" icon="fas fa-plus" label="New Technical Data Sheet" @click="startNewSheet" class="shadow-2" />
      </div>
      
      <!-- Asset Selection Table (Shown if no assetId is selected) -->
      <div class="col-12" v-if="!showForm"> 
        <q-card flat bordered class="rounded-borders shadow-1">
          <q-table
            :rows="filteredAssets"
            :columns="assetColumns"
            row-key="id"
            flat
            :filter="filter"
            class="asset-selection-table"
          >
            <template v-slot:header="props">
              <q-tr :props="props">
                <q-th
                  v-for="col in props.cols"
                  :key="col.name"
                  :props="props"
                  :class="'text-' + col.align"
                >
                  <div class="row items-center no-wrap" :class="col.align === 'center' ? 'justify-center' : (col.align === 'right' ? 'justify-end' : '')">
                    <!-- Sort Icon on the Left -->
                    <q-icon
                      v-if="col.sortable"
                      :name="props.pagination && props.pagination.sortBy === col.name ? (props.pagination.descending ? 'fas fa-arrow-down-long' : 'fas fa-arrow-up-long') : 'fas fa-arrow-up-long'"
                      size="12px"
                      class="q-mr-xs cursor-pointer sort-icon"
                      :class="{ 'active': props.pagination && props.pagination.sortBy === col.name }"
                      @click="props.sort(col)"
                    />
                    <span class="cursor-pointer" @click="col.sortable && props.sort(col)">{{ col.label }}</span>
                    <q-btn
                      v-if="col.name !== 'actions'"
                      flat
                      round
                      dense
                      size="xs"
                      icon="fas fa-filter"
                      class="q-ml-xs filter-btn"
                      :class="{ 'active': columnFilters[col.name] }"
                      :color="columnFilters[col.name] ? 'primary' : 'grey-5'"
                    >
                      <q-menu cover anchor="top middle">
                        <q-list style="min-width: 200px">
                          <q-item>
                            <q-input
                              v-model="columnFilters[col.name]"
                              :label="'Filter ' + col.label"
                              outlined
                              dense
                              autofocus
                              clearable
                            >
                              <template v-slot:append>
                                <q-icon name="fas fa-magnifying-glass" size="xs" />
                              </template>
                            </q-input>
                          </q-item>
                        </q-list>
                      </q-menu>
                    </q-btn>
                  </div>
                </q-th>
              </q-tr>
            </template>
            <template v-slot:top-right>
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Assets...">
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-unitRef="props">
              <q-td :props="props">
                <div class="row items-center no-wrap">
                  <q-avatar color="blue-1" text-color="primary" icon="fas fa-snowflake" size="32px" class="q-mr-md" />
                  <div>
                    <div class="text-weight-bold text-primary">{{ props.row.unitRef }}</div>
                    <div class="text-caption text-grey-7">{{ props.row.indoorModel }}</div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn 
                  color="primary" 
                  label="View Specs" 
                  icon="fas fa-file-lines" 
                  size="sm" 
                  unelevated
                  class="rounded-borders q-px-md"
                  @click="$router.push('/technical-data/' + props.row.id)" 
                />
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>

      <!-- Stepper Form (Shown if assetId is selected) -->
      <div class="col-12" v-if="showForm">
        <q-stepper
          v-model="step"
          ref="stepper"
          color="primary"
          active-color="primary"
          done-color="positive"
          animated
          flat
          bordered
          class="bg-white rounded-borders shadow-3 overflow-hidden"
          header-nav
        >
          <!-- Step 1: Unit Identification -->
          <q-step
            :name="1"
            title="Identification"
            caption="Model & Serial"
            icon="fas fa-fingerprint"
            :done="step > 1"
          >
            <q-tabs v-model="entryTab" dense class="text-grey q-mb-md" active-color="primary" indicator-color="primary" align="left" narrow-indicator>
              <q-tab name="manual" label="Manual Entry" icon="fas fa-keyboard" />
              <q-tab name="library" label="Import from Library" icon="fas fa-book" />
            </q-tabs>

            <q-tab-panels v-model="entryTab" animated class="q-mb-md bg-transparent">
              <q-tab-panel name="library" class="q-pa-none">
                <div class="row q-col-gutter-md items-center">
                  <div class="col-12 col-md-8">
                    <q-select
                      v-model="selectedLibraryItem"
                      :options="store.unitLibrary"
                      label="Search Equipment Library..."
                      outlined
                      dense
                      use-input
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
                  <div class="col-12 col-md-4 text-caption text-grey-7">
                    <q-icon name="fas fa-info-circle" class="q-mr-xs" />
                    Auto-fills tech specs from database
                  </div>
                </div>
              </q-tab-panel>
            </q-tab-panels>

            <div class="row items-center q-mb-lg">
              <q-avatar color="blue-1" text-color="primary" icon="fas fa-gear" size="48px" class="q-mr-md" />
              <div>
                <div class="text-h6 text-grey-9">Unit Identification</div>
                <div class="text-caption text-grey-7">Core equipment models and unique identifiers</div>
              </div>
            </div>

            <div class="row q-col-gutter-lg">
              <div class="col-12 col-md-6">
                <q-input v-model="form.manufacturer" label="Manufacturer" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="form.unitType" 
                  :options="['Cassette', '1 way blow cassette', 'Midwall', 'Underceiling', 'Console', 'Hide-away']" 
                  label="Type of Unit" 
                  outlined 
                  dense
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.indoorUnit" label="Indoor Unit (Model)" outlined dense bg-color="blue-0" />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.indoorSerial" label="Indoor Serial Number" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.outdoorUnit" label="Outdoor Unit (Model)" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.outdoorSerial" label="Outdoor Serial Number" outlined dense />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.unitRefNumber" label="Unit Reference Number" outlined dense hint="e.g. Ac1.01" />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.vendorArea" label="Area" outlined dense placeholder="e.g. First floor">
                  <template v-slot:prepend><q-icon name="fas fa-map" color="grey-6" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.vendorLocation" label="Location" outlined dense placeholder="e.g. Foschini">
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" color="grey-6" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.dateInstalled" label="Date Installed" type="date" outlined dense stack-label />
              </div>
            </div>
          </q-step>

          <!-- Step 2: Capacity & Performance -->
          <q-step
            :name="2"
            title="Performance"
            caption="Cooling & Power"
            icon="fas fa-snowflake"
            :done="step > 2"
          >
            <div class="row items-center q-mb-lg">
              <q-avatar color="cyan-1" text-color="cyan-9" icon="fas fa-bolt" size="48px" class="q-mr-md" />
              <div>
                <div class="text-h6 text-grey-9">Capacity & Power</div>
                <div class="text-caption text-grey-7">Cooling/Heating ratings and electrical specifications</div>
              </div>
            </div>

            <div class="row q-col-gutter-lg">
              <div class="col-6 col-md-3">
                <q-input v-model="form.coolingKw" label="Cooling (kW)" type="number" outlined dense bg-color="blue-0">
                   <template v-slot:append><span class="text-caption text-grey">kW</span></template>
                </q-input>
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.coolingBtu" label="Cooling (Btu/h)" type="number" outlined dense />
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.heatingKw" label="Heating (kW)" type="number" outlined dense bg-color="orange-0" />
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.heatingBtu" label="Heating (Btu/h)" type="number" outlined dense />
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.mca" label="MCA (Amps)" outlined dense>
                  <template v-slot:append><q-icon name="fas fa-bolt" size="xs" color="orange" /></template>
                </q-input>
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.mfa" label="MFA (Amps)" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="form.mode" 
                  :options="['Heat pump', 'Cooling only', 'Inverter heat pump', 'Inverter cooling only']" 
                  label="Operational Mode" 
                  outlined 
                  dense 
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.powerSupplyEvap" label="Power (Indoor)" outlined dense>
                  <template v-slot:prepend><q-icon name="fas fa-plug" color="grey-6" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.powerSupplyCond" label="Power (Outdoor)" outlined dense>
                  <template v-slot:prepend><q-icon name="fas fa-plug" color="grey-6" /></template>
                </q-input>
              </div>
            </div>
          </q-step>

          <!-- Step 3: Technical Specs -->
          <q-step
            :name="3"
            title="Technical"
            caption="Gas & Piping"
            icon="fas fa-faucet-drip"
            :done="step > 3"
          >
            <div class="row items-center q-mb-lg">
              <q-avatar color="teal-1" text-color="teal-9" icon="fas fa-screwdriver-wrench" size="48px" class="q-mr-md" />
              <div>
                <div class="text-h6 text-grey-9">Piping & Refrigerant</div>
                <div class="text-caption text-grey-7">Gas/Liquid lines and refrigerant charge details</div>
              </div>
            </div>

            <div class="row q-col-gutter-lg">
              <div class="col-12 col-md-4">
                <q-input v-model="form.liquidPipe" label="Liquid Pipe (mm/inch)" outlined dense />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.gasPipe" label="Gas Pipe (mm/inch)" outlined dense />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.refrigerantType" label="Refrigerant Gas" outlined dense bg-color="teal-0">
                  <template v-slot:prepend><q-icon name="fas fa-fire-burner" color="teal" /></template>
                </q-input>
              </div>
              <div class="col-6 col-md-4">
                <q-input v-model="form.refrigerantKg" label="Factory Charge (kg)" type="number" outlined dense />
              </div>
              <div class="col-6 col-md-4">
                <q-input v-model="form.maxVerticalPipe" label="Max Vertical Pipe (m)" type="number" outlined dense />
              </div>
              <div class="col-6 col-md-4">
                <q-input v-model="form.maxPipeLength" label="Max Pipe Length (m)" type="number" outlined dense>
                  <template v-slot:prepend><q-icon name="fas fa-arrows-left-right" size="xs" color="teal" /></template>
                </q-input>
              </div>

              <q-separator class="col-12 q-my-md" />
              
              <div class="col-12">
                <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center">
                  <q-icon name="fas fa-filter" color="primary" class="q-mr-sm" size="xs" />
                  Filters
                </div>
              </div>

              <div class="col-12 col-md-4">
                <q-input v-model="form.filterType" label="Filter Type" outlined dense placeholder="e.g. Washable, HEPA" />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.filterSize" label="Filter Size" outlined dense placeholder="e.g. 500x500x25" />
              </div>
              <div class="col-12 col-md-4">
                <q-input v-model="form.filterQty" label="Filter Qty" type="number" outlined dense />
              </div>
            </div>
          </q-step>

          <!-- Step 4: Ownership & Accounting -->
          <q-step
            :name="4"
            title="Asset Value"
            caption="Accounting"
            icon="fas fa-hand-holding-dollar"
          >
            <div class="row items-center q-mb-lg">
              <q-avatar color="green-1" text-color="green-9" icon="fas fa-receipt" size="48px" class="q-mr-md" />
              <div>
                <div class="text-h6 text-grey-9">Accounting & Ownership</div>
                <div class="text-caption text-grey-7">Financial tracking and service responsibility</div>
              </div>
            </div>

            <div class="row q-col-gutter-lg">
              <div class="col-12 col-md-6">
                <q-select v-model="form.serviceResponsibility" :options="['Owner', 'Landlord', 'Tenant', 'Contractor']" label="Service Responsibility" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-select v-model="form.ownership" :options="['Owner', 'Landlord', 'Tenant']" label="Equipment Ownership" outlined dense />
              </div>
              <div class="col-12 col-md-6">
                <q-input v-model="form.serviceProvider" label="Maintenance Provider" outlined dense />
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.equipmentValue" label="Asset Value" type="number" outlined dense prefix="R" />
              </div>
              <div class="col-6 col-md-3">
                <q-input v-model="form.depreciationPercent" label="Depreciation / Year" type="number" outlined dense suffix="%" />
              </div>
              <div class="col-12">
                <q-separator class="q-my-sm" />
              </div>
              <div class="col-12 col-md-6">
                <q-select 
                  v-model="form.serviceSchedule" 
                  :options="['Monthly', 'Quarterly', 'Bi-annual', 'Annual']" 
                  label="Service Schedule" 
                  outlined 
                  dense
                >
                  <template v-slot:prepend><q-icon name="fas fa-repeat" size="xs" color="primary" /></template>
                </q-select>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="form.serviceTime" 
                  label="Service Duration" 
                  outlined 
                  dense 
                  hint="e.g. 2 hours"
                >
                  <template v-slot:prepend><q-icon name="fas fa-clock" size="xs" color="primary" /></template>
                </q-input>
              </div>
            </div>
          </q-step>

          <!-- Navigation Buttons -->
          <template v-slot:navigation>
            <q-stepper-navigation>
              <div class="row justify-between items-center q-px-md q-pb-md">
                <q-btn v-if="step > 1" flat color="primary" @click="$refs.stepper.previous()" label="Back" class="q-mr-sm" />
                <q-btn v-else flat color="grey" @click="isNew ? isNew = false : $router.push('/technical-data')" label="Cancel" />
                
                <q-btn 
                  @click="step === 4 ? onSubmit() : $refs.stepper.next()" 
                  color="primary" 
                  :label="step === 4 ? (isNew ? 'Create Data Sheet' : 'Update Data Sheet') : 'Continue'" 
                  :icon-right="step === 4 ? 'fas fa-circle-check' : 'fas fa-arrow-right'"
                />
              </div>
            </q-stepper-navigation>
          </template>
        </q-stepper>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'TechnicalDataSheet',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const isNew = ref(false)
    const assetId = computed(() => route.params.assetId)
    const showForm = computed(() => !!assetId.value || isNew.value)

    const entryTab = ref('manual')
    const selectedLibraryItem = ref(null)

    const step = ref(1)
    const filter = ref('')
    const columnFilters = reactive({
      unitRef: '',
      customer: '',
      project: '',
      model: ''
    })

    const startNewSheet = () => {
      isNew.value = true
      step.value = 1
      entryTab.value = 'manual'
      Object.keys(form).forEach(key => {
        if (typeof form[key] === 'string') form[key] = ''
        else if (typeof form[key] === 'number') form[key] = null
      })
    }

    const importFromLibrary = (item) => {
      form.manufacturer = item.manufacturer
      form.unitType = item.unitType
      form.indoorUnit = item.indoorModel
      form.outdoorUnit = item.outdoorModel
      form.refrigerantType = item.refrigerantType
      form.refrigerantKg = item.refrigerantCharge
      form.serviceTime = item.serviceDuration
      
      entryTab.value = 'manual'
      $q.notify({
        color: 'info',
        message: `Imported specs for ${item.manufacturer} ${item.unitType}`,
        icon: 'fas fa-file-import'
      })
    }

    const assetColumns = [
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customerName', sortable: true },
      { name: 'project', label: 'Project', align: 'left', field: 'projectName', sortable: true },
      { name: 'model', label: 'Model', align: 'left', field: 'indoorModel', sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    const allAssets = computed(() => {
      const assets = []
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          p.assets.forEach(a => {
            assets.push({ ...a, customerName: c.name, projectName: p.name })
          })
        })
      })
      return assets
    })

    const filteredAssets = computed(() => {
      return allAssets.value.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = key === 'customer' 
            ? row.customerName 
            : key === 'project'
              ? row.projectName
              : key === 'model'
                ? row.indoorModel
                : row[key] || ''
          
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const targetAsset = computed(() => {
      if (!assetId.value) return null
      return allAssets.value.find(a => a.id === assetId.value)
    })

    const form = reactive({
      manufacturer: '',
      indoorUnit: '',
      unitType: '',
      indoorSerial: '',
      unitRefNumber: '',
      outdoorUnit: '',
      outdoorSerial: '',
      vendorArea: '',
      vendorLocation: '',
      dateInstalled: '',
      coolingKw: '',
      coolingBtu: '',
      heatingKw: '',
      heatingBtu: '',
      mca: '',
      mfa: '',
      mode: 'Heat pump',
      liquidPipe: '',
      gasPipe: '',
      refrigerantType: '',
      refrigerantKg: '',
      maxVerticalPipe: '',
      maxPipeLength: '',
      powerSupplyEvap: '',
      powerSupplyCond: '',
      evapAirFlow: '',
      condAirFlow: '',
      filterType: '',
      filterSize: '',
      filterQty: '',
      serviceResponsibility: 'Owner',
      ownership: 'Landlord',
      serviceProvider: 'Default Service Co.',
      equipmentValue: '',
      depreciationPercent: '',
      serviceSchedule: 'Monthly',
      serviceTime: '',
      vendorAddress: ''
    })

    const populateForm = () => {
      if (targetAsset.value) {
        form.manufacturer = targetAsset.value.manufacturer || ''
        form.indoorUnit = targetAsset.value.indoorModel || ''
        form.unitType = targetAsset.value.unitType || ''
        form.indoorSerial = targetAsset.value.serialNumber || ''
        form.unitRefNumber = targetAsset.value.unitRef || ''
        form.outdoorUnit = targetAsset.value.outdoorModel || ''
        form.outdoorSerial = targetAsset.value.outdoorSerial || ''
        form.refrigerantType = targetAsset.value.refrigerantType || ''
        form.refrigerantKg = targetAsset.value.refrigerantKg || ''
        form.maxVerticalPipe = targetAsset.value.maxVerticalPipe || ''
        form.maxPipeLength = targetAsset.value.maxPipeLength || ''
        form.powerSupplyEvap = targetAsset.value.powerSupplyEvap || ''
        form.powerSupplyCond = targetAsset.value.powerSupplyCond || ''
        form.filterType = targetAsset.value.filterType || ''
        form.filterSize = targetAsset.value.filterSize || ''
        form.filterQty = targetAsset.value.filterQty || ''
        form.serviceSchedule = targetAsset.value.serviceSchedule || 'Monthly'
        form.serviceTime = targetAsset.value.serviceTime || ''
        form.vendorLocation = targetAsset.value.vendorLocation || ''
        form.vendorArea = targetAsset.value.vendorArea || ''
        form.vendorAddress = targetAsset.value.vendorAddress || ''
      }
    }

    onMounted(populateForm)
    watch(assetId, populateForm)

    const onSubmit = () => {
      if (assetId.value) {
        // Find the customer and project for this asset to update it in the store
        store.customers.forEach(c => {
          c.projects.forEach(p => {
            const assetIndex = p.assets.findIndex(a => a.id === assetId.value)
            if (assetIndex !== -1) {
              // Update the asset with the form data
              const updatedAsset = { 
                ...p.assets[assetIndex],
                ...form,
                // Ensure model/serial fields from TDS map back to asset core fields if necessary
                manufacturer: form.manufacturer,
                indoorModel: form.indoorUnit,
                unitType: form.unitType,
                serialNumber: form.indoorSerial,
                outdoorModel: form.outdoorUnit,
                outdoorSerial: form.outdoorSerial,
                unitRef: form.unitRefNumber,
                maxVerticalPipe: form.maxVerticalPipe,
                maxPipeLength: form.maxPipeLength,
                powerSupplyEvap: form.powerSupplyEvap,
                powerSupplyCond: form.powerSupplyCond,
                filterType: form.filterType,
                filterSize: form.filterSize,
                filterQty: form.filterQty
              }
              store.updateAsset(c.id, p.id, assetId.value, updatedAsset)
            }
          })
        })
      }

      $q.notify({
        color: 'positive',
        message: 'Technical data sheet saved successfully',
        icon: 'fas fa-check'
      })
      router.push('/assets')
    }

    return {
      step,
      filter,
      columnFilters,
      assetId,
      isNew,
      showForm,
      startNewSheet,
      assetColumns,
      allAssets,
      filteredAssets,
      form,
      targetAsset,
      onSubmit,
      entryTab,
      selectedLibraryItem,
      importFromLibrary,
      store
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.asset-selection-table {
  background: transparent;
}
</style>
