<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-calendar-times me-2 text-warning"></i>
              Jours Fériés
            </h2>
            <button class="btn btn-warning" @click="showCreateModal">
              <i class="fas fa-plus me-2"></i>
              Nouveau Jour Férié
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
                <p class="text-muted mt-2">Chargement...</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-hover">
                  <thead class="table-dark">
                    <tr>
                      <th>Date</th>
                      <th>Nom</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="holiday in sortedHolidays" :key="holiday.id">
                      <td>
                        <i class="fas fa-calendar me-2 text-warning"></i>
                        {{ formatDate(holiday.date) }}
                      </td>
                      <td><strong>{{ holiday.name }}</strong></td>
                      <td>
                        <button 
                          class="btn btn-sm btn-outline-danger"
                          @click="confirmDelete(holiday)"
                          title="Supprimer"
                        >
                          <i class="fas fa-trash"></i>
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Modal Création -->
    <div class="modal fade" id="holidayModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i class="fas fa-plus"></i>
              Nouveau jour férié
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          
          <form @submit.prevent="saveHoliday">
            <div class="modal-body">
              <div v-if="formError" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ formError }}
              </div>
              
              <div class="mb-3">
                <label for="name" class="form-label">Nom du jour férié *</label>
                <input
                  type="text"
                  class="form-control"
                  id="name"
                  v-model="form.name"
                  required
                  placeholder="Ex: Jour de l'An, Fête du Travail..."
                >
              </div>
              
              <div class="mb-3">
                <label for="date" class="form-label">Date *</label>
                <input
                  type="date"
                  class="form-control"
                  id="date"
                  v-model="form.date"
                  required
                >
              </div>
            </div>
            
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Annuler
              </button>
              <button type="submit" class="btn btn-warning" :disabled="saving">
                <span v-if="saving">
                  <i class="fas fa-spinner fa-spin me-2"></i>
                  Sauvegarde...
                </span>
                <span v-else>
                  <i class="fas fa-save me-2"></i>
                  Créer
                </span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { Modal } from 'bootstrap'
import { ref, onMounted, computed } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

// State
const loading = ref(true)
const saving = ref(false)
const holidays = ref([])
const formError = ref('')

// Form data
const form = ref({
  name: '',
  date: ''
})

// Computed
const sortedHolidays = computed(() => {
  return [...holidays.value].sort((a, b) => new Date(a.date) - new Date(b.date))
})

// Methods
const fetchHolidays = async () => {
  try {
    loading.value = true
    const response = await apiService.getHolidays()
    holidays.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des jours fériés:', error)
  } finally {
    loading.value = false
  }
}

const showCreateModal = () => {
  resetForm()
  const modal = new Modal(document.getElementById('holidayModal'))
  modal.show()
}

const resetForm = () => {
  form.value = {
    name: '',
    date: ''
  }
  formError.value = ''
}

const saveHoliday = async () => {
  if (saving.value) return
  
  try {
    saving.value = true
    formError.value = ''
    
    await apiService.createHoliday(form.value)
    
    const modal = Modal.getInstance(document.getElementById('holidayModal'))
    modal.hide()
    await fetchHolidays()
    
  } catch (error) {
    formError.value = error.response?.data?.message || 'Erreur lors de la sauvegarde'
  } finally {
    saving.value = false
  }
}

const confirmDelete = async (holiday) => {
  if (confirm(`Êtes-vous sûr de vouloir supprimer "${holiday.name}" ?`)) {
    try {
      await apiService.deleteHoliday(holiday.id)
      await fetchHolidays()
    } catch (error) {
      console.error('Erreur lors de la suppression:', error)
    }
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

onMounted(() => {
  fetchHolidays()
})
</script>

<style scoped>
.modal-header {
  background: linear-gradient(135deg, #ffc107 0%, #fd7e14 100%);
  color: white;
  border-bottom: none;
}

.modal-header .btn-close {
  filter: invert(1);
}
</style>
