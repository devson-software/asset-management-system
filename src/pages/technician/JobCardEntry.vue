<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="bg-secondary text-white">
            <div class="row items-center no-wrap">
              <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
              <q-icon name="fas fa-file-invoice" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">Create New Job Card</div>
                <div class="text-subtitle2">Capture service or repair details</div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-sm field-stepper-wrapper">
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

              <q-stepper
                v-model="currentStep"
                vertical
                animated
                header-nav
                flat
                contracted
                class="field-stepper"
              >
                <q-step
                  v-if="entryMode === 'qr'"
                  :name="1"
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

                <q-step :name="entryMode === 'qr' ? 2 : 1" title="Job Card Input" icon="fas fa-pen-to-square">
                  <div class="q-gutter-y-lg">
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
                        />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-input v-model="form.checkInTime" label="Check-In Time" type="time" outlined dense stack-label bg-color="white" />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-input v-model="form.checkOutTime" label="Check-Out Time" type="time" outlined dense stack-label bg-color="white" />
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
                        />
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
                        />
                      </div>
                      <div class="col-12">
                        <q-select
                          v-model="form.assetId"
                          :options="assetOptions"
                          label="Select Target Asset"
                          outlined
                          dense
                          emit-value
                          map-options
                          required
                          use-input
                          @filter="filterAssets"
                          bg-color="white"
                        />
                      </div>
                    </div>

                    <q-separator />

                    <div class="text-subtitle1 text-weight-bold row items-center">
                      <q-icon name="fas fa-magnifying-glass-chart" color="orange-9" class="q-mr-sm" />
                      Fault Analysis & Diagnostics
                    </div>
                    <div class="row q-col-gutter-md">
                      <div class="col-12">
                        <q-toggle
                          v-model="form.faultFound"
                          label="Was a fault discovered during this site visit?"
                          color="negative"
                          class="text-weight-bold"
                        />
                      </div>
                      <div v-if="form.faultFound" class="col-12">
                        <q-input v-model="form.faultReported" label="Fault Reported" outlined dense bg-color="white" />
                      </div>
                      <div v-if="form.faultFound" class="col-12">
                        <q-input v-model="form.rootCause" label="Root Cause Found" outlined dense bg-color="white" />
                      </div>
                      <div v-if="form.faultFound" class="col-12">
                        <q-input v-model="form.remedy" label="Remedy Applied" outlined dense bg-color="white" />
                      </div>
                    </div>

                    <q-separator />

                    <div class="text-subtitle1 text-weight-bold row items-center">
                      <q-icon name="fas fa-gauge" color="primary" class="q-mr-sm" />
                      Key Readings
                    </div>
                    <div class="row q-col-gutter-md">
                      <div class="col-12 col-sm-4">
                        <q-input v-model="form.readings.suctionPressure" label="Suction Pressure" outlined dense bg-color="white" />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-input v-model="form.readings.dischargePressure" label="Discharge Pressure" outlined dense bg-color="white" />
                      </div>
                      <div class="col-12 col-sm-4">
                        <q-input v-model="form.readings.amps" label="Compressor Amps" outlined dense bg-color="white" />
                      </div>
                    </div>

                    <q-separator />

                    <div class="text-subtitle1 text-weight-bold row items-center">
                      <q-icon name="fas fa-comment" color="teal" class="q-mr-sm" />
                      Comments
                    </div>
                    <q-input v-model="form.comments" type="textarea" label="Technician Comments" outlined dense bg-color="white" />

                    <q-btn
                      class="full-width"
                      color="primary"
                      unelevated
                      label="Continue"
                      @click="goToStep(entryMode === 'qr' ? 3 : 2)"
                    />
                  </div>
                </q-step>

                <q-step v-if="entryMode === 'qr'" :name="3" title="Closing Scan" icon="fas fa-qrcode" :done="!!scanTimes.end">
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

                <q-step :name="entryMode === 'qr' ? 4 : 2" title="Signature" icon="fas fa-pen-fancy" :done="form.signed">
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
                      <div class="signature-pad q-mt-md flex flex-center text-grey-6">Sign here</div>
                      <q-checkbox
                        v-model="form.signed"
                        label="Signature Captured"
                        color="positive"
                        size="xl"
                        class="full-width signature-checkbox q-mt-md"
                      />
                      <q-btn
                        class="full-width q-mt-md"
                        :label="'Confirm & Finalize Job Card'"
                        type="submit"
                        color="secondary"
                        unelevated
                        icon="fas fa-cloud-arrow-up"
                        :disable="!form.signed"
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
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianJobCardEntry',
  setup() {
    const $q = useQuasar()
    const router = useRouter()
    const route = useRoute()
    const entryMode = ref('qr')
    const currentStep = ref(1)
    const scanTimes = reactive({ start: '', end: '' })

    const form = reactive({
      date: new Date().toISOString().slice(0, 10),
      checkInTime: '',
      checkOutTime: '',
      assetId: null,
      techId: null,
      workType: 'Maintenance',
      faultFound: false,
      faultReported: '',
      rootCause: '',
      remedy: '',
      comments: '',
      readings: {
        suctionPressure: '',
        dischargePressure: '',
        amps: '',
      },
      signed: false,
      signedBy: 'technician',
    })

    const entryModeOptions = [
      { label: 'QR Entry', value: 'qr' },
      { label: 'Manual Entry', value: 'manual' },
    ]

    const workTypeOptions = ['Maintenance', 'Repair', 'Installation', 'Emergency Callout', 'Warranty']

    const technicians = computed(() =>
      store.users
        .filter((u) => u.role === 'technician')
        .map((u) => ({ label: u.fullName, value: u.id })),
    )

    const assetOptionsAll = computed(() => {
      const rows = []
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          project.assets.forEach((asset) => {
            rows.push({
              label: `${asset.unitRef || 'Unit'} · ${project.name}`,
              value: asset.id,
              unitRef: asset.unitRef,
              customer: customer.fullName || customer.name,
            })
          })
        })
      })
      return rows
    })

    const filteredAssets = ref(assetOptionsAll.value)

    const filterAssets = (val, update) => {
      update(() => {
        if (!val) {
          filteredAssets.value = assetOptionsAll.value
          return
        }
        const needle = val.toLowerCase()
        filteredAssets.value = assetOptionsAll.value.filter((opt) =>
          opt.label.toLowerCase().includes(needle),
        )
      })
    }

    const onSubmit = () => {
      if (!form.signed) {
        $q.notify({ color: 'negative', message: 'Please capture a signature before saving.' })
        return
      }
      const selectedAsset = assetOptionsAll.value.find((a) => a.value === form.assetId)
      const selectedTech = technicians.value.find((t) => t.value === form.techId)
      if (!selectedAsset || !selectedTech) {
        $q.notify({ color: 'negative', message: 'Please select an asset and a technician' })
        return
      }
      const jobData = {
        date: form.date.replace(/-/g, '/'),
        unitRef: selectedAsset.unitRef,
        customer: selectedAsset.customer,
        tech: selectedTech.label,
        workType: form.workType,
        faultFound: form.faultFound,
        faultReported: form.faultReported,
        rootCause: form.rootCause,
        remedy: form.remedy,
        comments: form.comments,
        readings: { ...form.readings },
        signed: form.signed,
        signedBy: form.signedBy,
        startScanTime: scanTimes.start,
        closingScanTime: scanTimes.end,
      }
      store.addJobCard(jobData)
      $q.notify({
        color: 'positive',
        message: 'New job card finalized and saved',
        icon: 'fas fa-check-circle',
        position: 'top',
      })
      router.push('/field/projects')
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

    const goToStep = (step) => {
      currentStep.value = step
    }

    const simulateScan = (mode) => {
      if (mode === 'start') {
        scanTimes.start = new Date().toLocaleTimeString()
        goToStep(2)
      } else {
        scanTimes.end = new Date().toLocaleTimeString()
        goToStep(4)
      }
    }

    return {
      entryMode,
      entryModeOptions,
      currentStep,
      scanTimes,
      form,
      workTypeOptions,
      technicians,
      assetOptions: filteredAssets,
      filterAssets,
      onSubmit,
      goToProjectActions,
      goToStep,
      simulateScan,
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
:deep(.q-stepper__step-inner) {
  padding: 0px 0px 0px 11px !important;
}
</style>
