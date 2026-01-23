import { reactive } from 'vue'

export const store = reactive({
  customers: [
    {
      id: 'C1',
      name: 'Alpha Corp',
      contactName: 'John Smith',
      email: 'john@alphacorp.com',
      mobile: '+1 555-0101',
      address: '123 Business Ave, Tech City',
      vatNumber: 'VAT123456',
      projects: [
        {
          id: 'P1',
          name: 'Downtown Office',
          vendorLocation: 'Floor 12',
          siteAddress: '123 Business Ave, Tech City',
          assets: [
            { id: 'A1', unitRef: 'Ac1.01', indoorModel: 'AC200KNHPKH/EU', serialNumber: '55896331', refrigerantType: 'R410A', refrigerantKg: '4', status: 'Registered' }
          ]
        },
        {
          id: 'P6',
          name: 'North Branch',
          vendorLocation: 'Server Room',
          siteAddress: '45 Industrial Way, North Tech',
          assets: [
            { id: 'A4', unitRef: 'NB-01', indoorModel: 'LG-V3', serialNumber: 'SN1122', refrigerantType: 'R32', refrigerantKg: '2', status: 'Registered' }
          ]
        }
      ]
    },
    {
      id: 'C2',
      name: 'Global Logistics Hub',
      contactName: 'Sarah Jenkins',
      email: 's.jenkins@globallogistics.com',
      mobile: '+1 555-0202',
      address: '789 Terminal Way, Port District',
      vatNumber: 'VAT987654',
      projects: [
        {
          id: 'P2',
          name: 'Main Warehouse',
          vendorLocation: 'Loading Bay A',
          siteAddress: '789 Terminal Way, Port District',
          assets: [
            { id: 'A2', unitRef: 'WH-01', indoorModel: 'LG-INDUSTRIAL-V2', serialNumber: 'SN992233', refrigerantType: 'R32', refrigerantKg: '12', status: 'Registered' }
          ]
        },
        {
          id: 'P3',
          name: 'Admin Wing',
          vendorLocation: 'Roof Area',
          siteAddress: '789 Terminal Way, Port District',
          assets: []
        },
        {
          id: 'P7',
          name: 'Cold Storage',
          vendorLocation: 'Section B',
          siteAddress: '789 Terminal Way, Port District',
          assets: [
            { id: 'A5', unitRef: 'CS-01', indoorModel: 'CARRIER-MAX', serialNumber: 'SN5544', refrigerantType: 'R404A', refrigerantKg: '15', status: 'Registered' }
          ]
        }
      ]
    },
    {
      id: 'C3',
      name: 'City Mall Plaza',
      contactName: 'Robert Chen',
      email: 'management@citymall.com',
      mobile: '+1 555-0303',
      address: '456 Retail Boulevard, Central',
      vatNumber: 'VAT445566',
      projects: [
        {
          id: 'P4',
          name: 'Food Court',
          vendorLocation: 'Mezzanine Plant Room',
          siteAddress: '456 Retail Boulevard, Central',
          assets: [
            { id: 'A3', unitRef: 'FC-AC-01', indoorModel: 'DAIKIN-CASSETTE-X', serialNumber: 'DK887711', refrigerantType: 'R410A', refrigerantKg: '2.5', status: 'Registered' }
          ]
        }
      ]
    },
    {
      id: 'C4',
      name: 'St. Mary\'s Hospital',
      contactName: 'Dr. Emily White',
      email: 'e.white@hospital.org',
      mobile: '+1 555-0404',
      address: '101 Healthcare Lane, North Side',
      vatNumber: 'VAT112233',
      projects: [
        {
          id: 'P5',
          name: 'Operating Theatre 1',
          vendorLocation: 'Clean Room Filter Bank',
          siteAddress: '101 Healthcare Lane, North Side',
          assets: []
        },
        {
          id: 'P8',
          name: 'Emergency Wing',
          vendorLocation: 'Ground Floor Lobby',
          siteAddress: '101 Healthcare Lane, North Side',
          assets: [
            { id: 'A6', unitRef: 'EW-01', indoorModel: 'TRANE-XL', serialNumber: 'SN7788', refrigerantType: 'R410A', refrigerantKg: '5', status: 'Registered' }
          ]
        }
      ]
    }
  ],
  services: [
    { id: 1, date: '2026/01/14', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service' },
    { id: 2, date: '2026/01/15', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Quarterly Service' },
    { id: 3, date: '2026/01/15', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', project: 'Food Court', type: 'Monthly Service' },
    { id: 4, date: '2026/01/16', unitRef: 'EW-01', customer: 'St. Mary\'s Hospital', project: 'Emergency Wing', type: 'Annual Maintenance' },
    { id: 5, date: '2026/01/20', unitRef: 'NB-01', customer: 'Alpha Corp', project: 'North Branch', type: 'Monthly Service' },
    { id: 6, date: '2026/01/22', unitRef: 'CS-01', customer: 'Global Logistics Hub', project: 'Cold Storage', type: 'Breakdown Callout' },
    { id: 7, date: '2026/01/25', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service' },
    { id: 8, date: '2026/02/01', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service' },
    { id: 9, date: '2026/02/05', unitRef: 'NB-01', customer: 'Alpha Corp', project: 'North Branch', type: 'Quarterly Maintenance' },
    { id: 10, date: '2026/02/10', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Monthly Service' },
    { id: 11, date: '2026/02/15', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', project: 'Food Court', type: 'Filter Replacement' },
    { id: 12, date: '2026/02/20', unitRef: 'EW-01', customer: 'St. Mary\'s Hospital', project: 'Emergency Wing', type: 'Bi-Annual Check' },
    { id: 13, date: '2026/01/21', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service' },
    { id: 14, date: '2026/01/21', unitRef: 'NB-01', customer: 'Alpha Corp', project: 'North Branch', type: 'Emergency Callout' },
    { id: 15, date: '2026/01/21', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Quarterly Maintenance' },
    { id: 16, date: '2026/01/21', unitRef: 'CS-01', customer: 'Global Logistics Hub', project: 'Cold Storage', type: 'Monthly Service' },
    { id: 17, date: '2026/02/25', unitRef: 'NB-01', customer: 'Alpha Corp', project: 'North Branch', type: 'Monthly Service' },
    { id: 18, date: '2026/02/26', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Filter Cleaning' },
    { id: 19, date: '2026/02/27', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Monthly Service' },
    { id: 20, date: '2026/02/28', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', project: 'Food Court', type: 'Quarterly Service' },
    { id: 21, date: '2026/02/14', unitRef: 'EW-01', customer: 'St. Mary\'s Hospital', project: 'Emergency Wing', type: 'Valentine Day Check' }
  ],
  jobCards: [
    { id: 'JOB-2026-001', date: '2026/01/14', unitRef: 'Ac1.01', customer: 'Alpha Corp', tech: 'John Doe', faultFound: false },
    { id: 'JOB-2026-002', date: '2026/01/10', unitRef: 'WH-01', customer: 'Global Logistics Hub', tech: 'Sarah Jenkins', faultFound: true },
    { id: 'JOB-2026-003', date: '2026/01/08', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', tech: 'John Doe', faultFound: false },
    { id: 'JOB-2026-004', date: '2026/01/05', unitRef: 'EW-01', customer: 'St. Mary\'s Hospital', tech: 'Sarah Jenkins', faultFound: false },
    { id: 'JOB-2026-005', date: '2025/12/28', unitRef: 'NB-01', customer: 'Alpha Corp', tech: 'John Doe', faultFound: true },
    { id: 'JOB-2025-154', date: '2025/12/15', unitRef: 'Ac1.01', customer: 'Alpha Corp', tech: 'Sarah Jenkins', faultFound: false }
  ],
  commissioningRecords: [
    { id: 'COMM-001', date: '2026/01/16', unitRef: 'P-01', type: 'Pump', customer: 'Alpha Corp', project: 'Downtown Office', status: 'Completed' },
    { id: 'COMM-002', date: '2026/01/16', unitRef: 'F-01', type: 'Fan', customer: 'Global Logistics Hub', project: 'Main Warehouse', status: 'Completed' },
    { id: 'COMM-003', date: '2026/01/16', unitRef: 'DX-01', type: 'DX Split', customer: 'City Mall Plaza', project: 'Food Court', status: 'In Progress' }
  ],
  users: [
    { id: 'U1', username: 'admin', fullName: 'System Administrator', email: 'admin@jeramhvac.co.za', role: 'administrator', active: true },
    { id: 'U2', username: 'tech1', fullName: 'John Doe', email: 'john@jeramhvac.co.za', role: 'technician', active: true },
    { id: 'U3', username: 'tech2', fullName: 'Sarah Jenkins', email: 'sarah@jeramhvac.co.za', role: 'technician', active: true },
    { id: 'U4', username: 'client_manager', fullName: 'Alpha Corp Manager', email: 'manager@alphacorp.com', role: 'customer', active: true }
  ],
  currentUser: { id: 'U1', username: 'admin', role: 'administrator' }, // Mock logged-in user
  // Helper to add a customer
  addCustomer(customer) {
    this.customers.push({
      id: 'C' + Date.now(),
      ...customer,
      projects: []
    })
  },
  // Helper to update a customer
  updateCustomer(customerId, updatedCustomer) {
    const index = this.customers.findIndex(c => c.id === customerId)
    if (index !== -1) {
      this.customers[index] = { ...this.customers[index], ...updatedCustomer }
    }
  },
  // Helper to add a project to a customer
  addProject(customerId, project) {
    const customer = this.customers.find(c => c.id === customerId)
    if (customer) {
      customer.projects.push({
        id: 'P' + Date.now(),
        ...project,
        assets: []
      })
    }
  },
  // Helper to update a project
  updateProject(customerId, projectId, updatedProject) {
    const customer = this.customers.find(c => c.id === customerId)
    if (customer) {
      const index = customer.projects.findIndex(p => p.id === projectId)
      if (index !== -1) {
        customer.projects[index] = { ...customer.projects[index], ...updatedProject }
      }
    }
  },
  // Helper to add an asset to a project
  addAsset(customerId, projectId, asset) {
    const customer = this.customers.find(c => c.id === customerId)
    if (customer) {
      const project = customer.projects.find(p => p.id === projectId)
      if (project) {
        project.assets.push({
          id: 'A' + Date.now(),
          ...asset,
          status: 'Registered'
        })
      }
    }
  },
  // Helper to update an asset
  updateAsset(customerId, projectId, assetId, updatedAsset) {
    const customer = this.customers.find(c => c.id === customerId)
    if (customer) {
      const project = customer.projects.find(p => p.id === projectId)
      if (project) {
        const index = project.assets.findIndex(a => a.id === assetId)
        if (index !== -1) {
          project.assets[index] = { ...project.assets[index], ...updatedAsset }
        }
      }
    }
  },
  // Helper to add a service
  addService(service) {
    this.services.push({
      id: Date.now(),
      ...service
    })
  },
  // Helper to update a service
  updateService(id, updatedService) {
    const index = this.services.findIndex(s => s.id === id)
    if (index !== -1) {
      this.services[index] = { ...this.services[index], ...updatedService }
    }
  },
  // Helper to delete a service
  deleteService(id) {
    const index = this.services.findIndex(s => s.id === id)
    if (index !== -1) {
      this.services.splice(index, 1)
    }
  },
  // Helper to add a commissioning record
  addCommissioningRecord(record) {
    this.commissioningRecords.push({
      id: 'COMM-' + Date.now(),
      ...record
    })
  },
  // Helper to update a commissioning record
  updateCommissioningRecord(id, updatedRecord) {
    const index = this.commissioningRecords.findIndex(r => r.id === id)
    if (index !== -1) {
      this.commissioningRecords[index] = { ...this.commissioningRecords[index], ...updatedRecord }
    }
  },
  addJobCard(jobCard) {
    this.jobCards.unshift({
      id: 'JOB-' + new Date().getFullYear() + '-' + Math.floor(1000 + Math.random() * 9000),
      ...jobCard
    })
  },
  updateJobCard(id, updatedJobCard) {
    const index = this.jobCards.findIndex(j => j.id === id)
    if (index !== -1) {
      this.jobCards[index] = { ...this.jobCards[index], ...updatedJobCard }
    }
  },
  // User Management Helpers
  addUser(user) {
    this.users.push({
      id: 'U' + Date.now(),
      active: true,
      ...user
    })
  },
  updateUser(id, updatedUser) {
    const index = this.users.findIndex(u => u.id === id)
    if (index !== -1) {
      this.users[index] = { ...this.users[index], ...updatedUser }
    }
  },
  deleteUser(id) {
    const index = this.users.findIndex(u => u.id === id)
    if (index !== -1) {
      this.users.splice(index, 1)
    }
  }
})

