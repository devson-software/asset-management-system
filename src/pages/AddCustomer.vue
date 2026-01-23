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
                  <template v-slot:prepend><q-icon name="fas fa-mobile-screen-button" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.telephone" 
                  label="Telephone Number" 
                  outlined 
                  dense 
                  bg-color="white"
                >
                  <template v-slot:prepend><q-icon name="fas fa-phone-flip" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.address" 
                  label="Physical Business Address" 
                  type="textarea" 
                  outlined 
                  dense 
                  bg-color="white"
                  rows="3"
                >
                  <template v-slot:prepend><q-icon name="fas fa-location-dot" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
                <q-input 
                  v-model="customer.billingAddress" 
                  label="Billing Address" 
                  type="textarea" 
                  outlined 
                  dense 
                  bg-color="white"
                  rows="3"
                >
                  <template v-slot:prepend><q-icon name="fas fa-file-invoice" size="xs" /></template>
                </q-input>
              </div>
              <div class="col-12 col-md-6">
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
              <div class="col-12 col-md-6">
                <q-file
                  v-model="customer.picture"
                  label="Customer / Company Picture"
                  outlined
                  dense
                  bg-color="white"
                  accept="image/*"
                  @update:model-value="onFileChange"
                >
                  <template v-slot:prepend>
                    <q-icon name="fas fa-image" size="xs" />
                  </template>
                  <template v-slot:append v-if="customer.picture">
                    <q-avatar size="32px" square>
                      <img :src="customer.pictureUrl || 'https://cdn.quasar.dev/img/avatar.png'">
                    </q-avatar>
                  </template>
                </q-file>
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

    const customer = reactive({ 
      name: '', 
      contactName: '', 
      email: '', 
      mobile: '', 
      telephone: '', 
      address: '', 
      billingAddress: '', 
      vatNumber: '',
      picture: null,
      pictureUrl: null
    })

    if (isEdit) {
      const existingCustomer = store.customers.find(c => c.id === customerId)
      if (existingCustomer) {
        Object.assign(customer, existingCustomer)
      }
    }

    const onFileChange = (file) => {
      if (file) {
        customer.pictureUrl = URL.createObjectURL(file)
      }
    }

    const onSubmit = () => {
      const savedData = { ...customer }
      
      if (isEdit) {
        store.updateCustomer(customerId, savedData)
        $q.notify({ color: 'positive', message: 'Customer updated successfully' })
      } else {
        store.addCustomer(savedData)
        $q.notify({ color: 'positive', message: 'Customer created successfully' })
      }

      // Clear form
      Object.keys(customer).forEach(key => {
        if (key === 'picture') customer[key] = null
        else customer[key] = ''
      })
      customer.pictureUrl = null

      router.push('/customers')
    }

    return { customer, onSubmit, isEdit, onFileChange }
  }
})
</script>

<style scoped>
.form-card {
  max-width: 600px;
  margin: 0 auto;
}
</style>



