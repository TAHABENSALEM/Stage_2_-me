<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-receipt me-2 text-info"></i>
              Types de Notes de Frais
            </h2>
            <button class="btn btn-info" @click="showCreateModal">
              <i class="fas fa-plus me-2"></i>
              Nouveau Type
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
              
              <div v-else-if="expenseTypes.length === 0" class="text-center py-4">
                <i class="fas fa-receipt fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucun type de frais configuré</h5>
                <p class="text-muted">Commencez par créer le premier type de frais.</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-hover">
                  <thead class="table-dark">
                    <tr>
                      <th>Nom</th>
                      <th>Description</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="type in expenseTypes" :key="type.id">
                      <td>
                        <i class="fas fa-file-invoice-dollar me-2 text-info"></i>
                        <strong>{{ type.name }}</strong>
                      </td>
                      <td class="text-muted">{{ type.description || 'Aucune description' }}</td>
                      <td>
                        <div class="btn-group" role="group">
                          <button 
                            class="btn btn-sm btn-outline-primary"
                            @click="editType(type)"
                            title="Modifier"
                          >
                            <i class="fas fa-edit"></i>
                          </button>
                          <button 
                            class="btn btn-sm btn-outline-danger"
                            @click="confirmDelete(type)"
                            title="Supprimer"
                          >
                            <i class="fas fa-trash"></i>
                          </button>
                        </div>
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
    
    <!-- Modal Création/Édition -->
    <div class="modal fade" id="typeModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i :class="editingType ? 'fas fa-edit' : 'fas fa-plus'"></i>
              {{ editingType ? 'Modifier le type' : 'Nouveau type de frais' }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          
          <form @submit.prevent="saveType">
            <div class="modal-body">
              <div v-if="formError" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ formError }}
              </div>
              
              <div class="mb-3">
                <label for="name" class="form-label">Nom du type *</label>
                <input
                  type="text"
                  class="form-control"
                  id="name"
                  v-model="form.name"
                  required
                  placeholder="Ex: Transport, Restauration, Hébergement..."
                >
              </div>
              
              <div class="mb-3">
                <label for="description" class="form-label">Description</label>
                <textarea
                  class="form-control"
                  id="description"
                  v-model="form.description"
                  rows="3"
                  placeholder="Description optionnelle du type de frais"
                ></textarea>
              </div>
            </div>
            
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Annuler
              </button>
              <button type="submit" class="btn btn-info" :disabled="saving">
                <span v-if="saving">
                  <i class="fas fa-spinner fa-spin me-2"></i>
                  Sauvegarde...
                </span>
                <span v-else>
                  <i class="fas fa-save me-2"></i>
                  {{ editingType ? 'Modifier' : 'Créer' }}
                </span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
    
    <!-- Modal Confirmation Suppression -->
    <div class="modal fade" id="deleteModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title text-danger">
              <i class="fas fa-exclamation-triangle me-2"></i>
              Confirmer la suppression
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <p>Êtes-vous sûr de vouloir supprimer le type de frais :</p>
            <p class="fw-bold">{{ typeToDelete?.name }}</p>
            <p class="text-danger">
              <i class="fas fa-exclamation-triangle me-2"></i>
              Cette action supprimera également toutes les demandes associées.
            </p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
              Annuler
            </button>
            <button type="button" class="btn btn-danger" @click="deleteType" :disabled="deleting">
              <span v-if="deleting">
                <i class="fas fa-spinner fa-spin me-2"></i>
                Suppression...
              </span>
              <span v-else">
                <i class="fas fa-trash me-2"></i>
                Supprimer
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { Modal } from 'bootstrap'
import { ref, onMounted } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'

// State
const loading = ref(true)
const saving = ref(false)
const deleting = ref(false)
const expenseTypes = ref([])
const formError = ref('')
const editingType = ref(null)
const typeToDelete = ref(null)

// Form data
const form = ref({
  name: '',
  description: ''
})

// Methods
const fetchExpenseTypes = async () => {
  try {
    loading.value = true
    const response = await apiService.getExpenseTypes()
    expenseTypes.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des types de frais:', error)
  } finally {
    loading.value = false
  }
}

const showCreateModal = () => {
  editingType.value = null
  resetForm()
  const modal = new Modal(document.getElementById('typeModal'))
  modal.show()
}

const editType = (type) => {
  editingType.value = type
  form.value = {
    name: type.name,
    description: type.description || ''
  }
  formError.value = ''
  const modal = new Modal(document.getElementById('typeModal'))
  modal.show()
}

const resetForm = () => {
  form.value = {
    name: '',
    description: ''
  }
  formError.value = ''
}

const saveType = async () => {
  if (saving.value) return
  
  try {
    saving.value = true
    formError.value = ''
    
    if (editingType.value) {
      await apiService.updateExpenseType(editingType.value.id, form.value)
    } else {
      await apiService.createExpenseType(form.value)
    }
    
    const modal = Modal.getInstance(document.getElementById('typeModal'))
    modal.hide()
    await fetchExpenseTypes()
    
  } catch (error) {
    formError.value = error.response?.data?.message || 'Erreur lors de la sauvegarde'
  } finally {
    saving.value = false
  }
}

const confirmDelete = (type) => {
  typeToDelete.value = type
  const modal = new Modal(document.getElementById('deleteModal'))
  modal.show()
}

const deleteType = async () => {
  if (!typeToDelete.value || deleting.value) return
  
  try {
    deleting.value = true
    await apiService.deleteExpenseType(typeToDelete.value.id)
    
    const modal = Modal.getInstance(document.getElementById('deleteModal'))
    modal.hide()
    await fetchExpenseTypes()
    
  } catch (error) {
    console.error('Erreur lors de la suppression:', error)
  } finally {
    deleting.value = false
  }
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

.modal-header {
  background: linear-gradient(135deg, #17a2b8 0%, #20c997 100%);
  color: white;
  border-bottom: none;
}

.modal-header .btn-close {
  filter: invert(1);
}

.btn-group .btn {
  margin: 0 1px;
}
</style>
