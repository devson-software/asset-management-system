<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-lg">
        <q-btn flat round color="primary" icon="arrow_back" @click="$router.back()" class="q-mr-md" />
        <div>
          <div class="text-h4 text-primary text-weight-bold">Technical Data Sheet</div>
          <div class="text-subtitle1 text-grey-7" v-if="targetAsset">
            Configuring specifications for: <span class="text-weight-bold text-primary">{{ targetAsset.unitRef }}</span>
          </div>
          <div class="text-subtitle1 text-grey-7" v-else>
            Select an asset to view or manage technical specifications
          </div>
        </div>
      </div>
      
      <!-- Asset Selection Table (Shown if no assetId is selected) -->
      <q-card v-if="!assetId" flat bordered class="rounded-borders shadow-2 overflow-hidden">
        <q-toolbar class="bg-primary text-white q-py-md">
          <q-icon name="ac_unit" size="md" class="q-mr-sm" />
          <q-toolbar-title class="text-weight-bold">Active Equipment Inventory</q-toolbar-title>
        </q-toolbar>
        
        <q-table
          :rows="allAssets"
          :columns="assetColumns"
          row-key="id"
          flat
          :filter="filter"
          class="asset-selection-table"
        >
          <template v-slot:top-right>
            <q-input borderless dense debounce="300" v-model="filter" placeholder="Search by model, site or ref..." class="bg-grey-2 q-px-md rounded-borders">
              <template v-slot:append><q-icon name="search" /></template>
            </q-input>
          </template>

          <template v-slot:body-cell-unitRef="props">
            <q-td :props="props">
              <q-chip outline color="primary" text-color="primary" class="text-weight-bold" icon="label">
                {{ props.row.unitRef }}
              </q-chip>
            </q-td>
          </template>

          <template v-slot:body-cell-actions="props">
            <q-td :props="props">
              <q-btn 
                color="secondary" 
                label="Open Data Sheet" 
                icon="analytics" 
                size="sm" 
                unelevated
                class="rounded-borders"
                @click="$router.push('/technical-data/' + props.row.id)" 
              />
            </q-td>
          </template>
        </q-table>
      </q-card>

      <!-- Stepper Form (Shown if assetId is selected) -->
      <q-stepper
        v-if="assetId"
        v-model="step"
        ref="stepper"
        color="primary"
        active-color="primary"
        done-color="positive"
        animated
        flat
        bordered
        class="bg-white rounded-borders shadow-3 overflow-hidden"
      >
        <!-- Step 1: Unit Identification -->
        <q-step
          :name="1"
          title="Identification"
          caption="Model & Serial"
          icon="fingerprint"
          :done="step > 1"
        >
          <div class="row items-center q-mb-lg">
            <q-avatar color="blue-1" text-color="primary" icon="settings" size="48px" class="q-mr-md" />
            <div>
              <div class="text-h6 text-grey-9">Unit Identification</div>
              <div class="text-caption text-grey-7">Core equipment models and unique identifiers</div>
            </div>
          </div>

          <div class="row q-col-gutter-lg">
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
              <q-input v-model="form.areaLocation" label="Area / Location" outlined dense>
                <template v-slot:prepend><q-icon name="place" color="grey-6" /></template>
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
          icon="ac_unit"
          :done="step > 2"
        >
          <div class="row items-center q-mb-lg">
            <q-avatar color="cyan-1" text-color="cyan-9" icon="bolt" size="48px" class="q-mr-md" />
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
                <template v-slot:append><q-icon name="electric_bolt" size="xs" color="orange" /></template>
              </q-input>
            </div>
            <div class="col-6 col-md-3">
              <q-input v-model="form.mfa" label="MFA (Amps)" outlined dense />
            </div>
            <div class="col-12 col-md-6">
              <q-select v-model="form.mode" :options="['Heat Pump', 'Cooling Only', 'Inverter']" label="Operational Mode" outlined dense />
            </div>
          </div>
        </q-step>

        <!-- Step 3: Technical Specs -->
        <q-step
          :name="3"
          title="Technical"
          caption="Gas & Piping"
          icon="plumbing"
          :done="step > 3"
        >
          <div class="row items-center q-mb-lg">
            <q-avatar color="teal-1" text-color="teal-9" icon="construction" size="48px" class="q-mr-md" />
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
                <template v-slot:prepend><q-icon name="gas_meter" color="teal" /></template>
              </q-input>
            </div>
            <div class="col-6 col-md-6">
              <q-input v-model="form.refrigerantKg" label="Factory Charge (kg)" type="number" outlined dense />
            </div>
            <div class="col-6 col-md-6">
              <q-input v-model="form.maxPipeLength" label="Max Vertical Pipe (m)" type="number" outlined dense />
            </div>
            <q-separator class="col-12 q-my-sm" />
            <div class="col-12 col-md-6">
              <q-input v-model="form.powerSupplyEvap" label="Power (Indoor)" outlined dense />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="form.powerSupplyCond" label="Power (Outdoor)" outlined dense />
            </div>
          </div>
        </q-step>

        <!-- Step 4: Ownership & Accounting -->
        <q-step
          :name="4"
          title="Asset Value"
          caption="Accounting"
          icon="payments"
        >
          <div class="row items-center q-mb-lg">
            <q-avatar color="green-1" text-color="green-9" icon="receipt_long" size="48px" class="q-mr-md" />
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
          </div>
        </q-step>

        <!-- Navigation Buttons -->
        <template v-slot:navigation>
          <q-stepper-navigation>
            <div class="row justify-between items-center q-px-md q-pb-md">
              <q-btn v-if="step > 1" flat color="primary" @click="$refs.stepper.previous()" label="Back" class="q-mr-sm" />
              <q-btn v-else flat color="grey" @click="$router.back()" label="Cancel" />
              
              <q-btn 
                @click="step === 4 ? onSubmit() : $refs.stepper.next()" 
                color="primary" 
                :label="step === 4 ? 'Save Technical Data' : 'Continue'" 
                :icon-right="step === 4 ? 'check_circle' : 'arrow_forward'"
              />
            </div>
          </q-stepper-navigation>
        </template>
      </q-stepper>
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
    const step = ref(1)
    const filter = ref('')
    const assetId = computed(() => route.params.assetId)

    const assetColumns = [
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customerName', sortable: true },
      { name: 'project', label: 'Project', align: 'left', field: 'projectName', sortable: true },
      { name: 'model', label: 'Model', align: 'left', field: 'indoorModel' },
      { name: 'actions', label: 'Actions', align: 'right' }
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

    const targetAsset = computed(() => {
      if (!assetId.value) return null
      return allAssets.value.find(a => a.id === assetId.value)
    })

    const form = reactive({
      indoorUnit: '',
      indoorSerial: '',
      unitRefNumber: '',
      outdoorUnit: '',
      outdoorSerial: '',
      areaLocation: '',
      dateInstalled: '',
      coolingKw: '',
      coolingBtu: '',
      heatingKw: '',
      heatingBtu: '',
      mca: '',
      mfa: '',
      mode: 'Heat Pump',
      liquidPipe: '',
      gasPipe: '',
      refrigerantType: '',
      refrigerantKg: '',
      maxPipeLength: '',
      powerSupplyEvap: '',
      powerSupplyCond: '',
      evapAirFlow: '',
      condAirFlow: '',
      serviceResponsibility: 'Owner',
      ownership: 'Landlord',
      serviceProvider: 'Default Service Co.',
      equipmentValue: '',
      depreciationPercent: ''
    })

    const populateForm = () => {
      if (targetAsset.value) {
        form.indoorUnit = targetAsset.value.indoorModel || ''
        form.indoorSerial = targetAsset.value.serialNumber || ''
        form.unitRefNumber = targetAsset.value.unitRef || ''
        form.refrigerantType = targetAsset.value.refrigerantType || ''
        form.refrigerantKg = targetAsset.value.refrigerantKg || ''
      }
    }

    onMounted(populateForm)
    watch(assetId, populateForm)

    const onSubmit = () => {
      $q.notify({
        color: 'positive',
        message: 'Technical data sheet saved successfully',
        icon: 'check'
      })
      router.push('/assets')
    }

    return {
      step,
      filter,
      assetId,
      assetColumns,
      allAssets,
      form,
      targetAsset,
      onSubmit
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
