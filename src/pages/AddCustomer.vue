<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card shadow-2">
        <q-card-section class="bg-primary text-white">
          <div class="row items-center no-wrap">
            <q-icon name="fas fa-business-time" size="md" class="q-mr-md" />
            <div>
              <div class="text-h5">{{ isEdit ? 'Update Customer' : 'Register New Customer' }}</div>
              <div class="text-subtitle2">{{ isEdit ? 'Editing client profile' : 'Capture client details for the directory' }}</div>
            </div>
          </div>
        </q-card-section>

        <q-card-section class="q-pa-lg">
          <q-form @submit="onSubmit" class="q-gutter-y-lg">
            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.name" 
                  label="Customer Name" 
                  outlined 
                  dense 
                  required 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-building" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.contactName" 
                  label="Primary Contact Name" 
                  outlined 
                  dense 
                  required 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-user-tie" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.email" 
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
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.mobile" 
                  label="Mobile Number" 
                  outlined 
                  dense 
                  required 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-phone-flip" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12">
                <q-input 
                  v-model="customer.address" 
                  label="Business Address" 
                  type="textarea" 
                  outlined 
                  dense 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12">
                <q-input 
                  v-model="customer.vatNumber" 
                  label="VAT Number (Optional)" 
                  outlined 
                  dense 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-file-invoice-dollar" size="xs" /></template>
                </q-input>
              </div>
            </div>

            <div class="row q-gutter-sm justify-between q-mt-xl">
              <q-btn label="Cancel" flat color="grey-7" @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Customer' : 'Confirm & Save Client'" color="primary" type="submit" icon="fas fa-check-circle" class="q-px-md" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>

<script>
import { defineComponent, reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { store } from '../store'
import { useQuasar } from 'quasar'

export default defineComponent({
  name: 'AddCustomer',
  setup () {
    const router = useRouter()
    const route = useRoute()
    const $q = useQuasar()

    const customerId = route.params.customerId
    const isEdit = !!customerId

    const customer = reactive({ name: '', contactName: '', email: '', mobile: '', address: '', vatNumber: '' })

    if (isEdit) {
      const existingCustomer = store.customers.find(c => c.id === customerId)
      if (existingCustomer) {
        Object.assign(customer, existingCustomer)
      }
    }

    const onSubmit = () => {
      if (isEdit) {
        store.updateCustomer(customerId, { ...customer })
        $q.notify({ color: 'positive', message: 'Customer updated successfully' })
      } else {
        store.addCustomer({ ...customer })
        $q.notify({ color: 'positive', message: 'Customer created successfully' })
      }

      // Clear form
      Object.keys(customer).forEach(key => customer[key] = '')

      router.push('/customers')
    }

    return { customer, onSubmit, isEdit }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 600px;
  margin: 0 auto;
}
</style>



