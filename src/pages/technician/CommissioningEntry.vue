<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-1">
          <q-card-section class="bg-secondary text-white">
            <div class="row items-center no-wrap">
              <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
              <q-icon name="fas fa-clipboard-check" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">Commissioning Entry</div>
                <div class="text-subtitle2">
                  {{ isQrMode ? 'QR scan flow with signature' : 'Manual flow with signature' }}
                </div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <div class="q-mb-md">
                <q-btn-toggle
                  v-model="entryMode"
                  :options="entryModeOptions"
                  color="primary"
                  toggle-color="primary"
                  unelevated
                  spread
                />
              </div>

              <q-stepper v-model="currentStep" vertical animated header-nav class="field-job-stepper">
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

                <q-step
                  v-if="entryMode === 'qr'"
                  :name="2"
                  title="Start Scan"
                  icon="fas fa-qrcode"
                  :done="!!scanTimes.start"
                >
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-qrcode" class="q-mr-sm" /> Start Scan
                    </div>
                    <q-btn
                      :color="scanTimes.start ? 'positive' : 'primary'"
                      :label="scanTimes.start ? 'Start QR Scanned' : 'Scan Start QR'"
                      icon="fas fa-qrcode"
                      class="full-width"
                      @click="simulateScan('start')"
                      :disable="!!scanTimes.start"
                    />
                  </div>
                </q-step>

                <q-step
                  :name="entryMode === 'qr' ? 3 : 2"
                  :title="entryMode === 'qr' ? 'Commissioning Input' : 'Manual Input'"
                  icon="fas fa-pen-to-square"
                >
                  <div class="q-gutter-y-lg">
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
                    <q-btn
                      class="full-width"
                      color="primary"
                      unelevated
                      label="Continue"
                      :disable="!canContinueForm"
                      @click="goToStep(entryMode === 'qr' ? 4 : 3)"
                    />
                  </div>
                </q-step>

                <q-step
                  v-if="entryMode === 'qr'"
                  :name="4"
                  title="Closing Scan"
                  icon="fas fa-qrcode"
                  :done="!!scanTimes.end"
                >
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

                <q-step
                  :name="entryMode === 'qr' ? 5 : 3"
                  title="Signature"
                  icon="fas fa-pen-fancy"
                  :done="signature.signed"
                >
                  <div class="section-container bg-white">
                    <div class="text-subtitle1 text-primary q-mb-md row items-center">
                      <q-icon name="fas fa-pen-fancy" class="q-mr-sm" /> Signature
                    </div>
                    <div class="column">
                      <q-btn-toggle
                        v-model="signature.signedBy"
                        :options="signatureOptions"
                        color="primary"
                        toggle-color="primary"
                        unelevated
                        spread
                      />
                      <div class="signature-pad q-mt-md flex flex-center text-grey-6">
                        Sign here
                      </div>
                      <q-checkbox
                        v-model="signature.signed"
                        label="Signature Captured"
                        color="positive"
                        size="xl"
                        class="full-width signature-checkbox q-mt-md"
                      />
                      <q-btn
                        class="full-width q-mt-md"
                        label="Confirm & Save Commissioning"
                        type="submit"
                        color="secondary"
                        unelevated
                        icon="fas fa-cloud-arrow-up"
                        :disable="!signature.signed"
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
import { defineComponent, reactive, computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianCommissioningEntry',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const entryMode = ref(route.query.mode === 'qr' ? 'qr' : 'manual')
    const currentStep = ref(1)

    const entryModeOptions = [
      { label: 'Manual', value: 'manual' },
      { label: 'QR Scan', value: 'qr' },
    ]

    const selectedAssetId = ref('')
    const scanTimes = reactive({ start: '', end: '' })

    const form = reactive({
      date: new Date().toISOString().slice(0, 10),
      type: '',
      status: 'In Progress',
      notes: '',
    })

    const signature = reactive({
      signedBy: 'technician',
      signed: false,
    })

    const signatureOptions = [
      { label: 'Technician', value: 'technician' },
      { label: 'Customer', value: 'customer' },
    ]

    const typeOptions = ['Pump', 'Fan', 'DX Split']
    const statusOptions = ['In Progress', 'Completed']

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
          opt.label.toLowerCase().includes(needle)
        )
      })
    }

    const selectedAssetMeta = computed(() =>
      assetOptions.value.find((opt) => opt.value === selectedAssetId.value)
    )

    const canContinueForm = computed(() => {
      return !!selectedAssetId.value && !!form.date && !!form.type && !!form.status
    })

    const goToStep = (step) => {
      if (step > currentStep.value) {
        if (!selectedAssetId.value) return
        if (entryMode.value === 'qr' && currentStep.value === 2 && !scanTimes.start) return
        if (entryMode.value === 'qr' && step === 5 && !scanTimes.end) return
        if ((entryMode.value === 'qr' ? step >= 4 : step >= 3) && !canContinueForm.value) return
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
      if (!selectedAssetMeta.value || !signature.signed) return
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
        signedBy: signature.signedBy,
        signed: signature.signed,
      })
      router.push({
        path: '/field/commissioning',
        query: { customerId: customer.id, projectId: project.id },
      })
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

    return {
      entryMode,
      entryModeOptions,
      isQrMode: computed(() => entryMode.value === 'qr'),
      currentStep,
      selectedAssetId,
      assetOptionsFiltered,
      filterAssets,
      scanTimes,
      form,
      signature,
      signatureOptions,
      typeOptions,
      statusOptions,
      canContinueForm,
      goToStep,
      simulateScan,
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
