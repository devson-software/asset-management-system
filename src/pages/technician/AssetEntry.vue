<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-1">
          <q-card-section class="bg-primary text-white">
            <div class="row items-center no-wrap">
              <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
              <q-icon name="fas fa-boxes-stacked" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">Register Asset</div>
                <div class="text-subtitle2">Adding a new unit to the site register</div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-none">
            <q-tabs
              v-model="entryTab"
              dense
              class="text-grey"
              active-color="primary"
              indicator-color="primary"
              align="justify"
              narrow-indicator
            >
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
                  Importing from library pre-fills manufacturer, models, refrigerant, and service times.
                </q-banner>
              </q-tab-panel>
            </q-tab-panels>

            <q-form @submit="onSubmit" class="q-gutter-y-lg">

              <!-- Unit Identification -->
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

              <!-- Indoor / Outdoor Unit Cards -->
              <div class="row q-col-gutter-md">
                <div
                  v-if="showIndoorUnit"
                  :class="showOutdoorUnit ? 'col-12 col-md-6' : 'col-12'"
                >
                  <q-card flat bordered class="bg-grey-1">
                    <q-card-section class="q-pa-sm text-overline">Indoor Unit</q-card-section>
                    <q-card-section class="q-pt-none q-gutter-y-sm">
                      <q-input v-model="asset.indoorModel" label="Model Number" outlined dense required bg-color="white" />
                      <q-input v-model="asset.serialNumber" label="Serial Number" outlined dense required bg-color="white" />
                    </q-card-section>
                  </q-card>
                </div>
                <div
                  v-if="showOutdoorUnit"
                  :class="showIndoorUnit ? 'col-12 col-md-6' : 'col-12'"
                >
                  <q-card flat bordered class="bg-grey-1">
                    <q-card-section class="q-pa-sm text-overline">Outdoor Unit</q-card-section>
                    <q-card-section class="q-pt-none q-gutter-y-sm">
                      <q-input v-model="asset.outdoorModel" label="Model Number" outlined dense bg-color="white" />
                      <q-input v-model="asset.outdoorSerial" label="Serial Number" outlined dense bg-color="white" />
                    </q-card-section>
                  </q-card>
                </div>
              </div>

              <!-- Refrigerant -->
              <div v-if="showRefrigerant" class="row q-col-gutter-md">
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

              <!-- Service & Maintenance Plan -->
              <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center q-mt-md">
                <q-icon name="fas fa-calendar-check" size="xs" class="q-mr-sm" />
                Service &amp; Maintenance Plan
                <q-space />
                <span class="text-caption text-primary bg-blue-1 q-px-sm q-py-xs rounded-borders">
                  <q-icon name="fas fa-robot" size="10px" class="q-mr-xs" />
                  Auto-create schedule?
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

              <!-- Specific Location -->
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

              <div class="row q-gutter-sm justify-between q-mt-lg">
                <q-btn label="Cancel" flat color="grey-7" @click="goToProjectActions" />
                <q-btn
                  label="Complete Registration"
                  color="secondary"
                  type="submit"
                  icon="fas fa-cloud-arrow-up"
                  class="q-px-md"
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
import { useRoute, useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianAssetEntry',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const $q = useQuasar()

    const selection = reactive({
      customerId: route.query.customerId || '',
      projectId: route.query.projectId || '',
    })

    const entryTab = ref('manual')
    const selectedLibraryItem = ref(null)
    const nameplatePreview = ref('')

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
      vendorArea: '',
      vendorLocation: '',
      nameplatePhoto: null,
    })

    const unitTypeOptions = computed(() => {
      if (!asset.plantCategory) return []
      return store.plantHierachy[asset.plantCategory] || []
    })

    const showIndoorUnit = computed(() =>
      ['Direct expansion split units', 'VRF Indoor units', 'Fan coil units', 'Air handling units'].includes(
        asset.plantCategory,
      ),
    )

    const showOutdoorUnit = computed(() =>
      ['VRF condensing units', 'Package plant', 'Chiller'].includes(asset.plantCategory),
    )

    const showRefrigerant = computed(() =>
      ['Direct expansion split units', 'VRF condensing units', 'Package plant', 'Chiller'].includes(
        asset.plantCategory,
      ),
    )

    watch(
      () => asset.plantCategory,
      (next, prev) => {
        if (next !== prev) asset.unitType = ''
      },
    )

    const filterLibrary = (val, update) => update()

    const importFromLibrary = (item) => {
      asset.manufacturer = item.manufacturer
      asset.unitType = item.unitType
      asset.indoorModel = item.indoorModel
      asset.outdoorModel = item.outdoorModel
      asset.refrigerantType = item.refrigerantType
      asset.refrigerantKg = item.refrigerantCharge
      asset.serviceTime = item.serviceDuration

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
        icon: 'fas fa-file-import',
      })
    }

    const createValue = (val, done) => {
      if (val.length > 0) done(val, 'add-unique')
    }

    const onNameplateSelected = (file) => {
      if (!file) {
        nameplatePreview.value = ''
        return
      }
      nameplatePreview.value = URL.createObjectURL(file)
    }

    const startAutoFillScan = () => {
      if (store.unitLibrary?.length) {
        importFromLibrary(store.unitLibrary[0])
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

    const onSubmit = () => {
      if (!selection.customerId || !selection.projectId) {
        $q.notify({ color: 'negative', message: 'Customer and project context is missing.' })
        return
      }
      store.addAsset(selection.customerId, selection.projectId, { ...asset })
      $q.notify({ color: 'positive', message: 'Asset registered successfully' })

      const returnTo = route.query.returnTo
      if (returnTo) {
        router.push({
          path: returnTo,
          query: { customerId: selection.customerId, projectId: selection.projectId },
        })
        return
      }
      router.push({
        path: '/field/assets',
        query: { customerId: selection.customerId, projectId: selection.projectId },
      })
    }

    const goToProjectActions = () => {
      router.push({
        path: '/field/assets',
        query: { customerId: selection.customerId, projectId: selection.projectId },
      })
    }

    return {
      selection,
      asset,
      entryTab,
      selectedLibraryItem,
      nameplatePreview,
      unitTypeOptions,
      showIndoorUnit,
      showOutdoorUnit,
      showRefrigerant,
      store,
      filterLibrary,
      importFromLibrary,
      createValue,
      onNameplateSelected,
      startAutoFillScan,
      onSubmit,
      goToProjectActions,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
