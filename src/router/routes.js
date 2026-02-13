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
      { path: 'dashboard', component: () => import('pages/admin/Dashboard.vue') },
      { path: 'customers', component: () => import('pages/admin/CustomerRegistration.vue') },
      { path: 'projects', component: () => import('pages/admin/Projects.vue') },
      { path: 'projects/add', component: () => import('pages/admin/AddProject.vue') },
      { path: 'assets', component: () => import('pages/admin/Assets.vue') },
      { path: 'assets/add', component: () => import('pages/admin/AddAsset.vue') },
      { path: 'assets/:assetId', component: () => import('pages/admin/AssetDetails.vue') },
      { path: 'assets/:assetId/history', component: () => import('pages/admin/AssetServiceHistory.vue') },
      { path: 'quotations', component: () => import('pages/admin/Quotations.vue') },
      { path: 'customers/add', component: () => import('pages/admin/AddCustomer.vue') },
      { path: 'customers/edit/:customerId', component: () => import('pages/admin/AddCustomer.vue') },
      {
        path: 'customers/:customerId/add-project',
        component: () => import('pages/admin/AddProject.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/edit',
        component: () => import('pages/admin/AddProject.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/add-asset',
        component: () => import('pages/admin/AddAsset.vue'),
      },
      {
        path: 'customers/:customerId/projects/:projectId/assets/:assetId/edit',
        component: () => import('pages/admin/AddAsset.vue'),
      },
      { path: 'project-details', component: () => import('pages/admin/ProjectUnitDetails.vue') },
      { path: 'technical-data/:assetId?', component: () => import('pages/admin/TechnicalDataSheet.vue') },
      { path: 'qr-scanner', component: () => import('pages/QRScanner.vue') },
      { path: 'service-calendar', component: () => import('pages/ServiceCalendar.vue') },
      { path: 'service-entry/:assetId?', component: () => import('pages/ServiceForm.vue') },
      { path: 'job-cards', component: () => import('pages/admin/JobCardHistory.vue') },
      { path: 'job-cards/add', component: () => import('pages/AddJobCard.vue') },
      { path: 'job-cards/edit/:jobId', component: () => import('pages/AddJobCard.vue') },

      // Admin Routes
      { path: 'admin/users', component: () => import('pages/admin/AdminUserManagement.vue') },
      { path: 'admin/users/add', component: () => import('pages/admin/AddUser.vue') },
      { path: 'admin/users/edit/:userId', component: () => import('pages/admin/AddUser.vue') },
      { path: 'admin/teams', component: () => import('pages/admin/TeamManagement.vue') },

      { path: 'commissioning', component: () => import('pages/CommissioningList.vue') },
      {
        path: 'commissioning/new/:type/:assetId?',
        component: () => import('pages/admin/CommissioningForm.vue'),
      },
      {
        path: 'commissioning/view/:recordId',
        component: () => import('pages/admin/CommissioningForm.vue'),
      },
    ],
  },
  {
    path: '/field',
    component: () => import('layouts/FieldLayout.vue'),
    children: [
      { path: '', redirect: '/field/customers' },
      { path: 'customers', component: () => import('pages/technician/FieldCustomers.vue') },
      { path: 'projects', component: () => import('pages/technician/FieldProjects.vue') },
      { path: 'profile', component: () => import('pages/technician/FieldProfile.vue') },
      {
        path: 'projects/:projectId/actions',
        component: () => import('pages/technician/FieldProjectActions.vue'),
      },
      { path: 'service-schedule', component: () => import('pages/ServiceCalendar.vue') },
      { path: 'service-entry/:assetId?', component: () => import('pages/ServiceForm.vue') },
      { path: 'qr-scan', component: () => import('pages/QRScanner.vue') },
      { path: 'job-cards/add', component: () => import('pages/AddJobCard.vue') },
      { path: 'commissioning', component: () => import('pages/CommissioningList.vue') },
    ],
  },
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]

export default routes
