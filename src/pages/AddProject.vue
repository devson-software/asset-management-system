<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card shadow-2">
        <q-card-section class="bg-secondary text-white">
          <div class="row items-center no-wrap">
            <q-icon name="fas fa-map-location-dot" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5">{{ isEdit ? 'Update Project' : 'Capture New Project' }}</div>
              <div class="text-subtitle2">{{ isEdit ? 'Editing site details' : 'Assigning site to client' }}</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <!-- Customer Selection (Only if not pre-defined) -->
            <div v-if="isStandaloneAdd" class="row q-col-gutter-md">
              <div class="col-12">
                <q-select
                  v-model="project.customerId"
                  :options="customerOptions"
                  label="Assign to Customer"
                  outlined
                  dense
                  required
                  emit-value
                  map-options
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
                </q-select>
              </div>
            </div>

            <!-- Customer Context (Only if pre-defined or editing) -->
            <div v-else class="bg-blue-1 q-pa-md rounded-borders flex items-center">
              <q-avatar color="blue-2" text-color="primary" icon="fas fa-building" size="sm" class="q-mr-sm" />
              <div>
                <span class="text-grey-7">{{ isEdit ? 'Updating site for:' : 'Assigning project to:' }}</span>
                <span class="text-weight-bold text-primary q-ml-xs">{{ customerName }}</span>
              </div>
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="project.name" 
                  label="Project Name" 
                  outlined 
                  dense 
                  required 
                  placeholder="e.g. Downtown Office Tower"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-tag" size="xs" /></template>
                </q-input>
              </div>

              <div class="col-12 col-md-6">
                <q-file
                  v-model="project.picture"
                  label="Site / Project Picture"
                  outlined
                  dense
                  bg-color="white"
                  accept="image/*"
                  @update:model-value="onFileChange"
                >
                  <template v-slot:prepend>
                    <q-icon name="fas fa-image" size="xs" />
                  </template>
                  <template v-slot:append v-if="project.picture">
                    <q-avatar size="32px" square>
                      <img :src="project.pictureUrl || 'https://cdn.quasar.dev/img/avatar.png'">
                    </q-avatar>
                  </template>
                </q-file>
              </div>

              <div class="col-12">
                <q-input 
                  v-model="project.siteAddress" 
                  label="Physical Site Address" 
                  type="textarea" 
                  outlined 
                  dense 
                  required 
                  placeholder="Street address where units are installed"
                  bg-color="white"
                  rows="3"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                </q-input>
              </div>

              <div class="col-12">
                <q-input 
                  v-model="project.vendorLocation" 
                  label="Building Section / Internal Location" 
                  outlined 
                  dense 
                  placeholder="e.g. Roof Level, Basement, Plant Room"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-building-circle-check" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <div class="row q-gutter-sm justify-between q-mt-xl">
              <q-btn label="Cancel" flat color="grey-7" @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Project' : 'Confirm & Capture Project'" color="secondary" type="submit" icon="fas fa-check-circle" class="q-px-md" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AddProject',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()
    const customerId = route.params.customerId
    const projectId = route.params.projectId
    const isEdit = !!projectId
    const isStandaloneAdd = !customerId && !isEdit

    const customerOptions = computed(() => {
      return store.customers.map(c => ({ label: c.name, value: c.id }))
    })
    
    const customer = customerId ? store.customers.find(c => c.id === customerId) : null
    const customerName = computed(() => {
      if (customerId) return customer ? customer.name : 'Unknown'
      if (project.customerId) {
        const found = store.customers.find(c => c.id === project.customerId)
        return found ? found.name : 'Select Customer'
      }
      return 'Select Customer'
    })

    const project = reactive({ 
      name: '', 
      siteAddress: '', 
      vendorLocation: '',
      customerId: customerId || null,
      picture: null,
      pictureUrl: null
    })

    if (isEdit && customerId) {
      const targetCustomer = store.customers.find(c => c.id === customerId)
      if (targetCustomer) {
        const existingProject = targetCustomer.projects.find(p => p.id === projectId)
        if (existingProject) {
          Object.assign(project, existingProject)
        }
      }
    }

    const onFileChange = (file) => {
      if (file) {
        project.pictureUrl = URL.createObjectURL(file)
      }
    }

    const onSubmit = () => {
      const finalCustomerId = customerId || project.customerId
      
      if (!finalCustomerId) {
        $q.notify({ color: 'negative', message: 'Please select a customer' })
        return
      }

      const savedData = { ...project }
      delete savedData.picture // Don't store the File object in our simple store for now

      if (isEdit) {
        store.updateProject(finalCustomerId, projectId, savedData)
        $q.notify({ color: 'positive', message: 'Project updated successfully' })
      } else {
        store.addProject(finalCustomerId, savedData)
        $q.notify({ color: 'positive', message: 'Project added to ' + (customerName.value || 'the system') })
      }

      // Clear form
      Object.keys(project).forEach(key => {
        if (key === 'picture') project[key] = null
        else project[key] = ''
      })
      project.pictureUrl = null

      router.push({ path: '/projects', query: { customerId: finalCustomerId } })
    }

    return { project, customerName, onSubmit, isEdit, isStandaloneAdd, customerOptions, onFileChange }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 800px;
  margin: 0 auto;
}
</style>

