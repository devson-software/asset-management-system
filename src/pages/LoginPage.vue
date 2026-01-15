<template>
  <q-page class="flex flex-center bg-grey-1">
    <q-card style="width: 100%; max-width: 400px;" flat bordered>
      <q-card-section class="text-center">
        <div class="text-h5 text-primary q-mb-xs">Technician Portal</div>
        <div class="text-caption text-grey-7">Log in to access drawings and unit details</div>
      </q-card-section>

      <q-card-section>
        <q-form @submit="onLogin" class="q-gutter-md">
          <q-input
            v-model="username"
            label="Username"
            outlined
            dense
            required
            lazy-rules
            :rules="[ val => val && val.length > 0 || 'Please type something']"
          >
            <template v-slot:prepend>
              <q-icon name="person" />
            </template>
          </q-input>

          <q-input
            v-model="password"
            type="password"
            label="Password"
            outlined
            dense
            required
            lazy-rules
            :rules="[ val => val && val.length > 0 || 'Please type something']"
          >
            <template v-slot:prepend>
              <q-icon name="lock" />
            </template>
          </q-input>

          <div>
            <q-btn label="Log In" type="submit" color="primary" class="full-width" />
          </div>
        </q-form>
      </q-card-section>

      <q-separator />

      <q-card-section class="bg-grey-2 text-center text-caption">
        Access is restricted to authorized service technicians.
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script>
import { defineComponent, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'LoginPage',
  setup () {
    const router = useRouter()
    const $q = useQuasar()
    const username = ref('')
    const password = ref('')

    const onLogin = () => {
      // Mock login
      $q.notify({
        color: 'positive',
        message: 'Login successful',
        icon: 'lock_open'
      })
      router.push('/')
    }

    return {
      username,
      password,
      onLogin
    }
  }
})
</script>

<style scoped>
.full-width {
  width: 100%;
}
</style>

