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
          <q-btn
            color="primary"
            icon="fas fa-pen-to-square"
            label="Manual Add"
            class="shadow-1"
            @click="goToAdd('manual')"
          />
          <q-btn
            color="secondary"
            icon="fas fa-qrcode"
            label="QR Scan Add"
            class="shadow-1"
            @click="goToAdd('qr')"
          />
        </div>
      </div>

      <div class="col-12">
        <div v-if="recordRows.length" class="column q-gutter-md">
          <q-card
            v-for="record in recordRows"
            :key="record.id"
            flat
            bordered
            class="rounded-borders record-card"
          >
            <q-card-section>
              <div class="row items-center justify-between">
                <div class="text-subtitle1 text-weight-bold">{{ record.unitRef }}</div>
                <q-chip
                  dense
                  :color="record.status === 'Completed' ? 'green-1' : 'orange-1'"
                  :text-color="record.status === 'Completed' ? 'green-9' : 'orange-9'"
                  size="sm"
                >
                  {{ record.status }}
                </q-chip>
              </div>
              <div class="text-caption text-grey-7">{{ record.customer }} · {{ record.project }}</div>
              <div class="row q-gutter-x-md q-mt-sm">
                <div class="text-caption text-grey-7 row items-center">
                  <q-icon name="fas fa-calendar" size="12px" class="q-mr-xs" />
                  {{ record.date }}
                </div>
                <div class="text-caption text-grey-7 row items-center">
                  <q-icon name="fas fa-clipboard-check" size="12px" class="q-mr-xs" />
                  {{ record.type }}
                </div>
              </div>
            </q-card-section>
          </q-card>
        </div>

        <q-card v-else flat bordered class="rounded-borders">
          <q-card-section class="text-center text-grey-7">
            No commissioning records found for this project yet.
          </q-card-section>
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

    const goToAdd = (mode) => {
      router.push({
        path: '/field/commissioning/add',
        query: {
          customerId: customerId.value,
          projectId: projectId.value,
          mode,
        },
      })
    }

    return { recordRows, goToAdd, goToProjectActions }
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
