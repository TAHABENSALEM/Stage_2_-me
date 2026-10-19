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
              <i class="fas fa-history me-2 text-primary"></i>
              Historique de mes Demandes
            </h2>
          </div>
        </div>
      </div>
      
      <!-- Filtres -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-body">
              <div class="row">
                <div class="col-md-4 mb-3">
                  <label for="typeFilter" class="form-label">Type de demande</label>
                  <select class="form-select" id="typeFilter" v-model="filters.type">
                    <option value="">Tous les types</option>
                    <option value="leave">Demandes de congé</option>
                    <option value="expense">Notes de frais</option>
                  </select>
                </div>
                <div class="col-md-4 mb-3">
                  <label for="statusFilter" class="form-label">Statut</label>
                  <select class="form-select" id="statusFilter" v-model="filters.status">
                    <option value="">Tous les statuts</option>
                    <option value="0">En attente</option>
                    <option value="1">Approuvée</option>
                    <option value="2">Rejetée</option>
                    <option value="3">Annulée</option>
                  </select>
                </div>
                <div class="col-md-4 mb-3">
                  <label for="searchFilter" class="form-label">Recherche</label>
                  <input
                    type="text"
                    class="form-control"
                    id="searchFilter"
                    v-model="filters.search"
                    placeholder="Rechercher..."
                  >
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Liste des demandes -->
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement de l'historique...</p>
              </div>
              
              <div v-else-if="filteredRequests.length === 0" class="text-center py-4">
                <i class="fas fa-file-alt fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune demande trouvée</h5>
                <p class="text-muted">
                  {{ allRequests.length === 0 ? 'Vous n\'avez encore fait aucune demande.' : 'Aucune demande ne correspond à vos critères de recherche.' }}
                </p>
              </div>
              
              <div v-else>
                <!-- Statistiques -->
                <div class="row mb-4">
                  <div class="col-md-3">
                    <div class="stat-item text-center">
                      <div class="stat-number text-primary">{{ stats.total }}</div>
                      <div class="stat-label">Total</div>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="stat-item text-center">
                      <div class="stat-number text-warning">{{ stats.pending }}</div>
                      <div class="stat-label">En attente</div>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="stat-item text-center">
                      <div class="stat-number text-success">{{ stats.approved }}</div>
                      <div class="stat-label">Approuvées</div>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="stat-item text-center">
                      <div class="stat-number text-danger">{{ stats.rejected }}</div>
                      <div class="stat-label">Rejetées</div>
                    </div>
                  </div>
                </div>
                
                <hr>
                
                <!-- Liste -->
                <div class="table-responsive">
                  <table class="table table-hover">
                    <thead class="table-dark">
                      <tr>
                        <th>Type</th>
                        <th>Détails</th>
                        <th>Date de création</th>
                        <th>Statut</th>
                        <th>Révisé par</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="request in paginatedRequests" :key="`${request.type}-${request.id}`">
                        <td>
                          <div class="d-flex align-items-center">
                            <i 
                              :class="request.type === 'leave' ? 'fas fa-calendar-alt text-success' : 'fas fa-receipt text-info'"
                              class="me-2"
                            ></i>
                            <span class="fw-bold">
                              {{ request.type === 'leave' ? 'Congé' : 'Frais' }}
                            </span>
                          </div>
                        </td>
                        <td>
                          <div>
                            <div class="fw-bold">
                              {{ request.type === 'leave' ? request.leaveType?.name : request.expenseType?.name }}
                            </div>
                            <small class="text-muted">
                              <span v-if="request.type === 'leave'">
                                Du {{ formatDate(request.startDate) }} au {{ formatDate(request.endDate) }}
                              </span>
                              <span v-else>
                                {{ formatAmount(request.amount) }}
                              </span>
                            </small>
                          </div>
                        </td>
                        <td>
                          <span class="text-muted">{{ formatDate(request.createdAt) }}</span>
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
                          <span v-if="request.reviewedBy" class="text-muted">
                            {{ request.reviewedBy.fullName }}
                            <br>
                            <small>{{ formatDate(request.reviewedAt) }}</small>
                          </span>
                          <span v-else class="text-muted fst-italic">
                            En attente
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
                      <li v-for="page in visiblePages" :key="page" class="page-item" :class="{ active: page === currentPage }">
                        <button class="page-link" @click="currentPage = page">{{ page }}</button>
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
    </div>
    
    <!-- Modal Détails -->
    <div class="modal fade" id="detailsModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i :class="selectedRequest?.type === 'leave' ? 'fas fa-calendar-alt' : 'fas fa-receipt'"></i>
              Détails de la demande
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body" v-if="selectedRequest">
            <div class="row">
              <div class="col-md-6">
                <strong>Type :</strong>
                {{ selectedRequest.type === 'leave' ? 'Demande de congé' : 'Note de frais' }}
              </div>
              <div class="col-md-6">
                <strong>Statut :</strong>
                <span 
                  class="badge ms-2"
                  :class="getStatusBadgeClass(selectedRequest.status)"
                >
                  {{ getStatusText(selectedRequest.status) }}
                </span>
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
              <div class="mb-3">
                <strong>Description :</strong>
                <p class="mt-2">{{ selectedRequest.description }}</p>
              </div>
            </div>
            
            <!-- Informations de révision -->
            <div v-if="selectedRequest.reviewedBy" class="mt-4 p-3 bg-light rounded">
              <h6>Informations de révision</h6>
              <div class="row">
                <div class="col-md-6">
                  <strong>Révisé par :</strong> {{ selectedRequest.reviewedBy.fullName }}
                </div>
                <div class="col-md-6">
                  <strong>Date de révision :</strong> {{ formatDate(selectedRequest.reviewedAt) }}
                </div>
              </div>
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
      return typeName?.toLowerCase().includes(search) ||
             r.description?.toLowerCase().includes(search) ||
             r.reason?.toLowerCase().includes(search)
    })
  }
  
  return filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
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
  const pending = allRequests.value.filter(r => r.status === 0).length // Pending
  const approved = allRequests.value.filter(r => r.status === 1).length // Approved
  const rejected = allRequests.value.filter(r => r.status === 2).length // Rejected
  
  return { total, pending, approved, rejected }
})

// Methods
const fetchRequests = async () => {
  try {
    loading.value = true
    
    const [leaveResponse, expenseResponse] = await Promise.all([
      apiService.getMyLeaveRequests().catch(() => ({ data: [] })),
      apiService.getMyExpenseRequests().catch(() => ({ data: [] }))
    ])
    
    const leaveRequests = (leaveResponse.data || []).map(r => ({ ...r, type: 'leave' }))
    const expenseRequests = (expenseResponse.data || []).map(r => ({ ...r, type: 'expense' }))
    
    allRequests.value = [...leaveRequests, ...expenseRequests]
    
  } catch (error) {
    console.error('Erreur lors de la récupération des demandes:', error)
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
  return `${diffDays} jour${diffDays > 1 ? 's' : ''}`
}

// Watchers
watch(filteredRequests, () => {
  currentPage.value = 1
})

onMounted(() => {
  fetchRequests()
})
</script>

<style scoped>
.stat-item {
  padding: 15px;
}

.stat-number {
  font-size: 2rem;
  font-weight: bold;
}

.stat-label {
  font-size: 0.9rem;
  color: #6c757d;
}

.table-responsive {
  border-radius: 0.5rem;
  overflow: hidden;
}

.pagination .page-link {
  border: none;
  color: #495057;
}

.pagination .page-item.active .page-link {
  background-color: #007bff;
  border-color: #007bff;
}

.badge {
  font-size: 0.75em;
}

.modal-content {
  border: none;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.card {
  border: none;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}
</style>
