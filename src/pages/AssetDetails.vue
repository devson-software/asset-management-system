<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row q-col-gutter-lg">
        <!-- Asset Header -->
        <div class="col-12 flex justify-between items-center">
          <div>
            <div class="text-h4 text-weight-bold text-primary">{{ asset.unitRef }}</div>
            <div class="text-subtitle1 text-grey-7">{{ asset.indoorModel }} | {{ asset.customerName }}</div>
          </div>
          <div class="q-gutter-sm">
            <q-btn color="orange-8" icon="print" label="Print Label" @click="printQR" />
            <q-btn color="secondary" icon="analytics" label="Tech Data" :to="'/technical-data/' + asset.id" />
            <q-btn color="secondary" icon="handyman" label="Start Service" :to="'/service-entry'" />
            <q-btn flat color="primary" icon="arrow_back" label="Back to List" @click="$router.back()" />
          </div>
        </div>

        <!-- Detailed Information Cards -->
        <div class="col-12 col-md-6">
          <q-card flat bordered class="rounded-borders full-height">
            <q-card-section class="bg-blue-1 text-primary">
              <div class="text-h6">Core Specifications</div>
            </q-card-section>
            <q-list separator>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Indoor Unit Model</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.indoorModel }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Serial Number</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.serialNumber }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Refrigerant Type</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.refrigerantType }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Refrigerant Charge (kg)</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.refrigerantKg }} kg</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-md-6">
          <q-card flat bordered class="rounded-borders full-height">
            <q-card-section class="bg-green-1 text-positive">
              <div class="text-h6">Installation Context</div>
            </q-card-section>
            <q-list separator>
              <q-item>
                <q-item-section avatar><q-icon name="business" color="grey-7" /></q-item-section>
                <q-item-section>
                  <q-item-label caption>Customer</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.customerName }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section avatar><q-icon name="assignment" color="grey-7" /></q-item-section>
                <q-item-section>
                  <q-item-label caption>Project / Site</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.projectName }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section avatar><q-icon name="place" color="grey-7" /></q-item-section>
                <q-item-section>
                  <q-item-label caption>Site Address</q-item-label>
                  <q-item-label class="text-weight-bold">{{ asset.siteAddress }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section avatar><q-icon name="info" color="grey-7" /></q-item-section>
                <q-item-section>
                  <q-item-label caption>Status</q-item-label>
                  <q-badge :color="asset.status === 'Registered' ? 'positive' : 'orange'" class="q-px-sm" style="width: fit-content">
                    {{ asset.status }}
                  </q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <!-- History & Documentation Placeholder -->
        <div class="col-12">
          <q-card flat bordered class="rounded-borders">
            <q-tabs v-model="tab" dense class="text-grey" active-color="primary" indicator-color="primary" align="justify" narrow-indicator>
              <q-tab name="history" label="Service History" />
              <q-tab name="docs" label="Technical Drawings" />
            </q-tabs>

            <q-separator />

            <q-tab-panels v-model="tab" animated>
              <q-tab-panel name="history">
                <div class="text-center q-pa-xl text-grey-6">
                  <q-icon name="history" size="xl" />
                  <div class="q-mt-sm">Last service performed on 14.10.2025. No faults found.</div>
                </div>
              </q-tab-panel>

              <q-tab-panel name="docs">
                <div class="text-center q-pa-xl text-grey-6">
                  <q-icon name="picture_as_pdf" size="xl" />
                  <div class="q-mt-sm">PDF Drawing (A3) available. <q-btn flat color="primary" label="Open PDF" /></div>
                </div>
              </q-tab-panel>
            </q-tab-panels>
          </q-card>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AssetDetailsPage',
  setup () {
    const route = useRoute()
    const $q = useQuasar()
    const tab = ref('history')
    const assetId = route.params.assetId

    const asset = computed(() => {
      let foundAsset = null
      store.customers.forEach(customer => {
        customer.projects.forEach(project => {
          const a = project.assets.find(item => item.id === assetId)
          if (a) {
            foundAsset = {
              ...a,
              customerName: customer.name,
              projectName: project.name,
              siteAddress: project.siteAddress
            }
          }
        })
      })
      return foundAsset || { unitRef: 'Not Found', indoorModel: 'N/A' }
    })

    const printQR = () => {
      $q.notify({ message: `Sent QR sticker for ${asset.value.unitRef} to label printer.`, color: 'orange-8', icon: 'print' })
    }

    return { asset, tab, printQR }
  }
})
</script>

<style scoped>
.rounded-borders { border-radius: 12px; }
.full-height { height: 100%; }
</style>

