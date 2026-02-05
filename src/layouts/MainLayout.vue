<template>
  <q-layout view="hHh Lpr lFf">
    <q-header elevated class="bg-white text-grey-9 shadow-1" height-hint="64">
      <q-toolbar class="q-py-sm">
        <q-btn
          flat
          dense
          round
          icon="fas fa-bars"
          aria-label="Menu"
          @click="toggleLeftDrawer"
          class="q-mr-sm"
        />

        <q-toolbar-title class="text-weight-bold row items-center">
          <q-icon name="fas fa-snowflake" color="primary" size="32px" class="q-mr-sm" />
          <span class="gt-xs">HVAC Management Tool</span>
          <span class="xs">QR</span>
        </q-toolbar-title>

        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn round flat icon="fas fa-magnifying-glass" color="grey-7" class="gt-sm">
            <q-tooltip>Search Anything</q-tooltip>
          </q-btn>
          <q-btn round flat icon="fas fa-bell" color="grey-7">
            <q-badge color="negative" floating>2</q-badge>
            <q-tooltip>Notifications</q-tooltip>
          </q-btn>

          <q-separator vertical inset class="q-mx-sm gt-xs" />

          <!-- Dummy Logged In User -->
          <div
            class="row items-center no-wrap cursor-pointer profile-section q-pa-xs rounded-borders"
          >
            <div class="column items-end q-mr-md gt-xs">
              <div class="text-weight-bold text-subtitle2 line-height-1">
                {{ store.currentUser.name || store.currentUser.username }}
              </div>
              <div class="text-caption text-grey-6 line-height-1 text-capitalize">
                {{ store.currentUser.role }}
              </div>
            </div>
            <q-avatar size="40px" class="shadow-1">
              <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
              <q-badge color="positive" floating rounded />
            </q-avatar>

            <q-menu
              auto-close
              transition-show="scale"
              transition-hide="scale"
              class="rounded-borders shadow-2"
            >
              <q-list style="min-width: 200px">
                <q-item class="q-py-md">
                  <q-item-section avatar>
                    <q-avatar size="48px">
                      <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
                    </q-avatar>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label class="text-weight-bold">{{
                      store.currentUser.name || store.currentUser.username
                    }}</q-item-label>
                    <q-item-label caption>{{
                      store.currentUser.email || 'user@jeramhvac.co.za'
                    }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable>
                  <q-item-section avatar
                    ><q-icon name="fas fa-user-gear" color="primary"
                  /></q-item-section>
                  <q-item-section>My Profile</q-item-section>
                </q-item>
                <q-item clickable>
                  <q-item-section avatar
                    ><q-icon name="fas fa-gear" color="grey-7"
                  /></q-item-section>
                  <q-item-section>Account Settings</q-item-section>
                </q-item>
                <q-separator />
                <q-item clickable to="/login" class="text-negative">
                  <q-item-section avatar
                    ><q-icon name="fas fa-arrow-right-from-bracket" color="negative"
                  /></q-item-section>
                  <q-item-section>Sign Out</q-item-section>
                </q-item>
              </q-list>
            </q-menu>
          </div>
        </div>
      </q-toolbar>
    </q-header>

    <NavigationDrawer v-model="leftDrawerOpen" :isAdmin="isAdmin" />

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script>
import { defineComponent, ref, computed } from 'vue'
import { useMainStore } from '../stores/main'
import NavigationDrawer from '../components/EssentialLink.vue'

export default defineComponent({
  name: 'MainLayout',
  components: {
    NavigationDrawer,
  },
  setup() {
    const store = useMainStore()
    const leftDrawerOpen = ref(false)
    const isAdmin = computed(() => store.isAdmin)

    return {
      store,
      leftDrawerOpen,
      isAdmin,
      toggleLeftDrawer() {
        leftDrawerOpen.value = !leftDrawerOpen.value
      },
    }
  },
})
</script>

<style scoped>
.profile-section {
  transition: background 0.3s ease;
}

.profile-section:hover {
  background: #f1f3f4;
}

.line-height-1 {
  line-height: 1.2;
}

.rounded-borders {
  border-radius: 8px;
}
</style>
