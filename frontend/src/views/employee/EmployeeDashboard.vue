<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-tachometer-alt me-2 text-success"></i>
              Dashboard Employé
            </h2>
            <span class="badge bg-success fs-6">{{ currentDate }}</span>
          </div>
        </div>
      </div>
      
      <!-- Statistiques personnelles -->
      <div class="row mb-4">
        <div class="col-xl-3 col-md-6 mb-4">
          <div class="card border-left-primary shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                    Mes Demandes de Congé
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalLeaveRequests }}
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
          <div class="card border-left-success shadow h-100 py-2">
            <div class="card-body">
              <div class="row no-gutters align-items-center">
                <div class="col mr-2">
                  <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                    Congés Approuvés
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.approvedLeaveRequests }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-check-circle fa-2x text-gray-300"></i>
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
                    Mes Notes de Frais
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.totalExpenseRequests }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-file-invoice-dollar fa-2x text-gray-300"></i>
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
                    En Attente d'Approbation
                  </div>
                  <div class="h5 mb-0 font-weight-bold text-gray-800">
                    {{ stats.pendingRequests }}
                  </div>
                </div>
                <div class="col-auto">
                  <i class="fas fa-clock fa-2x text-gray-300"></i>
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
              <h6 class="m-0 font-weight-bold text-success">
                <i class="fas fa-bolt me-2"></i>
                Actions Rapides
              </h6>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/employee/leave-request" class="btn btn-success w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-calendar-plus fa-2x mb-2"></i>
                    <span>Demande de Congé</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/employee/expense-request" class="btn btn-info w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-file-invoice-dollar fa-2x mb-2"></i>
                    <span>Note de Frais</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/employee/history" class="btn btn-primary w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-history fa-2x mb-2"></i>
                    <span>Mon Historique</span>
                  </router-link>
                </div>
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/employee/approvals" class="btn btn-warning w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-tasks fa-2x mb-2"></i>
                    <span>Approbations</span>
                    <small v-if="stats.pendingForApproval > 0" class="badge bg-danger mt-1">
                      {{ stats.pendingForApproval }}
                    </small>
                  </router-link>
                </div>
              </div>
              
              <!-- Ligne supplémentaire pour les managers -->
              <div class="row" v-if="isManager">
                <div class="col-lg-3 col-md-6 mb-3">
                  <router-link to="/employee/my-decisions" class="btn btn-secondary w-100 h-100 d-flex flex-column justify-content-center">
                    <i class="fas fa-gavel fa-2x mb-2"></i>
                    <span>Mes Décisions</span>
                    <small class="text-light mt-1">Historique des approbations</small>
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Mes demandes récentes et Jours fériés -->
      <div class="row">
        <div class="col-xl-8 col-lg-7">
          <div class="card shadow">
            <div class="card-header py-3">
              <h6 class="m-0 font-weight-bold text-primary">
                <i class="fas fa-clock me-2"></i>
                Mes Demandes Récentes
              </h6>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement des demandes...</p>
              </div>
              
              <div v-else-if="recentRequests.length === 0" class="text-center py-4">
                <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                <p class="text-muted">Aucune demande récente</p>
                <small class="text-muted">Vos demandes de congé et notes de frais apparaîtront ici</small>
              </div>
              
              <div v-else>
                <div v-for="request in recentRequests" :key="`${request.type}-${request.id}`" class="mb-3">
                  <div class="d-flex justify-content-between align-items-center border p-3 rounded">
                    <div class="flex-grow-1">
                      <div class="d-flex align-items-center mb-2">
                        <i :class="request.type === 'leave' ? 'fas fa-calendar-alt text-primary' : 'fas fa-file-invoice-dollar text-info'" class="me-2"></i>
                        <strong>{{ request.type === 'leave' ? `Congé ${request.leaveType?.name || ''}` : `Frais ${request.expenseType?.name || ''}` }}</strong>
                      </div>
                      <p class="mb-2 text-muted">
                        <span v-if="request.type === 'leave'">
                          Du {{ formatDate(request.startDate) }} au {{ formatDate(request.endDate) }}
                        </span>
                        <span v-else>
                          Montant: {{ request.amount }}€
                        </span>
                      </p>
                      <small class="text-muted">
                        Créée le {{ formatDate(request.createdAt) }}
                      </small>
                    </div>
                    <div>
                      <span :class="`badge ${getStatusBadgeClass(request.status)}`">
                        {{ getStatusText(request.status) }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Jours fériés à venir -->
        <div class="col-xl-4 col-lg-5">
          <div class="card shadow">
            <div class="card-header py-3">
              <h6 class="m-0 font-weight-bold text-warning">
                <i class="fas fa-calendar-times me-2"></i>
                Jours Fériés à Venir
              </h6>
            </div>
            <div class="card-body">
              <div v-if="loadingHolidays" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement...</p>
              </div>
              
              <div v-else-if="upcomingHolidays.length === 0" class="text-center py-4">
                <i class="fas fa-calendar-times fa-2x text-muted mb-2"></i>
                <p class="text-muted small">Aucun jour férié à venir</p>
              </div>
              
              <div v-else>
                <div v-for="holiday in upcomingHolidays" :key="holiday.id" class="mb-3">
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <h6 class="mb-1">{{ holiday.name }}</h6>
                      <small class="text-muted">{{ formatDate(holiday.date) }}</small>
                    </div>
                    <div>
                      <i class="fas fa-calendar-times text-warning"></i>
                    </div>
                  </div>
                  <hr v-if="holiday !== upcomingHolidays[upcomingHolidays.length - 1]" class="my-2">
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
const loadingHolidays = ref(true)
const recentRequests = ref([])
const holidays = ref([])
const isManager = ref(false)
const stats = ref({
  totalLeaveRequests: 0,
  approvedLeaveRequests: 0,
  totalExpenseRequests: 0,
  pendingRequests: 0,
  pendingForApproval: 0
})

// Computed
const currentDate = computed(() => {
  return new Date().toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
})

const upcomingHolidays = computed(() => {
  const now = new Date()
  return holidays.value
    .filter(h => new Date(h.date) >= now)
    .sort((a, b) => new Date(a.date) - new Date(b.date))
    .slice(0, 5)
})

// Methods
const fetchMyRequests = async () => {
  try {
    loading.value = true
    
    const [leaveResponse, expenseResponse, pendingLeaveResponse, pendingExpenseResponse] = await Promise.all([
      apiService.getMyLeaveRequests().catch(() => ({ data: [] })),
      apiService.getMyExpenseRequests().catch(() => ({ data: [] })),
      apiService.getPendingLeaveRequests().catch(() => ({ data: [] })),
      apiService.getPendingExpenseRequests().catch(() => ({ data: [] }))
    ])
    
    // Combiner et marquer le type
    const leaveRequests = (leaveResponse.data || []).map(r => ({ ...r, type: 'leave' }))
    const expenseRequests = (expenseResponse.data || []).map(r => ({ ...r, type: 'expense' }))
    
    // Trier par date de création (plus récent en premier)
    recentRequests.value = [...leaveRequests, ...expenseRequests]
      .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      .slice(0, 10) // Limiter à 10 éléments
    
    // Calculer les statistiques
    stats.value = {
      totalLeaveRequests: leaveRequests.length,
      approvedLeaveRequests: leaveRequests.filter(r => r.status === 1).length, // Approved = 1
      totalExpenseRequests: expenseRequests.length,
      pendingRequests: [...leaveRequests, ...expenseRequests].filter(r => r.status === 0).length, // Pending = 0
      pendingForApproval: (pendingLeaveResponse.data || []).length + (pendingExpenseResponse.data || []).length
    }
    
    // Détecter si l'utilisateur est un manager (a des demandes à approuver ou en a déjà approuvé)
    const hasCurrentPendingRequests = (pendingLeaveResponse.data || []).length > 0 || (pendingExpenseResponse.data || []).length > 0
    if (hasCurrentPendingRequests) {
      isManager.value = true
    } else {
      // Vérifier s'il a déjà pris des décisions dans le passé
      try {
        const [leaveDecisionsResponse, expenseDecisionsResponse] = await Promise.all([
          apiService.getMyLeaveDecisions().catch(() => ({ data: [] })),
          apiService.getMyExpenseDecisions().catch(() => ({ data: [] }))
        ])
        const hasDecisionHistory = (leaveDecisionsResponse.data || []).length > 0 || (expenseDecisionsResponse.data || []).length > 0
        isManager.value = hasDecisionHistory
      } catch (error) {
        console.warn('Impossible de vérifier l\'historique des décisions:', error)
        isManager.value = false
      }
    }
    
  } catch (error) {
    console.error('Erreur lors de la récupération des demandes:', error)
  } finally {
    loading.value = false
  }
}

const fetchHolidays = async () => {
  try {
    loadingHolidays.value = true
    const response = await apiService.getHolidays()
    holidays.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des jours fériés:', error)
  } finally {
    loadingHolidays.value = false
  }
}

const getStatusBadgeClass = (status) => {
  // Gérer les valeurs numériques (enum) et les chaînes
  switch (status) {
    case 'Approved':
    case 1: return 'bg-success'
    case 'Rejected':
    case 2: return 'bg-danger'
    case 'Pending':
    case 0: return 'bg-warning'
    case 'Cancelled':
    case 3: return 'bg-secondary'
    default: return 'bg-secondary'
  }
}

const getStatusText = (status) => {
  // Gérer les valeurs numériques (enum) et les chaînes
  switch (status) {
    case 'Approved':
    case 1: return 'Approuvée'
    case 'Rejected': 
    case 2: return 'Rejetée'
    case 'Pending':
    case 0: return 'En attente'
    case 'Cancelled':
    case 3: return 'Annulée'
    default: return status
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

onMounted(() => {
  fetchMyRequests()
  fetchHolidays()
})
</script>

<style scoped>
/* Réutiliser les styles du dashboard admin */
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

.border {
  border: 1px solid #e3e6f0 !important;
}

.border:hover {
  border-color: #d1d3e2 !important;
  background-color: #f8f9fc;
}
</style>
