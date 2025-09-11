<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-sitemap me-2 text-info"></i>
              Organigramme de l'Entreprise
            </h2>
            <button class="btn btn-info" @click="refreshData">
              <i class="fas fa-sync-alt me-2"></i>
              Actualiser
            </button>
          </div>
        </div>
      </div>

      <!-- Barre de recherche -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-search me-2"></i>
                Rechercher un employé
              </h5>
            </div>
            <div class="card-body">
              <div class="input-group">
                <span class="input-group-text">
                  <i class="fas fa-search"></i>
                </span>
                <input
                  type="text"
                  class="form-control"
                  placeholder="Tapez le nom ou prénom de l'employé..."
                  v-model="searchTerm"
                  @input="filterEmployees"
                >
                <button 
                  class="btn btn-outline-secondary" 
                  @click="clearSearch"
                  :disabled="!searchTerm"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Liste des employés -->
      <div class="row">
        <div class="col-lg-6">
          <div class="card shadow">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-users me-2"></i>
                Liste des Employés ({{ filteredEmployees.length }})
              </h5>
            </div>
            <div class="card-body" style="max-height: 500px; overflow-y: auto;">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement...</p>
              </div>
              
              <div v-else-if="filteredEmployees.length === 0" class="text-center py-4">
                <i class="fas fa-user-slash fa-2x text-muted mb-3"></i>
                <p class="text-muted">Aucun employé trouvé</p>
              </div>
              
              <div v-else>
                <div 
                  v-for="employee in filteredEmployees" 
                  :key="employee.id"
                  class="employee-item p-3 mb-2 border rounded cursor-pointer"
                  :class="{ 'selected': selectedEmployee?.id === employee.id }"
                  @click="selectEmployee(employee)"
                >
                  <div class="d-flex align-items-center">
                    <div class="employee-avatar me-3">
                      <i class="fas fa-user-circle fa-2x" :class="getRoleColor(employee.role)"></i>
                    </div>
                    <div class="flex-grow-1">
                      <h6 class="mb-1">{{ employee.firstName }} {{ employee.lastName }}</h6>
                      <small class="text-muted">
                        <i class="fas fa-briefcase me-1"></i>
                        {{ employee.role }}
                      </small>
                      <br>
                      <small class="text-muted">
                        <i class="fas fa-envelope me-1"></i>
                        {{ employee.email }}
                      </small>
                    </div>
                    <div class="text-end">
                      <span class="badge" :class="getRoleBadgeClass(employee.role)">
                        {{ employee.role }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Détails de l'employé sélectionné -->
        <div class="col-lg-6">
          <div class="card shadow">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-user-check me-2"></i>
                Détails Hiérarchiques
              </h5>
            </div>
            <div class="card-body">
              <div v-if="!selectedEmployee" class="text-center py-5">
                <i class="fas fa-hand-pointer fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Sélectionnez un employé</h5>
                <p class="text-muted">Cliquez sur un employé dans la liste pour voir ses relations hiérarchiques</p>
              </div>
              
              <div v-else>
                <!-- Informations de l'employé sélectionné -->
                <div class="selected-employee-info mb-4 p-3 bg-light rounded">
                  <div class="d-flex align-items-center">
                    <i class="fas fa-user-circle fa-3x me-3" :class="getRoleColor(selectedEmployee.role)"></i>
                    <div>
                      <h5 class="mb-1">{{ selectedEmployee.firstName }} {{ selectedEmployee.lastName }}</h5>
                      <p class="mb-1">
                        <span class="badge" :class="getRoleBadgeClass(selectedEmployee.role)">
                          {{ selectedEmployee.role }}
                        </span>
                      </p>
                      <small class="text-muted">{{ selectedEmployee.email }}</small>
                    </div>
                  </div>
                </div>

                <!-- Supérieur direct -->
                <div class="mb-4">
                  <h6 class="text-primary">
                    <i class="fas fa-arrow-up me-2"></i>
                    Supérieur Direct
                  </h6>
                  <div v-if="selectedEmployeeManager" class="manager-card p-3 border rounded bg-primary bg-opacity-10">
                    <div class="d-flex align-items-center">
                      <i class="fas fa-user-tie fa-2x text-primary me-3"></i>
                      <div>
                        <h6 class="mb-1">{{ selectedEmployeeManager.firstName }} {{ selectedEmployeeManager.lastName }}</h6>
                        <small class="text-muted">
                          <i class="fas fa-briefcase me-1"></i>
                          {{ selectedEmployeeManager.role }}
                        </small>
                        <br>
                        <small class="text-muted">
                          <i class="fas fa-envelope me-1"></i>
                          {{ selectedEmployeeManager.email }}
                        </small>
                      </div>
                    </div>
                  </div>
                  <div v-else class="text-muted p-3 border rounded bg-light">
                    <i class="fas fa-crown me-2"></i>
                    Aucun supérieur (Top niveau)
                  </div>
                </div>

                <!-- Subordonnés directs -->
                <div>
                  <h6 class="text-success">
                    <i class="fas fa-arrow-down me-2"></i>
                    Subordonnés Directs ({{ selectedEmployeeSubordinates.length }})
                  </h6>
                  <div v-if="selectedEmployeeSubordinates.length > 0">
                    <div 
                      v-for="subordinate in selectedEmployeeSubordinates" 
                      :key="subordinate.id"
                      class="subordinate-card p-3 mb-2 border rounded bg-success bg-opacity-10"
                    >
                      <div class="d-flex align-items-center">
                        <i class="fas fa-user fa-2x text-success me-3"></i>
                        <div>
                          <h6 class="mb-1">{{ subordinate.firstName }} {{ subordinate.lastName }}</h6>
                          <small class="text-muted">
                            <i class="fas fa-briefcase me-1"></i>
                            {{ subordinate.role }}
                          </small>
                          <br>
                          <small class="text-muted">
                            <i class="fas fa-envelope me-1"></i>
                            {{ subordinate.email }}
                          </small>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div v-else class="text-muted p-3 border rounded bg-light">
                    <i class="fas fa-user-slash me-2"></i>
                    Aucun subordonné
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Statistiques -->
      <div class="row mt-4">
        <div class="col-md-3">
          <div class="stat-card bg-primary text-white">
            <div class="stat-icon">
              <i class="fas fa-users"></i>
            </div>
            <div class="stat-content">
              <h3>{{ users.length }}</h3>
              <p>Total Employés</p>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-success text-white">
            <div class="stat-icon">
              <i class="fas fa-user-tie"></i>
            </div>
            <div class="stat-content">
              <h3>{{ managers.length }}</h3>
              <p>Managers</p>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-warning text-white">
            <div class="stat-icon">
              <i class="fas fa-user-cog"></i>
            </div>
            <div class="stat-content">
              <h3>{{ admins.length }}</h3>
              <p>Administrateurs</p>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-info text-white">
            <div class="stat-icon">
              <i class="fas fa-building"></i>
            </div>
            <div class="stat-content">
              <h3>{{ getHierarchyLevels() }}</h3>
              <p>Niveaux Hiérarchiques</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

// State
const loading = ref(true)
const users = ref([])
const searchTerm = ref('')
const selectedEmployee = ref(null)

// Computed properties
const filteredEmployees = computed(() => {
  if (!searchTerm.value) {
    return users.value
  }
  
  const term = searchTerm.value.toLowerCase()
  return users.value.filter(user => 
    user.firstName.toLowerCase().includes(term) || 
    user.lastName.toLowerCase().includes(term) ||
    user.email.toLowerCase().includes(term)
  )
})

const employees = computed(() => 
  users.value.filter(user => user.role === 'Employé')
)

const managers = computed(() => 
  users.value.filter(user => {
    // Un manager est soit quelqu'un avec le rôle "Manager", soit quelqu'un qui a des subordonnés
    return user.role === 'Manager' || 
           users.value.some(subordinate => subordinate.managerId === user.id)
  })
)

const admins = computed(() => 
  users.value.filter(user => user.role === 'Admin')
)

const selectedEmployeeManager = computed(() => {
  if (!selectedEmployee.value || !selectedEmployee.value.managerId) {
    return null
  }
  return users.value.find(user => user.id === selectedEmployee.value.managerId)
})

const selectedEmployeeSubordinates = computed(() => {
  if (!selectedEmployee.value) {
    return []
  }
  return users.value.filter(user => user.managerId === selectedEmployee.value.id)
})

// Methods
const fetchUsers = async () => {
  try {
    loading.value = true
    const response = await apiService.getUsers()
    users.value = response.data || []
    console.log('Users récupérés:', users.value)
    console.log('Roles trouvés:', users.value.map(u => u.role))
    console.log('Relations managerId:', users.value.map(u => ({ name: u.firstName + ' ' + u.lastName, managerId: u.managerId })))
  } catch (error) {
    console.error('Erreur lors de la récupération des utilisateurs:', error)
  } finally {
    loading.value = false
  }
}

const refreshData = () => {
  selectedEmployee.value = null
  searchTerm.value = ''
  fetchUsers()
}

const filterEmployees = () => {
  // La filtration se fait automatiquement via computed
  selectedEmployee.value = null
}

const clearSearch = () => {
  searchTerm.value = ''
  selectedEmployee.value = null
}

const selectEmployee = (employee) => {
  selectedEmployee.value = employee
}

const getRoleColor = (role) => {
  switch (role) {
    case 'Admin':
      return 'text-warning'
    case 'Manager':
      return 'text-success'
    case 'Employé':
      return 'text-primary'
    default:
      return 'text-muted'
  }
}

const getRoleBadgeClass = (role) => {
  switch (role) {
    case 'Admin':
      return 'bg-warning text-dark'
    case 'Manager':
      return 'bg-success'
    case 'Employé':
      return 'bg-primary'
    default:
      return 'bg-secondary'
  }
}

const getHierarchyLevels = () => {
  const levels = new Set()
  
  const calculateLevel = (user, level = 0) => {
    levels.add(level)
    const subordinates = users.value.filter(u => u.managerId === user.id)
    subordinates.forEach(sub => calculateLevel(sub, level + 1))
  }
  
  const topLevel = users.value.filter(user => !user.managerId)
  topLevel.forEach(user => calculateLevel(user))
  
  return levels.size
}

// Lifecycle
onMounted(() => {
  fetchUsers()
})
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.employee-item {
  transition: all 0.3s ease;
  border: 2px solid #e9ecef !important;
}

.employee-item:hover {
  border-color: #007bff !important;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.employee-item.selected {
  border-color: #007bff !important;
  background-color: #f8f9fa;
  box-shadow: 0 4px 8px rgba(0,123,255,0.2);
}

.stat-card {
  border-radius: 10px;
  padding: 20px;
  text-align: center;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
  transition: transform 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
}

.stat-icon {
  font-size: 2.5rem;
  margin-bottom: 10px;
}

.stat-content h3 {
  font-size: 2rem;
  margin-bottom: 5px;
  font-weight: bold;
}

.stat-content p {
  margin-bottom: 0;
  font-size: 0.9rem;
  opacity: 0.9;
}

.manager-card, .subordinate-card {
  transition: transform 0.2s ease;
}

.manager-card:hover, .subordinate-card:hover {
  transform: translateX(5px);
}

.selected-employee-info {
  border-left: 4px solid #007bff;
}
</style>
