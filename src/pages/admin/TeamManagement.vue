<template>
  <q-page padding>
    <div class="row items-center justify-between q-mb-lg">
      <div>
        <div class="text-h4 text-weight-bold text-primary">Team Management</div>
        <div class="text-subtitle1 text-grey-7">Organize technicians into operational teams</div>
      </div>
      <q-btn
        color="primary"
        icon="fas fa-users-rectangle"
        label="Create New Team"
        unelevated
        @click="openTeamDialog()"
      />
    </div>

    <q-card flat bordered class="rounded-borders shadow-2">
      <q-table
        :rows="filteredTeams"
        :columns="columns"
        row-key="id"
        flat
        :pagination="{ rowsPerPage: 10 }"
        class="team-table"
      >
        <template v-slot:header="props">
          <q-tr :props="props">
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
                <!-- Modern Sort Icon on the Left -->
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

        <template v-slot:body-cell-color="props">
          <q-td :props="props">
            <div class="color-pill" :style="{ backgroundColor: props.value }"></div>
          </q-td>
        </template>

        <template v-slot:body-cell-leaderIds="props">
          <q-td :props="props">
            <div class="row items-center q-gutter-xs">
              <template v-if="props.value && props.value.length">
                <q-chip
                  v-for="id in props.value"
                  :key="id"
                  dense
                  color="primary"
                  text-color="white"
                  class="q-ma-none"
                >
                  <q-avatar size="18px" color="white" text-color="primary">
                    {{ getInitials(getUserName(id)) }}
                  </q-avatar>
                  <span class="q-ml-xs" style="font-size: 10px">{{ getUserName(id) }}</span>
                </q-chip>
              </template>
              <div v-else class="text-grey-5 italic">None</div>
            </div>
          </q-td>
        </template>

        <template v-slot:body-cell-assistantIds="props">
          <q-td :props="props">
            <div class="row items-center q-gutter-xs">
              <template v-if="props.value && props.value.length">
                <q-chip
                  v-for="id in props.value"
                  :key="id"
                  dense
                  color="secondary"
                  text-color="white"
                  class="q-ma-none"
                >
                  <q-avatar size="18px" color="white" text-color="secondary">
                    {{ getInitials(getUserName(id)) }}
                  </q-avatar>
                  <span class="q-ml-xs" style="font-size: 10px">{{ getUserName(id) }}</span>
                </q-chip>
              </template>
              <div v-else class="text-grey-5 italic">None</div>
            </div>
          </q-td>
        </template>

        <template v-slot:body-cell-actions="props">
          <q-td :props="props" align="right">
            <q-btn-dropdown flat round dense dropdown-icon="fa fas fa-ellipsis-v" no-icon-animation>
              <q-list dense style="min-width: 140px">
                <q-item clickable v-close-popup @click="openTeamDialog(props.row)">
                  <q-item-section avatar>
                    <q-icon name="fas fa-edit" color="blue" size="xs" />
                  </q-item-section>
                  <q-item-section>Edit Team</q-item-section>
                </q-item>
                <q-item clickable v-close-popup @click="confirmDelete(props.row)">
                  <q-item-section avatar>
                    <q-icon name="fas fa-trash" color="negative" size="xs" />
                  </q-item-section>
                  <q-item-section>Delete Team</q-item-section>
                </q-item>
              </q-list>
            </q-btn-dropdown>
          </q-td>
        </template>
      </q-table>
    </q-card>

    <!-- Add/Edit Team Dialog -->
    <q-dialog v-model="showDialog" persistent>
      <q-card style="min-width: 400px; border-radius: 12px">
        <q-card-section class="bg-primary text-white row items-center">
          <div class="text-h6">{{ isEditing ? 'Edit Team' : 'Create New Team' }}</div>
          <q-space />
          <q-btn icon="fas fa-times" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="saveTeam" class="q-gutter-md">
            <q-input
              v-model="form.name"
              label="Team Name"
              outlined
              dense
              required
              placeholder="e.g. Rapid Response 1"
            />

            <div class="row items-center q-gutter-x-sm">
              <div class="col">
                <q-input v-model="form.color" label="Team Identifier Color" outlined dense required>
                  <template v-slot:append>
                    <q-icon name="fas fa-palette" class="cursor-pointer">
                      <q-popup-proxy cover transition-show="scale" transition-hide="scale">
                        <q-color v-model="form.color" />
                      </q-popup-proxy>
                    </q-icon>
                  </template>
                </q-input>
              </div>
              <div class="color-preview" :style="{ backgroundColor: form.color }"></div>
            </div>

            <q-select
              v-model="form.leaderIds"
              :options="technicianOptions"
              label="Lead Technicians"
              outlined
              dense
              emit-value
              map-options
              multiple
              use-chips
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-user-tie" size="xs" /></template>
            </q-select>

            <q-select
              v-model="form.assistantIds"
              :options="technicianOptions"
              label="Assistant Technicians"
              outlined
              dense
              emit-value
              map-options
              multiple
              use-chips
              required
            >
              <template v-slot:prepend><q-icon name="fas fa-user-gear" size="xs" /></template>
            </q-select>

            <div class="row justify-end q-mt-lg">
              <q-btn label="Cancel" flat color="grey" v-close-popup />
              <q-btn
                :label="isEditing ? 'Update Team' : 'Create Team'"
                type="submit"
                color="primary"
                unelevated
                class="q-ml-sm"
              />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script>
import { defineComponent, reactive, ref, computed } from 'vue'
import { useQuasar } from 'quasar'
import { store } from '../../store'

export default defineComponent({
  name: 'TeamManagement',
  setup() {
    const $q = useQuasar()
    const showDialog = ref(false)
    const isEditing = ref(false)
    const editingId = ref(null)

    const form = reactive({
      name: '',
      color: '#1976D2',
      leaderIds: [],
      assistantIds: [],
    })

    const teams = computed(() => store.teams)

    const columnFilters = reactive({
      name: '',
      leaderIds: '',
      assistantIds: '',
    })

    const filteredTeams = computed(() => {
      return store.teams.filter((team) => {
        return Object.keys(columnFilters).every((key) => {
          if (!columnFilters[key]) return true

          if (key === 'leaderIds' || key === 'assistantIds') {
            const ids = team[key] || []
            const names = ids.map((id) => getUserName(id).toLowerCase())
            return names.some((name) => name.includes(columnFilters[key].toLowerCase()))
          }

          const val = team[key] || ''
          return String(val).toLowerCase().includes(columnFilters[key].toLowerCase())
        })
      })
    })

    const technicianOptions = computed(() => {
      return store.users
        .filter((u) => u.role === 'technician' && u.active)
        .map((u) => ({ label: u.fullName, value: u.id }))
    })

    const columns = [
      { name: 'color', label: 'ID', field: 'color', align: 'left', style: 'width: 50px' },
      { name: 'name', label: 'Team Name', field: 'name', align: 'left', sortable: true },
      { name: 'leaderIds', label: 'Lead Techs', field: 'leaderIds', align: 'left' },
      { name: 'assistantIds', label: 'Assistants', field: 'assistantIds', align: 'left' },
      { name: 'actions', label: '', field: 'actions', align: 'right' },
    ]

    const getUserName = (id) => {
      const user = store.users.find((u) => u.id === id)
      return user ? user.fullName : 'Not Assigned'
    }

    const getInitials = (name) => {
      if (!name) return '?'
      return name
        .split(' ')
        .map((n) => n[0])
        .join('')
        .toUpperCase()
        .substring(0, 2)
    }

    const openTeamDialog = (team = null) => {
      if (team) {
        isEditing.value = true
        editingId.value = team.id
        form.name = team.name
        form.color = team.color
        form.leaderIds = Array.isArray(team.leaderIds)
          ? [...team.leaderIds]
          : team.leaderId
            ? [team.leaderId]
            : []
        form.assistantIds = Array.isArray(team.assistantIds)
          ? [...team.assistantIds]
          : team.assistantId
            ? [team.assistantId]
            : []
      } else {
        isEditing.value = false
        editingId.value = null
        form.name = ''
        form.color = '#1976D2'
        form.leaderIds = []
        form.assistantIds = []
      }
      showDialog.value = true
    }

    const saveTeam = () => {
      if (isEditing.value) {
        store.updateTeam(editingId.value, { ...form })
        $q.notify({ color: 'positive', message: 'Team updated successfully', icon: 'check' })
      } else {
        store.addTeam({ ...form })
        $q.notify({ color: 'positive', message: 'New team created successfully', icon: 'check' })
      }
      showDialog.value = false
    }

    const confirmDelete = (team) => {
      $q.dialog({
        title: 'Delete Team',
        message: `Are you sure you want to delete ${team.name}?`,
        cancel: true,
        persistent: true,
        ok: { color: 'negative', label: 'Delete' },
      }).onOk(() => {
        store.deleteTeam(team.id)
        $q.notify({ color: 'positive', message: 'Team deleted' })
      })
    }

    return {
      teams,
      filteredTeams,
      columnFilters,
      columns,
      showDialog,
      isEditing,
      form,
      technicianOptions,
      getUserName,
      getInitials,
      openTeamDialog,
      saveTeam,
      confirmDelete,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
.color-pill {
  width: 24px;
  height: 24px;
  border-radius: 4px;
  border: 1px solid rgba(0, 0, 0, 0.1);
}
.color-preview {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: 2px solid #ddd;
}
.team-table :deep(thead tr th) {
  position: sticky;
  top: 0;
  z-index: 1;
}

.sort-icon {
  opacity: 0.2;
  transition: all 0.3s ease;
}
.sort-icon.active {
  opacity: 1;
  color: var(--q-primary);
}
.filter-btn {
  transition: all 0.3s ease;
}
.filter-btn.active {
  background: rgba(25, 118, 210, 0.1);
}
</style>
