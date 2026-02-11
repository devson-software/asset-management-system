<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div class="row items-center no-wrap">
          <q-btn flat round icon="fas fa-arrow-left" @click="$router.push('/field/customers')" />
          <div class="q-ml-sm">
            <div class="text-h5 text-weight-bold text-primary">Projects/Buildings</div>
            <div class="text-subtitle2 text-grey-7">Choose a site to view schedule</div>
          </div>
        </div>
        
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders bg-white q-pa-md">
          <div class="row q-col-gutter-md">
            <div class="col-12">
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
                <template v-slot:prepend>
                  <q-icon name="fas fa-users" size="xs" />
                </template>
              </q-select>
            </div>
          </div>
        </q-card>

        <q-banner v-if="!customer" rounded class="bg-orange-1 text-orange-9 q-mb-md">
          <template v-slot:avatar><q-icon name="fas fa-circle-exclamation" /></template>
          Please select a customer first.
        </q-banner>

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
            class="projects-table"
            @row-click="goToActions"
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

            <template v-slot:body-cell-name="props">
              <q-td :props="props">
                <div class="row items-center no-wrap">
                  <q-avatar size="32px" class="q-mr-md shadow-1">
                    <q-icon name="fas fa-location-dot" color="primary" />
                  </q-avatar>
                  <div>
                    <div class="text-weight-bold">{{ props.row.name }}</div>
                    <div class="text-caption text-grey-7">{{ props.row.vendorLocation || 'N/A' }}</div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-siteAddress="props">
              <q-td :props="props">
                <div class="text-caption text-grey-7">{{ props.value || 'No site address' }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-assetsCount="props">
              <q-td :props="props" class="text-center">
                <q-chip dense color="blue-1" text-color="primary" class="text-weight-bold">
                  {{ props.value }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props" class="text-right">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical" size="sm">
                  <q-menu auto-close class="rounded-borders shadow-2">
                    <q-list style="min-width: 180px">
                      <q-item clickable @click="goToActions(null, props.row)">
                        <q-item-section avatar>
                          <q-icon name="fas fa-list-check" color="primary" size="sm" />
                        </q-item-section>
                        <q-item-section>Project Actions</q-item-section>
                      </q-item>
                    </q-list>
                  </q-menu>
                </q-btn>
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
import { useRoute, useRouter } from 'vue-router'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'FieldProjects',
  setup() {
    const route = useRoute()
    const router = useRouter()
    const store = useMainStore()
    const selectedCustomerId = ref(route.query.customerId || null)
    const customerId = computed(() => selectedCustomerId.value || '')
    const filter = ref('')
    const columnFilters = reactive({
      name: '',
      siteAddress: '',
      assetsCount: '',
    })

    const customer = computed(() => {
      return store.customers.find((c) => c.id === customerId.value) || null
    })

    const projects = computed(() => {
      return customer.value?.projects || []
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

    watch(
      () => route.query.customerId,
      (nextId) => {
        selectedCustomerId.value = nextId || null
      },
    )

    watch(selectedCustomerId, (nextId) => {
      if (String(route.query.customerId || '') === String(nextId || '')) return
      router.push({
        path: '/field/projects',
        query: nextId ? { customerId: nextId } : {},
      })
    })

    const columns = [
      { name: 'name', label: 'Project/Building', align: 'left', field: 'name', sortable: true },
      {
        name: 'siteAddress',
        label: 'Site Address',
        align: 'left',
        field: 'siteAddress',
        sortable: true,
      },
      {
        name: 'assetsCount',
        label: 'Assets',
        align: 'center',
        field: 'assetsCount',
        sortable: true,
      },
      { name: 'actions', label: '', align: 'right' },
    ]

    const rows = computed(() => {
      return projects.value.map((p) => ({
        id: p.id,
        name: p.name,
        siteAddress: p.siteAddress || '',
        vendorLocation: p.vendorLocation || '',
        assetsCount: p.assets?.length || 0,
      }))
    })

    const filteredRows = computed(() => {
      return rows.value.filter((row) => {
        return Object.keys(columnFilters).every((key) => {
          if (!columnFilters[key]) return true
          const val = row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const goToActions = (evt, row) => {
      if (!row?.id) return
      router.push({
        path: `/field/projects/${row.id}/actions`,
        query: { customerId: customerId.value, projectId: row.id },
      })
    }

    return {
      customer,
      filter,
      columnFilters,
      columns,
      filteredRows,
      goToActions,
      selectedCustomerId,
      customerOptions: customerOptionsFiltered,
      filterCustomers,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.projects-table {
  background: transparent;
}
</style>
