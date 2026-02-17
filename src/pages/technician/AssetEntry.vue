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
                <div class="text-subtitle2">
                  {{ isQrMode ? 'QR scan guided entry with signature' : 'Manual entry with signature' }}
                </div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-select
                    v-model="selection.customerId"
                    :options="customerOptions"
                    label="Customer"
                    outlined
                    dense
                    emit-value
                    map-options
                    required
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-select
                    v-model="selection.projectId"
                    :options="projectOptions"
                    label="Project"
                    outlined
                    dense
                    emit-value
                    map-options
                    required
                    :disable="!selection.customerId"
                    bg-color="white"
                  />
                </div>
              </div>

              <div v-if="isQrMode" class="bg-blue-1 text-primary q-pa-md rounded-borders">
                <div class="row items-center justify-between">
                  <div class="text-subtitle2 text-weight-medium">Scan the asset QR to auto-fill details.</div>
                  <q-btn
                    color="primary"
                    icon="fas fa-qrcode"
                    label="Simulate QR Scan"
                    unelevated
                    @click="simulateScan"
                  />
                </div>
              </div>

              <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center">
                <q-icon name="fas fa-list-check" size="xs" class="q-mr-sm" />
                Asset Details
              </div>

              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.unitRef"
                    label="Unit Reference"
                    outlined
                    dense
                    required
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.manufacturer"
                    label="Manufacturer"
                    outlined
                    dense
                    required
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.unitType"
                    label="Unit Type"
                    outlined
                    dense
                    required
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.indoorModel"
                    label="Indoor Model"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.serialNumber"
                    label="Serial Number"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
                <div class="col-12 col-md-6">
                  <q-input
                    v-model="asset.installationDate"
                    type="date"
                    label="Installation Date"
                    outlined
                    dense
                    stack-label
                    bg-color="white"
                  />
                </div>
                <div class="col-12">
                  <q-input
                    v-model="asset.vendorLocation"
                    label="Location / Plant Room"
                    outlined
                    dense
                    bg-color="white"
                  />
                </div>
              </div>

              <div class="text-subtitle1 text-weight-bold text-grey-8 row items-center">
                <q-icon name="fas fa-pen-fancy" size="xs" class="q-mr-sm" />
                Signature
              </div>
              <div class="row q-col-gutter-md">
                <div class="col-12">
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
                </div>
              </div>

              <q-btn
                class="full-width"
                color="secondary"
                unelevated
                icon="fas fa-cloud-arrow-up"
                label="Save Asset"
                type="submit"
                :disable="!signature.signed"
              />
            </q-form>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianAssetEntry',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const isQrMode = computed(() => route.query.mode === 'qr')

    const selection = reactive({
      customerId: route.query.customerId || '',
      projectId: route.query.projectId || '',
    })

    const asset = reactive({
      unitRef: '',
      manufacturer: '',
      unitType: '',
      indoorModel: '',
      serialNumber: '',
      installationDate: '',
      vendorLocation: '',
    })

    const signature = reactive({
      signedBy: 'technician',
      signed: false,
    })

    const signatureOptions = [
      { label: 'Technician', value: 'technician' },
      { label: 'Customer', value: 'customer' },
    ]

    const customerOptions = computed(() =>
      store.customers.map((c) => ({ label: c.fullName, value: c.id }))
    )

    const projectOptions = computed(() => {
      const customer = store.customers.find((c) => c.id === selection.customerId)
      return customer ? customer.projects.map((p) => ({ label: p.name, value: p.id })) : []
    })

    const simulateScan = () => {
      const sample = store.unitLibrary[0]
      if (!sample) return
      asset.manufacturer = sample.manufacturer || ''
      asset.unitType = sample.unitType || ''
      asset.indoorModel = sample.indoorModel || ''
      asset.serialNumber = sample.serialNumber || ''
    }

    const onSubmit = () => {
      if (!selection.customerId || !selection.projectId || !signature.signed) return
      store.addAsset(selection.customerId, selection.projectId, {
        ...asset,
        signedBy: signature.signedBy,
        signed: signature.signed,
      })
      router.push({
        path: '/field/assets',
        query: { customerId: selection.customerId, projectId: selection.projectId },
      })
    }

    const goToProjectActions = () => {
      if (selection.projectId) {
        router.push({
          path: `/field/projects/${selection.projectId}/actions`,
          query: { customerId: selection.customerId, projectId: selection.projectId },
        })
        return
      }
      router.push('/field/projects')
    }

    return {
      isQrMode,
      selection,
      asset,
      signature,
      signatureOptions,
      customerOptions,
      projectOptions,
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
