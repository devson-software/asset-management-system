<template>
  <q-page padding :class="isFieldView ? 'field-schedule-page' : ''">
    <div class="row q-col-gutter-lg">
      <div
        class="col-12 flex justify-between items-center"
        :class="isFieldView ? 'field-schedule-header' : ''"
      >
        <div class="row items-center no-wrap">
          <q-btn
            v-if="isFieldView"
            flat
            round
            icon="fas fa-arrow-left"
            class="q-mr-sm"
            @click="goToProjectActions"
          />
          <div>
            <div class="text-h4 text-weight-bold text-primary">Service Schedule</div>
            <div class="text-subtitle1 text-grey-7">
              Plan and manage maintenance visits across all sites
            </div>
          </div>
        </div>
        <div class="q-gutter-sm">
          <q-btn
            v-if="!isFieldView"
            color="primary"
            icon="fas fa-plus"
            label="Schedule New Visit"
            @click="openAddDialog"
            class="shadow-2"
          />
          <q-btn
            v-if="!isFieldView"
            flat
            color="primary"
            icon="fas fa-paper-plane"
            label="Broadcast Schedule"
            @click="mailAll"
          />
        </div>
      </div>

      <div class="col-12">
        <q-card
          flat
          bordered
          class="rounded-borders q-pa-md bg-grey-1"
          :class="isFieldView ? 'field-schedule-filters' : ''"
        >
          <div class="row q-col-gutter-md items-center">
            <div v-if="!isFieldView" class="col-12 col-sm-3">
              <q-select
                v-model="customerFilter"
                :options="customerOptions"
                label="Filter by Customer"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              >
                <template v-slot:prepend
                  ><q-icon name="fas fa-business-time" size="xs" color="primary"
                /></template>
              </q-select>
            </div>
            <div v-if="!isFieldView" class="col-12 col-sm-3">
              <q-select
                v-model="projectFilter"
                :options="filterProjectOptions"
                label="Filter by Project"
                outlined
                dense
                clearable
                emit-value
                map-options
                :disable="!customerFilter"
                bg-color="white"
              >
                <template v-slot:prepend
                  ><q-icon name="fas fa-location-dot" size="xs" color="primary"
                /></template>
              </q-select>
            </div>
          <div :class="isFieldView ? 'col-12' : 'col-12 col-sm-3'">
              <q-select
                v-model="teamFilter"
                :options="teams"
                option-label="name"
                option-value="id"
                label="Filter by Team"
                outlined
                dense
                clearable
                emit-value
                map-options
                bg-color="white"
              >
                <template v-slot:prepend
                  ><q-icon name="fas fa-people-group" size="xs" color="primary"
                /></template>
                <template v-slot:option="scope">
                  <q-item v-bind="scope.itemProps">
                    <q-item-section avatar>
                      <div
                        :style="{
                          backgroundColor: scope.opt.color,
                          width: '12px',
                          height: '12px',
                          borderRadius: '50%',
                        }"
                      ></div>
                    </q-item-section>
                    <q-item-section>
                      <q-item-label>{{ scope.opt.name }}</q-item-label>
                    </q-item-section>
                  </q-item>
                </template>
              </q-select>
            </div>
          <div
            :class="
              isFieldView ? 'col-12 flex justify-end field-filter-reset' : 'col-12 col-sm-3 flex justify-end'
            "
          >
              <q-btn
                flat
                color="grey-7"
                icon="fas fa-filter-circle-xmark"
                label="Reset"
                @click="clearFilters"
                v-if="(isFieldView && teamFilter) || (!isFieldView && (customerFilter || projectFilter || teamFilter))"
              />
            </div>
          </div>
        </q-card>
      </div>

      <div class="col-12">
        <div class="row q-col-gutter-xl">
          <!-- Left Column: Calendar -->
          <div v-if="!isFieldView" class="col-12 col-md-5">
            <q-card flat bordered class="calendar-card overflow-hidden">
              <q-date
                v-model="selectedDate"
                :events="events"
                :event-color="getEventColor"
                class="full-width no-border"
                flat
                today-btn
              />
              <q-separator />
              <q-card-section class="bg-grey-1">
                <div class="text-subtitle2 q-mb-sm">Operational Teams</div>
                <div class="row q-gutter-sm">
                  <div
                    v-for="team in teams"
                    :key="team.id"
                    class="row items-center no-wrap bg-white q-pa-xs rounded-borders shadow-1 border-light"
                  >
                    <div
                      class="q-badge q-mr-xs"
                      :style="{
                        backgroundColor: team.color,
                        width: '12px',
                        height: '12px',
                        borderRadius: '50%',
                      }"
                    ></div>
                    <div class="text-caption text-weight-medium">{{ team.name }}</div>
                    <q-tooltip>
                      Lead: {{ getUserName(team.leaderId) }}<br />
                      Asst: {{ getUserName(team.assistantId) }}
                    </q-tooltip>
                  </div>
                </div>
              </q-card-section>
            </q-card>
          </div>

          <!-- Right Column: Service List -->
          <div :class="isFieldView ? 'col-12' : 'col-12 col-md-7'">
            <div class="row items-center justify-between q-mb-md">
              <div class="text-h6 text-grey-8">
                Services for <span class="text-primary text-weight-bold">{{ selectedDate }}</span>
              </div>
              <q-badge color="blue-1" text-color="primary" class="q-px-md q-py-xs text-weight-bold">
                {{ filteredServices.length }} Appointment{{
                  filteredServices.length === 1 ? '' : 's'
                }}
              </q-badge>
            </div>

            <transition-group
              appear
              enter-active-class="animated fadeIn"
              leave-active-class="animated fadeOut"
            >
              <q-card
                v-for="service in filteredServices"
                :key="service.id"
                flat
                bordered
                class="q-mb-md service-item-card"
              >
                <q-item class="q-py-md service-item-content">
                  <q-item-section avatar>
                    <q-avatar
                      color="blue-1"
                      text-color="primary"
                      icon="fas fa-screwdriver-wrench"
                    />
                  </q-item-section>

                  <q-item-section class="service-item-main">
                    <q-item-label class="text-weight-bold text-subtitle1">
                      {{ service.unitRef }}
                    </q-item-label>
                    <q-item-label class="text-grey-9">{{ service.customer }}</q-item-label>
                    <div class="row items-center q-gutter-x-md q-mt-xs">
                      <div class="text-caption text-grey-7 row items-center">
                        <q-icon name="fas fa-location-dot" size="10px" class="q-mr-xs" />
                        {{ service.project }}
                      </div>
                      <div
                        class="text-caption text-grey-7 row items-center"
                        v-if="service.duration"
                      >
                        <q-icon name="fas fa-hourglass-half" size="10px" class="q-mr-xs" />
                        {{ service.duration }}
                      </div>
                      <div
                        v-if="service.teamId"
                        class="text-caption row items-center"
                        :style="{ color: getTeamColor(service.teamId) }"
                      >
                        <q-icon name="fas fa-people-group" size="10px" class="q-mr-xs" />
                        {{ getTeamName(service.teamId) }}
                        <q-tooltip>
                          Team: {{ getTeamName(service.teamId) }}<br />
                          Lead: {{ getUserName(getTeam(service.teamId)?.leaderId) }}<br />
                          Asst: {{ getUserName(getTeam(service.teamId)?.assistantId) }}
                        </q-tooltip>
                      </div>
                    </div>

                    <div class="q-mt-sm">
                      <q-chip
                        dense
                        :color="getStatusColor(service.status)"
                        text-color="white"
                        icon="fas fa-circle-check"
                        size="sm"
                        class="text-weight-bold"
                      >
                        {{ service.status || 'Scheduled' }}
                      </q-chip>
                    </div>

                    <q-btn
                      v-if="isFieldView"
                      color="primary"
                      icon="fas fa-play"
                      label="Start Work"
                      class="q-mt-md full-width"
                      size="lg"
                      @click="startService(service)"
                    />

                    <!-- Tasks Preview in List -->
                    <div
                      v-if="store.serviceDefinitions && store.serviceDefinitions[service.type]"
                      class="q-mt-sm"
                    >
                      <div class="column q-gutter-y-xs">
                        <q-chip
                          v-for="(task, idx) in store.serviceDefinitions[service.type].tasks.slice(
                            0,
                            2,
                          )"
                          :key="idx"
                          outline
                          dense
                          size="9px"
                          color="blue-grey-3"
                          text-color="blue-grey-7"
                          icon="fas fa-check"
                        >
                          {{ task }}
                        </q-chip>
                        <div
                          v-if="store.serviceDefinitions[service.type].tasks.length > 2"
                          class="text-caption text-grey-5"
                          style="font-size: 10px; align-self: center"
                        >
                          +{{ store.serviceDefinitions[service.type].tasks.length - 2 }} more
                          <q-tooltip>
                            <div
                              v-for="(task, idx) in store.serviceDefinitions[service.type].tasks"
                              :key="idx"
                              class="q-mb-xs"
                            >
                              • {{ task }}
                            </div>
                          </q-tooltip>
                        </div>
                      </div>
                    </div>
                  </q-item-section>

                  <q-item-section side class="service-item-meta">
                    <div class="column items-end">
                      <q-chip
                        dense
                        color="orange-1"
                        text-color="orange-10"
                        icon="fas fa-clock"
                        size="sm"
                        class="text-weight-bold"
                      >
                        {{ service.type }}
                      </q-chip>
                      <div class="row q-mt-sm">
                        <q-btn v-if="!isFieldView" flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                          <q-menu auto-close class="rounded-borders shadow-2">
                            <q-list style="min-width: 150px">
                              <q-item v-if="!isFieldView" clickable @click="editService(service)">
                                <q-item-section avatar
                                  ><q-icon name="fas fa-edit" color="primary" size="sm"
                                /></q-item-section>
                                <q-item-section>Edit Visit</q-item-section>
                              </q-item>
                              <q-item
                                v-if="!isFieldView"
                                clickable
                                @click="confirmDelete(service)"
                                class="text-negative"
                              >
                                <q-item-section avatar
                                  ><q-icon name="fas fa-trash-can" color="negative" size="sm"
                                /></q-item-section>
                                <q-item-section>Cancel Visit</q-item-section>
                              </q-item>
                            </q-list>
                          </q-menu>
                        </q-btn>
                      </div>
                    </div>
                  </q-item-section>
                </q-item>
              </q-card>
            </transition-group>

            <div
              v-if="filteredServices.length === 0"
              class="column items-center justify-center q-pa-xl empty-state"
            >
              <q-icon name="fas fa-calendar-xmark" size="80px" color="grey-4" />
              <div class="text-h6 text-grey-5 q-mt-md">Quiet day! No services scheduled.</div>
              <q-btn
                flat
                color="primary"
                label="Schedule something?"
                @click="openAddDialog"
                class="q-mt-sm"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add/Edit Dialog -->
    <q-dialog v-model="showDialog" persistent>
      <q-card style="min-width: 450px" class="rounded-borders">
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">{{ isEditing ? 'Update Schedule' : 'Schedule New Service' }}</div>
          <q-space />
          <q-btn icon="fas fa-times" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section class="q-pt-lg">
          <q-form @submit="saveService" class="q-gutter-md">
            <q-select
              v-model="form.customerId"
              :options="customerOptions"
              label="Select Customer"
              outlined
              dense
              emit-value
              map-options
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-building" /></template>
            </q-select>

            <q-select
              v-model="form.projectId"
              :options="projectOptions"
              label="Select Project"
              outlined
              dense
              emit-value
              map-options
              :disable="!form.customerId"
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-map-marker-alt" /></template>
            </q-select>

            <q-select
              v-model="form.unitRef"
              :options="assetOptions"
              label="Select Asset (Unit Ref)"
              outlined
              dense
              emit-value
              map-options
              :disable="!form.projectId"
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-box" /></template>
            </q-select>

            <q-select
              v-model="form.type"
              :options="store.serviceDefinitions ? Object.keys(store.serviceDefinitions) : []"
              label="Service Type"
              outlined
              dense
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-tags" /></template>
            </q-select>

            <!-- Service Tasks Preview -->
            <div
              v-if="form.type && store.serviceDefinitions && store.serviceDefinitions[form.type]"
              class="bg-blue-50 q-pa-sm rounded-borders border-blue-1"
            >
              <div class="text-caption text-weight-bold text-primary q-mb-xs">
                Standard Tasks for {{ form.type }}:
              </div>
              <div
                v-for="(task, idx) in store.serviceDefinitions[form.type].tasks"
                :key="idx"
                class="text-caption row no-wrap q-mb-xs"
              >
                <q-icon name="fas fa-check-circle" color="primary" size="14px" class="q-mr-xs" />
                <div class="col">{{ task }}</div>
              </div>
            </div>

            <q-select
              v-model="form.teamId"
              :options="teams"
              option-label="name"
              option-value="id"
              label="Allocate Team"
              outlined
              dense
              emit-value
              map-options
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-people-group" /></template>
              <template v-slot:option="scope">
                <q-item v-bind="scope.itemProps">
                  <q-item-section avatar>
                    <div
                      :style="{
                        backgroundColor: scope.opt.color,
                        width: '12px',
                        height: '12px',
                        borderRadius: '50%',
                      }"
                    ></div>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label>{{ scope.opt.name }}</q-item-label>
                    <q-item-label caption>Lead: {{ getUserName(scope.opt.leaderId) }}</q-item-label>
                  </q-item-section>
                </q-item>
              </template>
            </q-select>

            <div class="row q-col-gutter-sm">
              <div class="col-6">
                <q-input
                  v-model="form.date"
                  label="Start Date"
                  outlined
                  dense
                  mask="####/##/##"
                  required
                >
                  <template v-slot:append>
                    <q-icon name="fas fa-calendar-alt" class="cursor-pointer">
                      <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                        <q-date v-model="form.date" mask="YYYY/MM/DD">
                          <div class="row items-center justify-end">
                            <q-btn v-close-popup label="Close" color="primary" flat />
                          </div>
                        </q-date>
                      </q-popup-proxy>
                    </q-icon>
                  </template>
                </q-input>
              </div>
              <div class="col-6">
                <q-input
                  v-model="form.endDate"
                  label="End Date"
                  outlined
                  dense
                  mask="####/##/##"
                  required
                >
                  <template v-slot:append>
                    <q-icon name="fas fa-calendar-alt" class="cursor-pointer">
                      <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                        <q-date v-model="form.endDate" mask="YYYY/MM/DD">
                          <div class="row items-center justify-end">
                            <q-btn v-close-popup label="Close" color="primary" flat />
                          </div>
                        </q-date>
                      </q-popup-proxy>
                    </q-icon>
                  </template>
                </q-input>
              </div>
            </div>

            <div class="row q-col-gutter-sm">
              <div class="col-6">
                <q-input v-model="form.time" label="Start Time" type="time" outlined dense required>
                  <template v-slot:prepend><q-icon name="fas fa-clock" /></template>
                </q-input>
              </div>
              <div class="col-6">
                <q-input
                  v-model="form.duration"
                  label="Duration"
                  outlined
                  dense
                  placeholder="e.g. 2.5 weeks"
                  required
                >
                  <template v-slot:prepend><q-icon name="fas fa-hourglass" /></template>
                </q-input>
              </div>
            </div>

            <div class="row justify-end q-mt-lg">
              <q-btn label="Cancel" flat color="grey-7" v-close-popup class="q-mr-sm" />
              <q-btn
                :label="isEditing ? 'Update Appointment' : 'Confirm Schedule'"
                color="primary"
                type="submit"
              />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed, reactive, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'ServiceCalendar',
  setup() {
    const $q = useQuasar()
    const route = useRoute()
    const router = useRouter()
    const selectedDate = ref(new Date().toISOString().substr(0, 10).replace(/-/g, '/'))
    const showDialog = ref(false)
    const isEditing = ref(false)
    const currentServiceId = ref(null)
    const customerFilter = ref(null)
    const projectFilter = ref(null)
    const teamFilter = ref(null)
    const isFieldView = computed(() => route.path.startsWith('/field'))

    const form = reactive({
      customerId: null,
      projectId: null,
      unitRef: null,
      type: 'Monthly Service',
      date: selectedDate.value,
      endDate: selectedDate.value,
      time: '08:00',
      duration: '2 hours',
      teamId: null,
    })

    const teams = computed(() => store.teams)

    const getTeam = (teamId) => {
      return store.teams.find((t) => t.id === teamId)
    }

    const events = computed(() => {
      const dates = []
      store.services.forEach((s) => {
        const customer = customerFilter.value
          ? store.customers.find((c) => c.id === customerFilter.value)
          : null
        const matchCustomer = !customerFilter.value || s.customer === customer?.name

        const project =
          customer && projectFilter.value
            ? customer.projects.find((p) => p.id === projectFilter.value)
            : null
        const matchProject = !projectFilter.value || s.project === project?.name

        const matchTeam = !teamFilter.value || s.teamId === teamFilter.value

        if (matchCustomer && matchProject && matchTeam) {
          // If it's a multi-day event
          if (s.endDate && s.endDate !== s.date) {
            let current = new Date(s.date.replace(/\//g, '-'))
            const end = new Date(s.endDate.replace(/\//g, '-'))
            while (current <= end) {
              dates.push(current.toISOString().split('T')[0].replace(/-/g, '/'))
              current.setDate(current.getDate() + 1)
            }
          } else {
            dates.push(s.date)
          }
        }
      })
      return [...new Set(dates)] // Unique dates
    })

    const filteredServices = computed(() => {
      return store.services.filter((s) => {
        let matchDate = false
        if (s.endDate && s.endDate !== s.date) {
          const selDate = new Date(selectedDate.value.replace(/\//g, '-')).setHours(0, 0, 0, 0)
          const startDate = new Date(s.date.replace(/\//g, '-')).setHours(0, 0, 0, 0)
          const endDate = new Date(s.endDate.replace(/\//g, '-')).setHours(0, 0, 0, 0)
          matchDate = selDate >= startDate && selDate <= endDate
        } else {
          matchDate = s.date === selectedDate.value
        }

        const customer = customerFilter.value
          ? store.customers.find((c) => c.id === customerFilter.value)
          : null
        const matchCustomer = !customerFilter.value || s.customer === customer?.name

        const project =
          customer && projectFilter.value
            ? customer.projects.find((p) => p.id === projectFilter.value)
            : null
        const matchProject = !projectFilter.value || s.project === project?.name

        const matchTeam = !teamFilter.value || s.teamId === teamFilter.value
        return matchDate && matchCustomer && matchProject && matchTeam
      })
    })

    // Options for selects
    const customerOptions = computed(() => {
      return store.customers.map((c) => ({ label: c.name, value: c.id }))
    })

    const filterProjectOptions = computed(() => {
      if (!customerFilter.value) return []
      const customer = store.customers.find((c) => c.id === customerFilter.value)
      return customer ? customer.projects.map((p) => ({ label: p.name, value: p.id })) : []
    })

    const projectOptions = computed(() => {
      if (!form.customerId) return []
      const customer = store.customers.find((c) => c.id === form.customerId)
      return customer ? customer.projects.map((p) => ({ label: p.name, value: p.id })) : []
    })

    const assetOptions = computed(() => {
      if (!form.projectId) return []
      let options = []
      store.customers.forEach((c) => {
        const p = c.projects.find((proj) => proj.id === form.projectId)
        if (p) {
          options = p.assets.map((a) => ({ label: a.unitRef, value: a.unitRef }))
        }
      })
      return options
    })

    // Watchers to clear nested selections and auto-fill duration
    watch(
      () => form.type,
      (newType) => {
        if (store.serviceDefinitions[newType]) {
          form.duration = store.serviceDefinitions[newType].duration
        }
      },
    )

    watch(
      () => form.customerId,
      () => {
        if (!isEditing.value) {
          form.projectId = null
          form.unitRef = null
        }
      },
    )
    watch(
      () => form.projectId,
      () => {
        if (!isEditing.value) {
          form.unitRef = null
        }
      },
    )

    watch(customerFilter, () => {
      projectFilter.value = null
    })

    const applyRouteFilters = () => {
      const queryCustomerId = route.query.customerId ? String(route.query.customerId) : null
      const queryProjectId = route.query.projectId ? String(route.query.projectId) : null

      if (queryProjectId && !queryCustomerId) {
        store.customers.forEach((c) => {
          if (c.projects.some((p) => p.id === queryProjectId)) {
            customerFilter.value = c.id
          }
        })
      } else if (queryCustomerId) {
        customerFilter.value = queryCustomerId
      }

      if (queryProjectId) {
        projectFilter.value = queryProjectId
      }
    }

    watch(
      () => route.query,
      () => applyRouteFilters(),
      { immediate: true },
    )

    const clearFilters = () => {
      customerFilter.value = null
      projectFilter.value = null
      teamFilter.value = null
    }

    const openAddDialog = () => {
      isEditing.value = false
      currentServiceId.value = null
      Object.assign(form, {
        customerId: null,
        projectId: null,
        unitRef: null,
        type: 'Monthly Service',
        date: selectedDate.value,
        endDate: selectedDate.value,
        time: '08:00',
        duration: '2 hours',
        teamId: null,
      })
      showDialog.value = true
    }

    const editService = (service) => {
      isEditing.value = true
      currentServiceId.value = service.id

      // Find IDs for the form
      let foundCustomerId = null
      let foundProjectId = null

      store.customers.forEach((c) => {
        c.projects.forEach((p) => {
          if (p.assets.some((a) => a.unitRef === service.unitRef)) {
            foundCustomerId = c.id
            foundProjectId = p.id
          }
        })
      })

      Object.assign(form, {
        customerId: foundCustomerId,
        projectId: foundProjectId,
        unitRef: service.unitRef,
        type: service.type,
        date: service.date,
        endDate: service.endDate || service.date,
        time: service.time || '08:00',
        duration: service.duration || '2 hours',
        teamId: service.teamId || null,
      })
      showDialog.value = true
    }

    const saveService = () => {
      const customer = store.customers.find((c) => c.id === form.customerId)
      const project = customer.projects.find((p) => p.id === form.projectId)

      const serviceData = {
        date: form.date,
        endDate: form.endDate,
        time: form.time,
        duration: form.duration,
        unitRef: form.unitRef,
        customer: customer.name,
        project: project.name,
        type: form.type,
        teamId: form.teamId,
      }

      if (isEditing.value) {
        store.updateService(currentServiceId.value, serviceData)
        $q.notify({ color: 'positive', message: 'Appointment updated successfully', icon: 'check' })
      } else {
        store.addService(serviceData)
        $q.notify({ color: 'positive', message: 'New service scheduled', icon: 'event_available' })
      }

      showDialog.value = false
    }

    const confirmDelete = (service) => {
      $q.dialog({
        title: 'Cancel Visit',
        message: `Are you sure you want to remove the ${service.type} for ${service.unitRef}?`,
        cancel: true,
        persistent: true,
        ok: { color: 'negative', label: 'Yes, Cancel Visit' },
      }).onOk(() => {
        store.deleteService(service.id)
        $q.notify({ color: 'negative', message: 'Visit cancelled', icon: 'delete' })
      })
    }

    const mailAll = () => {
      $q.notify({
        color: 'positive',
        message: 'Service schedules automatically sent to clients and service providers',
        icon: 'fas fa-paper-plane',
      })
    }

    const getEventColor = (date) => {
      const isDateMatch = (s, dStr) => {
        const d = dStr.replace(/\//g, '')
        const start = s.date.replace(/\//g, '')
        const end = (s.endDate || s.date).replace(/\//g, '')
        return d >= start && d <= end
      }

      // Find all services on this date that match active filters
      const servicesOnDate = store.services.filter((s) => {
        const customer = customerFilter.value
          ? store.customers.find((c) => c.id === customerFilter.value)
          : null
        const matchCustomer = !customerFilter.value || s.customer === customer?.name

        const project =
          customer && projectFilter.value
            ? customer.projects.find((p) => p.id === projectFilter.value)
            : null
        const matchProject = !projectFilter.value || s.project === project?.name

        const matchTeam = !teamFilter.value || s.teamId === teamFilter.value

        return matchCustomer && matchProject && matchTeam && isDateMatch(s, date)
      })

      if (servicesOnDate.length > 0) {
        // If team filter is active, show that team's color
        if (teamFilter.value) {
          const team = store.teams.find((t) => t.id === teamFilter.value)
          return team ? team.color : 'primary'
        }

        // Otherwise, prioritize the team with color #9C27B0 (Team Alpha) if they are scheduled
        const alphaService = servicesOnDate.find((s) => {
          const t = store.teams.find((team) => team.id === s.teamId)
          return t && t.color === '#9C27B0'
        })

        if (alphaService) return '#9C27B0'

        // Fallback to the first service's team color
        const firstTeam = store.teams.find((t) => t.id === servicesOnDate[0].teamId)
        return firstTeam ? firstTeam.color : 'primary'
      }

      return 'primary'
    }

    const getUserName = (id) => {
      const user = store.users.find((u) => u.id === id)
      return user ? user.fullName : 'Not Assigned'
    }

    const getTeamName = (teamId) => {
      const team = store.teams.find((t) => t.id === teamId)
      return team ? team.name : 'No Team Assigned'
    }

    const getTeamColor = (teamId) => {
      const team = store.teams.find((t) => t.id === teamId)
      return team ? team.color : 'grey-7'
    }

    const getStatusColor = (status) => {
      if (status === 'Completed') return 'positive'
      if (status === 'In Progress') return 'primary'
      if (status === 'Awaiting Signature') return 'amber-8'
      return 'grey-7'
    }

    const getAssetIdByUnitRef = (unitRef) => {
      let foundId = null
      store.customers.forEach((customer) => {
        customer.projects.forEach((project) => {
          const asset = project.assets.find((a) => a.unitRef === unitRef)
          if (asset) foundId = asset.id
        })
      })
      return foundId
    }

    const startService = (service) => {
      const assetId = getAssetIdByUnitRef(service.unitRef)
      const basePath = route.path.startsWith('/field') ? '/field/service-entry' : '/service-entry'
      if (assetId) {
        router.push({ path: `${basePath}/${assetId}`, query: { serviceId: service.id } })
      } else {
        router.push({ path: basePath, query: { serviceId: service.id } })
      }
    }

    const goToProjectActions = () => {
      const customerId = route.query.customerId ? String(route.query.customerId) : null
      const projectId = route.query.projectId ? String(route.query.projectId) : null
      if (customerId && projectId) {
        router.push({
          path: `/field/projects/${projectId}/actions`,
          query: { customerId, projectId },
        })
        return
      }
      router.push('/field/projects')
    }

    return {
      selectedDate,
      events,
      filteredServices,
      showDialog,
      isEditing,
      form,
      customerOptions,
      projectOptions,
      filterProjectOptions,
      assetOptions,
      customerFilter,
      projectFilter,
      teamFilter,
      teams,
      isFieldView,
      getUserName,
      getTeamName,
      getTeamColor,
      getStatusColor,
      getEventColor,
      getTeam,
      clearFilters,
      openAddDialog,
      editService,
      saveService,
      confirmDelete,
      mailAll,
      startService,
      goToProjectActions,
      store,
    }
  },
})
</script>

<style scoped>
.calendar-card {
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.05);
}

.service-item-card {
  border-radius: 12px;
  transition: all 0.3s ease;
  border: 1px solid #edf2f7;
}

.service-item-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
  border-color: #3182ce;
}

.empty-state {
  border: 2px dashed #e2e8f0;
  border-radius: 16px;
  background: #f8fafc;
}

.rounded-borders {
  border-radius: 12px;
}

.field-schedule-page {
  padding: 12px;
}

.field-schedule-header .text-h4 {
  font-size: 22px;
}

.field-schedule-header .text-subtitle1 {
  font-size: 13px;
}

.field-schedule-filters {
  padding: 10px 12px;
}

.field-schedule-filters .q-field__control {
  border-radius: 12px;
}

.field-filter-reset .q-btn {
  width: 100%;
}

.field-schedule-page .service-item-card {
  border-radius: 16px;
}

.field-schedule-page .service-item-card .q-item {
  align-items: flex-start;
  flex-direction: column;
  gap: 12px;
}

.field-schedule-page .service-item-main,
.field-schedule-page .service-item-meta {
  width: 100%;
}

.field-schedule-page .service-item-meta {
  align-items: flex-start;
}

.field-schedule-page .service-item-meta .column {
  align-items: flex-start;
}
</style>
