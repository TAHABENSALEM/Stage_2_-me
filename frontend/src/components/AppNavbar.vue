<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container-fluid">
      <!-- Brand -->
      <router-link class="navbar-brand" to="/">
        <i class="fas fa-building me-2"></i>
        Gestion Congés & Frais
      </router-link>
      
      <!-- Toggle button for mobile -->
      <button 
        class="navbar-toggler" 
        type="button" 
        data-bs-toggle="collapse" 
        data-bs-target="#navbarNav"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      
      <!-- Navigation Links -->
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav me-auto">
          <!-- Admin Links -->
          <template v-if="authStore.isAdmin">
            <li class="nav-item">
              <router-link class="nav-link" to="/admin">
                <i class="fas fa-tachometer-alt me-1"></i>
                Dashboard
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/admin/users">
                <i class="fas fa-users me-1"></i>
                Utilisateurs
              </router-link>
            </li>
            <li class="nav-item dropdown">
              <a 
                class="nav-link dropdown-toggle" 
                href="#" 
                id="configDropdown" 
                role="button" 
                data-bs-toggle="dropdown"
              >
                <i class="fas fa-cog me-1"></i>
                Configuration
              </a>
              <ul class="dropdown-menu">
                <li>
                  <router-link class="dropdown-item" to="/admin/leave-types">
                    <i class="fas fa-calendar-alt me-2"></i>
                    Types de congés
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/admin/expense-types">
                    <i class="fas fa-receipt me-2"></i>
                    Types de frais
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/admin/holidays">
                    <i class="fas fa-calendar-times me-2"></i>
                    Jours fériés
                  </router-link>
                </li>
              </ul>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/admin/organigram">
                <i class="fas fa-sitemap me-1"></i>
                Organigramme
              </router-link>
            </li>
          </template>
          
          <!-- Employee Links -->
          <template v-if="authStore.isEmployee">
            <li class="nav-item">
              <router-link class="nav-link" to="/employee">
                <i class="fas fa-tachometer-alt me-1"></i>
                Dashboard
              </router-link>
            </li>
            <li class="nav-item dropdown">
              <a 
                class="nav-link dropdown-toggle" 
                href="#" 
                id="requestDropdown" 
                role="button" 
                data-bs-toggle="dropdown"
              >
                <i class="fas fa-plus-circle me-1"></i>
                Nouvelles demandes
              </a>
              <ul class="dropdown-menu">
                <li>
                  <router-link class="dropdown-item" to="/employee/leave-request">
                    <i class="fas fa-calendar-plus me-2"></i>
                    Demande de congé
                  </router-link>
                </li>
                <li>
                  <router-link class="dropdown-item" to="/employee/expense-request">
                    <i class="fas fa-file-invoice-dollar me-2"></i>
                    Note de frais
                  </router-link>
                </li>
              </ul>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/employee/history">
                <i class="fas fa-history me-1"></i>
                Historique
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/employee/approvals">
                <i class="fas fa-tasks me-1"></i>
                Approbations
                <span v-if="pendingCount > 0" class="badge bg-warning text-dark ms-1">
                  {{ pendingCount }}
                </span>
              </router-link>
            </li>
          </template>
        </ul>
        
        <!-- User Info & Logout -->
        <ul class="navbar-nav">
          <!-- Informations utilisateur -->
          <li class="nav-item">
            <span class="navbar-text me-3">
              <i class="fas fa-user-circle me-1"></i>
              {{ authStore.user?.email }}
              <small class="text-light-emphasis ms-2">({{ authStore.user?.role }})</small>
            </span>
          </li>
          
          <!-- Bouton de déconnexion -->
          <li class="nav-item">
            <button class="btn btn-outline-light btn-sm" @click="logout">
              <i class="fas fa-sign-out-alt me-1"></i>
              Déconnexion
            </button>
          </li>
          
          <!-- Menu déroulant alternatif (si vous voulez le garder) -->
          <li class="nav-item dropdown d-none">
            <a 
              class="nav-link dropdown-toggle" 
              href="#" 
              id="userDropdown" 
              role="button" 
              data-bs-toggle="dropdown"
            >
              <i class="fas fa-user-circle me-1"></i>
              {{ authStore.user?.email }}
            </a>
            <ul class="dropdown-menu dropdown-menu-end">
              <li>
                <span class="dropdown-item-text">
                  <small class="text-muted">
                    Connecté en tant que: {{ authStore.user?.role }}
                  </small>
                </span>
              </li>
              <li><hr class="dropdown-divider"></li>
              <li>
                <a class="dropdown-item" href="#" @click="logout">
                  <i class="fas fa-sign-out-alt me-2"></i>
                  Déconnexion
                </a>
              </li>
            </ul>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import apiService from '@/services/api'
import { Dropdown } from 'bootstrap'

const authStore = useAuthStore()
const router = useRouter()
const pendingCount = ref(0)

const logout = () => {
  authStore.logout()
  router.push('/')
}

const fetchPendingCount = async () => {
  if (!authStore.isEmployee) return
  
  try {
    // Compter les demandes en attente pour cet employé (en tant que manager)
    const [leaveRequests, expenseRequests] = await Promise.all([
      apiService.getPendingLeaveRequests(),
      apiService.getPendingExpenseRequests()
    ])
    
    pendingCount.value = (leaveRequests.data?.length || 0) + (expenseRequests.data?.length || 0)
  } catch (error) {
    console.error('Erreur lors de la récupération des demandes en attente:', error)
  }
}

onMounted(() => {
  fetchPendingCount()
  
  // Actualiser le compteur toutes les 30 secondes
  setInterval(fetchPendingCount, 30000)
})
</script>

<style scoped>
.navbar-brand {
  font-weight: 600;
  font-size: 1.25rem;
}

.nav-link {
  font-weight: 500;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 0.375rem;
}

.nav-link.router-link-active {
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 0.375rem;
}

.dropdown-item:hover {
  background-color: #f8f9fa;
}

.badge {
  font-size: 0.65em;
}
</style>
