<template>
  <q-page padding>
    <div class="row q-col-gutter-lg justify-center">
      <div class="col-12 col-md-8">
        <div class="flex justify-between items-center q-mb-lg">
          <div>
            <div class="text-h4 text-weight-bold text-primary">{{ isEdit ? 'Edit User' : 'Add New User' }}</div>
            <div class="text-subtitle1 text-grey-7">{{ isEdit ? 'Update system access and role' : 'Create a new system user with specific roles' }}</div>
          </div>
          <q-btn flat color="grey-7" icon="fas fa-arrow-left" label="Back to Users" @click="$router.back()" />
        </div>

        <q-card flat bordered class="rounded-borders shadow-2">
          <q-card-section class="q-pa-lg">
            <q-form @submit="onSubmit" class="q-gutter-y-md">
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
                      <q-icon name="fas fa-user" color="primary" />
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
                  >
                    <template v-slot:prepend>
                      <q-icon name="fas fa-user-shield" color="secondary" />
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
                  />
                </div>

                <div class="col-12">
                  <q-input 
                    v-model="userForm.email" 
                    label="Email Address" 
                    type="email" 
                    outlined 
                    dense 
                    required 
                  />
                </div>

                <div class="col-12">
                  <q-toggle 
                    v-model="userForm.active" 
                    label="User Account Active" 
                    color="positive" 
                    class="text-weight-bold"
                  />
                </div>
              </div>

              <div class="row justify-end q-mt-xl q-gutter-sm">
                <q-btn label="Discard" flat color="grey-7" @click="$router.back()" />
                <q-btn 
                  :label="isEdit ? 'Update User Account' : 'Create User Account'" 
                  type="submit" 
                  color="primary" 
                  unelevated 
                  class="q-px-xl" 
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
