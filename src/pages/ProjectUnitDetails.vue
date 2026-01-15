<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="my-card">
        <q-card-section>
          <div class="text-h5 text-primary q-mb-md">Step 2: Add Unit Details</div>
          <div class="text-subtitle1 q-mb-lg">Project information, AC unit details, and file uploads</div>
        </q-card-section>

        <q-card-section>
          <q-form @submit="onSubmit" class="row q-col-gutter-md">
            <!-- Project Details -->
            <div class="col-12 col-md-6">
              <q-input v-model="project.name" label="Project Name" outlined dense required />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="project.vendorLocation" label="Vendor / Location" outlined dense />
            </div>
            <div class="col-12">
              <q-input v-model="project.siteAddress" label="Address of the Site" type="textarea" outlined dense required />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="project.buildingSection" label="Section of Building" outlined dense />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="project.drawingRef" label="Drawing Reference Number" outlined dense />
            </div>

            <q-separator class="col-12 q-my-lg" />

            <!-- Unit Details -->
            <div class="col-12">
              <div class="text-h6 q-mb-sm">Unit Specification</div>
              <q-select
                v-model="unit.type"
                :options="unitTypes"
                label="Select Type of Unit"
                outlined
                dense
                required
              />
            </div>

            <!-- Uploads -->
            <div class="col-12 col-md-6 q-mt-md">
              <q-file
                v-model="buildingImage"
                label="Upload Picture of Building/Site"
                outlined
                accept="image/*"
                hint="Visual reference for the site"
              >
                <template v-slot:prepend>
                  <q-icon name="photo_camera" />
                </template>
              </q-file>
            </div>

            <div class="col-12 col-md-6 q-mt-md">
              <q-file
                v-model="drawingPdf"
                label="Upload PDF Drawing (A0-A4)"
                outlined
                accept=".pdf"
                hint="Technicians will access this PDF"
              >
                <template v-slot:prepend>
                  <q-icon name="picture_as_pdf" />
                </template>
              </q-file>
            </div>

            <div class="col-12 q-mt-xl">
              <div class="row q-gutter-sm">
                <q-btn label="Back" flat color="grey" @click="$router.back()" />
                <q-btn label="Complete Asset Registration" type="submit" color="primary" icon-right="check_circle" />
              </div>
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'ProjectUnitDetails',
  setup () {
    const router = useRouter()
    const $q = useQuasar()

    const project = reactive({
      name: '',
      siteAddress: '',
      vendorLocation: '',
      buildingSection: '',
      drawingRef: ''
    })

    const unit = reactive({
      type: null
    })

    const unitTypes = [
      'Split System',
      'Multi-Split',
      'VRF/VRV',
      'Chiller',
      'Package Unit',
      'Window Unit'
    ]

    const buildingImage = ref(null)
    const drawingPdf = ref(null)

    const onSubmit = () => {
      // Logic to save unit and project details
      $q.notify({
        color: 'positive',
        message: 'Asset details saved. Next: Technical Data Sheet',
        icon: 'playlist_add_check'
      })
      router.push('/technical-data')
    }

    return {
      project,
      unit,
      unitTypes,
      buildingImage,
      drawingPdf,
      onSubmit
    }
  }
})
</script>

<style scoped>
.my-card {
  max-width: 900px;
  margin: 0 auto;
}
</style>

