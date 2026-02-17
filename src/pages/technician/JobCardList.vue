<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div class="row items-center no-wrap">
          <q-btn flat round icon="fas fa-arrow-left" class="q-mr-sm" @click="goToProjectActions" />
          <div>
            <div class="text-h5 text-weight-bold text-primary">Job Cards</div>
            <div class="text-subtitle2 text-grey-7">View job history and add new cards</div>
          </div>
        </div>
        <q-btn
          color="primary"
          icon="fas fa-plus"
          label="Add Job Card"
          class="shadow-1"
          @click="goToAdd"
        />
      </div>

      <div class="col-12">
        <div class="column q-gutter-md">
          <q-card
            v-for="job in jobCards"
            :key="job.id"
            flat
            bordered
            class="rounded-borders job-card-item"
          >
            <q-card-section>
              <div class="row items-center justify-between">
                <div class="text-subtitle1 text-weight-bold">{{ job.unitRef }}</div>
                <q-chip
                  dense
                  :color="job.faultFound ? 'negative' : 'positive'"
                  text-color="white"
                  size="sm"
                  class="text-weight-bold"
                >
                  {{ job.faultFound ? 'Fault Found' : 'No Fault' }}
                </q-chip>
              </div>
              <div class="text-caption text-grey-7">{{ job.customer }}</div>
              <div class="row q-gutter-x-md q-mt-sm">
                <div class="text-caption text-grey-7 row items-center">
                  <q-icon name="fas fa-calendar" size="12px" class="q-mr-xs" />
                  {{ job.date }}
                </div>
                <div class="text-caption text-grey-7 row items-center">
                  <q-icon name="fas fa-user-gear" size="12px" class="q-mr-xs" />
                  {{ job.tech }}
                </div>
              </div>
            </q-card-section>
          </q-card>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { store } from '../../store'

export default defineComponent({
  name: 'TechnicianJobCardList',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const jobCards = computed(() => store.jobCards)

    const goToProjectActions = () => {
      const projectId = route.query.projectId || ''
      const customerId = route.query.customerId || ''
      if (projectId) {
        router.push({ path: `/field/projects/${projectId}/actions`, query: { customerId, projectId } })
        return
      }
      router.push('/field/projects')
    }

    const goToAdd = () => {
      router.push('/field/job-cards/add')
    }

    return { jobCards, goToAdd, goToProjectActions }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}

.job-card-item {
  background: #fff;
}
</style>
