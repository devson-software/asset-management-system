const routes = [
  {
    path: '/login',
    component: () => import('layouts/AuthLayout.vue'),
    children: [
      { path: '', component: () => import('pages/LoginPage.vue') }
    ]
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', redirect: '/login' },
      { path: 'dashboard', component: () => import('pages/Dashboard.vue') },
      { path: 'customers', component: () => import('pages/CustomerRegistration.vue') },
      { path: 'projects', component: () => import('pages/Projects.vue') },
      { path: 'assets', component: () => import('pages/Assets.vue') },
      { path: 'assets/:assetId', component: () => import('pages/AssetDetails.vue') },
      { path: 'customers/add', component: () => import('pages/AddCustomer.vue') },
      { path: 'customers/edit/:customerId', component: () => import('pages/AddCustomer.vue') },
      { path: 'customers/:customerId/add-project', component: () => import('pages/AddProject.vue') },
      { path: 'customers/:customerId/projects/:projectId/edit', component: () => import('pages/AddProject.vue') },
      { path: 'customers/:customerId/projects/:projectId/add-asset', component: () => import('pages/AddAsset.vue') },
      { path: 'customers/:customerId/projects/:projectId/assets/:assetId/edit', component: () => import('pages/AddAsset.vue') },
      { path: 'project-details', component: () => import('pages/ProjectUnitDetails.vue') },
      { path: 'technical-data/:assetId?', component: () => import('pages/TechnicalDataSheet.vue') },
      { path: 'service-calendar', component: () => import('pages/ServiceCalendar.vue') },
      { path: 'service-entry/:assetId?', component: () => import('pages/ServiceForm.vue') },
      { path: 'job-cards', component: () => import('pages/JobCardHistory.vue') }
    ]
  },
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue')
  }
]

export default routes
