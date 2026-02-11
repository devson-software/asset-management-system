<template>
  <q-page padding class="bg-grey-1">
    <div class="row items-center q-col-gutter-md">
      <div class="col-12">
        <div class="text-h5 text-weight-bold text-primary">Profile</div>
        <div class="text-subtitle2 text-grey-7">Your account details</div>
      </div>
    </div>

    <q-card flat bordered class="rounded-borders bg-white q-mt-md">
      <q-card-section class="row items-center">
        <q-avatar size="64px" class="q-mr-md shadow-1">
          <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
        </q-avatar>
        <div>
          <div class="text-subtitle1 text-weight-bold">
            {{ currentUser.name || currentUser.username || 'Technician' }}
          </div>
          <div class="text-caption text-grey-7">
            {{ currentUser.role || 'technician' }}
          </div>
        </div>
      </q-card-section>
      <q-separator />
      <q-card-section>
        <div class="row q-col-gutter-md">
          <div class="col-12">
            <div class="text-caption text-grey-6">Email</div>
            <div class="text-subtitle2">{{ currentUser.email || 'user@jeramhvac.co.za' }}</div>
          </div>
          <div class="col-12">
            <div class="text-caption text-grey-6">Username</div>
            <div class="text-subtitle2">{{ currentUser.username || 'tech1' }}</div>
          </div>
        </div>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script>
import { defineComponent, computed } from 'vue'
import { useMainStore } from '../stores/main'

export default defineComponent({
  name: 'FieldProfile',
  setup() {
    const store = useMainStore()
    const currentUser = computed(() => store.currentUser || {})

    return { currentUser }
  },
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
