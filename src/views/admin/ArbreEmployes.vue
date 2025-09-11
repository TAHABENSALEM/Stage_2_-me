<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-users me-2 text-primary"></i>
              Annuaire des Employés
            </h2>
            <button class="btn btn-primary" @click="loadUsers">
              <i class="fas fa-sync-alt me-2"></i>
              Actualiser
            </button>
          </div>
        </div>
      </div>

      <!-- Barre de recherche -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card">
            <div class="card-body">
              <div class="row">
                <div class="col-md-8">
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="fas fa-search"></i>
                    </span>
                    <input 
                      type="text" 
                      class="form-control" 
                      placeholder="Rechercher par nom, email, rôle..."
                      v-model="searchQuery"
                    >
                  </div>
                </div>
                <div class="col-md-4">
                  <select class="form-select" v-model="filterRole">
                    <option value="">Tous les rôles</option>
                    <option value="Admin">Administrateur</option>
                    <option value="Manager">Manager</option>
                    <option value="Employé">Employé</option>
                  </select>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Statistiques rapides -->
      <div class="row mb-4">
        <div class="col-md-3">
          <div class="card text-center bg-primary text-white">
            <div class="card-body py-3">
              <h4>{{ totalUsers }}</h4>
              <small>Total Employés</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card text-center bg-success text-white">
            <div class="card-body py-3">
              <h4>{{ filteredUsers.length }}</h4>
              <small>Résultats</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card text-center bg-info text-white">
            <div class="card-body py-3">
              <h4>{{ selectedUser ? 1 : 0 }}</h4>
              <small>Sélectionné</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="card text-center bg-warning text-white">
            <div class="card-body py-3">
              <h4>{{ selectedUser ? getSubordinates(selectedUser).length : 0 }}</h4>
              <small>Subordonnés</small>
            </div>
          </div>
        </div>
      </div>
      
      <div class="row">
        <!-- Liste des utilisateurs -->
        <div class="col-md-6">
          <div class="card shadow">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-list me-2"></i>
                Liste des Employés
              </h5>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status"></div>
                <p class="text-muted mt-3">Chargement...</p>
              </div>
              
              <div v-else-if="error" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ error }}
              </div>
              
              <div v-else>
                <div v-if="filteredUsers.length === 0" class="text-center py-4">
                  <i class="fas fa-search fa-3x text-muted mb-3"></i>
                  <p class="text-muted">Aucun employé trouvé avec ces critères</p>
                </div>
                
                <div v-else class="user-list" style="max-height: 500px; overflow-y: auto;">
                  <div 
                    v-for="user in filteredUsers" 
                    :key="user.id"
                    class="user-item p-3 border-bottom cursor-pointer"
                    :class="{ 'selected': selectedUser && selectedUser.id === user.id }"
                    @click="selectUser(user)"
                  >
                    <div class="d-flex align-items-center">
                      <div class="user-avatar me-3">
                        <i class="fas fa-user-circle fa-2x text-secondary"></i>
                      </div>
                      <div class="user-info flex-grow-1">
                        <h6 class="mb-1">{{ getUserName(user) }}</h6>
                        <small class="text-muted d-block">{{ user.email }}</small>
                        <span class="badge mt-1" :class="getRoleBadgeClass(user.role)">
                          {{ user.role }}
                        </span>
                        <span v-if="isManager(user)" class="badge bg-success ms-2">
                          Manager
                        </span>
                      </div>
                      <div class="select-indicator">
                        <i v-if="selectedUser && selectedUser.id === user.id" 
                           class="fas fa-check-circle text-primary fa-lg"></i>
                        <i v-else class="fas fa-circle text-muted"></i>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Détails de l'employé sélectionné -->
        <div class="col-md-6">
          <div class="card shadow">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="fas fa-info-circle me-2"></i>
                Détails de l'Employé
              </h5>
            </div>
            <div class="card-body">
              <div v-if="!selectedUser" class="text-center py-5">
                <i class="fas fa-hand-pointer fa-3x text-muted mb-3"></i>
                <p class="text-muted">Cliquez sur un employé pour voir ses détails</p>
              </div>

              <div v-else>
                <!-- Informations personnelles -->
                <div class="employee-details mb-4">
                  <div class="text-center mb-4">
                    <i class="fas fa-user-circle fa-4x text-primary mb-2"></i>
                    <h4>{{ getUserName(selectedUser) }}</h4>
                    <p class="text-muted">{{ selectedUser.email }}</p>
                    <span class="badge fs-6 px-3 py-2" :class="getRoleBadgeClass(selectedUser.role)">
                      {{ selectedUser.role }}
                    </span>
                  </div>

                  <div class="row">
                    <div class="col-6">
                      <strong>ID:</strong> {{ selectedUser.id }}
                    </div>
                    <div class="col-6">
                      <strong>Manager ID:</strong> {{ selectedUser.managerId || 'Aucun' }}
                    </div>
                  </div>
                </div>

                <!-- Supérieur hiérarchique -->
                <div class="hierarchy-section mb-4">
                  <h6 class="border-bottom pb-2">
                    <i class="fas fa-user-tie me-2 text-success"></i>
                    Supérieur Direct
                  </h6>
                  <div v-if="getManager(selectedUser)">
                    <div class="manager-card p-3 bg-light rounded">
                      <div class="d-flex align-items-center">
                        <i class="fas fa-user-tie fa-2x text-success me-3"></i>
                        <div>
                          <h6 class="mb-1">{{ getUserName(getManager(selectedUser)) }}</h6>
                          <small class="text-muted">{{ getManager(selectedUser).email }}</small>
                          <br>
                          <span class="badge" :class="getRoleBadgeClass(getManager(selectedUser).role)">
                            {{ getManager(selectedUser).role }}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div v-else>
                    <p class="text-muted fst-italic">
                      <i class="fas fa-crown me-1"></i>
                      Cet employé n'a pas de supérieur (Top niveau)
                    </p>
                  </div>
                </div>

                <!-- Subordonnés -->
                <div class="subordinates-section">
                  <h6 class="border-bottom pb-2">
                    <i class="fas fa-users me-2 text-info"></i>
                    Subordonnés Directs ({{ getSubordinates(selectedUser).length }})
                  </h6>
                  <div v-if="getSubordinates(selectedUser).length > 0">
                    <div 
                      v-for="subordinate in getSubordinates(selectedUser)"
                      :key="subordinate.id"
                      class="subordinate-card p-2 mb-2 bg-light rounded cursor-pointer"
                      @click="selectUser(subordinate)"
                    >
                      <div class="d-flex align-items-center">
                        <i class="fas fa-user fa-lg text-info me-3"></i>
                        <div class="flex-grow-1">
                          <h6 class="mb-1">{{ getUserName(subordinate) }}</h6>
                          <small class="text-muted">{{ subordinate.email }}</small>
                          <br>
                          <span class="badge" :class="getRoleBadgeClass(subordinate.role)">
                            {{ subordinate.role }}
                          </span>
                        </div>
                        <i class="fas fa-arrow-right text-muted"></i>
                      </div>
                    </div>
                  </div>
                  <div v-else>
                    <p class="text-muted fst-italic">
                      <i class="fas fa-user me-1"></i>
                      Cet employé n'a pas de subordonnés
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

export default {
  name: 'ArbreEmployes',
  components: {
    AppNavbar
  },
  setup() {
    const users = ref([])
    const loading = ref(false)
    const error = ref('')
    const selectedUser = ref(null)
    const searchQuery = ref('')
    const filterRole = ref('')

    // Utilisateurs filtrés selon la recherche
    const filteredUsers = computed(() => {
      let result = users.value

      // Filtre par rôle
      if (filterRole.value) {
        result = result.filter(user => user.role === filterRole.value)
      }

      // Filtre par recherche de texte
      if (searchQuery.value.trim()) {
        const query = searchQuery.value.toLowerCase().trim()
        result = result.filter(user => {
          const name = getUserName(user).toLowerCase()
          const email = user.email.toLowerCase()
          const role = user.role.toLowerCase()
          return name.includes(query) || email.includes(query) || role.includes(query)
        })
      }

      return result
    })

    // Statistiques
    const totalUsers = computed(() => users.value.length)

    const loadUsers = async () => {
      try {
        loading.value = true
        error.value = ''
        console.log('🔄 Chargement des utilisateurs...')
        
        const response = await apiService.getUsers()
        users.value = response.data || []
        
        console.log('✅ Utilisateurs chargés:', users.value.length)
        console.log('📋 Données complètes:', users.value)
        
        // Auto-sélectionner le premier utilisateur s'il y en a
        if (users.value.length > 0 && !selectedUser.value) {
          selectedUser.value = users.value[0]
        }
        
      } catch (err) {
        console.error('❌ Erreur lors du chargement:', err)
        error.value = `Erreur lors du chargement des employés: ${err.message || 'Erreur inconnue'}`
      } finally {
        loading.value = false
      }
    }

    const selectUser = (user) => {
      selectedUser.value = user
      console.log('👤 Utilisateur sélectionné:', getUserName(user))
      console.log('📊 Manager:', getManager(user) ? getUserName(getManager(user)) : 'Aucun')
      console.log('👥 Subordonnés:', getSubordinates(user).map(s => getUserName(s)))
    }

    const getUserName = (user) => {
      if (!user) return ''
      return user.fullName || user.name || `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email
    }

    const getRoleBadgeClass = (role) => {
      switch (role) {
        case 'Admin': return 'bg-danger text-white'
        case 'Manager': return 'bg-warning text-dark'
        case 'Employé': return 'bg-info text-white'
        default: return 'bg-secondary text-white'
      }
    }

    const getManager = (user) => {
      if (!user || !user.managerId) return null
      return users.value.find(u => u.id === user.managerId) || null
    }

    const getSubordinates = (user) => {
      if (!user) return []
      return users.value.filter(u => u.managerId === user.id)
    }

    const isManager = (user) => {
      return getSubordinates(user).length > 0
    }

    onMounted(() => {
      loadUsers()
    })

    return {
      users,
      loading,
      error,
      selectedUser,
      searchQuery,
      filterRole,
      filteredUsers,
      totalUsers,
      loadUsers,
      selectUser,
      getUserName,
      getRoleBadgeClass,
      getManager,
      getSubordinates,
      isManager
    }
  }
}
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.user-item {
  transition: all 0.2s ease;
  border-radius: 8px;
}

.user-item:hover {
  background-color: #f8f9fa;
  transform: translateX(5px);
}

.user-item.selected {
  background-color: #e3f2fd;
  border-left: 4px solid #2196f3;
}

.user-list {
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.employee-details {
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border-radius: 12px;
  padding: 20px;
  border: 1px solid #e9ecef;
}

.manager-card {
  border: 1px solid #198754;
  transition: all 0.2s ease;
}

.manager-card:hover {
  box-shadow: 0 2px 8px rgba(25, 135, 84, 0.2);
  transform: translateY(-2px);
}

.subordinate-card {
  border: 1px solid #0dcaf0;
  transition: all 0.2s ease;
}

.subordinate-card:hover {
  box-shadow: 0 2px 8px rgba(13, 202, 240, 0.2);
  transform: translateY(-2px);
  background-color: #cff4fc !important;
}

.hierarchy-section h6,
.subordinates-section h6 {
  color: #495057;
  font-weight: 600;
  margin-bottom: 15px;
}

.select-indicator {
  font-size: 1.2rem;
}

.user-avatar {
  font-size: 1.5rem;
}

.card {
  border: none;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.card:hover {
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.input-group-text {
  background-color: #f8f9fa;
  border-color: #ced4da;
}

.badge {
  font-size: 0.75rem;
  padding: 0.375rem 0.75rem;
}

.bg-light {
  background-color: #f8f9fa !important;
}

/* Animation pour la sélection */
@keyframes selectUser {
  0% { transform: scale(1); }
  50% { transform: scale(1.02); }
  100% { transform: scale(1); }
}

.user-item.selected {
  animation: selectUser 0.3s ease;
}

/* Responsive */
@media (max-width: 768px) {
  .container-fluid {
    padding-left: 15px;
    padding-right: 15px;
  }
  
  .user-item {
    padding: 15px !important;
  }
  
  .employee-details {
    padding: 15px;
  }
}
</style>
