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
              <i class="fas fa-gavel me-2 text-primary"></i>
              Historique de mes Décisions
            </h2>
          </div>
        </div>
      </div>
      
      <!-- Statistiques -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card shadow-sm">
            <div class="card-body">
              <div class="row">
                <div class="col-md-3">
                  <div class="stat-item text-center">
                    <div class="stat-number text-primary">{{ stats.total }}</div>
                    <div class="stat-label">Total Décisions</div>
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
                <div class="col-md-3">
                  <div class="stat-item text-center">
                    <div class="stat-number text-info">{{ stats.recent }}</div>
                    <div class="stat-label">Cette semaine</div>
                  </div>
                </div>
              </div>
            </div>
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
                  <label for="statusFilter" class="form-label">Décision</label>
                  <select class="form-select" id="statusFilter" v-model="filters.status">
                    <option value="">Toutes les décisions</option>
                    <option value="1">Approuvées</option>
                    <option value="2">Rejetées</option>
                  </select>
                </div>
                <div class="col-md-4 mb-3">
                  <label for="searchFilter" class="form-label">Recherche</label>
                  <input
                    type="text"
                    class="form-control"
                    id="searchFilter"
                    v-model="filters.search"
                    placeholder="Nom d'employé, type..."
                  >
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Liste des décisions -->
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement de l'historique des décisions...</p>
              </div>
              
              <div v-else-if="filteredDecisions.length === 0" class="text-center py-4">
                <i class="fas fa-gavel fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucune décision trouvée</h5>
                <p class="text-muted">
                  {{ allDecisions.length === 0 ? 'Vous n\'avez encore pris aucune décision.' : 'Aucune décision ne correspond à vos critères de recherche.' }}
                </p>
              </div>
              
              <div v-else>
                <!-- Liste -->
                <div class="table-responsive">
                  <table class="table table-hover">
                    <thead class="table-dark">
                      <tr>
                        <th>Type</th>
                        <th>Employé</th>
                        <th>Détails</th>
                        <th>Date de décision</th>
                        <th>Ma décision</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="decision in paginatedDecisions" :key="`${decision.type}-${decision.id}`">
                        <td>
                          <div class="d-flex align-items-center">
                            <i 
                              :class="decision.type === 'leave' ? 'fas fa-calendar-alt text-success' : 'fas fa-receipt text-info'"
                              class="me-2"
                            ></i>
                            <span class="fw-bold">
                              {{ decision.type === 'leave' ? 'Congé' : 'Frais' }}
                            </span>
                          </div>
                        </td>
                        <td>
                          <div>
                            <div class="fw-bold">{{ decision.requester?.fullName }}</div>
                            <small class="text-muted">{{ decision.requester?.email }}</small>
                          </div>
                        </td>
                        <td>
                          <div>
                            <div class="fw-bold">
                              {{ decision.type === 'leave' ? decision.leaveType?.name : decision.expenseType?.name }}
                            </div>
                            <small class="text-muted">
                              <span v-if="decision.type === 'leave'">
                                Du {{ formatDate(decision.startDate) }} au {{ formatDate(decision.endDate) }}
                              </span>
                              <span v-else>
                                {{ formatAmount(decision.amount) }}
                              </span>
                            </small>
                          </div>
                        </td>
                        <td>
                          <span class="text-muted">{{ formatDate(decision.reviewedAt) }}</span>
                        </td>
                        <td>
                          <span 
                            class="badge"
                            :class="getStatusBadgeClass(decision.status)"
                          >
                            <i :class="getStatusIcon(decision.status)" class="me-1"></i>
                            {{ getStatusText(decision.status) }}
                          </span>
                        </td>
                        <td>
                          <button 
                            class="btn btn-sm btn-outline-primary"
                            @click="showDetails(decision)"
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

    <!-- Modal des détails -->
    <div class="modal fade" id="detailsModal" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i class="fas fa-info-circle me-2"></i>
              Détails de la décision
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body" v-if="selectedDecision">
            <div class="row">
              <div class="col-md-6">
                <h6>Informations sur l'employé</h6>
                <p><strong>Nom :</strong> {{ selectedDecision.requester?.fullName }}</p>
                <p><strong>Email :</strong> {{ selectedDecision.requester?.email }}</p>
              </div>
              <div class="col-md-6">
                <h6>Type de demande</h6>
                <p><strong>Catégorie :</strong> {{ selectedDecision.type === 'leave' ? 'Demande de congé' : 'Note de frais' }}</p>
                <p><strong>Type :</strong> {{ selectedDecision.type === 'leave' ? selectedDecision.leaveType?.name : selectedDecision.expenseType?.name }}</p>
              </div>
            </div>
            
            <hr>
            
            <div class="row" v-if="selectedDecision.type === 'leave'">
              <div class="col-md-6">
                <h6>Période de congé</h6>
                <p><strong>Début :</strong> {{ formatDate(selectedDecision.startDate) }}</p>
                <p><strong>Fin :</strong> {{ formatDate(selectedDecision.endDate) }}</p>
                <p><strong>Durée :</strong> {{ calculateDuration(selectedDecision.startDate, selectedDecision.endDate) }}</p>
              </div>
              <div class="col-md-6">
                <h6>Motif</h6>
                <p>{{ selectedDecision.reason || 'Aucun motif spécifié' }}</p>
              </div>
            </div>
            
            <div class="row" v-if="selectedDecision.type === 'expense'">
              <div class="col-md-6">
                <h6>Montant</h6>
                <p class="h4 text-info">{{ formatAmount(selectedDecision.amount) }}</p>
              </div>
              <div class="col-md-6">
                <h6>Description</h6>
                <p>{{ selectedDecision.description || 'Aucune description' }}</p>
              </div>
            </div>
            
            <hr>
            
            <div class="row">
              <div class="col-md-6">
                <h6>Ma décision</h6>
                <span 
                  class="badge fs-6"
                  :class="getStatusBadgeClass(selectedDecision.status)"
                >
                  {{ getStatusText(selectedDecision.status) }}
                </span>
              </div>
              <div class="col-md-6">
                <h6>Date de décision</h6>
                <p>{{ formatDate(selectedDecision.reviewedAt) }}</p>
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
const allDecisions = ref([])
const selectedDecision = ref(null)
const currentPage = ref(1)
const itemsPerPage = 10

// Filters
const filters = ref({
  type: '',
  status: '',
  search: ''
})

// Computed
const filteredDecisions = computed(() => {
  let filtered = [...allDecisions.value]
  
  if (filters.value.type) {
    filtered = filtered.filter(d => d.type === filters.value.type)
  }
  
  if (filters.value.status) {
    const statusValue = parseInt(filters.value.status)
    filtered = filtered.filter(d => d.status === statusValue)
  }
  
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    filtered = filtered.filter(d => {
      const typeName = d.type === 'leave' ? d.leaveType?.name : d.expenseType?.name
      const employeeName = d.requester?.fullName || ''
      const employeeEmail = d.requester?.email || ''
      return typeName?.toLowerCase().includes(search) ||
             employeeName.toLowerCase().includes(search) ||
             employeeEmail.toLowerCase().includes(search) ||
             d.description?.toLowerCase().includes(search) ||
             d.reason?.toLowerCase().includes(search)
    })
  }
  
  return filtered.sort((a, b) => new Date(b.reviewedAt) - new Date(a.reviewedAt))
})

const paginatedDecisions = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredDecisions.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredDecisions.value.length / itemsPerPage)
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
  const total = allDecisions.value.length
  const approved = allDecisions.value.filter(d => d.status === 1).length // Approved
  const rejected = allDecisions.value.filter(d => d.status === 2).length // Rejected
  
  // Décisions de cette semaine
  const oneWeekAgo = new Date()
  oneWeekAgo.setDate(oneWeekAgo.getDate() - 7)
  const recent = allDecisions.value.filter(d => new Date(d.reviewedAt) >= oneWeekAgo).length
  
  return { total, approved, rejected, recent }
})

// Methods
const fetchDecisions = async () => {
  try {
    loading.value = true
    
    const [leaveResponse, expenseResponse] = await Promise.all([
      apiService.getMyLeaveDecisions().catch(() => ({ data: [] })),
      apiService.getMyExpenseDecisions().catch(() => ({ data: [] }))
    ])
    
    const leaveDecisions = (leaveResponse.data || []).map(d => ({ ...d, type: 'leave' }))
    const expenseDecisions = (expenseResponse.data || []).map(d => ({ ...d, type: 'expense' }))
    
    allDecisions.value = [...leaveDecisions, ...expenseDecisions]
    
  } catch (error) {
    console.error('Erreur lors de la récupération des décisions:', error)
  } finally {
    loading.value = false
  }
}

const showDetails = (decision) => {
  selectedDecision.value = decision
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
watch(filteredDecisions, () => {
  currentPage.value = 1
})

onMounted(() => {
  fetchDecisions()
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
