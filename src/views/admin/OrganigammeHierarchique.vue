<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-sitemap me-2 text-primary"></i>
              Organigramme Hiérarchique
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
              <div class="stat-number">{{ managersCount }}</div>
              <div class="stat-label">Managers</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-user-tie"></i>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-info text-white">
            <div class="stat-content">
              <div class="stat-number">{{ employeesCount }}</div>
              <div class="stat-label">Employés</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-user"></i>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="stat-card bg-warning text-white">
            <div class="stat-content">
              <div class="stat-number">{{ maxHierarchyDepth }}</div>
              <div class="stat-label">Niveaux Hiérarchiques</div>
            </div>
            <div class="stat-icon">
              <i class="fas fa-layer-group"></i>
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
                <p class="text-muted mt-3">Chargement de l'organigramme...</p>
              </div>
              
              <div v-else-if="error" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ error }}
              </div>
              
              <div v-else-if="topLevelManagers.length === 0" class="text-center py-5">
                <i class="fas fa-sitemap fa-4x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune structure hiérarchique</h5>
                <p class="text-muted">
                  Définissez les relations manager-subordonné pour construire l'organigramme.
                  <br>
                  Modifiez les utilisateurs pour assigner des managers.
                </p>
              </div>
              
              <div v-else class="hierarchy-container">
                <!-- Légende -->
                <div class="legend mb-4">
                  <div class="d-flex flex-wrap gap-3">
                    <div class="legend-item">
                      <span class="legend-color bg-primary"></span>
                      <span>Directeur/Admin</span>
                    </div>
                    <div class="legend-item">
                      <span class="legend-color bg-success"></span>
                      <span>Manager</span>
                    </div>
                    <div class="legend-item">
                      <span class="legend-color bg-info"></span>
                      <span>Employé</span>
                    </div>
                  </div>
                </div>
                
                <!-- Arbre hiérarchique -->
                <div class="hierarchy-tree">
                  <div 
                    v-for="topManager in topLevelManagers" 
                    :key="topManager.id"
                    class="tree-branch"
                  >
                    <HierarchyNode 
                      :user="topManager" 
                      :level="0"
                      :expanded="expandedNodes.has(topManager.id)"
                      :users="users"
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
import api from '@/services/api'

// Composant pour afficher un nœud de la hiérarchie
const HierarchyNode = {
  name: 'HierarchyNode',
  props: {
    user: Object,
    level: Number,
    expanded: Boolean,
    users: Array
  },
  emits: ['toggle'],
  setup(props, { emit }) {
    const { user, level, users } = props
    
    const subordinates = computed(() => {
      return users.filter(u => u.managerId === user.id)
    })
    
    const hasSubordinates = computed(() => subordinates.value.length > 0)
    
    const nodeClass = computed(() => {
      const baseClass = 'hierarchy-node'
      const roleClass = getRoleClass(user.role)
      const levelClass = `level-${level}`
      return `${baseClass} ${roleClass} ${levelClass}`
    })
    
    const getRoleClass = (role) => {
      switch (role?.toLowerCase()) {
        case 'admin':
        case 'directeur':
          return 'role-admin'
        case 'manager':
        case 'chef':
          return 'role-manager'
        case 'employé':
        case 'employee':
        default:
          return 'role-employee'
      }
    }
    
    const getRoleIcon = (role) => {
      switch (role?.toLowerCase()) {
        case 'admin':
        case 'directeur':
          return 'fas fa-crown'
        case 'manager':
        case 'chef':
          return 'fas fa-user-tie'
        case 'employé':
        case 'employee':
        default:
          return 'fas fa-user'
      }
    }
    
    const toggle = () => {
      emit('toggle', user.id)
    }
    
    return {
      subordinates,
      hasSubordinates,
      nodeClass,
      getRoleIcon,
      toggle
    }
  },
  template: `
    <div class="node-container">
      <div :class="nodeClass" @click="toggle">
        <div class="node-content">
          <div class="node-header">
            <div class="node-avatar">
              <i :class="getRoleIcon(user.role)"></i>
            </div>
            <div class="node-info">
              <h6 class="node-name mb-1">{{ user.fullName }}</h6>
              <span class="node-role badge">{{ user.role }}</span>
            </div>
            <div class="node-toggle" v-if="hasSubordinates">
              <i v-if="expanded" class="fas fa-chevron-up"></i>
              <i v-else class="fas fa-chevron-down"></i>
            </div>
          </div>
          <div class="node-details">
            <small class="text-muted">
              <i class="fas fa-envelope me-1"></i>
              {{ user.email }}
            </small>
            <div v-if="hasSubordinates" class="mt-1">
              <small class="badge bg-secondary">
                {{ subordinates.length }} subordonné{{ subordinates.length > 1 ? 's' : '' }}
              </small>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Subordonnés -->
      <div v-if="expanded && hasSubordinates" class="subordinates">
        <div class="subordinates-line"></div>
        <div class="subordinates-container">
          <HierarchyNode
            v-for="subordinate in subordinates"
            :key="subordinate.id"
            :user="subordinate"
            :level="level + 1"
            :expanded="$parent.expandedNodes.has(subordinate.id)"
            :users="users"
            @toggle="$emit('toggle', $event)"
          />
        </div>
      </div>
    </div>
  `
}

export default {
  name: 'OrganigammeHierarchique',
  components: {
    AppNavbar,
    HierarchyNode
  },
  setup() {
    const users = ref([])
    const loading = ref(false)
    const error = ref('')
    const expandedNodes = ref(new Set())
    
    const topLevelManagers = computed(() => {
      return users.value.filter(user => !user.managerId || user.managerId === 0)
    })
    
    const totalUsers = computed(() => users.value.length)
    
    const managersCount = computed(() => {
      const managerIds = new Set(users.value.map(u => u.managerId).filter(id => id))
      return managerIds.size
    })
    
    const employeesCount = computed(() => {
      const managerIds = new Set(users.value.map(u => u.managerId).filter(id => id))
      return users.value.filter(u => !managerIds.has(u.id)).length
    })
    
    const maxHierarchyDepth = computed(() => {
      const getDepth = (userId, depth = 0) => {
        const subordinates = users.value.filter(u => u.managerId === userId)
        if (subordinates.length === 0) return depth
        return Math.max(...subordinates.map(s => getDepth(s.id, depth + 1)))
      }
      
      if (topLevelManagers.value.length === 0) return 0
      return Math.max(...topLevelManagers.value.map(m => getDepth(m.id, 1)))
    })
    
    const loadUsers = async () => {
      try {
        loading.value = true
        error.value = ''
        const response = await api.get('/Users')
        users.value = response.data || []
        console.log('Utilisateurs chargés pour organigramme:', users.value)
        console.log('Top level managers:', topLevelManagers.value)
      } catch (err) {
        console.error('Erreur lors du chargement des utilisateurs:', err)
        if (err.response?.status === 401) {
          error.value = 'Non autorisé. Veuillez vous connecter.'
        } else if (err.response?.status === 403) {
          error.value = 'Accès refusé. Droits administrateur requis.'
        } else {
          error.value = 'Erreur lors du chargement des utilisateurs: ' + (err.message || 'Erreur inconnue')
        }
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
      topLevelManagers,
      totalUsers,
      managersCount,
      employeesCount,
      maxHierarchyDepth,
      loadUsers,
      toggleNode,
      expandAll,
      collapseAll
    }
  }
}
</script>

<style scoped>
.stat-card {
  border-radius: 10px;
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

.hierarchy-tree {
  position: relative;
}

.tree-branch {
  margin-bottom: 20px;
}

.node-container {
  position: relative;
  margin-left: 20px;
}

.hierarchy-node {
  background: white;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  padding: 15px;
  margin-bottom: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.hierarchy-node:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.hierarchy-node.role-admin {
  border-color: #0d6efd;
  background: linear-gradient(135deg, #fff 0%, #f8f9ff 100%);
}

.hierarchy-node.role-manager {
  border-color: #198754;
  background: linear-gradient(135deg, #fff 0%, #f8fff9 100%);
}

.hierarchy-node.role-employee {
  border-color: #0dcaf0;
  background: linear-gradient(135deg, #fff 0%, #f8feff 100%);
}

.node-content {
  width: 100%;
}

.node-header {
  display: flex;
  align-items: center;
  gap: 12px;
}

.node-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  color: white;
}

.role-admin .node-avatar {
  background: #0d6efd;
}

.role-manager .node-avatar {
  background: #198754;
}

.role-employee .node-avatar {
  background: #0dcaf0;
}

.node-info {
  flex: 1;
}

.node-name {
  font-weight: 600;
  color: #333;
  margin: 0;
}

.node-role {
  font-size: 0.75rem;
  padding: 2px 8px;
}

.role-admin .node-role {
  background: #0d6efd;
}

.role-manager .node-role {
  background: #198754;
}

.role-employee .node-role {
  background: #0dcaf0;
}

.node-toggle {
  font-size: 1.2rem;
  color: #6c757d;
}

.node-details {
  margin-top: 8px;
  padding-top: 8px;
  border-top: 1px solid #f0f0f0;
}

.subordinates {
  position: relative;
  margin-top: 15px;
  margin-left: 30px;
}

.subordinates-line {
  position: absolute;
  left: -20px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #dee2e6;
}

.subordinates-line::before {
  content: '';
  position: absolute;
  left: -8px;
  top: 0;
  width: 18px;
  height: 2px;
  background: #dee2e6;
}

.subordinates-container {
  padding-left: 10px;
}

.level-0 {
  margin-left: 0;
}

.level-1 {
  margin-left: 10px;
}

.level-2 {
  margin-left: 20px;
}

.level-3 {
  margin-left: 30px;
}

@media (max-width: 768px) {
  .hierarchy-node {
    padding: 10px;
  }
  
  .node-header {
    gap: 8px;
  }
  
  .node-avatar {
    width: 32px;
    height: 32px;
    font-size: 1rem;
  }
  
  .subordinates {
    margin-left: 15px;
  }
  
  .stat-number {
    font-size: 1.5rem;
  }
}
</style>
