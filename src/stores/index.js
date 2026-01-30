import { defineStore } from '#q-app/wrappers'
import { createPinia } from 'pinia'
import { createPersistedState } from 'pinia-plugin-persistedstate'

/*
 * When adding new properties to the store, you should also
 * extend their type in TypeScript/JSDoc. You can find the file at:
 * ./src/types/store.d.ts or ./src/types/store.js (for JSDoc annotations)
 *
 * Learn more: https://pinia.vuejs.org/
 */

export default defineStore((/* { ssrContext } */) => {
  const pinia = createPinia()

  // You can add Pinia plugins here
  pinia.use(createPersistedState())

  return pinia
})
