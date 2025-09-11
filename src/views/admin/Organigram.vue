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
            <button class="btn btn-info" @click="refreshOrganigramme">
              <i class="fas fa-sync-alt me-2"></i>
              Actualiser
            </button>
          </div>
        </div>
      </div>
      
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement de l'organigramme...</p>
              </div>
              
              <div v-else-if="topLevelManagers.length === 0" class="text-center py-4">
                <i class="fas fa-sitemap fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune structure hiérarchique</h5>
                <p class="text-muted">Créez des utilisateurs et définissez leurs managers pour construire l'organigramme.</p>
              </div>
              
              <div v-else class="organigram-tree">
                <div v-for="manager in topLevelManagers" :key="manager.id" class="tree-root">
                  <UserNode :user="manager" :subordinates="getSubordinates(manager.id)" :level="0" />
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
              <h3>{{ totalEmployees }}</h3>
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
              <i class="fas fa-user"></i>
            </div>
            <div class="stat-content">
              <h3>{{ employees.length }}</h3>
              <p>Employés</p>
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

// Computed properties
const totalEmployees = computed(() => users.value.length)

const admins = computed(() => 
  users.value.filter(user => user.role === 'Admin')
)

const managers = computed(() => 
  users.value.filter(user => user.role === 'Manager')
)

const employees = computed(() => 
  users.value.filter(user => user.role === 'Employé')
)

const topLevelManagers = computed(() => 
  users.value.filter(user => !user.managerId)
)

// Methods
const fetchUsers = async () => {
  try {
    loading.value = true
    const response = await apiService.getUsers()
    users.value = response.data || []
    console.log('Users récupérés:', users.value)
  } catch (error) {
    console.error('Erreur lors de la récupération des utilisateurs:', error)
  } finally {
    loading.value = false
  }
}

const getSubordinates = (managerId) => {
  return users.value.filter(user => user.managerId === managerId)
}

const refreshOrganigramme = () => {
  fetchUsers()
}

// User Node Component (défini dans le même fichier)
const UserNode = {
  name: 'UserNode',
  props: ['user', 'subordinates', 'level'],
  setup(props) {
    const hasSubordinates = computed(() => props.subordinates && props.subordinates.length > 0)
    
    const getSubordinatesOf = (userId) => {
      return users.value.filter(user => user.managerId === userId)
    }
    
    const getRoleIcon = (role) => {
      switch (role) {
        case 'Admin': return 'fas fa-crown'
        case 'Manager': return 'fas fa-user-tie'
        case 'Employé': return 'fas fa-user'
        default: return 'fas fa-user'
      }
    }
    
    const getRoleColor = (role) => {
      switch (role) {
        case 'Admin': return 'danger'
        case 'Manager': return 'warning'
        case 'Employé': return 'info'
        default: return 'secondary'
      }
    }
    
    const getInitials = (fullName) => {
      return fullName.split(' ').map(name => name.charAt(0)).join('').toUpperCase()
    }
    
    return {
      hasSubordinates,
      getSubordinatesOf,
      getRoleIcon,
      getRoleColor,
      getInitials,
      users
    }
  },
  template: `
    <div class="user-node" :style="{ marginLeft: (level * 40) + 'px' }">
      <div class="user-card" :class="'level-' + level">
        <div class="d-flex align-items-center">
          <div class="user-avatar" :class="'bg-' + getRoleColor(user.role)">
            {{ getInitials(user.fullName) }}
          </div>
          <div class="user-info flex-grow-1">
            <h6 class="user-name mb-1">{{ user.fullName }}</h6>
            <div class="user-role">
              <i :class="getRoleIcon(user.role)" class="me-1"></i>
              <span class="badge" :class="'bg-' + getRoleColor(user.role)">{{ user.role }}</span>
            </div>
            <small class="text-muted">{{ user.email }}</small>
          </div>
          <div v-if="hasSubordinates" class="subordinates-count">
            <span class="badge bg-secondary">
              <i class="fas fa-users me-1"></i>
              {{ subordinates.length }}
            </span>
          </div>
        </div>
      </div>
      
      <!-- Ligne de connexion vers les subordonnés -->
      <div v-if="hasSubordinates" class="connection-line"></div>
      
      <!-- Subordonnés -->
      <div v-if="hasSubordinates" class="subordinates">
        <UserNode 
          v-for="subordinate in subordinates" 
          :key="subordinate.id"
          :user="subordinate" 
          :subordinates="getSubordinatesOf(subordinate.id)"
          :level="level + 1"
        />
      </div>
    </div>
  `
}

onMounted(() => {
  fetchUsers()
})
</script>

<style scoped>
.organigram-tree {
  padding: 20px;
}

.tree-root {
  margin-bottom: 30px;
}

.user-node {
  position: relative;
  margin-bottom: 15px;
}

.user-card {
  background: white;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  padding: 15px;
  transition: all 0.3s ease;
  position: relative;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.user-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.level-0 .user-card {
  border-left: 5px solid #dc3545;
  background: linear-gradient(135deg, #fff5f5 0%, #ffffff 100%);
}

.level-1 .user-card {
  border-left: 5px solid #ffc107;
  background: linear-gradient(135deg, #fffbf0 0%, #ffffff 100%);
}

.level-2 .user-card {
  border-left: 5px solid #0dcaf0;
  background: linear-gradient(135deg, #f0fcff 0%, #ffffff 100%);
}

.level-3 .user-card {
  border-left: 5px solid #198754;
  background: linear-gradient(135deg, #f0fff4 0%, #ffffff 100%);
}

.user-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  font-size: 18px;
  margin-right: 15px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
}

.user-info {
  min-width: 0;
}

.user-name {
  font-weight: 600;
  color: #333;
  margin-bottom: 5px;
}

.user-role {
  margin-bottom: 3px;
}

.subordinates-count {
  margin-left: 10px;
}

.connection-line {
  position: absolute;
  left: 25px;
  top: 100%;
  width: 2px;
  height: 20px;
  background: #dee2e6;
}

.subordinates {
  position: relative;
}

.subordinates::before {
  content: '';
  position: absolute;
  left: 25px;
  top: 0;
  width: 20px;
  height: 2px;
  background: #dee2e6;
}

.stat-card {
  padding: 20px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.stat-icon {
  font-size: 2.5em;
  margin-right: 20px;
  opacity: 0.8;
}

.stat-content h3 {
  margin: 0;
  font-size: 2em;
  font-weight: bold;
}

.stat-content p {
  margin: 0;
  font-size: 0.9em;
  opacity: 0.9;
}

/* Responsive */
@media (max-width: 768px) {
  .user-node {
    margin-left: 0 !important;
  }
  
  .user-card {
    margin-left: 0;
  }
  
  .subordinates {
    margin-left: 20px;
  }
}
</style>
                        <p>Employés Total</p>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-4">
                    <div class="stat-card bg-success text-white">
                      <div class="stat-icon">
                        <i class="fas fa-user-tie"></i>
                      </div>
                      <div class="stat-content">
                        <h3>{{ totalManagers }}</h3>
                        <p>Managers</p>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-4">
                    <div class="stat-card bg-info text-white">
                      <div class="stat-icon">
                        <i class="fas fa-layer-group"></i>
                      </div>
                      <div class="stat-content">
                        <h3>{{ maxDepth }}</h3>
                        <p>Niveaux Hiérarchiques</p>
                      </div>
                    </div>
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

<script setup>
import { ref, onMounted, computed } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

// State
const loading = ref(true)
const organigram = ref([])
const allUsers = ref([])

// Computed
const totalEmployees = computed(() => allUsers.value.length)
const totalManagers = computed(() => 
  allUsers.value.filter(user => 
    allUsers.value.some(u => u.managerId === user.id)
  ).length
)
const maxDepth = computed(() => {
  const getDepth = (userId, visited = new Set()) => {
    if (visited.has(userId)) return 0
    visited.add(userId)
    
    const subordinates = allUsers.value.filter(u => u.managerId === userId)
    if (subordinates.length === 0) return 1
    
    return 1 + Math.max(...subordinates.map(s => getDepth(s.id, new Set(visited))))
  }
  
  const rootUsers = allUsers.value.filter(u => !u.managerId)
  return rootUsers.length > 0 ? Math.max(...rootUsers.map(u => getDepth(u.id))) : 0
})

// Methods
const fetchOrganigram = async () => {
  try {
    loading.value = true
    const [orgResponse, usersResponse] = await Promise.all([
      apiService.getOrganigram(),
      apiService.getUsers()
    ])
    
    organigram.value = orgResponse.data || []
    allUsers.value = usersResponse.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération de l\'organigramme:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchOrganigram()
})
</script>

<!-- Composant enfant pour les nœuds de l'organigramme -->
<script>
import { defineComponent, h } from 'vue'

const OrganiggramNode = defineComponent({
  name: 'OrganiggramNode',
  props: {
    node: Object,
    level: Number
  },
  setup(props) {
    const getRoleIcon = (role) => {
      return role === 'Admin' ? 'fas fa-shield-alt' : 'fas fa-user'
    }
    
    const getRoleColor = (role) => {
      return role === 'Admin' ? 'danger' : 'primary'
    }
    
    return () => h('div', { class: 'org-node' }, [
      h('div', { 
        class: `org-card level-${props.level}`,
        style: { marginLeft: `${props.level * 40}px` }
      }, [
        h('div', { class: 'card border-0 shadow-sm' }, [
          h('div', { class: 'card-body p-3' }, [
            h('div', { class: 'd-flex align-items-center' }, [
              h('div', { class: 'me-3' }, [
                h('div', { 
                  class: `avatar bg-${getRoleColor(props.node.role)} text-white rounded-circle d-flex align-items-center justify-content-center`,
                  style: { width: '50px', height: '50px' }
                }, [
                  h('i', { class: getRoleIcon(props.node.role) })
                ])
              ]),
              h('div', { class: 'flex-grow-1' }, [
                h('h6', { class: 'mb-1' }, props.node.fullName),
                h('p', { class: 'text-muted mb-1 small' }, props.node.email),
                h('span', { 
                  class: `badge bg-${getRoleColor(props.node.role)}`
                }, props.node.role)
              ]),
              props.node.subordinates && props.node.subordinates.length > 0 ? 
                h('div', { class: 'ms-3' }, [
                  h('span', { class: 'badge bg-secondary' }, `${props.node.subordinates.length} subordonnés`)
                ]) : null
            ])
          ])
        ])
      ]),
      // Afficher les subordonnés récursivement
      props.node.subordinates ? 
        props.node.subordinates.map(subordinate => 
          h(OrganiggramNode, {
            key: subordinate.id,
            node: subordinate,
            level: props.level + 1
          })
        ) : null
    ])
  }
})

export default OrganiggramNode
</script>

<style scoped>
.organigramme-container {
  max-height: 70vh;
  overflow-y: auto;
}

.org-node {
  margin-bottom: 15px;
}

.org-card {
  transition: all 0.3s ease;
}

.org-card:hover {
  transform: translateX(5px);
}

.level-0 {
  border-left: 4px solid #007bff;
}

.level-1 {
  border-left: 4px solid #28a745;
}

.level-2 {
  border-left: 4px solid #ffc107;
}

.level-3 {
  border-left: 4px solid #dc3545;
}

.stat-card {
  padding: 20px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  margin-bottom: 15px;
}

.stat-icon {
  font-size: 2.5em;
  margin-right: 20px;
  opacity: 0.8;
}

.stat-content h3 {
  margin: 0;
  font-size: 2em;
  font-weight: bold;
}

.stat-content p {
  margin: 0;
  font-size: 0.9em;
  opacity: 0.9;
}

.avatar {
  font-size: 1.2em;
}
</style>
