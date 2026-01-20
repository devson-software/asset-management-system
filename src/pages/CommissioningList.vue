<template>
  <q-page padding>
    <div class="q-pa-md">
      <div class="row items-center q-mb-lg">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Commissioning Master</div>
          <div class="text-subtitle1 text-grey-7">CIBSE & ASHRAE Aligned HVAC Commissioning</div>
        </div>
        <q-space />
        <q-btn-dropdown color="primary" label="New Commissioning Record" icon="add" class="shadow-2">
          <q-list>
            <q-item clickable v-close-popup @click="startComm('Pump')">
              <q-item-section avatar><q-icon name="water" color="blue" /></q-item-section>
              <q-item-section><q-item-label>Pump Commissioning</q-item-label></q-item-section>
            </q-item>
            <q-item clickable v-close-popup @click="startComm('Fan')">
              <q-item-section avatar><q-icon name="air" color="teal" /></q-item-section>
              <q-item-section><q-item-label>Fan Commissioning</q-item-label></q-item-section>
            </q-item>
            <q-item clickable v-close-popup @click="startComm('DX Split')">
              <q-item-section avatar><q-icon name="ac_unit" color="orange" /></q-item-section>
              <q-item-section><q-item-label>DX Split Unit</q-item-label></q-item-section>
            </q-item>
          </q-list>
        </q-btn-dropdown>
      </div>

      <q-card flat bordered class="rounded-borders shadow-1">
        <q-table
          :rows="store.commissioningRecords"
          :columns="columns"
          row-key="id"
          flat
          :filter="filter"
        >
          <template v-slot:top-right>
            <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Records...">
              <template v-slot:append><q-icon name="search" /></template>
            </q-input>
          </template>

          <template v-slot:body-cell-status="props">
            <q-td :props="props">
              <q-badge :color="props.row.status === 'Completed' ? 'positive' : 'orange'" rounded class="q-px-sm">
                {{ props.row.status }}
              </q-badge>
            </q-td>
          </template>

          <template v-slot:body-cell-actions="props">
            <q-td :props="props">
              <q-btn flat round color="grey-7" icon="more_vert">
                <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                  <q-list style="min-width: 150px">
                    <q-item clickable @click="viewRecord(props.row)">
                      <q-item-section avatar><q-icon name="visibility" color="primary" size="sm" /></q-item-section>
                      <q-item-section>View Report</q-item-section>
                    </q-item>
                    <q-item clickable @click="deleteRecord(props.row)" class="text-negative">
                      <q-item-section avatar><q-icon name="delete" color="negative" size="sm" /></q-item-section>
                      <q-item-section>Delete</q-item-section>
                    </q-item>
                  </q-list>
                </q-menu>
              </q-btn>
            </q-td>
          </template>
        </q-table>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref } from 'vue'
import { store } from '../store'
import { useRouter } from 'vue-router'

export default defineComponent({
  name: 'CommissioningList',
  setup () {
    const router = useRouter()
    const filter = ref('')
    const columns = [
      { name: 'id', label: 'ID', align: 'left', field: 'id', sortable: true },
      { name: 'date', label: 'Date', align: 'left', field: 'date', sortable: true },
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      { name: 'type', label: 'Type', align: 'left', field: 'type', sortable: true },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer', sortable: true },
      { name: 'status', label: 'Status', align: 'center', field: 'status' },
      { name: 'actions', label: '', align: 'right' }
    ]

    const startComm = (type) => {
      router.push(`/commissioning/new/${type}`)
    }

    const viewRecord = (record) => {
      router.push(`/commissioning/view/${record.id}`)
    }

    const deleteRecord = (record) => {
      const index = store.commissioningRecords.findIndex(r => r.id === record.id)
      if (index !== -1) {
        store.commissioningRecords.splice(index, 1)
      }
    }

    return { store, filter, columns, startComm, viewRecord, deleteRecord }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
