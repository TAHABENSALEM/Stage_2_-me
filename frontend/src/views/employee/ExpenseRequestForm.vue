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
              <i class="fas fa-file-invoice-dollar me-2 text-info"></i>
              Nouvelle Note de Frais
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
                    <label for="expenseTypeId" class="form-label">Type de frais *</label>
                    <select 
                      class="form-select" 
                      id="expenseTypeId" 
                      v-model="form.expenseTypeId" 
                      required
                      :disabled="submitting"
                    >
                      <option value="">Sélectionnez un type de frais</option>
                      <option 
                        v-for="type in expenseTypes" 
                        :key="type.id" 
                        :value="type.id"
                      >
                        {{ type.name }}
                      </option>
                    </select>
                    <small v-if="selectedExpenseType" class="form-text text-muted">
                      {{ selectedExpenseType.description }}
                    </small>
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <label for="amount" class="form-label">Montant *</label>
                    <div class="input-group">
                      <input
                        type="number"
                        class="form-control"
                        id="amount"
                        v-model="form.amount"
                        required
                        min="0.01"
                        step="0.01"
                        :disabled="submitting"
                        placeholder="0.00"
                      >
                      <span class="input-group-text">€</span>
                    </div>
                  </div>
                </div>
                
                <div class="mb-3">
                  <label for="description" class="form-label">Description des frais *</label>
                  <textarea
                    class="form-control"
                    id="description"
                    v-model="form.description"
                    rows="4"
                    required
                    :disabled="submitting"
                    placeholder="Décrivez en détail les frais engagés (date, lieu, contexte professionnel...)"
                  ></textarea>
                  <small class="form-text text-muted">
                    Soyez précis dans votre description pour faciliter la validation.
                  </small>
                </div>
                
                <!-- Informations supplémentaires -->
                <div v-if="form.expenseTypeId && form.amount" class="alert alert-info">
                  <h6><i class="fas fa-info-circle me-2"></i>Résumé de votre demande</h6>
                  <ul class="mb-0">
                    <li><strong>Type :</strong> {{ selectedExpenseType?.name }}</li>
                    <li><strong>Montant :</strong> {{ formatAmount(form.amount) }}</li>
                    <li v-if="form.description"><strong>Description :</strong> {{ form.description.substring(0, 100) }}{{ form.description.length > 100 ? '...' : '' }}</li>
                  </ul>
                </div>
                
                <!-- Conseils -->
                <div class="card bg-light mb-3">
                  <div class="card-body">
                    <h6 class="card-title">
                      <i class="fas fa-lightbulb me-2"></i>
                      Conseils pour votre demande
                    </h6>
                    <ul class="mb-0 small text-muted">
                      <li>Conservez tous vos justificatifs (factures, tickets, reçus)</li>
                      <li>Indiquez la date, le lieu et le contexte professionnel</li>
                      <li>Pour les repas : précisez s'il s'agit d'un déjeuner/dîner d'affaires</li>
                      <li>Pour les transports : indiquez le trajet et la raison du déplacement</li>
                      <li>Les frais doivent être liés à votre activité professionnelle</li>
                    </ul>
                  </div>
                </div>
                
                <!-- Actions -->
                <div class="d-flex justify-content-between">
                  <router-link to="/employee" class="btn btn-secondary">
                    <i class="fas fa-times me-2"></i>
                    Annuler
                  </router-link>
                  
                  <button 
                    type="submit" 
                    class="btn btn-info"
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
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

const router = useRouter()

// State
const submitting = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const expenseTypes = ref([])

// Form data
const form = ref({
  expenseTypeId: '',
  amount: '',
  description: ''
})

// Computed
const selectedExpenseType = computed(() => {
  return expenseTypes.value.find(type => type.id == form.value.expenseTypeId)
})

const isFormValid = computed(() => {
  return form.value.expenseTypeId && 
         form.value.amount && 
         parseFloat(form.value.amount) > 0 &&
         form.value.description.trim().length > 0
})

// Methods
const fetchExpenseTypes = async () => {
  try {
    const response = await apiService.getExpenseTypes()
    expenseTypes.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des types de frais:', error)
    errorMessage.value = 'Erreur lors du chargement des types de frais'
  }
}

const submitRequest = async () => {
  if (submitting.value || !isFormValid.value) return
  
  try {
    submitting.value = true
    errorMessage.value = ''
    successMessage.value = ''
    
    const requestData = {
      expenseTypeId: parseInt(form.value.expenseTypeId),
      amount: parseFloat(form.value.amount),
      description: form.value.description.trim()
    }
    
    await apiService.createExpenseRequest(requestData)
    
    successMessage.value = 'Votre note de frais a été soumise avec succès !'
    
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

const formatAmount = (amount) => {
  if (!amount) return '0,00 €'
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'EUR'
  }).format(parseFloat(amount))
}

onMounted(() => {
  fetchExpenseTypes()
})
</script>

<style scoped>
.card {
  border: none;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.form-control:focus,
.form-select:focus {
  border-color: #17a2b8;
  box-shadow: 0 0 0 0.25rem rgba(23, 162, 184, 0.25);
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
  font-weight: 600;
}

.bg-light {
  background-color: #f8f9fa !important;
}

.card-title {
  color: #495057;
}
</style>
