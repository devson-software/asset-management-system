import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useMainStore = defineStore('main', () => {
  // State
  const customers = ref([
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
            {
              id: 'A1',
              unitRef: 'Ac1.01',
              unitType: 'Cassette Unit',
              manufacturer: 'Samsung',
              indoorModel: 'AC200KNHPKH/EU',
              serialNumber: '55896331',
              outdoorModel: 'AC200KXAPKH/EU',
              outdoorSerial: 'OUT-998877',
              vendorArea: 'Office Block A',
              refrigerantType: 'R410A',
              refrigerantKg: '4',
              status: 'Registered',
            },
          ],
        },
        {
          id: 'P6',
          name: 'North Branch',
          vendorLocation: 'Server Room',
          siteAddress: '45 Industrial Way, North Tech',
          assets: [
            {
              id: 'A4',
              unitRef: 'NB-01',
              unitType: 'Midwall Split',
              manufacturer: 'LG',
              indoorModel: 'LG-V3',
              serialNumber: 'SN1122',
              outdoorModel: 'LG-V3-OUT',
              outdoorSerial: 'LG-OS-4433',
              vendorArea: 'Server Wing',
              refrigerantType: 'R32',
              refrigerantKg: '2',
              status: 'Registered',
            },
          ],
        },
      ],
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
            {
              id: 'A2',
              unitRef: 'WH-01',
              unitType: 'Hideaway Unit',
              manufacturer: 'LG',
              indoorModel: 'LG-INDUSTRIAL-V2',
              serialNumber: 'SN992233',
              outdoorModel: 'LG-IND-OUT-V2',
              outdoorSerial: 'LG-OS-9988',
              vendorArea: 'Warehouse Floor',
              refrigerantType: 'R32',
              refrigerantKg: '12',
              status: 'Registered',
            },
          ],
        },
        {
          id: 'P3',
          name: 'Admin Wing',
          vendorLocation: 'Roof Area',
          siteAddress: '789 Terminal Way, Port District',
          assets: [],
        },
      ],
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
          assets: [],
        },
        {
          id: 'P5',
          name: 'Cinema Complex',
          vendorLocation: 'Roof Plant',
          siteAddress: '456 Retail Boulevard, Central',
          assets: [],
        },
      ],
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
          id: 'P6',
          name: 'Operating Theatre 1',
          vendorLocation: 'Clean Room Filter Bank',
          siteAddress: '101 Healthcare Lane, North Side',
          assets: [],
        },
        {
          id: 'P7',
          name: 'Emergency Wing',
          vendorLocation: 'Ground Floor Lobby',
          siteAddress: '101 Healthcare Lane, North Side',
          assets: [],
        },
      ],
    },
  ])

  const today = new Date().toISOString().split('T')[0].replace(/-/g, '/')

  const services = ref([
    {
      id: 'S1',
      unitRef: 'Ac1.01',
      type: 'Maintenance',
      date: '2026/01/30',
      time: '09:00',
      duration: '2 hours',
      teamId: 'T1',
      projectName: 'Downtown Office',
      customer: 'Alpha Corp',
      status: 'Scheduled',
    },
    {
      id: 'S2',
      unitRef: 'WH-01',
      type: 'Repair',
      date: '2026/01/30',
      time: '14:00',
      duration: '4 hours',
      teamId: 'T2',
      projectName: 'Main Warehouse',
      customer: 'Global Logistics Hub',
      status: 'Scheduled',
    },
    {
      id: 'S3',
      unitRef: 'Ac1.01',
      type: 'Monthly Service',
      date: today,
      time: '08:30',
      duration: '2 hours',
      teamId: 'T1',
      projectName: 'Downtown Office',
      customer: 'Alpha Corp',
      status: 'Scheduled',
    },
    {
      id: 'S4',
      unitRef: 'WH-01',
      type: 'Quarterly Service',
      date: today,
      time: '11:00',
      duration: '4 hours',
      teamId: 'T2',
      projectName: 'Main Warehouse',
      customer: 'Global Logistics Hub',
      status: 'In Progress',
    },
    {
      id: 'S5',
      unitRef: 'NB-01',
      type: 'Breakdown Callout',
      date: today,
      time: '15:00',
      duration: '3 hours',
      teamId: 'T1',
      projectName: 'North Branch',
      customer: 'Alpha Corp',
      status: 'Pending',
    },
  ])

  const jobCards = ref([
    {
      id: 'JC1',
      unitRef: 'Ac1.01',
      customer: 'Alpha Corp',
      project: 'Downtown Office',
      date: '2026-01-25',
      technician: 'Mike Johnson',
      issues: ['Unit not cooling properly', 'Strange noise from indoor unit'],
      actions: ['Cleaned filters', 'Checked refrigerant levels', 'Replaced faulty capacitor'],
      status: 'Completed',
    },
  ])

  const commissioningRecords = ref([
    {
      id: 'CR1',
      unitRef: 'Ac1.01',
      customer: 'Alpha Corp',
      project: 'Downtown Office',
      date: '2026-01-20',
      technician: 'Mike Johnson',
      data: {
        voltage: '220V',
        current: '5.2A',
        temperature: '25°C',
        pressure: '35 bar',
      },
    },
  ])

  const users = ref([
    {
      id: 'U1',
      username: 'admin',
      password: 'admin',
      role: 'administrator',
      name: 'Administrator',
    },
    { id: 'U2', username: 'tech1', password: 'tech1', role: 'technician', name: 'Mike Johnson' },
    { id: 'U3', username: 'tech2', password: 'tech2', role: 'technician', name: 'Sarah Davis' },
  ])

  const teams = ref([
    { id: 'T1', name: 'Team Alpha', color: '#1976d2', members: ['U2'] },
    { id: 'T2', name: 'Team Beta', color: '#388e3c', members: ['U3'] },
  ])

  const currentUser = ref({
    id: 'U1',
    username: 'admin',
    role: 'administrator',
    name: 'Administrator',
  })

  const lastDashboardVisit = ref(null)

  // Getters
  const isAdmin = computed(() => currentUser.value?.role === 'administrator')

  const allAssets = computed(() => {
    const assets = []
    customers.value.forEach((c) => {
      c.projects.forEach((p) => {
        p.assets.forEach((a) => {
          const serviceCount = services.value.filter((s) => s.unitRef === a.unitRef).length
          assets.push({
            ...a,
            customerName: c.name,
            projectName: p.name,
            serviceCount: serviceCount,
          })
        })
      })
    })
    return assets
  })

  const totalProjects = computed(() => {
    let count = 0
    customers.value.forEach((c) => {
      count += c.projects.length
    })
    return count
  })

  const todaysServices = computed(() => {
    const today = new Date().toISOString().split('T')[0].replace(/-/g, '/')
    return services.value.filter((s) => s.date === today)
  })

  const welcomeMessage = computed(() => {
    const today = new Date().toDateString()
    const isFirstVisitToday = lastDashboardVisit.value !== today

    if (isFirstVisitToday) {
      const hour = new Date().getHours()
      let greeting = 'Good morning'

      if (hour >= 12 && hour < 17) {
        greeting = 'Good afternoon'
      } else if (hour >= 17) {
        greeting = 'Good evening'
      }

      return `${greeting}, ${currentUser.value?.name || 'User'}!`
    } else {
      return `Welcome back, ${currentUser.value?.name || 'User'}!`
    }
  })

  // Actions
  const getTeamColor = (teamId) => {
    const team = teams.value.find((t) => t.id === teamId)
    return team ? team.color : '#9e9e9e'
  }

  const addCustomer = (customer) => {
    customers.value.push({
      id: 'C' + Date.now(),
      ...customer,
    })
  }

  const updateCustomer = (id, updatedCustomer) => {
    const index = customers.value.findIndex((c) => c.id === id)
    if (index !== -1) {
      customers.value[index] = { ...customers.value[index], ...updatedCustomer }
    }
  }

  const deleteCustomer = (id) => {
    const index = customers.value.findIndex((c) => c.id === id)
    if (index !== -1) {
      customers.value.splice(index, 1)
    }
  }

  const addUser = (user) => {
    users.value.push({
      id: 'U' + Date.now(),
      ...user,
    })
  }

  const updateUser = (id, updatedUser) => {
    const index = users.value.findIndex((u) => u.id === id)
    if (index !== -1) {
      users.value[index] = { ...users.value[index], ...updatedUser }
    }
  }

  const deleteUser = (id) => {
    const index = users.value.findIndex((u) => u.id === id)
    if (index !== -1) {
      users.value.splice(index, 1)
    }
  }

  const addTeam = (team) => {
    teams.value.push({
      id: 'T' + Date.now(),
      ...team,
    })
  }

  const updateTeam = (id, updatedTeam) => {
    const index = teams.value.findIndex((t) => t.id === id)
    if (index !== -1) {
      teams.value[index] = { ...teams.value[index], ...updatedTeam }
    }
  }

  const deleteTeam = (id) => {
    const index = teams.value.findIndex((t) => t.id === id)
    if (index !== -1) {
      teams.value.splice(index, 1)
    }
  }

  const markDashboardVisited = () => {
    lastDashboardVisit.value = new Date().toDateString()
  }

  return {
    // State
    customers,
    services,
    jobCards,
    commissioningRecords,
    users,
    teams,
    currentUser,

    // Getters
    isAdmin,
    allAssets,
    totalProjects,
    todaysServices,
    welcomeMessage,

    // Actions
    getTeamColor,
    addCustomer,
    updateCustomer,
    deleteCustomer,
    addUser,
    updateUser,
    deleteUser,
    addTeam,
    updateTeam,
    deleteTeam,
    markDashboardVisited,
  }
})
