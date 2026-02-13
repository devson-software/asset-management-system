<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <!-- Asset Header -->
      <div class="col-12 flex justify-between items-center">
        <div class="row items-center no-wrap">
          <q-btn
            flat
            round
            color="primary"
            icon="fas fa-arrow-left"
            @click="$router.back()"
            class="q-mr-md"
          />
          <div>
            <div class="text-h4 text-weight-bold text-primary">{{ asset.unitRef }}</div>
            <div class="text-subtitle1 text-grey-7">
              {{ asset.indoorModel }} | {{ asset.customerName }}
            </div>
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn color="orange-8" icon="fas fa-print" label="Print Label" @click="printQR" />
          <q-btn
            color="secondary"
            icon="fas fa-chart-line"
            label="Tech Data"
            :to="'/technical-data/' + asset.id"
          />
          <q-btn
            color="secondary"
            icon="fas fa-tools"
            label="Start Service"
            :to="'/service-entry'"
          />
        </div>
      </div>

      <!-- Detailed Information Cards -->
      <div class="col-12 col-md-6 col-lg-4">
        <q-card flat bordered class="rounded-borders full-height">
          <q-card-section class="bg-blue-1 text-primary">
            <div class="text-h6">Core Specifications</div>
          </q-card-section>
          <q-list separator>
            <q-item>
              <q-item-section>
                <q-item-label caption>Manufacturer</q-item-label>
                <q-item-label class="text-weight-bold">{{
                  asset.manufacturer || 'N/A'
                }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Unit Type</q-item-label>
                <q-item-label class="text-weight-bold text-primary">{{
                  asset.unitType || 'N/A'
                }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Indoor Unit Model / Serial</q-item-label>
                <q-item-label class="text-weight-bold"
                  >{{ asset.indoorModel }} | {{ asset.serialNumber }}</q-item-label
                >
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Outdoor Unit Model / Serial</q-item-label>
                <q-item-label class="text-weight-bold"
                  >{{ asset.outdoorModel || 'N/A' }} |
                  {{ asset.outdoorSerial || 'N/A' }}</q-item-label
                >
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Refrigerant Type / Charge</q-item-label>
                <q-item-label class="text-weight-bold"
                  >{{ asset.refrigerantType }} | {{ asset.refrigerantKg }} kg</q-item-label
                >
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Installation Date</q-item-label>
                <q-item-label class="text-weight-bold">{{ asset.installationDate || 'N/A' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="asset.serviceTime">
              <q-item-section>
                <q-item-label caption>Service Schedule / Duration</q-item-label>
                <q-item-label class="text-weight-bold text-primary">
                  <q-icon name="fas fa-repeat" size="xs" class="q-mr-xs" />
                  {{ asset.serviceSchedule }} |
                  <q-icon name="fas fa-clock" size="xs" class="q-ml-sm q-mr-xs" />
                  {{ asset.serviceTime }}
                </q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card>
      </div>

      <div class="col-12 col-md-6 col-lg-4">
        <q-card flat bordered class="rounded-borders full-height">
          <q-card-section class="bg-orange-1 text-orange-9">
            <div class="text-h6">Vendor Details</div>
          </q-card-section>
          <q-list separator>
            <q-item>
              <q-item-section avatar><q-icon name="fas fa-map" color="orange-9" /></q-item-section>
              <q-item-section>
                <q-item-label caption>Area</q-item-label>
                <q-item-label class="text-weight-bold">{{
                  asset.vendorArea || 'N/A'
                }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-location-dot" color="orange-9"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Specific Location</q-item-label>
                <q-item-label class="text-weight-bold">{{
                  asset.vendorLocation || 'N/A'
                }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-map-pin" color="orange-9"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Physical Address</q-item-label>
                <q-item-label class="text-weight-bold">{{
                  asset.siteAddress || 'N/A'
                }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card>
      </div>

      <div class="col-12 col-md-6 col-lg-4">
        <q-card flat bordered class="rounded-borders full-height">
          <q-card-section class="bg-green-1 text-positive">
            <div class="text-h6">Installation Context</div>
          </q-card-section>
          <q-list separator>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-building" color="grey-7"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Customer</q-item-label>
                <q-item-label class="text-weight-bold">{{ asset.customerName }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-clipboard-list" color="grey-7"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Project / Site</q-item-label>
                <q-item-label class="text-weight-bold">{{ asset.projectName }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-map-marker-alt" color="grey-7"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Site Address</q-item-label>
                <q-item-label class="text-weight-bold">{{ asset.siteAddress }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section avatar
                ><q-icon name="fas fa-info-circle" color="grey-7"
              /></q-item-section>
              <q-item-section>
                <q-item-label caption>Status</q-item-label>
                <q-badge
                  :color="asset.status === 'Registered' ? 'positive' : 'orange'"
                  class="q-px-sm"
                  style="width: fit-content"
                >
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
          <q-tabs
            v-model="tab"
            dense
            class="text-grey"
            active-color="primary"
            indicator-color="primary"
            align="justify"
            narrow-indicator
          >
            <q-tab name="history" label="Service History" />
            <q-tab name="tasks" label="Maintenance Plan" />
            <q-tab name="docs" label="Technical Drawings" />
          </q-tabs>

          <q-separator />

          <q-tab-panels v-model="tab" animated>
            <q-tab-panel name="history" class="q-pa-none">
              <q-table
                :rows="assetServiceHistory"
                :columns="historyColumns"
                row-key="id"
                flat
                class="history-table"
                :pagination="{ rowsPerPage: 5 }"
              >
                <template v-slot:header="props">
                  <q-tr :props="props">
                    <q-th
                      v-for="col in props.cols"
                      :key="col.name"
                      :props="props"
                      :class="'text-' + col.align"
                    >
                      <div
                        class="row items-center no-wrap"
                        :class="
                          col.align === 'center'
                            ? 'justify-center'
                            : col.align === 'right'
                              ? 'justify-end'
                              : ''
                        "
                      >
                        <q-icon
                          v-if="col.sortable"
                          :name="
                            props.pagination && props.pagination.sortBy === col.name
                              ? props.pagination.descending
                                ? 'fas fa-arrow-down-long'
                                : 'fas fa-arrow-up-long'
                              : 'fas fa-arrow-up-long'
                          "
                          size="10px"
                          class="q-mr-xs cursor-pointer sort-icon"
                          :class="{
                            active: props.pagination && props.pagination.sortBy === col.name,
                          }"
                          @click="props.sort(col)"
                        />
                        <span>{{ col.label }}</span>
                      </div>
                    </q-th>
                  </q-tr>
                </template>

                <template v-slot:body-cell-type="props">
                  <q-td :props="props">
                    <q-chip
                      dense
                      :color="
                        props.value.includes('Breakdown') || props.value.includes('Emergency')
                          ? 'red-1'
                          : 'blue-1'
                      "
                      :text-color="
                        props.value.includes('Breakdown') || props.value.includes('Emergency')
                          ? 'red-9'
                          : 'blue-9'
                      "
                      class="text-weight-bold"
                      size="sm"
                    >
                      {{ props.value.toUpperCase() }}
                    </q-chip>
                  </q-td>
                </template>

                <template v-slot:body-cell-actions="props">
                  <q-td :props="props">
                    <q-btn
                      flat
                      dense
                      color="primary"
                      label="Details"
                      :to="'/service-entry/' + assetId"
                      size="sm"
                    />
                  </q-td>
                </template>
              </q-table>

              <div
                v-if="assetServiceHistory.length === 0"
                class="column items-center q-pa-xl text-grey-5 bg-white"
              >
                <q-icon name="fas fa-folder-open" size="64px" color="grey-3" />
                <div class="text-subtitle1 q-mt-md">No service records found.</div>
              </div>
            </q-tab-panel>

            <q-tab-panel name="tasks" class="q-pa-md">
              <div class="row q-col-gutter-md">
                <div
                  v-for="(def, type) in store.serviceDefinitions"
                  :key="type"
                  class="col-12 col-md-6 col-lg-4"
                >
                  <q-card flat bordered class="rounded-borders full-height bg-grey-1">
                    <q-card-section class="q-pb-none flex justify-between items-center">
                      <div class="text-subtitle1 text-weight-bold text-primary">{{ type }}</div>
                      <q-chip
                        dense
                        color="blue-1"
                        text-color="primary"
                        icon="fas fa-clock"
                        size="sm"
                      >
                        {{ def.duration }}
                      </q-chip>
                    </q-card-section>
                    <q-card-section>
                      <q-list dense>
                        <q-item
                          v-for="(task, idx) in def.tasks"
                          :key="idx"
                          class="q-px-none min-height-0"
                        >
                          <q-item-section avatar class="min-width-0 q-pr-sm">
                            <q-icon name="fas fa-check-circle" color="primary" size="14px" />
                          </q-item-section>
                          <q-item-section class="text-caption">{{ task }}</q-item-section>
                        </q-item>
                      </q-list>
                    </q-card-section>
                  </q-card>
                </div>
              </div>
            </q-tab-panel>

            <q-tab-panel name="docs">
              <div class="text-center q-pa-xl text-grey-6">
                <q-icon name="fas fa-file-pdf" size="xl" />
                <div class="q-mt-sm">
                  PDF Drawing (A3) available. <q-btn flat color="primary" label="Open PDF" />
                </div>
              </div>
            </q-tab-panel>
          </q-tab-panels>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { store } from '../../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AssetDetailsPage',
  setup() {
    const route = useRoute()
    const $q = useQuasar()
    const tab = ref('history')
    const assetId = route.params.assetId

    const historyColumns = [
      { name: 'date', label: 'Service Date', align: 'left', field: 'date', sortable: true },
      { name: 'type', label: 'Service Type', align: 'left', field: 'type', sortable: true },
      { name: 'actions', label: '', align: 'right' },
    ]

    const asset = computed(() => {
      let foundAsset = null
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          const a = project.assets.find((item) => item.id === assetId)
          if (a) {
            foundAsset = {
              ...a,
              customerName: customer.name,
              projectName: project.name,
              siteAddress: project.siteAddress,
            }
          }
        })
      })
      return foundAsset || { unitRef: 'Not Found', indoorModel: 'N/A' }
    })

    const assetServiceHistory = computed(() => {
      if (!asset.value || !asset.value.unitRef) return []
      return store.services
        .filter((s) => s.unitRef === asset.value.unitRef)
        .sort((a, b) => new Date(b.date) - new Date(a.date))
    })

    const printQR = () => {
      $q.notify({
        message: `Sent QR sticker for ${asset.value.unitRef} to label printer.`,
        color: 'orange-8',
        icon: 'fas fa-print',
      })
    }

    return { asset, tab, printQR, assetServiceHistory, historyColumns, assetId, store }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.full-height {
  height: 100%;
}
.min-height-0 {
  min-height: unset;
  padding: 4px 0;
}
.min-width-0 {
  min-width: unset;
}
</style>
