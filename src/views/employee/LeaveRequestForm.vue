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
              <i class="fas fa-calendar-plus me-2 text-success"></i>
              Nouvelle Demande de Congé
            </h2>
          </div>
        </div>
      </div>
      
      <div class="row justify-content-center">
        <div class="col-lg-8">
          <div class="card shadow">
            <div class="card-body">
              <!-- Messages -->
              <div v-if="errorMessage" class="alert alert-danger" role="alert">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ errorMessage }}
              </div>
              
              <div v-if="successMessage" class="alert alert-success" role="alert">
                <i class="fas fa-check-circle me-2"></i>
                {{ successMessage }}
              </div>
              
              <!-- Formulaire -->
              <form @submit.prevent="submitRequest">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label for="leaveTypeId" class="form-label">Type de congé *</label>
                    <select 
                      class="form-select" 
                      id="leaveTypeId" 
                      v-model="form.leaveTypeId" 
                      required
                      :disabled="submitting"
                    >
                      <option value="">Sélectionnez un type de congé</option>
                      <option 
                        v-for="type in leaveTypes" 
                        :key="type.id" 
                        :value="type.id"
                      >
                        {{ type.name }}
                      </option>
                    </select>
                    <small v-if="selectedLeaveType" class="form-text text-muted">
                      {{ selectedLeaveType.description }}
                    </small>
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <label class="form-label">Durée prévue</label>
                    <div class="input-group">
                      <span class="input-group-text">
                        <i class="fas fa-calendar-day"></i>
                      </span>
                      <input 
                        type="text" 
                        class="form-control" 
                        :value="calculatedDuration" 
                        readonly
                        placeholder="Sélectionnez les dates"
                      >
                    </div>
                  </div>
                </div>
                
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label for="startDate" class="form-label">Date de début *</label>
                    <input
                      type="date"
                      class="form-control"
                      id="startDate"
                      v-model="form.startDate"
                      required
                      :min="minDate"
                      :disabled="submitting"
                      @change="validateDates"
                    >
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <label for="endDate" class="form-label">Date de fin *</label>
                    <input
                      type="date"
                      class="form-control"
                      id="endDate"
                      v-model="form.endDate"
                      required
                      :min="form.startDate || minDate"
                      :disabled="submitting"
                      @change="validateDates"
                    >
                  </div>
                </div>
                
                <div class="mb-3">
                  <label for="reason" class="form-label">Motif (optionnel)</label>
                  <textarea
                    class="form-control"
                    id="reason"
                    v-model="form.reason"
                    rows="4"
                    :disabled="submitting"
                    placeholder="Décrivez le motif de votre demande de congé..."
                  ></textarea>
                </div>
                
                <!-- Informations supplémentaires -->
                <div v-if="form.startDate && form.endDate" class="alert alert-info">
                  <h6><i class="fas fa-info-circle me-2"></i>Résumé de votre demande</h6>
                  <ul class="mb-0">
                    <li><strong>Type :</strong> {{ selectedLeaveType?.name }}</li>
                    <li><strong>Période :</strong> Du {{ formatDate(form.startDate) }} au {{ formatDate(form.endDate) }}</li>
                    <li><strong>Durée :</strong> {{ calculatedDuration }}</li>
                    <li v-if="conflictingHolidays.length > 0" class="text-warning">
                      <strong>⚠️ Jours fériés inclus :</strong> {{ conflictingHolidays.map(h => h.name).join(', ') }}
                    </li>
                  </ul>
                </div>
                
                <!-- Actions -->
                <div class="d-flex justify-content-between">
                  <router-link to="/employee" class="btn btn-secondary">
                    <i class="fas fa-times me-2"></i>
                    Annuler
                  </router-link>
                  
                  <button 
                    type="submit" 
                    class="btn btn-success"
                    :disabled="submitting || !isFormValid"
                  >
                    <span v-if="submitting">
                      <i class="fas fa-spinner fa-spin me-2"></i>
                      Envoi en cours...
                    </span>
                    <span v-else>
                      <i class="fas fa-paper-plane me-2"></i>
                      Soumettre la demande
                    </span>
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

const router = useRouter()

// State
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const leaveTypes = ref([])
const holidays = ref([])

// Form data
const form = ref({
  leaveTypeId: '',
  startDate: '',
  endDate: '',
  reason: ''
})

// Computed
const minDate = computed(() => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow.toISOString().split('T')[0]
})

const selectedLeaveType = computed(() => {
  return leaveTypes.value.find(type => type.id == form.value.leaveTypeId)
})

const calculatedDuration = computed(() => {
  if (!form.value.startDate || !form.value.endDate) return ''
  
  const start = new Date(form.value.startDate)
  const end = new Date(form.value.endDate)
  const diffTime = Math.abs(end - start)
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1
  
  return `${diffDays} jour${diffDays > 1 ? 's' : ''}`
})

const conflictingHolidays = computed(() => {
  if (!form.value.startDate || !form.value.endDate) return []
  
  const start = new Date(form.value.startDate)
  const end = new Date(form.value.endDate)
  
  return holidays.value.filter(holiday => {
    const holidayDate = new Date(holiday.date)
    return holidayDate >= start && holidayDate <= end
  })
})

const isFormValid = computed(() => {
  return form.value.leaveTypeId && 
         form.value.startDate && 
         form.value.endDate &&
         new Date(form.value.endDate) >= new Date(form.value.startDate)
})

// Methods
const fetchLeaveTypes = async () => {
  try {
    const response = await apiService.getLeaveTypes()
    leaveTypes.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des types de congés:', error)
    errorMessage.value = 'Erreur lors du chargement des types de congés'
  }
}

const fetchHolidays = async () => {
  try {
    const response = await apiService.getHolidays()
    holidays.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des jours fériés:', error)
  }
}

const validateDates = () => {
  if (form.value.startDate && form.value.endDate) {
    if (new Date(form.value.endDate) < new Date(form.value.startDate)) {
      errorMessage.value = 'La date de fin doit être postérieure à la date de début'
      form.value.endDate = ''
    } else {
      errorMessage.value = ''
    }
  }
}

const submitRequest = async () => {
  if (submitting.value || !isFormValid.value) return
  
  try {
    submitting.value = true
    errorMessage.value = ''
    successMessage.value = ''
    
    const requestData = {
      leaveTypeId: parseInt(form.value.leaveTypeId),
      startDate: form.value.startDate,
      endDate: form.value.endDate,
      reason: form.value.reason || null
    }
    
    await apiService.createLeaveRequest(requestData)
    
    successMessage.value = 'Votre demande de congé a été soumise avec succès !'
    
    // Rediriger vers le dashboard après 2 secondes
    setTimeout(() => {
      router.push('/employee')
    }, 2000)
    
  } catch (error) {
    console.error('Erreur lors de la soumission:', error)
    errorMessage.value = error.response?.data?.message || 'Erreur lors de la soumission de la demande'
  } finally {
    submitting.value = false
  }
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString).toLocaleDateString('fr-FR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// Watchers
watch(() => form.value.startDate, () => {
  if (form.value.endDate && new Date(form.value.endDate) < new Date(form.value.startDate)) {
    form.value.endDate = ''
  }
})

onMounted(() => {
  fetchLeaveTypes()
  fetchHolidays()
})
</script>

<style scoped>
.card {
  border: none;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.form-control:focus,
.form-select:focus {
  border-color: #28a745;
  box-shadow: 0 0 0 0.25rem rgba(40, 167, 69, 0.25);
}

.alert {
  border: none;
  border-radius: 0.5rem;
}

.btn {
  font-weight: 600;
  padding: 0.75rem 1.5rem;
}

.input-group-text {
  background-color: #f8f9fa;
  border-color: #ced4da;
}
</style>
