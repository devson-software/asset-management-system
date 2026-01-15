<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card shadow-2">
        <q-card-section class="bg-secondary text-white">
          <div class="row items-center no-wrap">
            <q-icon name="add_business" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5">{{ isEdit ? 'Update Project' : 'Capture New Project' }}</div>
              <div class="text-subtitle2">{{ isEdit ? 'Editing site details' : 'Assigning site to client' }}</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <!-- Customer Context -->
            <div class="bg-blue-1 q-pa-md rounded-borders flex items-center">
              <q-avatar color="blue-2" text-color="primary" icon="business" size="sm" class="q-mr-sm" />
              <div>
                <span class="text-grey-7">Assigning project to:</span>
                <span class="text-weight-bold text-primary q-ml-xs">{{ customerName }}</span>
              </div>
            </div>

            <div class="row q-col-gutter-md">
              <div class="col-12">
                <q-input 
                  v-model="project.name" 
                  label="Project Name" 
                  outlined 
                  dense 
                  required 
                  placeholder="e.g. Downtown Office Tower"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="label" /></template>
                </q-input>
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
                >
                  <template v-slot:prepend><q-icon name="place" /></template>
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
                  <template v-slot:prepend><q-icon name="apartment" /></template>
                </q-input>
              </div>
            </div>

            <div class="row q-gutter-sm justify-between q-mt-xl">
              <q-btn label="Cancel" flat color="grey-7" @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Project' : 'Confirm & Capture Project'" color="secondary" type="submit" icon="check_circle" class="q-px-md" />
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
    
    const customer = store.customers.find(c => c.id === customerId)
    const customerName = computed(() => customer ? customer.name : 'Unknown')

    const project = reactive({ name: '', siteAddress: '', vendorLocation: '' })

    if (isEdit && customer) {
      const existingProject = customer.projects.find(p => p.id === projectId)
      if (existingProject) {
        Object.assign(project, existingProject)
      }
    }

    const onSubmit = () => {
      if (isEdit) {
        store.updateProject(customerId, projectId, { ...project })
        $q.notify({ color: 'positive', message: 'Project updated successfully' })
      } else {
        store.addProject(customerId, { ...project })
        $q.notify({ color: 'positive', message: 'Project added to ' + customerName.value })
      }

      // Clear form
      Object.keys(project).forEach(key => project[key] = '')

      router.push({ path: '/projects', query: { customerId } })
    }

    return { project, customerName, onSubmit, isEdit }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 600px;
  margin: 0 auto;
}
</style>

