<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h5 text-weight-bold text-primary">Customers</div>
          <div class="text-subtitle2 text-grey-7">Choose a client to view projects</div>
        </div>
        
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders bg-white q-pa-md">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-sm-6">
              <q-select
                v-model="selectedCustomerId"
                :options="customerOptions"
                label="Search Customers"
                outlined
                dense
                emit-value
                map-options
                use-input
                input-debounce="0"
                @filter="filterCustomers"
                option-label="label"
                option-value="value"
                clearable
                bg-color="white"
              >
                <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
              </q-select>
            </div>
            <div class="col-12 col-sm-6">
              <q-select
                v-model="selectedProjectId"
                :options="projectOptions"
                label="Search Projects/Buildings"
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
                :disable="!selectedCustomerId"
                bg-color="white"
              >
                <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
              </q-select>
            </div>
          </div>
        </q-card>
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="filteredRows"
            :columns="columns"
            row-key="id"
            flat
            :filter="filter"
            :pagination="{ rowsPerPage: 10 }"
            class="customers-table"
            @row-click="goToProjects"
          >
            <template v-slot:header="props">
              <q-tr :props="props" class="cursor-pointer">
                <q-th
                  v-for="col in props.cols"
                  :key="col.name"
                  :props="props"
                  :class="'text-' + col.align"
                >
                  <div
                    class="row items-center no-wrap"
                    :class="
                      col.align === 'center'
                        ? 'justify-center'
                        : col.align === 'right'
                          ? 'justify-end'
                          : ''
                    "
                  >
                    <q-icon
                      v-if="col.sortable"
                      :name="
                        props.pagination && props.pagination.sortBy === col.name
                          ? props.pagination.descending
                            ? 'fas fa-arrow-down-long'
                            : 'fas fa-arrow-up-long'
                          : 'fas fa-arrow-up-long'
                      "
                      size="12px"
                      class="q-mr-xs cursor-pointer sort-icon"
                      :class="{ active: props.pagination && props.pagination.sortBy === col.name }"
                      @click="props.sort(col)"
                    />
                    <span class="cursor-pointer" @click="col.sortable && props.sort(col)">{{
                      col.label
                    }}</span>
                    <q-btn
                      v-if="col.name !== 'actions'"
                      flat
                      round
                      dense
                      size="xs"
                      icon="fas fa-filter"
                      class="q-ml-xs filter-btn"
                      :class="{ active: columnFilters[col.name] }"
                      :color="columnFilters[col.name] ? 'primary' : 'grey-5'"
                    >
                      <q-menu cover anchor="top middle">
                        <q-list style="min-width: 200px">
                          <q-item>
                            <q-input
                              v-model="columnFilters[col.name]"
                              :label="'Filter ' + col.label"
                              outlined
                              dense
                              autofocus
                              clearable
                            >
                              <template v-slot:append>
                                <q-icon name="fas fa-magnifying-glass" size="xs" />
                              </template>
                            </q-input>
                          </q-item>
                        </q-list>
                      </q-menu>
                    </q-btn>
                  </div>
                </q-th>
              </q-tr>
            </template>

            <template v-slot:body-cell-fullName="props">
              <q-td :props="props">
                <div class="row items-center no-wrap">
                  <q-avatar size="32px" class="q-mr-md shadow-1">
                    <q-icon name="fas fa-building" color="primary" />
                  </q-avatar>
                  <div>
                    <div class="text-weight-bold">{{ props.row.fullName }}</div>
                    <div class="text-caption text-grey-7">
                      {{ props.row.contactName || 'No contact' }}
                    </div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-invitation="props">
              <q-td :props="props" class="text-center">
                <q-chip dense color="blue-1" text-color="blue-9" class="text-weight-bold">
                  {{ props.row.invitationStatus }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-mobile="props">
              <q-td :props="props">
                <q-chip outline color="grey-7" size="sm" icon="fas fa-phone" dense>
                  {{ props.value || 'N/A' }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-vatNumber="props">
              <q-td :props="props">
                <div class="text-caption text-grey-7">{{ props.value || 'N/A' }}</div>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed, ref, reactive, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'FieldCustomers',
  setup() {
    const router = useRouter()
    const store = useMainStore()
    const filter = ref('')
    const selectedCustomerId = ref(null)
    const selectedProjectId = ref(null)

    const columnFilters = reactive({
      fullName: '',
      email: '',
      mobile: '',
      vatNumber: '',
      invitation: '',
    })

    const columns = [
      { name: 'fullName', label: 'Customer', align: 'left', field: 'fullName', sortable: true },
      { name: 'email', label: 'Email Address', align: 'left', field: 'email', sortable: true },
      { name: 'mobile', label: 'Mobile', align: 'left', field: 'mobile', sortable: true },
      { name: 'vatNumber', label: 'VAT Number', align: 'left', field: 'vatNumber', sortable: true },
      {
        name: 'invitation',
        label: 'Projects',
        align: 'center',
        field: 'invitationStatus',
        sortable: true,
      },
    ]

    const rows = computed(() => {
      return store.customers.map((c) => ({
        id: c.id,
        fullName: c.name,
        contactName: c.contactName,
        email: c.email || 'N/A',
        mobile: c.mobile || 'N/A',
        vatNumber: c.vatNumber || 'N/A',
        invitationStatus: `${c.projects?.length || 0} Projects`,
        projects: c.projects || [],
      }))
    })

    const customerOptions = computed(() => {
      return store.customers.map((c) => ({ label: c.name, value: c.id }))
    })

    const customerOptionsFiltered = ref(customerOptions.value || [])

    const filterCustomers = (val, update) => {
      update(() => {
        if (!val) {
          customerOptionsFiltered.value = customerOptions.value
          return
        }
        const needle = val.toLowerCase()
        customerOptionsFiltered.value = customerOptions.value.filter((c) =>
          c.label.toLowerCase().includes(needle),
        )
      })
    }

    const projectOptions = computed(() => {
      if (!selectedCustomerId.value) return []
      const customer = store.customers.find((c) => c.id === selectedCustomerId.value)
      return customer ? customer.projects.map((p) => ({ label: p.name, value: p.id })) : []
    })

    const projectOptionsFiltered = ref(projectOptions.value || [])

    watch(projectOptions, (next) => {
      projectOptionsFiltered.value = next || []
    })

    watch(selectedCustomerId, () => {
      selectedProjectId.value = null
      projectOptionsFiltered.value = projectOptions.value || []
    })

    watch(selectedProjectId, (nextProjectId) => {
      if (!nextProjectId) return
      router.push({
        path: `/field/projects/${nextProjectId}/actions`,
        query: { customerId: selectedCustomerId.value, projectId: nextProjectId },
      })
    })

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

    const filteredRows = computed(() => {
      return rows.value.filter((row) => {
        if (selectedCustomerId.value && row.id !== selectedCustomerId.value) return false
        if (selectedProjectId.value) {
          const matchProject = row.projects.some((p) => p.id === selectedProjectId.value)
          if (!matchProject) return false
        }
        return Object.keys(columnFilters).every((key) => {
          if (!columnFilters[key]) return true
          const val = row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const goToProjects = (evt, row) => {
      if (!row?.id) return
      router.push({ path: '/field/projects', query: { customerId: row.id } })
    }

    return {
      filter,
      columnFilters,
      columns,
      filteredRows,
      goToProjects,
      selectedCustomerId,
      selectedProjectId,
      customerOptions: customerOptionsFiltered,
      projectOptions: projectOptionsFiltered,
      filterCustomers,
      filterProjects,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.customers-table {
  background: transparent;
}
</style>
