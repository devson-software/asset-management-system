<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Job Card History</div>
          <div class="text-subtitle1 text-grey-7">
            Digital archive of all technical service and repair records
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn-dropdown
            color="secondary"
            icon="fas fa-plus"
            label="Create Job Card"
            class="shadow-2"
          >
            <q-list>
              <q-item clickable v-close-popup to="/qr-scanner">
                <q-item-section avatar>
                  <q-icon name="fas fa-qrcode" color="primary" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">Scan QR Code</q-item-label>
                  <q-item-label caption>Quick identification</q-item-label>
                </q-item-section>
              </q-item>

              <q-item clickable v-close-popup to="/job-cards/add">
                <q-item-section avatar>
                  <q-icon name="fas fa-keyboard" color="grey-7" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">Add Manually</q-item-label>
                  <q-item-label caption>Search for unit manually</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
          <q-btn
            color="green-7"
            icon="fas fa-file-excel"
            label="Export XLSX"
            @click="exportToExcel"
            class="shadow-2"
          />
          <q-btn
            outline
            color="primary"
            icon="fas fa-paper-plane"
            label="Email Summary"
            @click="emailSummary"
            class="shadow-2"
          />
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="filteredRows"
            :columns="columns"
            row-key="id"
            flat
            :filter="filter"
            class="job-cards-table"
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
                    <!-- Sort Icon on the Left -->
                    <q-icon
                      v-if="col.sortable"
                      :name="
                        props.pagination && props.pagination.sortBy === col.name
                          ? props.pagination.descending
                            ? 'fas fa-arrow-down-long'
                            : 'fas fa-arrow-up-long'
                          : 'fas fa-arrow-up-long'
                      "
                      size="12px"
                      class="q-mr-xs cursor-pointer sort-icon"
                      :class="{ active: props.pagination && props.pagination.sortBy === col.name }"
                      @click="props.sort(col)"
                    />
                    <span class="cursor-pointer" @click="col.sortable && props.sort(col)">{{
                      col.label
                    }}</span>
                    <q-btn
                      v-if="col.name !== 'actions'"
                      flat
                      round
                      dense
                      size="xs"
                      icon="fas fa-filter"
                      class="q-ml-xs filter-btn"
                      :class="{ active: columnFilters[col.name] }"
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
              <q-input
                borderless
                dense
                debounce="300"
                v-model="filter"
                placeholder="Search Job Cards..."
              >
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-id="props">
              <q-td :props="props">
                <div class="text-weight-bold text-primary">{{ props.row.id }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip
                  :color="props.row.faultFound ? 'red-1' : 'green-1'"
                  :text-color="props.row.faultFound ? 'red-9' : 'green-9'"
                  :icon="
                    props.row.faultFound ? 'fas fa-triangle-exclamation' : 'fas fa-circle-check'
                  "
                  label="Completed"
                  dense
                  class="text-weight-bold"
                />
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu
                    auto-close
                    transition-show="scale"
                    transition-hide="scale"
                    class="rounded-borders shadow-2"
                  >
                    <q-list style="min-width: 150px">
                      <q-item clickable :to="'/job-cards/edit/' + props.row.id">
                        <q-item-section avatar
                          ><q-icon name="fas fa-edit" color="primary" size="sm"
                        /></q-item-section>
                        <q-item-section>Edit Job Card</q-item-section>
                      </q-item>
                      <q-item clickable @click="viewJobCard(props.row)">
                        <q-item-section avatar
                          ><q-icon name="fas fa-eye" color="grey-7" size="sm"
                        /></q-item-section>
                        <q-item-section>View Details</q-item-section>
                      </q-item>
                      <q-item clickable>
                        <q-item-section avatar
                          ><q-icon name="fas fa-file-pdf" color="secondary" size="sm"
                        /></q-item-section>
                        <q-item-section>Download PDF</q-item-section>
                      </q-item>
                      <q-item v-if="props.row.faultFound" clickable>
                        <q-item-section avatar
                          ><q-icon name="fas fa-file-invoice-dollar" color="orange-9" size="sm"
                        /></q-item-section>
                        <q-item-section>Issue Quote</q-item-section>
                      </q-item>
                    </q-list>
                  </q-menu>
                </q-btn>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>

      <div class="col-12">
        <q-banner rounded class="bg-blue-1 text-primary q-pa-md">
          <template v-slot:avatar>
            <q-icon name="fas fa-cloud-arrow-up" color="primary" />
          </template>
          <div class="text-subtitle2 text-weight-bold">Cloud Access Secured</div>
          <div class="text-body2">
            Job cards are digitally stored and permanently linked to assets for historical tracking.
          </div>
        </q-banner>
      </div>
    </div>

    <!-- View Job Card Dialog -->
    <q-dialog v-model="showViewDialog">
      <q-card style="min-width: 600px; border-radius: 16px">
        <q-card-section class="bg-primary text-white row items-center">
          <div class="text-h6">Job Card Detail - {{ selectedJobCard?.id }}</div>
          <q-space />
          <q-btn icon="fas fa-times" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section class="q-pa-lg" v-if="selectedJobCard">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-md-6">
              <div class="text-caption text-grey-7">Service Date</div>
              <div class="text-subtitle1 text-weight-bold">{{ selectedJobCard.date }}</div>
            </div>
            <div class="col-12 col-md-6">
              <div class="text-caption text-grey-7">Technician</div>
              <div class="text-subtitle1 text-weight-bold">{{ selectedJobCard.tech }}</div>
            </div>
            <div class="col-12 col-md-6">
              <div class="text-caption text-grey-7">Asset Reference</div>
              <div class="text-subtitle1 text-weight-bold text-primary">
                {{ selectedJobCard.unitRef }}
              </div>
            </div>
            <div class="col-12 col-md-6">
              <div class="text-caption text-grey-7">Customer</div>
              <div class="text-subtitle1 text-weight-bold">{{ selectedJobCard.customer }}</div>
            </div>

            <q-separator class="col-12 q-my-sm" />

            <div class="col-12">
              <div class="text-caption text-grey-7 q-mb-xs">Status</div>
              <q-chip
                :color="selectedJobCard.faultFound ? 'red-1' : 'green-1'"
                :text-color="selectedJobCard.faultFound ? 'red-9' : 'green-9'"
                :icon="
                  selectedJobCard.faultFound ? 'fas fa-triangle-exclamation' : 'fas fa-circle-check'
                "
                :label="selectedJobCard.faultFound ? 'Fault Reported' : 'System Clear'"
                class="text-weight-bold"
              />
            </div>

            <div class="col-12">
              <div class="text-caption text-grey-7 q-mb-xs">Technician Notes</div>
              <div
                class="q-pa-md bg-grey-1 rounded-borders text-body2"
                style="min-height: 100px; white-space: pre-wrap"
              >
                {{ selectedJobCard.comments || 'No specific notes recorded.' }}
              </div>
            </div>
          </div>
        </q-card-section>

        <q-card-actions align="right" class="q-pa-md bg-grey-1">
          <q-btn flat label="Close" color="primary" v-close-popup />
          <q-btn color="secondary" icon="fas fa-file-pdf" label="Download PDF" unelevated />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed, reactive } from 'vue'
import { useQuasar } from 'quasar'
import { store } from '../../store'
import * as XLSX from 'xlsx'

export default defineComponent({
  name: 'JobCardHistory',
  setup() {
    const $q = useQuasar()
    const filter = ref('')
    const showViewDialog = ref(false)
    const selectedJobCard = ref(null)
    const columnFilters = reactive({
      id: '',
      date: '',
      unitRef: '',
      customer: '',
      tech: '',
      status: '',
    })

    const columns = [
      { name: 'id', label: 'Job #', align: 'left', field: 'id', sortable: true },
      { name: 'date', label: 'Service Date', align: 'left', field: 'date', sortable: true },
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer', sortable: true },
      { name: 'tech', label: 'Technician', align: 'left', field: 'tech', sortable: true },
      { name: 'status', label: 'Status', align: 'center', field: 'status', sortable: true },
      { name: 'actions', label: '', align: 'right' },
    ]

    const rows = computed(() => store.jobCards)

    const filteredRows = computed(() => {
      return rows.value.filter((row) => {
        return Object.keys(columnFilters).every((key) => {
          if (!columnFilters[key]) return true
          const val = row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const viewJobCard = (jobCard) => {
      selectedJobCard.value = jobCard
      showViewDialog.value = true
    }

    const emailSummary = () => {
      $q.notify({
        color: 'positive',
        message: 'Monthly job cards mailed to all specified people',
        icon: 'fas fa-envelope',
      })
    }

    const exportToExcel = () => {
      $q.notify({
        message: 'Exporting job card history to Excel...',
        color: 'green-7',
        icon: 'file_download',
      })

      const exportData = filteredRows.value.map((r) => ({
        'Job #': r.id,
        'Service Date': r.date,
        'Unit Ref': r.unitRef,
        Customer: r.customer,
        Technician: r.tech,
        Status: r.faultFound ? 'Fault Reported' : 'System Clear',
        Comments: r.comments || '',
      }))

      const worksheet = XLSX.utils.json_to_sheet(exportData)
      const workbook = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(workbook, worksheet, 'Job Card History')

      XLSX.writeFile(workbook, `job_card_history_${new Date().toISOString().split('T')[0]}.xlsx`)
    }

    return {
      columns,
      rows,
      filteredRows,
      filter,
      columnFilters,
      showViewDialog,
      selectedJobCard,
      viewJobCard,
      emailSummary,
      exportToExcel,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
