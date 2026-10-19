<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-sitemap me-2 text-primary"></i>
              Organigramme de l'Entreprise
            </h2>
            <div>
              <button class="btn btn-outline-primary me-2" @click="expandAll">
                <i class="fas fa-expand-arrows-alt me-1"></i>
                Tout développer
              </button>
              <button class="btn btn-outline-secondary me-2" @click="collapseAll">
                <i class="fas fa-compress-arrows-alt me-1"></i>
                Tout réduire
              </button>
              <button class="btn btn-primary" @click="loadUsers">
                <i class="fas fa-sync-alt me-2"></i>
                Actualiser
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Statistiques -->
      <div class="row mb-4">
        <div class="col-md-3">
          <div class="stat-card bg-primary text-white">
            <div class="stat-content">
              <div class="stat-number">{{ totalUsers }}</div>
              <div class="stat-label">Total Employés</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-users"></i>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-success text-white">
            <div class="stat-content">
              <div class="stat-number">{{ realManagersCount }}</div>
              <div class="stat-label">Managers avec Équipe</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-user-tie"></i>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-info text-white">
            <div class="stat-content">
              <div class="stat-number">{{ regularEmployeesCount }}</div>
              <div class="stat-label">Employés de Base</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-user"></i>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-warning text-white">
            <div class="stat-content">
              <div class="stat-number">{{ adminCount }}</div>
              <div class="stat-label">Administrateurs</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-user-shield"></i>
            </div>
          </div>
        </div>
      </div>
      
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loading" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Chargement...</span>
                </div>
                <p class="text-muted mt-3">Construction de l'organigramme...</p>
              </div>
              
              <div v-else-if="error" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ error }}
              </div>
              
              <div v-else-if="topLevelUsers.length === 0" class="text-center py-5">
                <i class="fas fa-sitemap fa-4x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune structure hiérarchique définie</h5>
                <p class="text-muted">
                  Créez des utilisateurs et définissez les relations hiérarchiques (manager-employé)
                  <br>
                  pour construire l'organigramme de l'entreprise.
                </p>
                <router-link to="/admin/users" class="btn btn-primary">
                  <i class="fas fa-users me-2"></i>
                  Gérer les Utilisateurs
                </router-link>
              </div>
              
              <div v-else>
                <!-- Instructions -->
                <div class="alert alert-info mb-4">
                  <i class="fas fa-info-circle me-2"></i>
                  <strong>Organigramme Hiérarchique :</strong> 
                  Cliquez sur un employé pour développer/réduire son équipe. 
                  Chaque employé est connecté à son supérieur direct et à ses subordonnés.
                </div>
                
                <!-- Légende -->
                <div class="legend mb-4">
                  <h6 class="mb-3"><i class="fas fa-palette me-2"></i>Légende des Rôles</h6>
                  <div class="d-flex flex-wrap gap-4">
                    <div class="legend-item">
                      <span class="legend-color bg-danger"></span>
                      <span>Directeur/Admin (Top niveau)</span>
                    </div>
                    <div class="legend-item">
                      <span class="legend-color bg-success"></span>
                      <span>Manager (Avec équipe)</span>
                    </div>
                    <div class="legend-item">
                      <span class="legend-color bg-info"></span>
                      <span>Employé (Sans équipe)</span>
                    </div>
                  </div>
                </div>
                
                <!-- Arbre hiérarchique -->
                <div class="organization-tree">
                  <div 
                    v-for="topUser in topLevelUsers" 
                    :key="topUser.id"
                    class="tree-root-branch"
                  >
                    <EmployeeNode 
                      :employee="topUser" 
                      :level="0"
                      :expanded="expandedNodes.has(topUser.id)"
                      :all-users="users"
                      @toggle="toggleNode"
                    />
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

// Composant pour afficher un employé dans l'arbre hiérarchique
const EmployeeNode = {
  name: 'EmployeeNode',
  props: {
    employee: Object,
    level: Number,
    expanded: Boolean,
    allUsers: Array
  },
  emits: ['toggle'],
  setup(props, { emit }) {
    const { employee, level, allUsers } = props
    
    // Calculer les subordonnés directs
    const directSubordinates = computed(() => {
      return allUsers.filter(user => user.managerId === employee.id)
    })
    
    const hasSubordinates = computed(() => directSubordinates.value.length > 0)
    
    // Déterminer le type d'employé basé sur sa position hiérarchique
    const employeeType = computed(() => {
      if (level === 0) return 'director' // Top niveau = Directeur
      if (hasSubordinates.value) return 'manager' // A des subordonnés = Manager
      return 'employee' // Pas de subordonnés = Employé de base
    })
    
    const nodeClass = computed(() => {
      const baseClass = 'employee-node'
      const typeClass = `employee-${employeeType.value}`
      const levelClass = `level-${level}`
      return `${baseClass} ${typeClass} ${levelClass}`
    })
    
    const getEmployeeIcon = (type) => {
      switch (type) {
        case 'director':
          return 'fas fa-crown'
        case 'manager':
          return 'fas fa-user-tie'
        default:
          return 'fas fa-user'
      }
    }
    
    const getEmployeeTypeLabel = (type) => {
      switch (type) {
        case 'director':
          return 'Directeur'
        case 'manager':
          return 'Manager'
        default:
          return 'Employé'
      }
    }
    
    const toggle = () => {
      emit('toggle', employee.id)
    }
    
    return {
      directSubordinates,
      hasSubordinates,
      employeeType,
      nodeClass,
      getEmployeeIcon,
      getEmployeeTypeLabel,
      toggle
    }
  },
  template: `
    <div class="employee-container">
      <!-- Nœud de l'employé -->
      <div :class="nodeClass" @click="toggle">
        <div class="employee-content">
          <div class="employee-header">
            <div class="employee-avatar">
              <i :class="getEmployeeIcon(employeeType)"></i>
            </div>
            <div class="employee-info">
              <h6 class="employee-name mb-1">{{ employee.fullName }}</h6>
              <span class="employee-type badge">{{ getEmployeeTypeLabel(employeeType) }}</span>
              <div class="employee-email">
                <small class="text-muted">
                  <i class="fas fa-envelope me-1"></i>
                  {{ employee.email }}
                </small>
              </div>
            </div>
            <div class="employee-toggle" v-if="hasSubordinates">
              <i v-if="expanded" class="fas fa-chevron-up"></i>
              <i v-else class="fas fa-chevron-down"></i>
            </div>
          </div>
          
          <!-- Info supplémentaires -->
          <div v-if="hasSubordinates" class="team-info">
            <small class="badge bg-secondary">
              {{ directSubordinates.length }} membre{{ directSubordinates.length > 1 ? 's' : '' }} d'équipe
            </small>
          </div>
        </div>
      </div>
      
      <!-- Subordonnés directs -->
      <div v-if="expanded && hasSubordinates" class="subordinates-container">
        <div class="hierarchy-line"></div>
        <div class="subordinates-list">
          <EmployeeNode
            v-for="subordinate in directSubordinates"
            :key="subordinate.id"
            :employee="subordinate"
            :level="level + 1"
            :expanded="$parent.expandedNodes.has(subordinate.id)"
            :all-users="allUsers"
            @toggle="$emit('toggle', $event)"
          />
        </div>
      </div>
    </div>
  `
}

export default {
  name: 'OrganigrammeEntreprise',
  components: {
    AppNavbar,
    EmployeeNode
  },
  setup() {
    const users = ref([])
    const loading = ref(false)
    const error = ref('')
    const expandedNodes = ref(new Set())
    
    // Utilisateurs de niveau supérieur (sans manager ou manager = 0)
    const topLevelUsers = computed(() => {
      return users.value.filter(user => !user.managerId || user.managerId === 0 || user.managerId === null)
    })
    
    const totalUsers = computed(() => users.value.length)
    
    // Compter les vrais managers (qui ont des subordonnés)
    const realManagersCount = computed(() => {
      console.log('Calcul des managers - Tous les utilisateurs:', users.value)
      const managersWithTeam = users.value.filter(user => {
        const hasSubordinates = users.value.some(subordinate => subordinate.managerId === user.id)
        console.log(`Utilisateur ${user.fullName || user.name} (ID: ${user.id}) a des subordonnés:`, hasSubordinates)
        if (hasSubordinates) {
          const subordinates = users.value.filter(sub => sub.managerId === user.id)
          console.log(`  Subordonnés de ${user.fullName || user.name}:`, subordinates.map(s => s.fullName || s.name))
        }
        return hasSubordinates
      })
      console.log('Managers avec équipe:', managersWithTeam.map(m => m.fullName || m.name))
      return managersWithTeam.length
    })
    
    // Compter les employés réguliers (sans équipe)
    const regularEmployeesCount = computed(() => {
      const managersWithTeam = users.value.filter(user => {
        return users.value.some(subordinate => subordinate.managerId === user.id)
      })
      const regularEmployees = users.value.filter(user => {
        const isManager = users.value.some(subordinate => subordinate.managerId === user.id)
        return !isManager
      })
      console.log('Employés sans équipe:', regularEmployees.map(e => e.fullName || e.name))
      return regularEmployees.length
    })
    
    // Compter les administrateurs
    const adminCount = computed(() => {
      const admins = users.value.filter(user => user.role === 'Admin')
      console.log('Administrateurs:', admins.map(a => a.fullName || a.name))
      return admins.length
    })
    
    // Calculer la profondeur maximale de la hiérarchie
    const maxHierarchyDepth = computed(() => {
      const getDepth = (userId, depth = 0) => {
        const subordinates = users.value.filter(u => u.managerId === userId)
        if (subordinates.length === 0) return depth
        return Math.max(...subordinates.map(s => getDepth(s.id, depth + 1)))
      }
      
      if (topLevelUsers.value.length === 0) return 0
      return Math.max(...topLevelUsers.value.map(u => getDepth(u.id, 1)))
    })
    
    const loadUsers = async () => {
      try {
        loading.value = true
        error.value = ''
        const response = await apiService.getUsers()
        users.value = response.data || []
        console.log('=== DEBUGGING ORGANIGRAMME ===')
        console.log('Organigramme - Nombre d\'utilisateurs chargés:', users.value.length)
        console.log('Organigramme - Données complètes des utilisateurs:', users.value)
        
        // Affichage détaillé des relations
        users.value.forEach(user => {
          console.log(`Utilisateur: ${user.fullName || user.name || user.email}`)
          console.log(`  - ID: ${user.id}`)
          console.log(`  - Manager ID: ${user.managerId}`)
          console.log(`  - Rôle: ${user.role}`)
          console.log('  ---')
        })
        
        console.log('Organigramme - Top level users:', topLevelUsers.value)
        
        // Développer automatiquement le premier niveau
        topLevelUsers.value.forEach(user => {
          expandedNodes.value.add(user.id)
        })
        
      } catch (err) {
        console.error('Erreur lors du chargement des utilisateurs:', err)
        error.value = 'Erreur lors du chargement de l\'organigramme'
      } finally {
        loading.value = false
      }
    }
    
    const toggleNode = (userId) => {
      if (expandedNodes.value.has(userId)) {
        expandedNodes.value.delete(userId)
      } else {
        expandedNodes.value.add(userId)
      }
    }
    
    const expandAll = () => {
      users.value.forEach(user => {
        expandedNodes.value.add(user.id)
      })
    }
    
    const collapseAll = () => {
      expandedNodes.value.clear()
    }
    
    onMounted(() => {
      loadUsers()
    })
    
    return {
      users,
      loading,
      error,
      expandedNodes,
      topLevelUsers,
      totalUsers,
      realManagersCount,
      regularEmployeesCount,
      adminCount,
      loadUsers,
      toggleNode,
      expandAll,
      collapseAll
    }
  }
}
</script>

<style scoped>
/* Statistiques */
.stat-card {
  border-radius: 12px;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.stat-content {
  flex: 1;
}

.stat-number {
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.9;
}

.stat-icon {
  font-size: 2.5rem;
  opacity: 0.7;
}

/* Légende */
.legend {
  background: #f8f9fa;
  padding: 15px;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  display: inline-block;
}

/* Arbre organisationnel */
.organization-tree {
  position: relative;
  padding: 20px;
}

.tree-root-branch {
  margin-bottom: 30px;
}

.employee-container {
  position: relative;
  margin-bottom: 15px;
}

/* Nœuds des employés */
.employee-node {
  background: white;
  border: 3px solid #e9ecef;
  border-radius: 12px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  max-width: 400px;
}

.employee-node:hover {
  transform: translateY(-3px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
}

/* Styles par type d'employé */
.employee-director {
  border-color: #dc3545;
  background: linear-gradient(135deg, #fff 0%, #fff5f5 100%);
}

.employee-manager {
  border-color: #198754;
  background: linear-gradient(135deg, #fff 0%, #f8fff9 100%);
}

.employee-employee {
  border-color: #0dcaf0;
  background: linear-gradient(135deg, #fff 0%, #f8feff 100%);
}

/* Contenu du nœud */
.employee-content {
  width: 100%;
}

.employee-header {
  display: flex;
  align-items: center;
  gap: 15px;
}

.employee-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.4rem;
  color: white;
}

.employee-director .employee-avatar {
  background: #dc3545;
}

.employee-manager .employee-avatar {
  background: #198754;
}

.employee-employee .employee-avatar {
  background: #0dcaf0;
}

.employee-info {
  flex: 1;
}

.employee-name {
  font-weight: 700;
  color: #333;
  margin: 0;
  font-size: 1.1rem;
}

.employee-type {
  font-size: 0.8rem;
  padding: 3px 10px;
  border-radius: 15px;
}

.employee-director .employee-type {
  background: #dc3545;
}

.employee-manager .employee-type {
  background: #198754;
}

.employee-employee .employee-type {
  background: #0dcaf0;
}

.employee-email {
  margin-top: 5px;
}

.employee-toggle {
  font-size: 1.4rem;
  color: #6c757d;
}

.team-info {
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px solid #f0f0f0;
}

/* Lignes hiérarchiques */
.subordinates-container {
  position: relative;
  margin-top: 20px;
  margin-left: 40px;
}

.hierarchy-line {
  position: absolute;
  left: -25px;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(to bottom, #007bff, #6c757d);
  border-radius: 2px;
}

.hierarchy-line::before {
  content: '';
  position: absolute;
  left: -10px;
  top: 0;
  width: 23px;
  height: 3px;
  background: #007bff;
  border-radius: 2px;
}

.subordinates-list {
  padding-left: 20px;
}

/* Niveaux hiérarchiques */
.level-0 {
  margin-left: 0;
}

.level-1 {
  margin-left: 20px;
}

.level-2 {
  margin-left: 40px;
}

.level-3 {
  margin-left: 60px;
}

/* Responsive */
@media (max-width: 768px) {
  .employee-node {
    padding: 15px;
    max-width: 100%;
  }
  
  .employee-header {
    gap: 10px;
  }
  
  .employee-avatar {
    width: 40px;
    height: 40px;
    font-size: 1.2rem;
  }
  
  .subordinates-container {
    margin-left: 20px;
  }
  
  .stat-number {
    font-size: 1.5rem;
  }
}
</style>
