<template>
  <div>
    <AppNavbar />
    <div class="container-fluid mt-4">
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-header py-3">
              <h6 class="m-0 font-weight-bold text-primary">
                <i class="fas fa-history me-2"></i>
                Historique des Approbations
              </h6>
            </div>
            <div class="card-body">
              <!-- Statistiques -->
              <div class="row mb-4">
                <div class="col-xl-3 col-md-6 mb-4">
                  <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                      <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                          <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                            Total Approuvées
                          </div>
                          <div class="h5 mb-0 font-weight-bold text-gray-800">
                            {{ stats.approved }}
                          </div>
                        </div>
                        <div class="col-auto">
                          <i class="fas fa-check fa-2x text-gray-300"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                
                <div class="col-xl-3 col-md-6 mb-4">
                  <div class="card border-left-danger shadow h-100 py-2">
                    <div class="card-body">
                      <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                          <div class="text-xs font-weight-bold text-danger text-uppercase mb-1">
                            Total Rejetées
                          </div>
                          <div class="h5 mb-0 font-weight-bold text-gray-800">
                            {{ stats.rejected }}
                          </div>
                        </div>
                        <div class="col-auto">
                          <i class="fas fa-times fa-2x text-gray-300"></i>
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
                            {{ stats.leaveApproved }}
                          </div>
                        </div>
                        <div class="col-auto">
                          <i class="fas fa-calendar-check fa-2x text-gray-300"></i>
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
                            Frais Approuvés
                          </div>
                          <div class="h5 mb-0 font-weight-bold text-gray-800">
                            {{ stats.expenseApproved }}
                          </div>
                        </div>
                        <div class="col-auto">
                          <i class="fas fa-receipt fa-2x text-gray-300"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Filtres -->
              <div class="row mb-3">
                <div class="col-md-3">
                  <select v-model="filters.type" class="form-select">
                    <option value="">Tous les types</option>
                    <option value="leave">Demandes de congé</option>
                    <option value="expense">Notes de frais</option>
                  </select>
                </div>
                <div class="col-md-3">
                  <select v-model="filters.status" class="form-select">
                    <option value="">Tous les statuts</option>
                    <option value="1">Approuvées</option>
                    <option value="2">Rejetées</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <input 
                    type="text" 
                    v-model="filters.search" 
                    class="form-control" 
                    placeholder="Rechercher par employé ou type..."
                  >
                </div>
              </div>

              <!-- Loading -->
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="mt-2 text-muted">Chargement de l'historique...</p>
              </div>

              <!-- Table -->
              <div v-else-if="paginatedRequests.length > 0" class="table-responsive">
                <table class="table table-bordered">
                  <thead class="table-light">
                    <tr>
                      <th>Employé</th>
                      <th>Type de demande</th>
                      <th>Date de demande</th>
                      <th>Date d'approbation</th>
                      <th>Statut</th>
                      <th>Commentaire</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="request in paginatedRequests" :key="`${request.type}-${request.id}`">
                      <td>
                        <div class="d-flex align-items-center">
                          <div class="avatar me-3">
                            <i class="fas fa-user-circle fa-2x text-muted"></i>
                          </div>
                          <div>
                            <div class="fw-bold">{{ request.requester?.fullName }}</div>
                            <small class="text-muted">{{ request.requester?.email }}</small>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div>
                          <i :class="request.type === 'leave' ? 'fas fa-calendar-alt text-success' : 'fas fa-receipt text-info'" class="me-2"></i>
                          <strong>{{ request.type === 'leave' ? 'Congé' : 'Note de frais' }}</strong>
                        </div>
                        <div class="text-muted small">
                          {{ request.type === 'leave' ? request.leaveType?.name : request.expenseType?.name }}
                        </div>
                        <div v-if="request.type === 'leave'" class="text-muted small">
                          Du {{ formatDate(request.startDate) }} au {{ formatDate(request.endDate) }}
                        </div>
                        <div v-else class="text-muted small">
                          {{ formatAmount(request.amount) }}
                        </div>
                      </td>
                      <td>
                        <span class="text-muted">{{ formatDate(request.createdAt) }}</span>
                      </td>
                      <td>
                        <span class="text-muted">{{ formatDate(request.reviewedAt) }}</span>
                      </td>
                      <td>
                        <span 
                          class="badge"
                          :class="getStatusBadgeClass(request.status)"
                        >
                          <i :class="getStatusIcon(request.status)" class="me-1"></i>
                          {{ getStatusText(request.status) }}
                        </span>
                      </td>
                      <td>
                        <span v-if="request.reviewComment" class="text-muted">
                          {{ request.reviewComment }}
                        </span>
                        <span v-else class="text-muted fst-italic">
                          Aucun commentaire
                        </span>
                      </td>
                      <td>
                        <button 
                          class="btn btn-sm btn-outline-primary"
                          @click="showDetails(request)"
                          title="Voir les détails"
                        >
                          <i class="fas fa-eye"></i>
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- No data -->
              <div v-else class="text-center py-5">
                <i class="fas fa-history fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune approbation trouvée</h5>
                <p class="text-muted">Vous n'avez encore approuvé ou rejeté aucune demande.</p>
              </div>

              <!-- Pagination -->
              <div v-if="totalPages > 1" class="d-flex justify-content-center mt-4">
                <nav>
                  <ul class="pagination">
                    <li class="page-item" :class="{ disabled: currentPage === 1 }">
                      <button class="page-link" @click="currentPage = 1" :disabled="currentPage === 1">
                        <i class="fas fa-angle-double-left"></i>
                      </button>
                    </li>
                    <li class="page-item" :class="{ disabled: currentPage === 1 }">
                      <button class="page-link" @click="currentPage--" :disabled="currentPage === 1">
                        <i class="fas fa-angle-left"></i>
                      </button>
                    </li>
                    <li 
                      v-for="page in visiblePages" 
                      :key="page" 
                      class="page-item" 
                      :class="{ active: page === currentPage }"
                    >
                      <button class="page-link" @click="currentPage = page">
                        {{ page }}
                      </button>
                    </li>
                    <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                      <button class="page-link" @click="currentPage++" :disabled="currentPage === totalPages">
                        <i class="fas fa-angle-right"></i>
                      </button>
                    </li>
                    <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                      <button class="page-link" @click="currentPage = totalPages" :disabled="currentPage === totalPages">
                        <i class="fas fa-angle-double-right"></i>
                      </button>
                    </li>
                  </ul>
                </nav>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de détails -->
    <div class="modal fade" id="detailsModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i :class="selectedRequest?.type === 'leave' ? 'fas fa-calendar-alt' : 'fas fa-receipt'"></i>
              Détails de l'approbation
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body" v-if="selectedRequest">
            <div class="row">
              <div class="col-md-6">
                <strong>Employé :</strong>
                {{ selectedRequest.requester?.fullName }}
              </div>
              <div class="col-md-6">
                <strong>Type :</strong>
                {{ selectedRequest.type === 'leave' ? 'Demande de congé' : 'Note de frais' }}
              </div>
            </div>
            
            <hr>
            
            <!-- Détails spécifiques -->
            <div v-if="selectedRequest.type === 'leave'">
              <div class="row mb-3">
                <div class="col-md-6">
                  <strong>Type de congé :</strong> {{ selectedRequest.leaveType?.name }}
                </div>
                <div class="col-md-6">
                  <strong>Durée :</strong> {{ calculateDuration(selectedRequest.startDate, selectedRequest.endDate) }}
                </div>
              </div>
              <div class="row mb-3">
                <div class="col-md-6">
                  <strong>Date de début :</strong> {{ formatDate(selectedRequest.startDate) }}
                </div>
                <div class="col-md-6">
                  <strong>Date de fin :</strong> {{ formatDate(selectedRequest.endDate) }}
                </div>
              </div>
              <div v-if="selectedRequest.reason" class="mb-3">
                <strong>Motif :</strong>
                <p class="mt-2">{{ selectedRequest.reason }}</p>
              </div>
            </div>

            <div v-else>
              <div class="row mb-3">
                <div class="col-md-6">
                  <strong>Type de frais :</strong> {{ selectedRequest.expenseType?.name }}
                </div>
                <div class="col-md-6">
                  <strong>Montant :</strong> {{ formatAmount(selectedRequest.amount) }}
                </div>
              </div>
              <div v-if="selectedRequest.description" class="mb-3">
                <strong>Description :</strong>
                <p class="mt-2">{{ selectedRequest.description }}</p>
              </div>
            </div>

            <hr>

            <!-- Informations d'approbation -->
            <div class="row mb-3">
              <div class="col-md-6">
                <strong>Statut :</strong>
                <span 
                  class="badge ms-2"
                  :class="getStatusBadgeClass(selectedRequest.status)"
                >
                  {{ getStatusText(selectedRequest.status) }}
                </span>
              </div>
              <div class="col-md-6">
                <strong>Date d'approbation :</strong> {{ formatDate(selectedRequest.reviewedAt) }}
              </div>
            </div>
            
            <div v-if="selectedRequest.reviewComment" class="mb-3">
              <strong>Votre commentaire :</strong>
              <p class="mt-2 p-3 bg-light rounded">{{ selectedRequest.reviewComment }}</p>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
              Fermer
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { Modal } from 'bootstrap'
import { ref, onMounted, computed, watch } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

// State
const loading = ref(true)
const allRequests = ref([])
const selectedRequest = ref(null)
const currentPage = ref(1)
const itemsPerPage = 10

// Filters
const filters = ref({
  type: '',
  status: '',
  search: ''
})

// Computed
const filteredRequests = computed(() => {
  let filtered = [...allRequests.value]
  
  if (filters.value.type) {
    filtered = filtered.filter(r => r.type === filters.value.type)
  }
  
  if (filters.value.status) {
    // Convertir le filtre en nombre pour la comparaison
    const statusValue = parseInt(filters.value.status)
    filtered = filtered.filter(r => r.status === statusValue)
  }
  
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    filtered = filtered.filter(r => {
      const typeName = r.type === 'leave' ? r.leaveType?.name : r.expenseType?.name
      const employeeName = r.requester?.fullName || ''
      const employeeEmail = r.requester?.email || ''
      return typeName?.toLowerCase().includes(search) ||
             employeeName.toLowerCase().includes(search) ||
             employeeEmail.toLowerCase().includes(search) ||
             r.description?.toLowerCase().includes(search) ||
             r.reason?.toLowerCase().includes(search)
    })
  }
  
  return filtered.sort((a, b) => new Date(b.reviewedAt) - new Date(a.reviewedAt))
})

const paginatedRequests = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredRequests.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredRequests.value.length / itemsPerPage)
})

const visiblePages = computed(() => {
  const pages = []
  const start = Math.max(1, currentPage.value - 2)
  const end = Math.min(totalPages.value, currentPage.value + 2)
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

const stats = computed(() => {
  const total = allRequests.value.length
  const approved = allRequests.value.filter(r => r.status === 1).length // Approved
  const rejected = allRequests.value.filter(r => r.status === 2).length // Rejected
  const leaveApproved = allRequests.value.filter(r => r.type === 'leave' && r.status === 1).length // Approved
  const expenseApproved = allRequests.value.filter(r => r.type === 'expense' && r.status === 1).length // Approved
  
  return { total, approved, rejected, leaveApproved, expenseApproved }
})

// Methods
const fetchApprovalHistory = async () => {
  try {
    loading.value = true
    
    const [leaveResponse, expenseResponse] = await Promise.all([
      apiService.getApprovedLeaveRequests().catch(() => ({ data: [] })),
      apiService.getApprovedExpenseRequests().catch(() => ({ data: [] }))
    ])
    
    const leaveRequests = (leaveResponse.data || [])
      .filter(r => r.status === 1 || r.status === 2) // Approved or Rejected
      .map(r => ({ ...r, type: 'leave' }))
    
    const expenseRequests = (expenseResponse.data || [])
      .filter(r => r.status === 'Approved' || r.status === 'Rejected')
      .map(r => ({ ...r, type: 'expense' }))
    
    allRequests.value = [...leaveRequests, ...expenseRequests]
    
  } catch (error) {
    console.error('Erreur lors de la récupération de l\'historique:', error)
  } finally {
    loading.value = false
  }
}

const showDetails = (request) => {
  selectedRequest.value = request
  const modal = new Modal(document.getElementById('detailsModal'))
  modal.show()
}

const getStatusBadgeClass = (status) => {
  // Gérer les valeurs numériques (enum) et les chaînes
  switch (status) {
    case 'Approved':
    case 1: return 'bg-success'
    case 'Rejected':
    case 2: return 'bg-danger'
    case 'Pending':
    case 0: return 'bg-warning text-dark'
    case 'Cancelled':
    case 3: return 'bg-secondary'
    default: return 'bg-secondary'
  }
}

const getStatusIcon = (status) => {
  // Gérer les valeurs numériques (enum) et les chaînes
  switch (status) {
    case 'Approved':
    case 1: return 'fas fa-check'
    case 'Rejected':
    case 2: return 'fas fa-times'
    case 'Pending':
    case 0: return 'fas fa-clock'
    case 'Cancelled':
    case 3: return 'fas fa-ban'
    default: return 'fas fa-question'
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
  return `${diffDays} jour(s)`
}

// Watchers
watch([() => filters.value.type, () => filters.value.status, () => filters.value.search], () => {
  currentPage.value = 1
})

onMounted(() => {
  fetchApprovalHistory()
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

.border-left-danger {
  border-left: 0.25rem solid #e74a3b !important;
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

.avatar {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.table th {
  background-color: #f8f9fc;
  border-color: #e3e6f0;
  font-size: 0.875rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.page-link {
  color: #858796;
  border-color: #dddfeb;
}

.page-item.active .page-link {
  background-color: #5a5c69;
  border-color: #5a5c69;
}

.page-link:hover {
  color: #5a5c69;
  background-color: #eaecf4;
  border-color: #dddfeb;
}
</style>
