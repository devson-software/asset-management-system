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

              <div class="col-12 col-md-6">
                <q-input 
                  v-model="project.timeAllocation" 
                  label="Time Allocation" 
                  outlined 
                  dense 
                  placeholder="e.g. 2 weeks, 3 days"
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-clock" size="xs" /></template>
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
                  rows="3"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                </q-input>
              </div>

              <!-- Project Drawings (PDF) -->
              <div class="col-12">
                <div class="text-subtitle1 text-weight-bold row items-center q-mb-sm">
                  <q-icon name="fas fa-file-pdf" color="red-7" class="q-mr-sm" />
                  Project Drawings (PDF, A0–A3)
                </div>
                <div class="text-caption text-grey-7 q-mb-sm">
                  Upload one or more project drawings so technicians can access them on their phones.
                </div>
                <div class="q-gutter-sm">
                  <div
                    v-for="(drawing, index) in project.drawings"
                    :key="index"
                    class="q-pa-sm bg-grey-1 rounded-borders row items-start q-col-gutter-sm"
                  >
                    <div class="col-12 col-md-5">
                      <q-input
                        v-model="drawing.title"
                        label="Drawing Reference / Title"
                        outlined
                        dense
                        bg-color="white"
                        placeholder="e.g. A0 – Roof Plan, A1 – AHU Layout"
                      />
                    </div>
                    <div class="col-12 col-md-6">
                      <q-file
                        v-model="drawing.file"
                        label="Upload Drawing (PDF)"
                        outlined
                        dense
                        accept="application/pdf"
                        bg-color="white"
                      >
                        <template v-slot:prepend>
                          <q-icon name="fas fa-file-pdf" color="red-7" />
                        </template>
                      </q-file>
                      <div v-if="drawing.fileName" class="text-caption text-grey-7 q-mt-xs">
                        Saved as: {{ drawing.fileName }}
                      </div>
                    </div>
                    <div class="col-auto">
                      <q-btn
                        v-if="project.drawings.length > 1"
                        flat
                        dense
                        round
                        color="negative"
                        icon="fas fa-trash-can"
                        @click="removeDrawing(index)"
                      />
                    </div>
                  </div>
                  <q-btn
                    color="secondary"
                    outline
                    icon="fas fa-plus"
                    label="Add Drawing"
                    @click="addDrawing"
                  />
                </div>
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
import { store } from '../../store'
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
      pictureUrl: null,
      timeAllocation: '',
      drawings: [],
    })

    if (isEdit && customerId) {
      const targetCustomer = store.customers.find(c => c.id === customerId)
      if (targetCustomer) {
        const existingProject = targetCustomer.projects.find(p => p.id === projectId)
        if (existingProject) {
          Object.assign(project, existingProject)
          // Normalise drawings so UI has fileName field even without File objects
          if (existingProject.drawings && Array.isArray(existingProject.drawings)) {
            project.drawings = existingProject.drawings.map((d) => ({
              title: d.title || '',
              file: null,
              fileName: d.fileName || '',
            }))
          }
        }
      }
    }

    const onFileChange = (file) => {
      if (file) {
        project.pictureUrl = URL.createObjectURL(file)
      }
    }

    const addDrawing = () => {
      project.drawings.push({
        title: '',
        file: null,
        fileName: '',
      })
    }

    const removeDrawing = (index) => {
      project.drawings.splice(index, 1)
    }

    const onSubmit = () => {
      const finalCustomerId = customerId || project.customerId
      
      if (!finalCustomerId) {
        $q.notify({ color: 'negative', message: 'Please select a customer' })
        return
      }

      const savedData = { ...project }
      delete savedData.picture // Don't store the File object in our simple store for now
      // Map drawings to a serializable format (reference + file name only)
      savedData.drawings = (project.drawings || []).map((d) => ({
        title: d.title || '',
        fileName: d.file?.name || d.fileName || '',
      }))

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

    return {
      project,
      customerName,
      onSubmit,
      isEdit,
      isStandaloneAdd,
      customerOptions,
      onFileChange,
      addDrawing,
      removeDrawing,
    }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 800px;
  margin: 0 auto;
}
</style>

