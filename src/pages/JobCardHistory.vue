<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Job Card History</div>
          <div class="text-subtitle1 text-grey-7">Cloud-stored digital records of all technical work</div>
        </div>
        <q-btn color="primary" icon="fas fa-paper-plane" label="Email Monthly Summary" @click="emailSummary" class="shadow-2" />
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="rows"
            :columns="columns"
            row-key="id"
            flat
          >
            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip 
                  :color="props.row.faultFound ? 'red-1' : 'green-1'" 
                  :text-color="props.row.faultFound ? 'red-9' : 'green-9'"
                  :icon="props.row.faultFound ? 'fas fa-triangle-exclamation' : 'fas fa-circle-check'"
                  label="Completed"
                  dense
                />
              </q-td>
            </template>
            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable>
                        <q-item-section avatar><q-icon name="fas fa-eye" color="primary" size="sm" /></q-item-section>
                        <q-item-section>View Job Card</q-item-section>
                      </q-item>
                      <q-item clickable>
                        <q-item-section avatar><q-icon name="fas fa-file-pdf" color="secondary" size="sm" /></q-item-section>
                        <q-item-section>Download PDF</q-item-section>
                      </q-item>
                      <q-item v-if="props.row.faultFound" clickable>
                        <q-item-section avatar><q-icon name="fas fa-file-invoice-dollar" color="orange-9" size="sm" /></q-item-section>
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
        <q-banner rounded class="bg-blue-1">
          <template v-slot:avatar>
            <q-icon name="fas fa-cloud-arrow-up" color="blue-9" />
          </template>
          <div class="text-subtitle2">Cloud Access Secured</div>
          <div class="text-body2">
            Job cards are stored on the cloud and accessible by the client or service provider for that specific unit or month.
          </div>
        </q-banner>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref } from 'vue'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'JobCardHistory',
  setup () {
    const $q = useQuasar()

    const columns = [
      { name: 'id', label: 'Job #', align: 'left', field: 'id', sortable: true },
      { name: 'date', label: 'Service Date', align: 'left', field: 'date', sortable: true },
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer', sortable: true },
      { name: 'tech', label: 'Technician', align: 'left', field: 'tech' },
      { name: 'status', label: 'Status', align: 'center', field: 'status' },
      { name: 'actions', label: '', align: 'right' }
    ]

    const rows = store.jobCards

    const emailSummary = () => {
      $q.notify({
        color: 'positive',
        message: 'Monthly job cards mailed to all specified people',
        icon: 'send'
      })
    }

    return {
      columns,
      rows,
      emailSummary
    }
  }
})
</script>

