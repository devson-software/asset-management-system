<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Reports</div>
          <div class="text-subtitle1 text-grey-7">
            Summarised service history per project and plant
          </div>
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders q-pa-md">
          <div class="row q-col-gutter-md items-center">
            <div class="col-12 col-sm-4">
              <q-select
                v-model="selectedCustomerId"
                :options="customerOptions"
                label="Customer"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              >
                <template v-slot:prepend>
                  <q-icon name="fas fa-building-user" size="xs" color="primary" />
                </template>
              </q-select>
            </div>
            <div class="col-12 col-sm-4">
              <q-select
                v-model="selectedProjectId"
                :options="projectOptions"
                label="Project / Building"
                outlined
                dense
                clearable
                emit-value
                map-options
                :disable="!selectedCustomerId"
                bg-color="white"
              >
                <template v-slot:prepend>
                  <q-icon name="fas fa-location-dot" size="xs" color="primary" />
                </template>
              </q-select>
            </div>
            <div class="col-12 col-sm-2">
              <q-input
                v-model="fromDate"
                label="From Date"
                outlined
                dense
                mask="####/##/##"
                bg-color="white"
              >
                <template v-slot:prepend>
                  <q-icon name="fas fa-calendar-day" size="xs" color="primary" />
                </template>
              </q-input>
            </div>
            <div class="col-12 col-sm-2">
              <q-input
                v-model="toDate"
                label="To Date"
                outlined
                dense
                mask="####/##/##"
                bg-color="white"
              >
                <template v-slot:prepend>
                  <q-icon name="fas fa-calendar-check" size="xs" color="primary" />
                </template>
              </q-input>
            </div>
          </div>
        </q-card>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="rows"
            :columns="columns"
            row-key="id"
            flat
            class="reports-table"
          >
            <template v-slot:no-data>
              <div class="full-width text-center q-pa-lg text-grey-6">
                Select a customer and project to view service history.
              </div>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed, ref } from 'vue'
import { store } from '../../store'

export default defineComponent({
  name: 'Reports',
  setup() {
    const selectedCustomerId = ref(null)
    const selectedProjectId = ref(null)
    const fromDate = ref(null)
    const toDate = ref(null)

    const customerOptions = computed(() =>
      store.customers.map((c) => ({ label: c.name, value: c.id })),
    )

    const projectOptions = computed(() => {
      if (!selectedCustomerId.value) return []
      const customer = store.customers.find((c) => c.id === selectedCustomerId.value)
      if (!customer) return []
      return customer.projects.map((p) => ({ label: p.name, value: p.id }))
    })

    const assetsIndex = computed(() => {
      const map = new Map()
      store.customers.forEach((c) => {
        c.projects.forEach((p) => {
          p.assets.forEach((a) => {
            map.set(a.unitRef, {
              customerId: c.id,
              projectId: p.id,
              customerName: c.name,
              projectName: p.name,
              unitRef: a.unitRef,
              unitType: a.unitType,
            })
          })
        })
      })
      return map
    })

    const columns = [
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef', sortable: true },
      {
        name: 'project',
        label: 'Project',
        align: 'left',
        field: 'project',
        sortable: true,
      },
      { name: 'date', label: 'Service Date', align: 'left', field: 'date', sortable: true },
      { name: 'type', label: 'Service Type', align: 'left', field: 'type', sortable: true },
      { name: 'technician', label: 'Technician', align: 'left', field: 'technician', sortable: true },
      { name: 'status', label: 'Status', align: 'left', field: 'status', sortable: false },
    ]

    const rows = computed(() => {
      const services = store.services || []
      if (!selectedCustomerId.value || !selectedProjectId.value) return []

      const from = fromDate.value
      const to = toDate.value

      return services
        .filter((s) => {
          const meta = assetsIndex.value.get(s.unitRef)
          if (!meta) return false
          if (meta.customerId !== selectedCustomerId.value) return false
          if (meta.projectId !== selectedProjectId.value) return false

          if (from && s.date < from) return false
          if (to && s.date > to) return false
          return true
        })
        .map((s) => {
          const meta = assetsIndex.value.get(s.unitRef)
          return {
            id: `${s.unitRef}-${s.date}`,
            unitRef: s.unitRef,
            project: meta?.projectName || s.project,
            date: s.date,
            type: s.type,
            technician: '', // can be enriched later
            status: s.status || 'Scheduled',
          }
        })
    })

    return {
      selectedCustomerId,
      selectedProjectId,
      fromDate,
      toDate,
      customerOptions,
      projectOptions,
      columns,
      rows,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>

