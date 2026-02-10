<template>
  <q-page padding class="bg-grey-1">
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h5 text-weight-bold text-primary">Select Customer</div>
          <div class="text-subtitle2 text-grey-7">Choose a client to view projects</div>
        </div>
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

            <template v-slot:top-right>
              <q-input
                borderless
                dense
                debounce="300"
                v-model="filter"
                placeholder="Search Customers..."
              >
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
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

            <template v-slot:body-cell-role="props">
              <q-td :props="props">
                <q-chip dense square color="orange-1" text-color="orange-9" class="text-weight-bold">
                  {{ props.row.role.toUpperCase() }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-invitation="props">
              <q-td :props="props" class="text-center">
                <q-chip dense color="blue-1" text-color="blue-9" class="text-weight-bold">
                  {{ props.row.invitationStatus }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-active="props">
              <q-td :props="props">
                <q-badge :color="props.row.active ? 'positive' : 'grey-5'" rounded>
                  {{ props.row.active ? 'Active' : 'Inactive' }}
                </q-badge>
              </q-td>
            </template>
          </q-table>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, computed, ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'FieldCustomers',
  setup() {
    const router = useRouter()
    const store = useMainStore()
    const filter = ref('')

    const columnFilters = reactive({
      fullName: '',
      email: '',
      role: '',
      invitation: '',
      active: '',
    })

    const columns = [
      { name: 'fullName', label: 'Customer', align: 'left', field: 'fullName', sortable: true },
      { name: 'email', label: 'Email Address', align: 'left', field: 'email', sortable: true },
      { name: 'role', label: 'System Role', align: 'left', field: 'role', sortable: true },
      {
        name: 'invitation',
        label: 'Projects',
        align: 'center',
        field: 'invitationStatus',
        sortable: true,
      },
      { name: 'active', label: 'Status', align: 'left', field: 'active', sortable: true },
    ]

    const rows = computed(() => {
      return store.customers.map((c) => ({
        id: c.id,
        fullName: c.name,
        contactName: c.contactName,
        email: c.email || 'N/A',
        role: 'customer',
        invitationStatus: `${c.projects?.length || 0} Projects`,
        active: true,
      }))
    })

    const filteredRows = computed(() => {
      return rows.value.filter((row) => {
        return Object.keys(columnFilters).every((key) => {
          if (!columnFilters[key]) return true
          const val =
            key === 'active'
              ? row.active
                ? 'Active'
                : 'Inactive'
              : row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const goToProjects = (evt, row) => {
      if (!row?.id) return
      router.push({ path: '/field/projects', query: { customerId: row.id } })
    }

    return { filter, columnFilters, columns, filteredRows, goToProjects }
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
