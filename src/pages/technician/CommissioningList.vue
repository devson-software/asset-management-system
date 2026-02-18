<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12">
        <div class="row items-center q-gutter-sm">
          <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
          <div>
            <div class="text-h5 text-weight-bold text-primary">Commissioning</div>
            <div class="text-subtitle2 text-grey-7">Scan or manually create commissioning reports</div>
          </div>
          <q-space />
          <q-btn-dropdown
            color="primary"
            icon="fas fa-plus"
            label="Add Commissioning"
            class="shadow-1"
          >
            <q-list style="min-width: 200px">
              <q-item clickable v-close-popup @click="goToAdd('manual')">
                <q-item-section avatar>
                  <q-icon name="fas fa-pen-to-square" color="primary" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">Manual Add</q-item-label>
                  <q-item-label caption>Enter details manually</q-item-label>
                </q-item-section>
              </q-item>
              <q-item clickable v-close-popup @click="goToAdd('qr')">
                <q-item-section avatar>
                  <q-icon name="fas fa-qrcode" color="secondary" />
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-bold">QR Scan Add</q-item-label>
                  <q-item-label caption>Scan asset QR code</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-btn-dropdown>
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders q-pa-md">
          <q-table
            :rows="recordRows"
            :columns="columns"
            row-key="id"
            flat
            class="master-table"
          >
            <template v-slot:body-cell-status="props">
              <q-td :props="props">
                <q-chip
                  dense
                  :color="props.row.status === 'Completed' ? 'green-1' : 'orange-1'"
                  :text-color="props.row.status === 'Completed' ? 'green-9' : 'orange-9'"
                  size="sm"
                >
                  {{ props.row.status }}
                </q-chip>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianCommissioningList',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const customerId = computed(() => route.query.customerId || '')
    const projectId = computed(() => route.query.projectId || '')

    const goToProjectActions = () => {
      if (projectId.value) {
        router.push({
          path: `/field/projects/${projectId.value}/actions`,
          query: { customerId: customerId.value, projectId: projectId.value },
        })
        return
      }
      router.push('/field/projects')
    }

    const recordRows = computed(() => {
      return store.commissioningRecords.filter((record) => {
        if (customerId.value && record.customerId && record.customerId !== customerId.value) return false
        if (projectId.value && record.projectId && record.projectId !== projectId.value) return false
        if (customerId.value && !record.customerId) {
          const customer = store.customers.find((c) => c.fullName === record.customer)
          if (!customer || customer.id !== customerId.value) return false
        }
        if (projectId.value && !record.projectId) {
          const match = store.customers.some((c) =>
            c.projects.some((p) => p.id === projectId.value && p.name === record.project)
          )
          if (!match) return false
        }
        return true
      })
    })

    const columns = [
      { name: 'id', label: 'ID', align: 'left', field: 'id' },
      { name: 'date', label: 'Date', align: 'left', field: 'date' },
      { name: 'unitRef', label: 'Unit Ref', align: 'left', field: 'unitRef' },
      { name: 'customer', label: 'Customer', align: 'left', field: 'customer' },
      { name: 'project', label: 'Project', align: 'left', field: 'project' },
      { name: 'type', label: 'Type', align: 'left', field: 'type' },
      { name: 'status', label: 'Status', align: 'left' },
    ]

    const goToAdd = (mode) => {
      router.push({
        path: '/field/commissioning-master',
        query: {
          customerId: customerId.value,
          projectId: projectId.value,
          mode,
        },
      })
    }

    return { recordRows, columns, goToAdd, goToProjectActions }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}

.record-card {
  background: #fff;
}
</style>
