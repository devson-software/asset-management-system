<template>
  <q-page padding>
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="bg-primary text-white">
            <div class="row items-center no-wrap">
              <q-icon name="fas fa-user-gear" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">
                  {{ isEdit ? 'Update User Account' : 'Register New User' }}
                </div>
                <div class="text-subtitle2">
                  {{
                    isEdit
                      ? 'Managing system access and permissions'
                      : 'Create a new profile for a technician or administrator'
                  }}
                </div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <div class="row q-col-gutter-md">
                <!-- Profile Picture Upload -->
                <div class="col-12 flex justify-center q-mb-md">
                  <div class="column items-center">
                    <q-avatar size="120px" class="shadow-3 border-primary">
                      <img
                        v-if="userForm.pictureUrl"
                        :source="userForm.pictureUrl"
                        :src="userForm.pictureUrl"
                      />
                      <q-icon v-else name="fas fa-user" color="grey-4" size="64px" />
                      <q-btn
                        round
                        dense
                        color="primary"
                        icon="fas fa-camera"
                        class="absolute-bottom-right"
                        size="sm"
                        @click="$refs.fileInput.pickFiles()"
                      />
                    </q-avatar>
                    <div class="text-caption text-grey-7 q-mt-sm">Profile Picture</div>
                    <q-file
                      ref="fileInput"
                      v-model="userForm.picture"
                      @update:model-value="onFileChange"
                      style="display: none"
                      accept="image/*"
                    />
                  </div>
                </div>

                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="userForm.username"
                    label="Username"
                    outlined
                    dense
                    required
                    :readonly="isEdit"
                    :bg-color="isEdit ? 'grey-1' : 'white'"
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-user-tag" size="xs" color="primary" />
                    </template>
                  </q-input>
                </div>

                <div class="col-12 col-sm-6">
                  <q-select
                    v-model="userForm.role"
                    :options="roleOptions"
                    label="System Role"
                    outlined
                    dense
                    required
                    bg-color="white"
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-user-shield" size="xs" color="secondary" />
                    </template>
                  </q-select>
                </div>

                <div class="col-12 col-sm-6">
                  <q-input
                    v-model="userForm.fullName"
                    label="Full Name"
                    outlined
                    dense
                    required
                    bg-color="white"
                  >
                    <template v-slot:prepend><q-icon name="fas fa-id-card" size="xs" /></template>
                  </q-input>
                </div>

                <div class="col-12 col-sm-6">
                  <div
                    class="q-pa-sm bg-blue-1 rounded-borders border-primary flex items-center full-height"
                  >
                    <q-icon name="fas fa-paper-plane" color="primary" class="q-mr-sm" />
                    <div class="text-caption text-grey-9">
                      An invitation email will be sent to the user to set their password.
                    </div>
                  </div>
                </div>

                <div class="col-12">
                  <q-input
                    v-model="userForm.email"
                    label="Email Address"
                    type="email"
                    outlined
                    dense
                    required
                    bg-color="white"
                  >
                    <template v-slot:prepend><q-icon name="fas fa-envelope" size="xs" /></template>
                  </q-input>
                </div>

                <div class="col-12">
                  <div class="q-pa-md bg-grey-1 rounded-borders border-dashed">
                    <q-toggle
                      v-model="userForm.active"
                      label="User account is currently active"
                      color="positive"
                      class="text-weight-bold"
                    />
                  </div>
                </div>
              </div>

              <div class="row justify-between q-mt-xl">
                <q-btn label="Discard & Return" flat color="grey-7" @click="$router.back()" />
                <q-btn
                  :label="isEdit ? 'Update User Account' : 'Confirm & Send Invitation'"
                  type="submit"
                  color="primary"
                  unelevated
                  class="q-px-lg"
                  icon="fas fa-paper-plane"
                />
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive, ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'AddUser',
  setup() {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()

    const userId = route.params.userId
    const isEdit = !!userId
    const showPassword = ref(false)
    const fileInput = ref(null)

    const userForm = reactive({
      username: '',
      fullName: '',
      email: '',
      role: 'technician',
      password: '',
      active: true,
      picture: null,
      pictureUrl: '',
    })

    const roleOptions = [
      'administrator',
      'Commissioning technician',
      'Engineer',
      'Senior technician',
      'Tier 1 Technician',
      'Tier 2 Technician',
      'customer',
    ]

    const onFileChange = (file) => {
      if (file) {
        userForm.pictureUrl = URL.createObjectURL(file)
      }
    }

    onMounted(() => {
      if (isEdit) {
        const existingUser = store.users.find((u) => u.id === userId)
        if (existingUser) {
          Object.assign(userForm, { ...existingUser })
        }
      }
    })

    const onSubmit = () => {
      if (isEdit) {
        store.updateUser(userId, { ...userForm })
        $q.notify({
          color: 'positive',
          message: 'User account updated successfully',
          icon: 'fas fa-check-circle',
          position: 'top',
        })
      } else {
        // Check if username already exists
        const exists = store.users.some((u) => u.username === userForm.username)
        if (exists) {
          $q.notify({
            color: 'negative',
            message: 'Username already exists',
            icon: 'fas fa-circle-exclamation',
          })
          return
        }

        store.addUser({ ...userForm })
        $q.notify({
          color: 'positive',
          message: 'New user created successfully',
          icon: 'fas fa-check-circle',
          position: 'top',
        })
      }

      // Clear form
      Object.assign(userForm, {
        username: '',
        fullName: '',
        email: '',
        role: 'technician',
        password: '',
        active: true,
        picture: null,
        pictureUrl: '',
      })

      router.push('/admin/users')
    }

    return {
      userForm,
      isEdit,
      onSubmit,
      roleOptions,
      showPassword,
      onFileChange,
      fileInput,
    }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}
</style>
