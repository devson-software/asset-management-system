const routes = [
  {
    path: '/login',
    component: () => import('layouts/AuthLayout.vue'),
    children: [{ path: '', component: () => import('pages/LoginPage.vue') }],
  },
  {
    path: '/complete-registration',
    component: () => import('pages/CompleteRegistration.vue'),
  },
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', redirect: '/login' },
      { path: 'dashboard', component: () => import('pages/Dashboard.vue') },
      { path: 'customers', component: () => import('pages/CustomerRegistration.vue') },
      { path: 'projects', component: () => import('pages/Projects.vue') },
      { path: 'projects/add', component: () => import('pages/AddProject.vue') },
      { path: 'assets', component: () => import('pages/Assets.vue') },
      { path: 'assets/add', component: () => import('pages/AddAsset.vue') },
      { path: 'assets/:assetId', component: () => import('pages/AssetDetails.vue') },
      { path: 'assets/:assetId/history', component: () => import('pages/AssetServiceHistory.vue') },
      { path: 'quotations', component: () => import('pages/Quotations.vue') },
      { path: 'customers/add', component: () => import('pages/AddCustomer.vue') },
      { path: 'customers/edit/:customerId', component: () => import('pages/AddCustomer.vue') },
      {
        path: 'customers/:customerId/add-project',
        component: () => import('pages/AddProject.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/edit',
        component: () => import('pages/AddProject.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/add-asset',
        component: () => import('pages/AddAsset.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/assets/:assetId/edit',
        component: () => import('pages/AddAsset.vue'),
      },
      { path: 'project-details', component: () => import('pages/ProjectUnitDetails.vue') },
      { path: 'technical-data/:assetId?', component: () => import('pages/TechnicalDataSheet.vue') },
      { path: 'qr-scanner', component: () => import('pages/QRScanner.vue') },
      { path: 'service-calendar', component: () => import('pages/ServiceCalendar.vue') },
      { path: 'service-entry/:assetId?', component: () => import('pages/ServiceForm.vue') },
      { path: 'job-cards', component: () => import('pages/JobCardHistory.vue') },
      { path: 'job-cards/add', component: () => import('pages/AddJobCard.vue') },
      { path: 'job-cards/edit/:jobId', component: () => import('pages/AddJobCard.vue') },

      // Admin Routes
      { path: 'admin/users', component: () => import('pages/AdminUserManagement.vue') },
      { path: 'admin/users/add', component: () => import('pages/AddUser.vue') },
      { path: 'admin/users/edit/:userId', component: () => import('pages/AddUser.vue') },
      { path: 'admin/teams', component: () => import('pages/TeamManagement.vue') },

      { path: 'commissioning', component: () => import('pages/CommissioningList.vue') },
      {
        path: 'commissioning/new/:type/:assetId?',
        component: () => import('pages/CommissioningForm.vue'),
      },
      {
        path: 'commissioning/view/:recordId',
        component: () => import('pages/CommissioningForm.vue'),
      },
    ],
  },
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]

export default routes
