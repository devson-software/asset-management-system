<template>
  <q-page class="login-page flex flex-center" :style="{ backgroundImage: `url(${currentBg})` }">
    <div class="login-overlay"></div>
    <q-card class="login-card shadow-24" flat bordered>
      <q-card-section class="text-center q-pt-xl login-card-section-top">
        <q-avatar
          size="100px"
          font-size="52px"
          color="primary"
          text-color="white"
          icon="fas fa-screwdriver-wrench"
          class="q-mb-md shadow-5 login-avatar"
        />
        <div class="text-h4 text-weight-bolder text-primary q-mb-xs">QR service</div>
        <div class="text-subtitle2 text-grey-7">HVAC Management Tool</div>
      </q-card-section>

      <q-card-section class="q-px-xl q-pb-xl login-card-section-body">
        <q-form @submit="onLogin" class="q-gutter-md">
          <q-input
            v-model="username"
            label="Username"
            outlined
            rounded
            bg-color="white"
            dense
            required
            lazy-rules
            :rules="[ val => val && val.length > 0 || 'Please type something']"
          >
            <template v-slot:prepend>
              <q-icon name="fas fa-user" color="primary" />
            </template>
          </q-input>

          <q-input
            v-model="password"
            type="password"
            label="Password"
            outlined
            rounded
            bg-color="white"
            dense
            required
            lazy-rules
            :rules="[ val => val && val.length > 0 || 'Please type something']"
          >
            <template v-slot:prepend>
              <q-icon name="fas fa-lock" color="primary" />
            </template>
          </q-input>

          <div class="q-mt-lg">
            <q-btn 
              label="Sign In" 
              type="submit" 
              color="primary" 
              rounded 
              size="lg"
              class="full-width text-weight-bold" 
              unelevated
            />
          </div>
        </q-form>
      </q-card-section>

      <q-separator inset />

      <q-card-section class="text-center text-caption q-py-md text-grey-6 login-card-section-footer">
        &copy; 2026 Devson Software - Secure Access Only
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script>
import { defineComponent, ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'LoginPage',
  setup () {
    const router = useRouter()
    const $q = useQuasar()
    const mainStore = useMainStore()
    const username = ref('')
    const password = ref('')

    const images = [
      'https://images.unsplash.com/photo-1621905251918-48416bd8575a?q=80&w=1920',
      'https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?q=80&w=1920',
    ]

    const currentBgIndex = ref(0)
    const currentBg = ref(images[0])
    let interval = null

    onMounted(() => {
      interval = setInterval(() => {
        currentBgIndex.value = (currentBgIndex.value + 1) % images.length
        currentBg.value = images[currentBgIndex.value]
      }, 5000)
    })

    onUnmounted(() => {
      if (interval) clearInterval(interval)
    })

    const onLogin = () => {
      let foundUser = null

      if (username.value === 'admin' && password.value === 'HVAC@Admin2026!') {
        foundUser = mainStore.users.find(u => u.username === 'admin')
      } else if (username.value === 'tech1' && password.value === 'HVAC@Tech2026!') {
        foundUser = mainStore.users.find(u => u.username === 'tech1')
      } else if (username.value === 'tech2' && password.value === 'HVAC@Tech2026!') {
        foundUser = mainStore.users.find(u => u.username === 'tech2')
      } else if (username.value === 'tech' && password.value === 'tech') {
        foundUser = mainStore.users.find(u => u.username === 'tech1')
      }

      if (foundUser) {
        mainStore.currentUser = foundUser
        $q.notify({
          color: 'positive',
          message: `Login successful (${foundUser.role})`,
          icon: foundUser.role === 'administrator' ? 'fas fa-user-shield' : 'fas fa-screwdriver-wrench'
        })
        router.push(foundUser.role === 'technician' ? '/field/customers' : '/dashboard')
      } else {
        $q.notify({
          color: 'negative',
          message: 'Invalid username or password',
          icon: 'fas fa-circle-exclamation'
        })
      }
    }

    return {
      username,
      password,
      currentBg,
      onLogin
    }
  }
})
</script>

<style scoped>
.login-page {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  position: relative;
  transition: background-image 1.5s ease-in-out;
}

.login-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, rgba(26, 54, 93, 0.8) 0%, rgba(42, 67, 101, 0.7) 100%);
  z-index: 1;
}

.login-card {
  width: 100%;
  max-width: 450px;
  border-radius: 20px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  z-index: 2;
  border: none;
  margin: 16px;
}

.full-width {
  width: 100%;
}

@media (max-width: 480px) {
  .login-card {
    max-width: 100%;
    border-radius: 16px;
  }

  .login-card-section-top {
    padding-top: 20px !important;
  }

  .login-card-section-body {
    padding-left: 16px !important;
    padding-right: 16px !important;
    padding-bottom: 20px !important;
  }

  .login-card-section-footer {
    padding-top: 10px !important;
    padding-bottom: 10px !important;
  }

  .login-avatar {
    width: 72px !important;
    height: 72px !important;
    font-size: 38px !important;
  }
}
</style>

