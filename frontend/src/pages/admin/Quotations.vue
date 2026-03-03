<template>
  <q-page padding>
    <div class="row items-center justify-between q-mb-lg">
      <div>
        <div class="text-h4 text-weight-bold text-primary">Quotations</div>
        <div class="text-subtitle1 text-grey-7">Manage cost estimations and client proposals</div>
      </div>
      <q-btn 
        color="primary" 
        icon="fas fa-file-invoice-dollar" 
        label="Create New Quotation" 
        unelevated 
      />
    </div>

    <q-card flat bordered class="rounded-borders shadow-2 q-pa-xl column items-center justify-center bg-grey-1">
      <q-icon name="fas fa-file-signature" size="64px" color="grey-4" />
      <div class="text-h6 text-grey-5 q-mt-md">Quotation module is coming soon.</div>
      <div class="text-subtitle2 text-grey-4">You'll be able to generate and track professional quotes here.</div>
    </q-card>
  </q-page>
</template>

<script>
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'QuotationsPage'
})
</script>

<style scoped>
.rounded-borders {
  border-radius: 12px;
}
</style>
