<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">Service Schedule</div>
          <div class="text-subtitle1 text-grey-7">Plan and manage maintenance visits across all sites</div>
        </div>
        <div class="q-gutter-sm">
          <q-btn 
            color="primary" 
            icon="fas fa-plus" 
            label="Schedule New Visit" 
            @click="openAddDialog" 
            class="shadow-2"
          />
          <q-btn 
            flat 
            color="primary" 
            icon="fas fa-paper-plane" 
            label="Broadcast Schedule" 
            @click="mailAll" 
          />
        </div>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders q-pa-md bg-grey-1">
          <div class="row q-col-gutter-md items-center">
            <div class="col-12 col-sm-5">
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
                <template v-slot:prepend><q-icon name="fas fa-business-time" size="xs" color="primary" /></template>
              </q-select>
            </div>
            <div class="col-12 col-sm-5">
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
                <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" color="primary" /></template>
              </q-select>
            </div>
            <div class="col-12 col-sm-2 flex justify-end">
              <q-btn 
                flat 
                color="grey-7" 
                icon="fas fa-filter-circle-xmark" 
                label="Reset" 
                @click="clearFilters" 
                v-if="customerFilter || projectFilter" 
              />
            </div>
          </div>
        </q-card>
      </div>

      <div class="col-12">
        <div class="row q-col-gutter-xl">
          <!-- Left Column: Calendar -->
          <div class="col-12 col-md-5">
            <q-card flat bordered class="calendar-card overflow-hidden">
              <q-date
                v-model="selectedDate"
                :events="events"
                event-color="primary"
                class="full-width no-border"
                flat
                today-btn
              />
              <q-separator />
              <q-card-section class="bg-grey-1">
                <div class="row items-center no-wrap">
                  <div class="q-badge bg-primary q-mr-sm" style="width: 10px; height: 10px; border-radius: 50%"></div>
                  <div class="text-caption text-grey-8">Indicates dates with scheduled services</div>
                </div>
              </q-card-section>
            </q-card>
          </div>

          <!-- Right Column: Service List -->
          <div class="col-12 col-md-7">
            <div class="row items-center justify-between q-mb-md">
              <div class="text-h6 text-grey-8">
                Services for <span class="text-primary text-weight-bold">{{ selectedDate }}</span>
              </div>
              <q-badge color="blue-1" text-color="primary" class="q-px-md q-py-xs text-weight-bold">
                {{ filteredServices.length }} Appointments
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
                <q-item class="q-py-md">
                  <q-item-section avatar>
                    <q-avatar color="blue-1" text-color="primary" icon="fas fa-screwdriver-wrench" />
                  </q-item-section>

                  <q-item-section>
                    <q-item-label class="text-weight-bold text-subtitle1">
                      {{ service.unitRef }}
                    </q-item-label>
                    <q-item-label class="text-grey-9">{{ service.customer }}</q-item-label>
                    <q-item-label caption class="row items-center">
                      <q-icon name="fas fa-location-dot" size="12px" class="q-mr-xs" />
                      {{ service.project }}
                    </q-item-label>
                  </q-item-section>

                  <q-item-section side>
                    <div class="column items-end">
                      <q-chip dense color="orange-1" text-color="orange-10" icon="fas fa-clock" size="sm" class="text-weight-bold">
                        {{ service.type }}
                      </q-chip>
                      <div class="row q-mt-sm">
                        <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                          <q-menu auto-close class="rounded-borders shadow-2">
                            <q-list style="min-width: 150px">
                              <q-item clickable @click="editService(service)">
                                <q-item-section avatar><q-icon name="fas fa-edit" color="primary" size="sm" /></q-item-section>
                                <q-item-section>Edit Visit</q-item-section>
                              </q-item>
                              <q-item clickable @click="confirmDelete(service)" class="text-negative">
                                <q-item-section avatar><q-icon name="fas fa-trash-can" color="negative" size="sm" /></q-item-section>
                                <q-item-section>Cancel Visit</q-item-section>
                              </q-item>
                              <q-separator />
                              <q-item clickable :to="'/service-entry'">
                                <q-item-section avatar><q-icon name="fas fa-play" color="positive" size="sm" /></q-item-section>
                                <q-item-section>Start Work</q-item-section>
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

            <div v-if="filteredServices.length === 0" class="column items-center justify-center q-pa-xl empty-state">
              <q-icon name="fas fa-calendar-xmark" size="80px" color="grey-4" />
              <div class="text-h6 text-grey-5 q-mt-md">Quiet day! No services scheduled.</div>
              <q-btn flat color="primary" label="Schedule something?" @click="openAddDialog" class="q-mt-sm" />
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
          <q-btn icon="close" flat round dense v-close-popup />
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
              <template v-slot:prepend><q-icon name="business" /></template>
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
              <template v-slot:prepend><q-icon name="place" /></template>
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
              <template v-slot:prepend><q-icon name="ac_unit" /></template>
            </q-select>

            <q-select
              v-model="form.type"
              :options="['Monthly Service', 'Quarterly Service', 'Annual Maintenance', 'Breakdown Callout']"
              label="Service Type"
              outlined
              dense
              required
            >
              <template v-slot:prepend><q-icon name="category" /></template>
            </q-select>

            <q-input 
              v-model="form.date" 
              label="Scheduled Date" 
              outlined 
              dense 
              mask="####/##/##" 
              required
            >
              <template v-slot:append>
                <q-icon name="event" class="cursor-pointer">
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

            <div class="row justify-end q-mt-lg">
              <q-btn label="Cancel" flat color="grey-7" v-close-popup class="q-mr-sm" />
              <q-btn :label="isEditing ? 'Update Appointment' : 'Confirm Schedule'" color="primary" type="submit" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { defineComponent, ref, computed, reactive, watch } from 'vue'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'ServiceCalendar',
  setup () {
    const $q = useQuasar()
    const selectedDate = ref(new Date().toISOString().substr(0, 10).replace(/-/g, '/'))
    const showDialog = ref(false)
    const isEditing = ref(false)
    const currentServiceId = ref(null)
    const customerFilter = ref(null)
    const projectFilter = ref(null)

    const form = reactive({
      customerId: null,
      projectId: null,
      unitRef: null,
      type: 'Monthly Service',
      date: selectedDate.value
    })

    const events = computed(() => {
      return store.services
        .filter(s => {
          const matchCustomer = !customerFilter.value || s.customer === store.customers.find(c => c.id === customerFilter.value)?.name
          const matchProject = !projectFilter.value || s.project === store.customers.find(c => c.id === customerFilter.value)?.projects.find(p => p.id === projectFilter.value)?.name
          return matchCustomer && matchProject
        })
        .map(s => s.date)
    })

    const filteredServices = computed(() => {
      return store.services.filter(s => {
        const matchDate = s.date === selectedDate.value
        const matchCustomer = !customerFilter.value || s.customer === store.customers.find(c => c.id === customerFilter.value)?.name
        const matchProject = !projectFilter.value || s.project === store.customers.find(c => c.id === customerFilter.value)?.projects.find(p => p.id === projectFilter.value)?.name
        return matchDate && matchCustomer && matchProject
      })
    })

    // Options for selects
    const customerOptions = computed(() => {
      return store.customers.map(c => ({ label: c.name, value: c.id }))
    })

    const filterProjectOptions = computed(() => {
      if (!customerFilter.value) return []
      const customer = store.customers.find(c => c.id === customerFilter.value)
      return customer ? customer.projects.map(p => ({ label: p.name, value: p.id })) : []
    })

    const projectOptions = computed(() => {
      if (!form.customerId) return []
      const customer = store.customers.find(c => c.id === form.customerId)
      return customer ? customer.projects.map(p => ({ label: p.name, value: p.id })) : []
    })

    const assetOptions = computed(() => {
      if (!form.projectId) return []
      let options = []
      store.customers.forEach(c => {
        const p = c.projects.find(proj => proj.id === form.projectId)
        if (p) {
          options = p.assets.map(a => ({ label: a.unitRef, value: a.unitRef }))
        }
      })
      return options
    })

    // Watchers to clear nested selections
    watch(() => form.customerId, () => {
      if (!isEditing.value) {
        form.projectId = null
        form.unitRef = null
      }
    })
    watch(() => form.projectId, () => {
      if (!isEditing.value) {
        form.unitRef = null
      }
    })

    watch(customerFilter, () => {
      projectFilter.value = null
    })

    const clearFilters = () => {
      customerFilter.value = null
      projectFilter.value = null
    }

    const openAddDialog = () => {
      isEditing.value = false
      currentServiceId.value = null
      Object.assign(form, {
        customerId: null,
        projectId: null,
        unitRef: null,
        type: 'Monthly Service',
        date: selectedDate.value
      })
      showDialog.value = true
    }

    const editService = (service) => {
      isEditing.value = true
      currentServiceId.value = service.id
      
      // Find IDs for the form
      let foundCustomerId = null
      let foundProjectId = null
      
      store.customers.forEach(c => {
        c.projects.forEach(p => {
          if (p.assets.some(a => a.unitRef === service.unitRef)) {
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
        date: service.date
      })
      showDialog.value = true
    }

    const saveService = () => {
      const customer = store.customers.find(c => c.id === form.customerId)
      const project = customer.projects.find(p => p.id === form.projectId)

      const serviceData = {
        date: form.date,
        unitRef: form.unitRef,
        customer: customer.name,
        project: project.name,
        type: form.type
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
        ok: { color: 'negative', label: 'Yes, Cancel Visit' }
      }).onOk(() => {
        store.deleteService(service.id)
        $q.notify({ color: 'negative', message: 'Visit cancelled', icon: 'delete' })
      })
    }

    const mailAll = () => {
      $q.notify({
        color: 'positive',
        message: 'Service schedules automatically sent to clients and service providers',
        icon: 'send'
      })
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
      clearFilters,
      openAddDialog,
      editService,
      saveService,
      confirmDelete,
      mailAll
    }
  }
})
</script>

<style scoped>
.calendar-card {
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.05);
}

.service-item-card {
  border-radius: 12px;
  transition: all 0.3s ease;
  border: 1px solid #edf2f7;
}

.service-item-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.05);
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
</style>

