<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12">
        <q-card flat bordered class="rounded-borders shadow-1 commissioning-master-card">
          <q-card-section class="bg-secondary text-white">
            <div class="row items-center no-wrap">
              <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
              <q-icon name="fas fa-clipboard-check" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">Commissioning Master</div>
                <div class="text-subtitle2">Select asset, scan, capture notes, and sign off</div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-sm field-stepper-wrapper">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <q-stepper
                v-model="currentStep"
                vertical
                animated
                header-nav
                flat
                contracted
                class="field-stepper"
              >
                <q-step :name="1" title="Select Asset" icon="fas fa-snowflake" :done="!!selectedAssetId">
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-snowflake" class="q-mr-sm" /> Asset Selection
                    </div>
                    <q-select
                      v-model="selectedAssetId"
                      :options="assetOptionsFiltered"
                      label="Select Asset"
                      outlined
                      dense
                      emit-value
                      map-options
                      required
                      use-input
                      bg-color="white"
                      @filter="filterAssets"
                    />
                    <div class="row q-col-gutter-sm q-mt-sm">
                      <div class="col-12 col-sm-6">
                        <q-btn
                          class="full-width"
                          color="primary"
                          unelevated
                          icon="fas fa-pen-to-square"
                          label="Add Asset (Manual)"
                          @click="goToAddAsset('manual')"
                        />
                      </div>
                      <div class="col-12 col-sm-6">
                        <q-btn
                          class="full-width"
                          color="secondary"
                          unelevated
                          icon="fas fa-qrcode"
                          label="Add Asset (Scan)"
                          @click="goToAddAsset('qr')"
                        />
                      </div>
                    </div>
                    <q-btn
                      class="full-width q-mt-md"
                      color="primary"
                      unelevated
                      label="Continue"
                      :disable="!selectedAssetId"
                      @click="goToStep(2)"
                    />
                  </div>
                </q-step>

                <q-step :name="2" title="Open Scan" icon="fas fa-qrcode" :done="!!scanTimes.start">
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-qrcode" class="q-mr-sm" /> Open Scan
                    </div>
                    <q-btn
                      :color="scanTimes.start ? 'positive' : 'primary'"
                      :label="scanTimes.start ? 'Open QR Scanned' : 'Scan Open QR'"
                      icon="fas fa-qrcode"
                      class="full-width"
                      @click="simulateScan('start')"
                      :disable="!!scanTimes.start"
                    />
                  </div>
                </q-step>

                <q-step :name="3" title="Commissioning Input" icon="fas fa-pen-to-square">
                  <div class="q-gutter-y-lg">
                    <div class="section-container bg-white">
                      <div class="text-subtitle1 text-weight-bold text-primary q-mb-md row items-center">
                        <q-icon name="fas fa-circle-info" size="xs" class="q-mr-sm" />
                        Asset / Project Information
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.project" label="Project Name" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.customer" label="Client" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.site" label="Site / Location" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.assetId" label="Asset ID" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.manufacturer" label="Manufacturer" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.model" label="Model" outlined dense readonly bg-color="grey-1" />
                        </div>
                        <div class="col-12 col-md-6">
                          <q-input v-model="form.serialNo" label="Serial No" outlined dense readonly bg-color="grey-1" />
                        </div>
                      </div>
                    </div>

                    <div class="row q-col-gutter-md">
                      <div class="col-12 col-sm-4">
                        <q-input
                          v-model="form.date"
                          label="Commission Date"
                          type="date"
                          outlined
                          dense
                          stack-label
                          required
                          bg-color="white"
                        />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-select
                          v-model="form.type"
                          :options="typeOptions"
                          label="Commission Type"
                          outlined
                          dense
                          required
                          bg-color="white"
                        />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-select
                          v-model="form.status"
                          :options="statusOptions"
                          label="Status"
                          outlined
                          dense
                          required
                          bg-color="white"
                        />
                      </div>
                      <div class="col-12">
                        <q-input
                          v-model="form.notes"
                          type="textarea"
                          label="Notes"
                          outlined
                          dense
                          bg-color="white"
                        />
                      </div>
                    </div>

                    <q-separator />

                    <div class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-bolt" size="xs" class="q-mr-sm" />
                        Electrical Measurements
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.elec.l1l2" label="L1-L2 (V)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.elec.l2l3" label="L2-L3 (V)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.elec.l1l3" label="L1-L3 (V)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.elec.ratedAmps" label="Rated Amps" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.elec.measuredAmps" label="Measured Amps" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-select v-model="form.elec.pass" :options="['Pass', 'Fail']" label="Result" outlined dense bg-color="white" />
                        </div>
                      </div>
                    </div>

                    <q-separator />

                    <div v-if="!form.type" class="text-caption text-grey-7">
                      Select a commission type to show the form.
                    </div>

                    <div v-if="form.type === 'Pump'" class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-faucet-drip" size="xs" class="q-mr-sm" />
                        Pump Measurements
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.suction" label="Suction Pressure (kPa / bar)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.discharge" label="Discharge Pressure (kPa / bar)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.flowRate" label="Calculated Flow Rate (L/s)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.waterTemp" label="Water Temperature (°C)" outlined dense bg-color="white" />
                        </div>
                      </div>
                    </div>

                    <div v-if="form.type === 'Fan'" class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-wind" size="xs" class="q-mr-sm" />
                        Fan Measurements
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.airflow" label="Measured Airflow (L/s)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.staticPressure" label="External Static Pressure (Pa)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.fanSpeed" label="Fan Speed (RPM or %)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.vibration" label="Vibration Level (mm/s)" outlined dense bg-color="white" />
                        </div>
                      </div>
                    </div>

                    <div v-if="form.type === 'DX Split'" class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-snowflake" size="xs" class="q-mr-sm" />
                        DX Split Measurements
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.returnAir" label="Return Air Temp (Off Coil) °C" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.supplyAir" label="Supply Air Temp (On Coil) °C" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.suctionLine" label="Suction Line Temp °C" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-4">
                          <q-input v-model="form.perf.liquidLine" label="Liquid Line Temp °C" outlined dense bg-color="white" />
                        </div>
                      </div>
                    </div>

                    <q-separator />

                    <div class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-clipboard-check" size="xs" class="q-mr-sm" />
                        Functional Checks
                      </div>
                      <q-list separator class="border-grey rounded-borders bg-white">
                        <q-item
                          v-for="(check, index) in functionalChecks"
                          :key="index"
                          tag="label"
                          v-ripple
                        >
                          <q-item-section side top>
                            <q-checkbox v-model="check.value" color="positive" />
                          </q-item-section>
                          <q-item-section>
                            <q-item-label class="text-grey-9">{{ check.label }}</q-item-label>
                          </q-item-section>
                          <q-item-section side>
                            <q-badge :color="check.value ? 'positive' : 'grey-4'" class="q-px-sm">{{
                              check.value ? 'YES' : 'NO'
                            }}</q-badge>
                          </q-item-section>
                        </q-item>
                      </q-list>
                    </div>

                    <q-separator />

                    <div class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-primary row items-center">
                        <q-icon name="fas fa-file-lines" size="xs" class="q-mr-sm" />
                        Open Commissioning Jobcard
                      </div>
                      <q-input
                        v-model="form.jobcardNotes"
                        type="textarea"
                        label="Jobcard Notes"
                        outlined
                        dense
                        bg-color="white"
                      />
                      <q-file
                        v-model="form.jobcardPhotos"
                        label="Upload Pictures"
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

                    <q-separator />

                    <div class="q-gutter-y-md">
                      <div class="text-subtitle1 text-weight-bold text-grey-9 row items-center">
                        <q-icon name="fas fa-signature" size="xs" class="q-mr-sm" />
                        Summary & Sign-off
                      </div>
                      <div class="row q-col-gutter-md">
                        <div class="col-12 col-sm-6">
                          <q-input v-model="form.engineer" label="Commissioned By (Engineer)" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12 col-sm-6">
                          <q-input v-model="form.witness" label="Witness" outlined dense bg-color="white" />
                        </div>
                        <div class="col-12">
                          <q-input v-model="form.comments" label="Outstanding Issues / Comments" type="textarea" outlined dense bg-color="white" />
                        </div>
                      </div>
                    </div>

                    <q-btn
                      class="full-width"
                      color="primary"
                      unelevated
                      label="Continue"
                      :disable="!canContinueForm"
                      @click="goToStep(4)"
                    />
                  </div>
                </q-step>

                <q-step :name="4" title="Closing Scan" icon="fas fa-qrcode" :done="!!scanTimes.end">
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-qrcode" class="q-mr-sm" /> Closing Scan
                    </div>
                    <q-btn
                      :color="scanTimes.end ? 'positive' : 'primary'"
                      :label="scanTimes.end ? 'Exit QR Scanned' : 'Scan Exit QR'"
                      icon="fas fa-qrcode"
                      class="full-width"
                      @click="simulateScan('end')"
                      :disable="!!scanTimes.end"
                    />
                  </div>
                </q-step>

                <q-step :name="5" title="Signature" icon="fas fa-pen-fancy" :done="signature.confirmed">
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-pen-fancy" class="q-mr-sm" /> Signature
                    </div>
                    <div class="column">
                      <div class="row q-col-gutter-sm">
                        <div class="col-6">
                          <q-btn
                            class="full-width"
                            label="Technician"
                            color="primary"
                            :outline="!signature.techSigned"
                            :text-color="signature.techSigned ? 'white' : 'primary'"
                            unelevated
                            @click="signature.techSigned = !signature.techSigned"
                          />
                        </div>
                        <div class="col-6">
                          <q-btn
                            class="full-width"
                            label="Customer"
                            color="secondary"
                            :outline="!signature.customerSigned"
                            :text-color="signature.customerSigned ? 'white' : 'secondary'"
                            unelevated
                            @click="signature.customerSigned = !signature.customerSigned"
                          />
                        </div>
                      </div>
                      <div class="signature-pad q-mt-md flex flex-center text-grey-6">
                        Sign here
                      </div>
                      <q-checkbox
                        v-model="signature.confirmed"
                        label="Signature Captured"
                        color="positive"
                        size="xl"
                        class="full-width signature-checkbox q-mt-md"
                      />
                      <q-btn
                        class="full-width q-mt-md"
                        label="Save & Add Another Asset"
                        type="submit"
                        color="secondary"
                        unelevated
                        icon="fas fa-cloud-arrow-up"
                        :disable="!hasSignature || !signature.confirmed"
                      />
                    </div>
                  </div>
                </q-step>
              </q-stepper>
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
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianCommissioningMaster',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const currentStep = ref(1)
    const selectedAssetId = ref('')
    const scanTimes = reactive({ start: '', end: '' })

    const form = reactive({
      date: new Date().toISOString().slice(0, 10),
      type: '',
      status: 'In Progress',
      notes: '',
      project: '',
      customer: '',
      site: '',
      assetId: '',
      manufacturer: '',
      model: '',
      serialNo: '',
      jobcardNotes: '',
      jobcardPhotos: [],
      elec: {
        l1l2: '',
        l2l3: '',
        l1l3: '',
        ratedAmps: '',
        measuredAmps: '',
        pass: 'Pass',
      },
      perf: {
        suction: '',
        discharge: '',
        flowRate: '',
        airflow: '',
        waterTemp: '',
        staticPressure: '',
        fanSpeed: '',
        vibration: '',
        returnAir: '',
        supplyAir: '',
        suctionLine: '',
        liquidLine: '',
      },
      engineer: 'Jeram HVAC',
      witness: '',
      comments: '',
    })

    const signature = reactive({
      techSigned: false,
      customerSigned: false,
      confirmed: false,
    })

    const typeOptions = ['Pump', 'Fan', 'DX Split']
    const statusOptions = ['In Progress', 'Completed']
    const functionalChecks = reactive([
      { label: 'Correct rotation / airflow direction', value: false },
      { label: 'No abnormal noise or vibration', value: false },
      { label: 'Isolation valves open and tagged', value: false },
      { label: 'Safety interlocks proven', value: false },
      { label: 'Controller / remote operation verified', value: false },
      { label: 'Condensate drainage free and clear', value: false },
    ])

    const assetOptions = computed(() => {
      const rows = []
      const customerId = route.query.customerId || ''
      const projectId = route.query.projectId || ''
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          if (customerId && customer.id !== customerId) return
          if (projectId && project.id !== projectId) return
          project.assets.forEach((asset) => {
            rows.push({
              label: `${asset.unitRef || 'Unit'} · ${project.name}`,
              value: asset.id,
              asset,
              customer,
              project,
            })
          })
        })
      })
      return rows
    })

    const assetOptionsFiltered = ref(assetOptions.value)

    const filterAssets = (val, update) => {
      update(() => {
        if (!val) {
          assetOptionsFiltered.value = assetOptions.value
          return
        }
        const needle = val.toLowerCase()
        assetOptionsFiltered.value = assetOptions.value.filter((opt) =>
          opt.label.toLowerCase().includes(needle),
        )
      })
    }

    const selectedAssetMeta = computed(() =>
      assetOptions.value.find((opt) => opt.value === selectedAssetId.value),
    )

    const canContinueForm = computed(() => {
      return !!selectedAssetId.value && !!form.date && !!form.type && !!form.status
    })

    const hasSignature = computed(() => signature.techSigned || signature.customerSigned)

    const deriveCommissioningType = (asset) => {
      const category = (asset.plantCategory || '').toLowerCase()
      const unitType = (asset.unitType || '').toLowerCase()
      if (category.includes('pump') || unitType.includes('pump')) return 'Pump'
      if (category.includes('fan') || unitType.includes('fan')) return 'Fan'
      if (category.includes('dx') || category.includes('vrf') || category.includes('split')) return 'DX Split'
      if (unitType.includes('dx') || unitType.includes('vrf') || unitType.includes('split')) return 'DX Split'
      return ''
    }

    const goToStep = (step) => {
      if (step > currentStep.value) {
        if (!selectedAssetId.value) return
        if (currentStep.value === 2 && !scanTimes.start) return
        if (step === 5 && !scanTimes.end) return
        if (step >= 4 && !canContinueForm.value) return
      }
      currentStep.value = step
    }

    const simulateScan = (type) => {
      if (type === 'start') {
        scanTimes.start = new Date().toISOString()
        goToStep(3)
      }
      if (type === 'end') {
        scanTimes.end = new Date().toISOString()
        goToStep(5)
      }
    }

    const onSubmit = () => {
      if (!selectedAssetMeta.value || !signature.confirmed || !hasSignature.value) return
      const { asset, customer, project } = selectedAssetMeta.value
      store.addCommissioningRecord({
        date: form.date.replaceAll('-', '/'),
        unitRef: asset.unitRef || '',
        type: form.type,
        customer: customer.fullName,
        project: project.name,
        customerId: customer.id,
        projectId: project.id,
        status: form.status,
        projectName: form.project,
        customerName: form.customer,
        site: form.site,
        assetId: form.assetId,
        manufacturer: form.manufacturer,
        model: form.model,
        serialNo: form.serialNo,
        elec: { ...form.elec },
        perf: { ...form.perf },
        functionalChecks: functionalChecks.map((c) => ({ ...c })),
        engineer: form.engineer,
        witness: form.witness,
        comments: form.comments,
        jobcardNotes: form.jobcardNotes,
        jobcardPhotos: [...form.jobcardPhotos],
        signedBy: signature.techSigned ? 'technician' : 'customer',
        signedTech: signature.techSigned,
        signedCustomer: signature.customerSigned,
        signed: signature.confirmed,
      })
      resetForm()
      currentStep.value = 1
    }

    const goToProjectActions = () => {
      const projectId = route.query.projectId || ''
      const customerId = route.query.customerId || ''
      if (projectId) {
        router.push({ path: `/field/projects/${projectId}/actions`, query: { customerId, projectId } })
        return
      }
      router.push('/field/projects')
    }

    const goToAddAsset = (mode) => {
      router.push({
        path: '/field/assets/add',
        query: {
          customerId: route.query.customerId || '',
          projectId: route.query.projectId || '',
          mode,
          returnTo: '/field/commissioning-master',
        },
      })
    }

    const resetForm = () => {
      selectedAssetId.value = ''
      scanTimes.start = ''
      scanTimes.end = ''
      form.date = new Date().toISOString().slice(0, 10)
      form.type = ''
      form.status = 'In Progress'
      form.notes = ''
      form.project = ''
      form.customer = ''
      form.site = ''
      form.assetId = ''
      form.manufacturer = ''
      form.model = ''
      form.serialNo = ''
      form.jobcardNotes = ''
      form.jobcardPhotos = []
      form.elec = { l1l2: '', l2l3: '', l1l3: '', ratedAmps: '', measuredAmps: '', pass: '' }
      form.perf = {
        suction: '',
        discharge: '',
        flowRate: '',
        airflow: '',
        waterTemp: '',
        staticPressure: '',
        fanSpeed: '',
        vibration: '',
        returnAir: '',
        supplyAir: '',
        suctionLine: '',
        liquidLine: '',
      }
      form.engineer = 'Jeram HVAC'
      form.witness = ''
      form.comments = ''
      functionalChecks.forEach((check) => {
        check.value = false
      })
      signature.techSigned = false
      signature.customerSigned = false
      signature.confirmed = false
    }

    watch(
      () => selectedAssetId.value,
      (val) => {
        if (!val) return
        const selected = assetOptions.value.find((opt) => opt.value === val)
        if (selected?.asset) {
          const inferredType = deriveCommissioningType(selected.asset)
          if (inferredType) form.type = inferredType
          form.project = selected.project?.name || ''
          form.customer = selected.customer?.fullName || selected.customer?.name || ''
          form.site = selected.project?.siteAddress || ''
          form.assetId = selected.asset?.id || ''
          form.manufacturer = selected.asset?.manufacturer || ''
          form.model = selected.asset?.indoorModel || selected.asset?.outdoorModel || ''
          form.serialNo = selected.asset?.serialNumber || selected.asset?.outdoorSerial || ''
        }
      },
    )

    return {
      currentStep,
      selectedAssetId,
      assetOptionsFiltered,
      filterAssets,
      scanTimes,
      form,
      signature,
      typeOptions,
      statusOptions,
      canContinueForm,
      hasSignature,
      functionalChecks,
      goToStep,
      simulateScan,
      onSubmit,
      goToProjectActions,
      goToAddAsset,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
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
.field-stepper-wrapper {
  padding-left: 8px !important;
  padding-right: 8px !important;
}
.field-stepper .q-stepper__content,
.field-stepper .q-stepper__step-content,
.field-stepper .q-stepper__step-content-inner {
  padding: 0 !important;
  margin-left: 0 !important;
}
.field-stepper :deep(.q-stepper__header) {
  padding: 0 !important;
  margin-left: 0 !important;
}
.field-stepper :deep(.q-stepper__tab) {
  padding: 6px 0 !important;
  min-height: 44px;
  margin-left: 0 !important;
}
.field-stepper :deep(.q-stepper__title) {
  font-size: 14px;
}
.field-stepper :deep(.q-stepper__caption) {
  font-size: 12px;
}
</style>
