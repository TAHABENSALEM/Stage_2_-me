<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex align-items-center mb-4">
            <router-link to="/employee" class="btn btn-outline-secondary me-3">
              <i class="fas fa-arrow-left"></i>
            </router-link>
            <h2>
              <i class="fas fa-tasks me-2 text-warning"></i>
              Demandes à Approuver
            </h2>
            <span v-if="pendingCount > 0" class="badge bg-warning text-dark ms-3 fs-6">
              {{ pendingCount }} en attente
            </span>
          </div>
        </div>
      </div>
      
      <!-- Onglets -->
      <div class="row mb-4">
        <div class="col-12">
          <ul class="nav nav-tabs" id="requestTabs" role="tablist">
            <li class="nav-item" role="presentation">
              <button 
                class="nav-link active" 
                id="leave-tab" 
                data-bs-toggle="tab" 
                data-bs-target="#leave-pane" 
                type="button"
              >
                <i class="fas fa-calendar-alt me-2"></i>
                Demandes de Congé
                <span v-if="pendingLeaveRequests.length > 0" class="badge bg-warning text-dark ms-2">
                  {{ pendingLeaveRequests.length }}
                </span>
              </button>
            </li>
            <li class="nav-item" role="presentation">
              <button 
                class="nav-link" 
                id="expense-tab" 
                data-bs-toggle="tab" 
                data-bs-target="#expense-pane" 
                type="button"
              >
                <i class="fas fa-receipt me-2"></i>
                Notes de Frais
                <span v-if="pendingExpenseRequests.length > 0" class="badge bg-warning text-dark ms-2">
                  {{ pendingExpenseRequests.length }}
                </span>
              </button>
            </li>
          </ul>
        </div>
      </div>
      
      <div class="tab-content" id="requestTabContent">
        <!-- Demandes de Congé -->
        <div class="tab-pane fade show active" id="leave-pane" role="tabpanel">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loadingLeave" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement des demandes de congé...</p>
              </div>
              
              <div v-else-if="pendingLeaveRequests.length === 0" class="text-center py-4">
                <i class="fas fa-calendar-check fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune demande de congé en attente</h5>
                <p class="text-muted">Toutes les demandes de congé ont été traitées.</p>
              </div>
              
              <div v-else>
                <div v-for="request in pendingLeaveRequests" :key="request.id" class="mb-4">
                  <div class="card border-warning">
                    <div class="card-header bg-warning text-dark">
                      <div class="d-flex justify-content-between align-items-center">
                        <h6 class="mb-0">
                          <i class="fas fa-calendar-alt me-2"></i>
                          Demande de {{ request.leaveType?.name }}
                        </h6>
                        <small>
                          Soumise le {{ formatDate(request.createdAt) }}
                        </small>
                      </div>
                    </div>
                    <div class="card-body">
                      <div class="row">
                        <div class="col-md-8">
                          <div class="row mb-3">
                            <div class="col-sm-6">
                              <strong>Demandeur :</strong>
                              <div>{{ request.requester?.fullName }}</div>
                              <small class="text-muted">{{ request.requester?.email }}</small>
                            </div>
                            <div class="col-sm-6">
                              <strong>Période :</strong>
                              <div>Du {{ formatDate(request.startDate) }}</div>
                              <div>Au {{ formatDate(request.endDate) }}</div>
                              <small class="text-info">
                                ({{ calculateDuration(request.startDate, request.endDate) }})
                              </small>
                            </div>
                          </div>
                          
                          <div v-if="request.reason" class="mb-3">
                            <strong>Motif :</strong>
                            <p class="mt-1 mb-0">{{ request.reason }}</p>
                          </div>
                        </div>
                        <div class="col-md-4 text-end">
                          <div class="d-grid gap-2">
                            <button 
                              class="btn btn-success"
                              @click="handleLeaveRequest(request.id, 'Approved')"
                              :disabled="processingRequests.has(request.id)"
                            >
                              <span v-if="processingRequests.has(request.id)">
                                <i class="fas fa-spinner fa-spin me-2"></i>
                                Traitement...
                              </span>
                              <span v-else>
                                <i class="fas fa-check me-2"></i>
                                Approuver
                              </span>
                            </button>
                            <button 
                              class="btn btn-danger"
                              @click="handleLeaveRequest(request.id, 'Rejected')"
                              :disabled="processingRequests.has(request.id)"
                            >
                              <span v-if="processingRequests.has(request.id)">
                                <i class="fas fa-spinner fa-spin me-2"></i>
                                Traitement...
                              </span>
                              <span v-else>
                                <i class="fas fa-times me-2"></i>
                                Rejeter
                              </span>
                            </button>
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
        
        <!-- Notes de Frais -->
        <div class="tab-pane fade" id="expense-pane" role="tabpanel">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loadingExpense" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement des notes de frais...</p>
              </div>
              
              <div v-else-if="pendingExpenseRequests.length === 0" class="text-center py-4">
                <i class="fas fa-receipt fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune note de frais en attente</h5>
                <p class="text-muted">Toutes les notes de frais ont été traitées.</p>
              </div>
              
              <div v-else>
                <div v-for="request in pendingExpenseRequests" :key="request.id" class="mb-4">
                  <div class="card border-info">
                    <div class="card-header bg-info text-white">
                      <div class="d-flex justify-content-between align-items-center">
                        <h6 class="mb-0">
                          <i class="fas fa-receipt me-2"></i>
                          Note de frais - {{ request.expenseType?.name }}
                        </h6>
                        <small>
                          Soumise le {{ formatDate(request.createdAt) }}
                        </small>
                      </div>
                    </div>
                    <div class="card-body">
                      <div class="row">
                        <div class="col-md-8">
                          <div class="row mb-3">
                            <div class="col-sm-6">
                              <strong>Demandeur :</strong>
                              <div>{{ request.requester?.fullName }}</div>
                              <small class="text-muted">{{ request.requester?.email }}</small>
                            </div>
                            <div class="col-sm-6">
                              <strong>Montant :</strong>
                              <div class="h5 text-info mb-0">
                                {{ formatAmount(request.amount) }}
                              </div>
                            </div>
                          </div>
                          
                          <div class="mb-3">
                            <strong>Description :</strong>
                            <p class="mt-1 mb-0">{{ request.description }}</p>
                          </div>
                        </div>
                        <div class="col-md-4 text-end">
                          <div class="d-grid gap-2">
                            <button 
                              class="btn btn-success"
                              @click="handleExpenseRequest(request.id, 'Approved')"
                              :disabled="processingRequests.has(request.id)"
                            >
                              <span v-if="processingRequests.has(request.id)">
                                <i class="fas fa-spinner fa-spin me-2"></i>
                                Traitement...
                              </span>
                              <span v-else>
                                <i class="fas fa-check me-2"></i>
                                Approuver
                              </span>
                            </button>
                            <button 
                              class="btn btn-danger"
                              @click="handleExpenseRequest(request.id, 'Rejected')"
                              :disabled="processingRequests.has(request.id)"
                            >
                              <span v-if="processingRequests.has(request.id)">
                                <i class="fas fa-spinner fa-spin me-2"></i>
                                Traitement...
                              </span>
                              <span v-else">
                                <i class="fas fa-times me-2"></i>
                                Rejeter
                              </span>
                            </button>
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
      
      <!-- Toast de notification -->
      <div class="toast-container position-fixed bottom-0 end-0 p-3">
        <div id="successToast" class="toast" role="alert">
          <div class="toast-header">
            <i class="fas fa-check-circle text-success me-2"></i>
            <strong class="me-auto">Succès</strong>
            <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
          </div>
          <div class="toast-body">
            {{ toastMessage }}
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
import { Toast } from 'bootstrap'

// State
const loadingLeave = ref(true)
const loadingExpense = ref(true)
const pendingLeaveRequests = ref([])
const pendingExpenseRequests = ref([])
const processingRequests = ref(new Set())
const toastMessage = ref('')

// Computed
const pendingCount = computed(() => {
  return pendingLeaveRequests.value.length + pendingExpenseRequests.value.length
})

// Methods
const fetchPendingLeaveRequests = async () => {
  try {
    loadingLeave.value = true
    const response = await apiService.getPendingLeaveRequests()
    pendingLeaveRequests.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des demandes de congé:', error)
  } finally {
    loadingLeave.value = false
  }
}

const fetchPendingExpenseRequests = async () => {
  try {
    loadingExpense.value = true
    const response = await apiService.getPendingExpenseRequests()
    pendingExpenseRequests.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des notes de frais:', error)
  } finally {
    loadingExpense.value = false
  }
}

const handleLeaveRequest = async (requestId, action) => {
  if (processingRequests.value.has(requestId)) return
  
  try {
    processingRequests.value.add(requestId)
    
    // Convertir l'action en format attendu par le backend
    const actionData = {
      Approve: action === 'Approved',
      Comment: ''
    }
    
    await apiService.updateLeaveRequest(requestId, actionData)
    
    // Supprimer la demande de la liste
    pendingLeaveRequests.value = pendingLeaveRequests.value.filter(r => r.id !== requestId)
    
    // Afficher un toast de succès
    showToast(`Demande ${action === 'Approved' ? 'approuvée' : 'rejetée'} avec succès`)
    
  } catch (error) {
    console.error('Erreur lors du traitement de la demande:', error)
    showToast('Erreur lors du traitement de la demande', 'error')
  } finally {
    processingRequests.value.delete(requestId)
  }
}

const handleExpenseRequest = async (requestId, action) => {
  if (processingRequests.value.has(requestId)) return
  
  try {
    processingRequests.value.add(requestId)
    
    // Convertir l'action en format attendu par le backend
    const actionData = {
      Approve: action === 'Approved',
      Comment: ''
    }
    
    await apiService.updateExpenseRequest(requestId, actionData)
    
    // Supprimer la demande de la liste
    pendingExpenseRequests.value = pendingExpenseRequests.value.filter(r => r.id !== requestId)
    
    // Afficher un toast de succès
    showToast(`Note de frais ${action === 'Approved' ? 'approuvée' : 'rejetée'} avec succès`)
    
  } catch (error) {
    console.error('Erreur lors du traitement de la demande:', error)
    showToast('Erreur lors du traitement de la demande', 'error')
  } finally {
    processingRequests.value.delete(requestId)
  }
}

const showToast = (message, type = 'success') => {
  toastMessage.value = message
  const toastElement = document.getElementById('successToast')
  const toast = new Toast(toastElement)
  toast.show()
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const formatAmount = (amount) => {
  if (!amount) return '0,00 €'
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'EUR'
  }).format(amount)
}

const calculateDuration = (startDate, endDate) => {
  if (!startDate || !endDate) return ''
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end - start)
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1
  return `${diffDays} jour${diffDays > 1 ? 's' : ''}`
}

onMounted(() => {
  fetchPendingLeaveRequests()
  fetchPendingExpenseRequests()
})
</script>

<style scoped>
.card {
  border: none;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.nav-tabs .nav-link {
  border: none;
  color: #6c757d;
  font-weight: 500;
}

.nav-tabs .nav-link.active {
  background-color: #fff;
  border-color: #dee2e6 #dee2e6 #fff;
  color: #495057;
  font-weight: 600;
}

.border-warning {
  border-color: #ffc107 !important;
  border-width: 2px !important;
}

.border-info {
  border-color: #17a2b8 !important;
  border-width: 2px !important;
}

.card-header {
  font-weight: 600;
}

.btn {
  font-weight: 600;
}

.toast-container {
  z-index: 1060;
}

.badge {
  font-size: 0.75em;
}
</style>
