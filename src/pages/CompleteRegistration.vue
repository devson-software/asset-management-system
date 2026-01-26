<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <q-page class="flex flex-center bg-blue-grey-1">
        <div class="col-12 col-md-5 q-pa-lg">
          <q-card flat bordered class="rounded-borders shadow-10 bg-white">
            <q-card-section class="bg-primary text-white text-center q-pa-xl">
              <q-avatar size="80px" color="white" class="q-mb-md">
                <q-icon name="fas fa-user-check" color="primary" />
              </q-avatar>
              <div class="text-h4 text-weight-bold">Complete Your Profile</div>
              <div class="text-subtitle1 opacity-80">Welcome to HVAC Management Tool</div>
            </q-card-section>

            <q-card-section class="q-pa-xl">
              <div class="text-center q-mb-lg text-grey-8">
                Hello <span class="text-weight-bold">{{ inviteeName }}</span>, please set your password and profile details to activate your account.
              </div>

              <q-form @submit="onCompleteRegistration" class="q-gutter-y-lg">
                <q-input 
                  v-model="form.password" 
                  label="Create Password" 
                  :type="showPassword ? 'text' : 'password'" 
                  outlined 
                  required
                >
                  <template v-slot:prepend><q-icon name="fas fa-lock" /></template>
                  <template v-slot:append>
                    <q-icon 
                      :name="showPassword ? 'fas fa-eye' : 'fas fa-eye-slash'" 
                      @click="showPassword = !showPassword" 
                      class="cursor-pointer"
                    />
                  </template>
                </q-input>

                <q-input 
                  v-model="form.confirmPassword" 
                  label="Confirm Password" 
                  :type="showPassword ? 'text' : 'password'" 
                  outlined 
                  required
                  :rules="[val => val === form.password || 'Passwords do not match']"
                >
                  <template v-slot:prepend><q-icon name="fas fa-check-double" /></template>
                </q-input>

                <q-separator class="q-my-md" />

                <div class="row items-center q-col-gutter-md">
                  <div class="col-12 col-sm-4 text-center">
                    <q-avatar size="80px" class="shadow-1">
                      <img v-if="form.pictureUrl" :src="form.pictureUrl">
                      <q-icon v-else name="fas fa-camera" color="grey-4" />
                    </q-avatar>
                  </div>
                  <div class="col-12 col-sm-8">
                    <q-file
                      v-model="form.picture"
                      label="Upload Profile Photo"
                      outlined
                      dense
                      accept="image/*"
                      @update:model-value="onFileChange"
                    >
                      <template v-slot:prepend><q-icon name="fas fa-image" /></template>
                    </q-file>
                  </div>
                </div>

                <q-btn 
                  label="Activate Account & Sign In" 
                  type="submit" 
                  color="primary" 
                  class="full-width q-mt-lg" 
                  size="lg" 
                  unelevated
                />
              </q-form>
            </q-card-section>
          </q-card>
          
          <div class="text-center q-mt-md text-grey-6">
            © 2026 HVAC Management Tool - QR Service
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script>
import { defineComponent, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'CompleteRegistration',
  setup () {
    const router = useRouter()
    const $q = useQuasar()
    const showPassword = ref(false)
    const inviteeName = ref('John Doe') // Simulated

    const form = reactive({
      password: '',
      confirmPassword: '',
      picture: null,
      pictureUrl: ''
    })

    const onFileChange = (file) => {
      if (file) {
        form.pictureUrl = URL.createObjectURL(file)
      }
    }

    const onCompleteRegistration = () => {
      $q.notify({
        color: 'positive',
        message: 'Account activated! Redirecting to dashboard...',
        icon: 'fas fa-circle-check'
      })
      setTimeout(() => {
        router.push('/dashboard')
      }, 1500)
    }

    return { form, showPassword, inviteeName, onFileChange, onCompleteRegistration }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 20px;
}
.shadow-10 {
  box-shadow: 0 10px 30px rgba(0,0,0,0.1);
}
</style>
