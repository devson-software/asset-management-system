<template>
  <q-page padding>
    <div class="row q-col-gutter-lg">
      <div class="col-12 flex justify-between items-center">
        <div>
          <div class="text-h4 text-weight-bold text-primary">User Management</div>
          <div class="text-subtitle1 text-grey-7">Manage system users, technicians, and administrators</div>
        </div>
        <q-btn color="primary" icon="fas fa-user-plus" label="Add New User" to="/admin/users/add" class="shadow-2" />
      </div>

      <div class="col-12">
        <q-card flat bordered class="rounded-borders">
          <q-table
            :rows="filteredRows"
            :columns="columns"
            row-key="id"
            flat
            :filter="filter"
            class="users-table"
          >
            <template v-slot:header="props">
              <q-tr :props="props">
                <q-th
                  v-for="col in props.cols"
                  :key="col.name"
                  :props="props"
                  :class="'text-' + col.align"
                >
                  <div class="row items-center no-wrap" :class="col.align === 'center' ? 'justify-center' : (col.align === 'right' ? 'justify-end' : '')">
                    <q-icon
                      v-if="col.sortable"
                      :name="props.pagination && props.pagination.sortBy === col.name ? (props.pagination.descending ? 'fas fa-arrow-down-long' : 'fas fa-arrow-up-long') : 'fas fa-arrow-up-long'"
                      size="12px"
                      class="q-mr-xs cursor-pointer sort-icon"
                      :class="{ 'active': props.pagination && props.pagination.sortBy === col.name }"
                      @click="props.sort(col)"
                    />
                    <span class="cursor-pointer" @click="col.sortable && props.sort(col)">{{ col.label }}</span>
                    <q-btn
                      v-if="col.name !== 'actions'"
                      flat
                      round
                      dense
                      size="xs"
                      icon="fas fa-filter"
                      class="q-ml-xs filter-btn"
                      :class="{ 'active': columnFilters[col.name] }"
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
              <q-input borderless dense debounce="300" v-model="filter" placeholder="Search Users...">
                <template v-slot:append>
                  <q-icon name="fas fa-magnifying-glass" size="xs" />
                </template>
              </q-input>
            </template>

            <template v-slot:body-cell-fullName="props">
              <q-td :props="props">
                <div class="row items-center no-wrap">
                  <q-avatar size="32px" class="q-mr-md shadow-1">
                    <img v-if="props.row.pictureUrl" :src="props.row.pictureUrl">
                    <q-icon v-else :name="props.row.role === 'administrator' ? 'fas fa-user-shield' : (props.row.role === 'customer' ? 'fas fa-building-user' : 'fas fa-user-gear')" color="primary" />
                  </q-avatar>
                  <div>
                    <div class="text-weight-bold">{{ props.row.fullName }}</div>
                    <div class="text-caption text-grey-7">@{{ props.row.username }}</div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-role="props">
              <q-td :props="props">
                <q-chip 
                  :color="props.row.role === 'administrator' ? 'indigo-1' : (props.row.role === 'customer' ? 'orange-1' : 'teal-1')" 
                  :text-color="props.row.role === 'administrator' ? 'indigo-9' : (props.row.role === 'customer' ? 'orange-9' : 'teal-9')"
                  dense
                  square
                  class="text-weight-bold"
                >
                  {{ props.row.role.toUpperCase() }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-invitation="props">
              <q-td :props="props" class="text-center">
                <q-chip
                  v-if="props.row.invitationStatus"
                  :color="props.row.invitationStatus === 'Completed' ? 'green-1' : 'blue-1'"
                  :text-color="props.row.invitationStatus === 'Completed' ? 'green-9' : 'blue-9'"
                  dense
                  class="text-weight-bold"
                >
                  {{ props.row.invitationStatus }}
                </q-chip>
                <span v-else class="text-grey-5">-</span>
              </q-td>
            </template>

            <template v-slot:body-cell-active="props">
              <q-td :props="props">
                <q-badge :color="props.row.active ? 'positive' : 'grey-5'" rounded>
                  {{ props.row.active ? 'Active' : 'Inactive' }}
                </q-badge>
              </q-td>
            </template>

            <template v-slot:body-cell-actions="props">
              <q-td :props="props">
                <q-btn flat round color="grey-7" icon="fas fa-ellipsis-vertical">
                  <q-menu auto-close transition-show="scale" transition-hide="scale" class="rounded-borders shadow-2">
                    <q-list style="min-width: 150px">
                      <q-item clickable :to="'/admin/users/edit/' + props.row.id">
                        <q-item-section avatar>
                          <q-icon name="fas fa-edit" color="primary" size="sm" />
                        </q-item-section>
                        <q-item-section>Edit Info</q-item-section>
                      </q-item>
                      <q-item clickable @click="resendInvite(props.row)" v-if="props.row.invitationStatus === 'Pending'">
                        <q-item-section avatar>
                          <q-icon name="fas fa-paper-plane" color="blue-8" size="sm" />
                        </q-item-section>
                        <q-item-section>Resend Invite</q-item-section>
                      </q-item>
                      <q-separator />
                      <q-item clickable class="text-negative" @click="confirmDelete(props.row)">
                        <q-item-section avatar>
                          <q-icon name="fas fa-trash-can" color="negative" size="sm" />
                        </q-item-section>
                        <q-item-section>Remove User</q-item-section>
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

    <!-- Delete Confirmation -->
    <q-dialog v-model="showDeleteDialog" persistent>
      <q-card style="border-radius: 16px;">
        <q-card-section class="row items-center">
          <q-avatar icon="fas fa-triangle-exclamation" color="negative" text-color="white" />
          <span class="q-ml-sm text-h6">Confirm Deletion</span>
        </q-card-section>

        <q-card-section class="q-pt-none">
          Are you sure you want to delete user <strong>{{ userToDelete?.fullName }}</strong>? This action cannot be undone.
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancel" color="primary" v-close-popup />
          <q-btn flat label="Delete" color="negative" @click="deleteUser" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive, computed } from 'vue'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AdminUserManagement',
  setup () {
    const $q = useQuasar()
    const filter = ref('')
    const showDeleteDialog = ref(false)
    const userToDelete = ref(null)
    const columnFilters = reactive({
      fullName: '',
      email: '',
      role: '',
      active: ''
    })

    const columns = [
      { name: 'fullName', label: 'User', align: 'left', field: 'fullName', sortable: true },
      { name: 'email', label: 'Email Address', align: 'left', field: 'email', sortable: true },
      { name: 'role', label: 'System Role', align: 'left', field: 'role', sortable: true },
      { name: 'invitation', label: 'Invitation', align: 'center', field: 'invitationStatus', sortable: true },
      { name: 'active', label: 'Status', align: 'left', field: 'active', sortable: true },
      { name: 'actions', label: '', align: 'right' }
    ]

    const filteredRows = computed(() => {
      return store.users.filter(row => {
        return Object.keys(columnFilters).every(key => {
          if (!columnFilters[key]) return true
          const val = key === 'active'
            ? (row.active ? 'Active' : 'Inactive')
            : row[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const confirmDelete = (user) => {
      userToDelete.value = user
      showDeleteDialog.value = true
    }

    const deleteUser = () => {
      if (userToDelete.value) {
        store.deleteUser(userToDelete.value.id)
        $q.notify({ color: 'positive', message: 'User deleted successfully', icon: 'fas fa-trash' })
        showDeleteDialog.value = false
        userToDelete.value = null
      }
    }

    const resendInvite = (user) => {
      $q.notify({
        color: 'info',
        message: `Invitation email resent to ${user.email}`,
        icon: 'fas fa-paper-plane'
      })
    }

    return {
      store,
      filter,
      columnFilters,
      columns,
      filteredRows,
      showDeleteDialog,
      userToDelete,
      confirmDelete,
      deleteUser,
      resendInvite
    }
  }
})
</script>

<style scoped>
.users-table {
  background: transparent;
}
.rounded-borders {
  border-radius: 12px;
}
</style>
