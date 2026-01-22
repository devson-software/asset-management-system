<template>
  <q-page padding>
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="bg-primary text-white">
            <div class="row items-center no-wrap">
              <q-icon name="fas fa-user-gear" size="md" class="q-mr-md" />
              <div>
                <div class="text-h5">{{ isEdit ? 'Update User Account' : 'Register New User' }}</div>
                <div class="text-subtitle2">{{ isEdit ? 'Managing system access and permissions' : 'Create a new profile for a technician or administrator' }}</div>
              </div>
            </div>
          </q-card-section>

          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-lg">
              <div class="row q-col-gutter-md">
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
                    :options="['administrator', 'technician']" 
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

                <div class="col-12">
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
                  :label="isEdit ? 'Update User Account' : 'Confirm & Create User'" 
                  type="submit" 
                  color="primary" 
                  unelevated 
                  class="q-px-lg" 
                  icon="fas fa-user-check" 
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
import { defineComponent, reactive, computed, ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { store } from '../store'

export default defineComponent({
  name: 'AddUser',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()

    const userId = route.params.userId
    const isEdit = !!userId

    const userForm = reactive({
      username: '',
      fullName: '',
      email: '',
      role: 'technician',
      active: true
    })

    onMounted(() => {
      if (isEdit) {
        const existingUser = store.users.find(u => u.id === userId)
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
          position: 'top'
        })
      } else {
        // Check if username already exists
        const exists = store.users.some(u => u.username === userForm.username)
        if (exists) {
          $q.notify({
            color: 'negative',
            message: 'Username already exists',
            icon: 'fas fa-circle-exclamation'
          })
          return
        }

        store.addUser({ ...userForm })
        $q.notify({
          color: 'positive',
          message: 'New user created successfully',
          icon: 'fas fa-check-circle',
          position: 'top'
        })
      }

      // Clear form
      Object.assign(userForm, {
        username: '',
        fullName: '',
        email: '',
        role: 'technician',
        active: true
      })

      router.push('/admin/users')
    }

    return {
      userForm,
      isEdit,
      onSubmit
    }
  }
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 16px;
}
</style>
