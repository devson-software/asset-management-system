<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div class="row items-center no-wrap">
          <q-btn flat round icon="fas fa-arrow-left" @click="$router.back()" />
          <div class="q-ml-sm">
            <div class="text-h5 text-weight-bold text-primary">Project Actions</div>
            <div class="text-subtitle2 text-grey-7">
              {{  'Select an action below' }}
            </div>
          </div>
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders bg-white q-mb-md">
          <q-card-section>
            <q-select
              v-model="selectedProjectId"
              :options="projectOptions"
              label="Select Project/Building"
              outlined
              dense
              emit-value
              map-options
              use-input
              input-debounce="0"
              @filter="filterProjects"
              option-label="label"
              option-value="value"
              clearable
              bg-color="white"
            >
              <template v-slot:prepend>
                <q-icon name="fas fa-location-dot" size="xs" />
              </template>
            </q-select>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12">
        <q-card v-if="projectDetails" flat bordered class="rounded-borders bg-white q-mb-md">
          <q-card-section>
            <div class="text-subtitle1 text-weight-bold">{{ projectDetails.name }}</div>
            <div class="text-caption text-grey-7">
              {{
                projectDetails.siteAddress ||
                projectDetails.vendorLocation ||
                'No site address on file'
              }}
            </div>
          </q-card-section>
        </q-card>
      </div>

      <div class="col-12">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6">
            <q-card flat bordered class="rounded-borders action-card full-height" @click="goToSchedule">
              <q-card-section class="row items-center">
                <q-avatar
                  color="blue-1"
                  text-color="primary"
                  icon="fas fa-calendar-check"
                  size="48px"
                />
                <div class="q-ml-md">
                  <div class="text-subtitle1 text-weight-bold">Service</div>
                  <div class="text-caption text-grey-7">View and start scheduled services</div>
                </div>
                <q-space />
                <q-icon name="fas fa-chevron-right" color="grey-5" />
              </q-card-section>
            </q-card>
          </div>

          <div class="col-12 col-sm-6">
            <q-card
              flat
              bordered
              class="rounded-borders action-card full-height"
              @click="goToCommissioning"
            >
              <q-card-section class="row items-center">
                <q-avatar
                  color="green-1"
                  text-color="green-8"
                  icon="fas fa-clipboard-check"
                  size="48px"
                />
                <div class="q-ml-md">
                  <div class="text-subtitle1 text-weight-bold">Commissioning</div>
                  <div class="text-caption text-grey-7">Capture commissioning data</div>
                </div>
                <q-space />
                <q-icon name="fas fa-chevron-right" color="grey-5" />
              </q-card-section>
            </q-card>
          </div>

          <div class="col-12 col-sm-6">
            <q-card
              flat
              bordered
              class="rounded-borders action-card full-height"
              @click="goToAssets"
            >
              <q-card-section class="row items-center">
                <q-avatar
                  color="orange-1"
                  text-color="orange-9"
                  icon="fas fa-boxes-stacked"
                  size="48px"
                />
                <div class="q-ml-md">
                  <div class="text-subtitle1 text-weight-bold">Assets</div>
                  <div class="text-caption text-grey-7">View assets for this project</div>
                </div>
                <q-space />
                <q-icon name="fas fa-chevron-right" color="grey-5" />
              </q-card-section>
            </q-card>
          </div>

          <div class="col-12 col-sm-6">
            <q-card
              flat
              bordered
              class="rounded-borders action-card full-height"
              @click="goToJobCards"
            >
              <q-card-section class="row items-center">
                <q-avatar
                  color="purple-1"
                  text-color="purple-8"
                  icon="fas fa-file-contract"
                  size="48px"
                />
                <div class="q-ml-md">
                  <div class="text-subtitle1 text-weight-bold">Job Card</div>
                  <div class="text-caption text-grey-7">Create or review job cards</div>
                </div>
                <q-space />
                <q-icon name="fas fa-chevron-right" color="grey-5" />
              </q-card-section>
            </q-card>
          </div>
        </div>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'FieldProjectActions',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const store = useMainStore()
    const customerId = computed(() => route.query.customerId || '')
    const projectId = computed(() => route.params.projectId || '')
    const selectedProjectId = ref(route.params.projectId || null)

    const projectDetails = computed(() => {
      if (!customerId.value || !selectedProjectId.value) return null
      const customer = store.customers.find((c) => c.id === customerId.value)
      return customer?.projects.find((p) => p.id === selectedProjectId.value) || null
    })

    const projectName = computed(() => projectDetails.value?.name || '')

    const projectOptions = computed(() => {
      if (!customerId.value) return []
      const customer = store.customers.find((c) => c.id === customerId.value)
      return customer ? customer.projects.map((p) => ({ label: p.name, value: p.id })) : []
    })

    const projectOptionsFiltered = ref(projectOptions.value || [])

    const filterProjects = (val, update) => {
      update(() => {
        if (!val) {
          projectOptionsFiltered.value = projectOptions.value
          return
        }
        const needle = val.toLowerCase()
        projectOptionsFiltered.value = projectOptions.value.filter((p) =>
          p.label.toLowerCase().includes(needle),
        )
      })
    }

    watch(projectOptions, (next) => {
      projectOptionsFiltered.value = next || []
    })

    watch(
      () => route.params.projectId,
      (nextId) => {
        selectedProjectId.value = nextId || null
      },
    )

    watch(selectedProjectId, (nextId) => {
      if (!nextId) return
      if (String(route.params.projectId || '') === String(nextId || '')) return
      router.push({
        path: `/field/projects/${nextId}/actions`,
        query: { customerId: customerId.value, projectId: nextId },
      })
    })

    const goToSchedule = () => {
      router.push({
        path: '/field/service-schedule',
        query: { customerId: customerId.value, projectId: selectedProjectId.value || projectId.value },
      })
    }

    const goToCommissioning = () => {
      router.push('/field/commissioning')
    }

    const goToAssets = () => {
      const targetProjectId = selectedProjectId.value || projectId.value
      router.push({ path: '/assets', query: { projectId: targetProjectId } })
    }

    const goToJobCards = () => {
      router.push('/field/job-cards/add')
    }

    return {
      projectName,
      projectDetails,
      selectedProjectId,
      projectOptions: projectOptionsFiltered,
      filterProjects,
      goToSchedule,
      goToCommissioning,
      goToAssets,
      goToJobCards,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}
.action-card {
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.06);
}
</style>
