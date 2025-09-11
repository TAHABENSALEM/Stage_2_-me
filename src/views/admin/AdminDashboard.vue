<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-tachometer-alt me-2 text-primary"></i>
              Dashboard Administrateur
            </h2>
            <span class="badge bg-primary fs-6">{{ currentDate }}</span>
          </div>
        </div>
      </div>
      
      <!-- Statistiques -->
      <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-4">
          <div class="card border-left-primary shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                    Utilisateurs Actifs
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalUsers }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-users fa-2x text-gray-300"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-xl-3 col-md-6 mb-4">
          <div class="card border-left-success shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                    Types de Congés
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalLeaveTypes }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-calendar-alt fa-2x text-gray-300"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-xl-3 col-md-6 mb-4">
          <div class="card border-left-info shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
                    Types de Frais
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalExpenseTypes }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-receipt fa-2x text-gray-300"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-xl-3 col-md-6 mb-4">
          <div class="card border-left-warning shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                    Jours Fériés
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalHolidays }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-calendar-times fa-2x text-gray-300"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Actions rapides -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-header py-3">
              <h6 class="m-0 font-weight-bold text-primary">
                <i class="fas fa-bolt me-2"></i>
                Actions Rapides
              </h6>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/admin/users" class="btn btn-primary w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-user-plus fa-2x mb-2"></i>
                    <span>Gérer les Utilisateurs</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/admin/leave-types" class="btn btn-success w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-calendar-plus fa-2x mb-2"></i>
                    <span>Types de Congés</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/admin/expense-types" class="btn btn-info w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-file-invoice-dollar fa-2x mb-2"></i>
                    <span>Types de Frais</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/admin/organigram" class="btn btn-warning w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-sitemap fa-2x mb-2"></i>
                    <span>Organigramme</span>
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Activité récente -->
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-header py-3">
              <h6 class="m-0 font-weight-bold text-primary">
                <i class="fas fa-clock me-2"></i>
                Activité Récente du Système
              </h6>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement des données...</p>
              </div>
              
              <div v-else class="text-center py-4">
                <i class="fas fa-chart-bar fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Tableau de bord opérationnel</h5>
                <p class="text-muted">
                  Le système est configuré et prêt à être utilisé par les employés.
                </p>
                
                <div class="row mt-4">
                  <div class="col-md-4">
                    <div class="text-success">
                      <i class="fas fa-check-circle fa-2x"></i>
                      <p class="mt-2 mb-0">Système Actif</p>
                    </div>
                  </div>
                  <div class="col-md-4">
                    <div class="text-info">
                      <i class="fas fa-database fa-2x"></i>
                      <p class="mt-2 mb-0">Base de Données</p>
                    </div>
                  </div>
                  <div class="col-md-4">
                    <div class="text-warning">
                      <i class="fas fa-shield-alt fa-2x"></i>
                      <p class="mt-2 mb-0">Sécurisé</p>
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

const loading = ref(true)
const stats = ref({
  totalUsers: 0,
  totalLeaveTypes: 0,
  totalExpenseTypes: 0,
  totalHolidays: 0
})

const currentDate = computed(() => {
  return new Date().toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

const fetchStats = async () => {
  try {
    loading.value = true
    
    const [users, leaveTypes, expenseTypes, holidays] = await Promise.all([
      apiService.getUsers().catch(() => ({ data: [] })),
      apiService.getLeaveTypes().catch(() => ({ data: [] })),
      apiService.getExpenseTypes().catch(() => ({ data: [] })),
      apiService.getHolidays().catch(() => ({ data: [] }))
    ])
    
    stats.value = {
      totalUsers: users.data?.length || 0,
      totalLeaveTypes: leaveTypes.data?.length || 0,
      totalExpenseTypes: expenseTypes.data?.length || 0,
      totalHolidays: holidays.data?.length || 0
    }
  } catch (error) {
    console.error('Erreur lors de la récupération des statistiques:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchStats()
})
</script>

<style scoped>
.border-left-primary {
  border-left: 0.25rem solid #4e73df !important;
}

.border-left-success {
  border-left: 0.25rem solid #1cc88a !important;
}

.border-left-info {
  border-left: 0.25rem solid #36b9cc !important;
}

.border-left-warning {
  border-left: 0.25rem solid #f6c23e !important;
}

.text-xs {
  font-size: 0.7rem;
}

.font-weight-bold {
  font-weight: 700 !important;
}

.text-gray-800 {
  color: #5a5c69 !important;
}

.text-gray-300 {
  color: #dddfeb !important;
}

.card {
  transition: all 0.3s;
}

.card:hover {
  transform: translateY(-2px);
}

.btn {
  transition: all 0.3s;
  min-height: 100px;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.shadow {
  box-shadow: 0 0.15rem 1.75rem 0 rgba(58, 59, 69, 0.15) !important;
}
</style>
