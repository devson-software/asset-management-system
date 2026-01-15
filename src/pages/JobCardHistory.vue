<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-lg">
        <div class="text-h5 text-primary">Job Card Cloud Storage</div>
        <q-space />
        <q-btn color="secondary" icon="mail" label="Email Monthly Summary" @click="emailSummary" />
      </div>

      <q-card flat bordered>
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
                :icon="props.row.faultFound ? 'warning' : 'check_circle'"
                label="Completed"
                dense
              />
            </q-td>
          </template>
          <template v-slot:body-cell-actions="props">
            <q-td :props="props">
              <q-btn flat round color="grey-7" icon="more_vert">
                <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                  <q-list style="min-width: 150px">
                    <q-item clickable>
                      <q-item-section avatar><q-icon name="visibility" color="primary" size="sm" /></q-item-section>
                      <q-item-section>View Job Card</q-item-section>
                    </q-item>
                    <q-item clickable>
                      <q-item-section avatar><q-icon name="download" color="secondary" size="sm" /></q-item-section>
                      <q-item-section>Download PDF</q-item-section>
                    </q-item>
                    <q-item v-if="props.row.faultFound" clickable>
                      <q-item-section avatar><q-icon name="request_quote" color="orange-9" size="sm" /></q-item-section>
                      <q-item-section>Issue Quote</q-item-section>
                    </q-item>
                  </q-list>
                </q-menu>
              </q-btn>
            </q-td>
          </template>
        </q-table>
      </q-card>

      <q-banner rounded class="bg-blue-1 q-mt-xl">
        <template v-slot:avatar>
          <q-icon name="cloud_done" color="blue-9" />
        </template>
        <div class="text-subtitle2">Cloud Access</div>
        <div class="text-body2">
          Job cards are stored on the cloud and accessible by the client or service provider for that specific unit or month.
        </div>
      </q-banner>
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

