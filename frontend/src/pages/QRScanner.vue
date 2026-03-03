<template>
  <q-page class="flex flex-center bg-grey-2">
    <div class="scanner-container text-center q-pa-lg">
      <q-card flat bordered class="scanner-card shadow-10">
        <q-card-section class="bg-primary text-white">
          <div class="text-h6 row items-center justify-center">
            <q-icon name="fas fa-qrcode" class="q-mr-sm" />
            START SCAN
          </div>
          <div class="text-subtitle2 opacity-80">Align QR code to identify HVAC unit</div>
        </q-card-section>

        <!-- Simulated Scanner for Demo/Testing -->
        <q-card-section class="q-pa-none relative-position">
          <div class="scanner-wrapper">
            <!-- Simulated scanning animation -->
            <div class="scanning-line"></div>
            <div class="scanner-overlay">
              <div class="scanner-target"></div>
            </div>
            
            <div class="absolute-center text-white text-center" style="z-index: 5">
              <q-icon name="fas fa-camera" size="64px" class="opacity-40 q-mb-md" />
              <div class="text-h6 opacity-60">Camera Preview</div>
              <div class="text-caption opacity-40">(Simulated for Development)</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-md">
          <div class="text-subtitle2 text-grey-8 q-mb-md">
            Identify asset by scanning the QR label attached to the unit.
          </div>

          <!-- Dummy Selection for Demo Purposes -->
          <div class="bg-blue-1 q-pa-md rounded-borders q-mb-md">
            <div class="text-caption text-primary q-mb-sm text-weight-bold">DEMO: SIMULATE SCAN RESULT</div>
            <q-select
              v-model="simulatedAsset"
              :options="assetOptions"
              label="Select unit to simulate scan"
              outlined
              dense
              bg-color="white"
              emit-value
              map-options
            />
            <q-btn
              color="primary"
              label="Simulate Successful Scan"
              icon="fas fa-bolt"
              class="full-width q-mt-sm"
              @click="onSimulateScan"
              :disable="!simulatedAsset"
            />
          </div>
          
          <div class="row q-gutter-sm justify-center">
            <q-btn flat color="grey-7" label="Back" icon="fas fa-arrow-left" @click="$router.back()" />
            <q-btn flat color="secondary" label="Manual Entry" icon="fas fa-keyboard" to="/assets" />
          </div>
        </q-card-section>
      </q-card>
      
      <div class="q-mt-xl text-grey-6 text-caption">
        <q-icon name="fas fa-shield-halved" class="q-mr-xs" />
        Secure Asset Identification Loop
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'QRScannerPage',
  setup() {
    const router = useRouter()
    const $q = useQuasar()
    const simulatedAsset = ref(null)

    const assetOptions = computed(() => {
      const options = []
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          p.assets.forEach(a => {
            options.push({
              label: `${a.unitRef} - ${c.name}`,
              value: a.id
            })
          })
        })
      })
      return options
    })

    const onSimulateScan = () => {
      if (!simulatedAsset.value) return

      const assetId = simulatedAsset.value
      $q.loading.show({
        message: 'Identifying unit...'
      })

      // Simulate network delay
      setTimeout(() => {
        $q.loading.hide()
        $q.notify({
          color: 'positive',
          message: 'Asset identified successfully!',
          icon: 'fas fa-check-circle'
        })
        router.push(`/assets/${assetId}`)
      }, 1000)
    }

    return {
      simulatedAsset,
      assetOptions,
      onSimulateScan
    }
  }
})
</script>

<style scoped>
.scanner-container {
  width: 100%;
  max-width: 500px;
}
.scanner-card {
  border-radius: 20px;
  overflow: hidden;
}
.scanner-wrapper {
  height: 300px;
  background: #1a1a1a;
  position: relative;
  overflow: hidden;
}
.scanner-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  pointer-events: none;
  z-index: 2;
}
.scanner-target {
  width: 180px;
  height: 180px;
  border: 2px solid rgba(255, 255, 255, 0.8);
  border-radius: 20px;
  box-shadow: 0 0 0 1000px rgba(0, 0, 0, 0.4);
}
.scanning-line {
  position: absolute;
  width: 100%;
  height: 2px;
  background: rgba(25, 118, 210, 0.8);
  box-shadow: 0 0 10px rgba(25, 118, 210, 1);
  top: 0;
  z-index: 3;
  animation: scan 2s linear infinite;
}
@keyframes scan {
  0% { top: 10%; }
  50% { top: 90%; }
  100% { top: 10%; }
}
.opacity-80 {
  opacity: 0.8;
}
.opacity-60 {
  opacity: 0.6;
}
.opacity-40 {
  opacity: 0.4;
}
.full-width {
  width: 100%;
}
</style>
