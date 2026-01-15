<template>
  <q-page padding>
    <div class="q-pa-md">
      <q-card flat bordered class="form-card">
        <q-card-section>
          <div class="text-h5 text-primary">{{ isEdit ? 'Edit Customer' : 'Add New Customer' }}</div>
          <div class="text-subtitle2 text-grey-7">{{ isEdit ? 'Update client details' : 'Step 1: Register a new client in the system' }}</div>
        </q-card-section>

        <q-card-section>
          <q-form @submit="onSubmit" class="q-gutter-md">
            <q-input v-model="customer.name" label="Customer Name" outlined dense required />
            <q-input v-model="customer.contactName" label="Contact Name" outlined dense required />
            <q-input v-model="customer.email" label="Email" type="email" outlined dense required />
            <q-input v-model="customer.mobile" label="Mobile Number" outlined dense required />
            <q-input v-model="customer.address" label="Business Address" type="textarea" outlined dense />
            <q-input v-model="customer.vatNumber" label="VAT Number" outlined dense />

            <div class="row q-gutter-sm justify-end q-mt-lg">
              <q-btn label="Cancel" flat @click="$router.back()" />
              <q-btn :label="isEdit ? 'Update Customer' : 'Create Customer'" color="primary" type="submit" />
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



