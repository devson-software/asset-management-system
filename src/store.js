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
          timeAllocation: '2 weeks',
          assets: [
            { id: 'A1', unitRef: 'Ac1.01', unitType: 'Cassette Unit', indoorModel: 'AC200KNHPKH/EU', serialNumber: '55896331', refrigerantType: 'R410A', refrigerantKg: '4', status: 'Registered' }
          ]
        },
        {
          id: 'P6',
          name: 'North Branch',
          vendorLocation: 'Server Room',
          siteAddress: '45 Industrial Way, North Tech',
          assets: [
            { id: 'A4', unitRef: 'NB-01', unitType: 'Midwall Split', indoorModel: 'LG-V3', serialNumber: 'SN1122', refrigerantType: 'R32', refrigerantKg: '2', status: 'Registered' }
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
          timeAllocation: '10 days',
          assets: [
            { id: 'A2', unitRef: 'WH-01', unitType: 'Hideaway Unit', indoorModel: 'LG-INDUSTRIAL-V2', serialNumber: 'SN992233', refrigerantType: 'R32', refrigerantKg: '12', status: 'Registered' }
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
            { id: 'A5', unitRef: 'CS-01', unitType: 'Under Ceiling', indoorModel: 'CARRIER-MAX', serialNumber: 'SN5544', refrigerantType: 'R404A', refrigerantKg: '15', status: 'Registered' }
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
          timeAllocation: '3 days',
          assets: [
            { id: 'A3', unitRef: 'FC-AC-01', unitType: 'Cassette Unit', indoorModel: 'DAIKIN-CASSETTE-X', serialNumber: 'DK887711', refrigerantType: 'R410A', refrigerantKg: '2.5', status: 'Registered' }
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
            { id: 'A6', unitRef: 'EW-01', unitType: 'Rooftop Package', indoorModel: 'TRANE-XL', serialNumber: 'SN7788', refrigerantType: 'R410A', refrigerantKg: '5', status: 'Registered' }
          ]
        }
      ]
    }
  ],
  services: [
    { id: 1, date: '2026/01/14', endDate: '2026/01/14', time: '08:00', duration: '2 hours', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service', teamId: 'T1' },
    { id: 2, date: '2026/01/15', endDate: '2026/01/15', time: '09:00', duration: '4 hours', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Quarterly Service', teamId: 'T2' },
    { id: 3, date: '2026/01/15', endDate: '2026/01/15', time: '13:00', duration: '2 hours', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', project: 'Food Court', type: 'Monthly Service', teamId: 'T1' },
    { id: 4, date: '2026/01/16', endDate: '2026/01/16', time: '08:30', duration: '1 day', unitRef: 'EW-01', customer: 'St. Mary\'s Hospital', project: 'Emergency Wing', type: 'Annual Maintenance', teamId: 'T2' },
    { id: 5, date: '2026/01/20', endDate: '2026/02/06', time: '08:00', duration: '2.5 weeks', unitRef: 'NB-01', customer: 'Alpha Corp', project: 'North Branch', type: 'Monthly Service', teamId: 'T1' },
    { id: 6, date: '2026/01/26', endDate: '2026/01/26', time: '08:00', duration: '2 hours', unitRef: 'Ac1.01', customer: 'Alpha Corp', project: 'Downtown Office', type: 'Monthly Service', teamId: 'T1' },
    { id: 7, date: '2026/01/26', endDate: '2026/01/26', time: '10:30', duration: '4 hours', unitRef: 'WH-01', customer: 'Global Logistics Hub', project: 'Main Warehouse', type: 'Quarterly Service', teamId: 'T2' },
    { id: 8, date: '2026/01/26', endDate: '2026/01/26', time: '14:00', duration: '2 hours', unitRef: 'FC-AC-01', customer: 'City Mall Plaza', project: 'Food Court', type: 'Monthly Service', teamId: 'T1' }
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
    { id: 'U1', username: 'admin', fullName: 'System Administrator', email: 'admin@jeramhvac.co.za', role: 'administrator', active: true, invitationStatus: 'Completed' },
    { id: 'U2', username: 'tech1', fullName: 'John Doe', email: 'john@jeramhvac.co.za', role: 'technician', active: true, invitationStatus: 'Completed' },
    { id: 'U3', username: 'tech2', fullName: 'Sarah Jenkins', email: 'sarah@jeramhvac.co.za', role: 'technician', active: true, invitationStatus: 'Pending' },
    { id: 'U4', username: 'client_manager', fullName: 'Alpha Corp Manager', email: 'manager@alphacorp.com', role: 'customer', active: true, invitationStatus: 'Completed' }
  ],
  teams: [
    { id: 'T1', name: 'Team Alpha', color: '#9C27B0', leaderId: 'U2', assistantId: 'U3' },
    { id: 'T2', name: 'Team Beta', color: '#26A69A', leaderId: 'U3', assistantId: 'U2' }
  ],
  serviceDefinitions: {
    'Monthly Service': {
      duration: '2 hours',
      tasks: [
        'Clean filters and return air grilles',
        'Check condensate drain and pump operation',
        'Inspect for unusual noise or vibration',
        'Check controller settings and operation'
      ]
    },
    'Quarterly Service': {
      duration: '4 hours',
      tasks: [
        'All Monthly Service tasks',
        'Check refrigerant pipe insulation',
        'Inspect electrical connections and tighten',
        'Clean outdoor unit coils (brush/vacuum)',
        'Check compressor amperage draw'
      ]
    },
    'Bi-Annual Check': {
      duration: '6 hours',
      tasks: [
        'All Quarterly Service tasks',
        'Deep clean evaporator and condenser coils',
        'Check and record suction/discharge pressures',
        'Test safety controls and cut-outs',
        'Check fan motor bearings'
      ]
    },
    'Annual Maintenance': {
      duration: '1 day',
      tasks: [
        'All Bi-Annual tasks',
        'Chemical clean of all coils',
        'Check for refrigerant leaks (leak test)',
        'Paint/treat any minor corrosion',
        'Full performance verification and report'
      ]
    },
    'Breakdown Callout': {
      duration: '3 hours',
      tasks: [
        'Fault diagnosis and troubleshooting',
        'Identify failed components',
        'Check system operating parameters',
        'Provide repair recommendation/quote'
      ]
    },
    'Installation': {
      duration: '2 days',
      tasks: [
        'Unpack and inspect equipment',
        'Mount indoor and outdoor units',
        'Install refrigerant pipework and insulation',
        'Electrical wiring and control cabling',
        'Pressure test and vacuum dehydrate',
        'Commissioning and performance test'
      ]
    }
  },
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
          status: 'Registered',
          serviceTime: asset.serviceTime || 0,
          vendorAddress: asset.vendorAddress || '',
          vendorLocation: asset.vendorLocation || '',
          vendorArea: asset.vendorArea || ''
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
      pictureUrl: user.pictureUrl || '',
      password: '', // Password will be set during invitation completion
      invitationStatus: 'Pending', // Pending, Completed, Expired
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
  },
  // Team Management Helpers
  addTeam(team) {
    this.teams.push({
      id: 'T' + Date.now(),
      ...team
    })
  },
  updateTeam(id, updatedTeam) {
    const index = this.teams.findIndex(t => t.id === id)
    if (index !== -1) {
      this.teams[index] = { ...this.teams[index], ...updatedTeam }
    }
  },
  deleteTeam(id) {
    const index = this.teams.findIndex(t => t.id === id)
    if (index !== -1) {
      this.teams.splice(index, 1)
    }
  }
})

