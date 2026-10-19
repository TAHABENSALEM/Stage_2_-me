<template>
  <div>
    <AppNavbar />
    
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>
              <i class="fas fa-users me-2 text-primary"></i>
              Gestion des Utilisateurs
            </h2>
            <button class="btn btn-primary" @click="showCreateModal">
              <i class="fas fa-plus me-2"></i>
              Nouvel Utilisateur
            </button>
          </div>
        </div>
      </div>
      
      <!-- Liste des utilisateurs -->
      <div class="row">
        <div class="col-12">
          <div class="card shadow">
            <div class="card-body">
              <!-- Barre de recherche -->
              <div class="row mb-3">
                <div class="col-md-6">
                  <div class="input-group">
                    <span class="input-group-text">
                      <i class="fas fa-search"></i>
                    </span>
                    <input
                      type="text"
                      class="form-control"
                      placeholder="Rechercher un utilisateur..."
                      v-model="searchTerm"
                    >
                  </div>
                </div>
                <div class="col-md-6 text-end">
                  <span class="text-muted">
                    {{ filteredUsers.length }} utilisateur(s) trouvé(s)
                  </span>
                </div>
              </div>
              
              <!-- Tableau -->
              <div v-if="loading" class="text-center py-4">
                <i class="fas fa-spinner fa-spin fa-2x text-muted"></i>
                <p class="text-muted mt-2">Chargement des utilisateurs...</p>
              </div>
              
              <div v-else-if="users.length === 0" class="text-center py-4">
                <i class="fas fa-users fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Aucun utilisateur trouvé</h5>
                <p class="text-muted">Commencez par créer le premier utilisateur.</p>
              </div>
              
              <div v-else class="table-responsive">
                <table class="table table-hover">
                  <thead class="table-dark">
                    <tr>
                      <th>Nom Complet</th>
                      <th>Email</th>
                      <th>Rôle</th>
                      <th>Manager</th>
                      <th>Mot de passe changé</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="user in filteredUsers" :key="user.id">
                      <td>
                        <i class="fas fa-user me-2 text-muted"></i>
                        {{ user.fullName }}
                      </td>
                      <td>{{ user.email }}</td>
                      <td>
                        <span 
                          class="badge"
                          :class="user.role === 'Admin' ? 'bg-danger' : 'bg-primary'"
                        >
                          <i :class="user.role === 'Admin' ? 'fas fa-shield-alt' : 'fas fa-user'"></i>
                          {{ user.role }}
                        </span>
                      </td>
                      <td>
                        <span v-if="user.manager" class="text-muted">
                          {{ user.manager.fullName }}
                        </span>
                        <span v-else class="text-muted fst-italic">Aucun</span>
                      </td>
                      <td>
                        <span 
                          class="badge"
                          :class="user.passwordChanged ? 'bg-success' : 'bg-warning'"
                        >
                          <i :class="user.passwordChanged ? 'fas fa-check' : 'fas fa-exclamation-triangle'"></i>
                          {{ user.passwordChanged ? 'Oui' : 'Non' }}
                        </span>
                      </td>
                      <td>
                        <div class="btn-group" role="group">
                          <button 
                            class="btn btn-sm btn-outline-primary"
                            @click="editUser(user)"
                            title="Modifier"
                          >
                            <i class="fas fa-edit"></i>
                          </button>
                          <button 
                            class="btn btn-sm btn-outline-danger"
                            @click="confirmDelete(user)"
                            title="Supprimer"
                            :disabled="user.role === 'Admin'"
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
    <div class="modal fade" id="userModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i :class="editingUser ? 'fas fa-edit' : 'fas fa-plus'"></i>
              {{ editingUser ? 'Modifier l\'utilisateur' : 'Nouvel utilisateur' }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          
          <form @submit.prevent="saveUser">
            <div class="modal-body">
              <div v-if="formError" class="alert alert-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ formError }}
              </div>
              
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label for="fullName" class="form-label">Nom complet *</label>
                  <input
                    type="text"
                    class="form-control"
                    id="fullName"
                    v-model="form.fullName"
                    required
                  >
                </div>
                <div class="col-md-6 mb-3">
                  <label for="email" class="form-label">Email *</label>
                  <input
                    type="email"
                    class="form-control"
                    id="email"
                    v-model="form.email"
                    required
                  >
                </div>
              </div>
              
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label for="role" class="form-label">Rôle *</label>
                  <select class="form-select" id="role" v-model="form.role" required>
                    <option value="Employé">Employé</option>
                    <option value="Admin">Admin</option>
                  </select>
                </div>
                <div class="col-md-6 mb-3">
                  <label for="managerId" class="form-label">Manager</label>
                  <select class="form-select" id="managerId" v-model="form.managerId">
                    <option value="">Aucun manager</option>
                    <option 
                      v-for="user in availableManagers" 
                      :key="user.id" 
                      :value="user.id"
                    >
                      {{ user.fullName }} ({{ user.email }})
                    </option>
                  </select>
                </div>
              </div>
              
              <div v-if="!editingUser" class="mb-3">
                <label for="password" class="form-label">Mot de passe initial *</label>
                <input
                  type="password"
                  class="form-control"
                  id="password"
                  v-model="form.password"
                  required
                  minlength="6"
                >
                <small class="form-text text-muted">
                  L'utilisateur devra changer ce mot de passe lors de sa première connexion.
                </small>
              </div>
            </div>
            
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Annuler
              </button>
              <button type="submit" class="btn btn-primary" :disabled="saving">
                <span v-if="saving">
                  <i class="fas fa-spinner fa-spin me-2"></i>
                  Sauvegarde...
                </span>
                <span v-else>
                  <i class="fas fa-save me-2"></i>
                  {{ editingUser ? 'Modifier' : 'Créer' }}
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
            <p>Êtes-vous sûr de vouloir supprimer l'utilisateur :</p>
            <p class="fw-bold">{{ userToDelete?.fullName }} ({{ userToDelete?.email }})</p>
            <p class="text-danger">
              <i class="fas fa-exclamation-triangle me-2"></i>
              Cette action est irréversible.
            </p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
              Annuler
            </button>
            <button type="button" class="btn btn-danger" @click="deleteUser" :disabled="deleting">
              <span v-if="deleting">
                <i class="fas fa-spinner fa-spin me-2"></i>
                Suppression...
              </span>
              <span v-else>
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
import { ref, onMounted, computed } from 'vue'
import AppNavbar from '@/components/AppNavbar.vue'
import apiService from '@/services/api'
import { Modal } from 'bootstrap'

// State
const loading = ref(true)
const saving = ref(false)
const deleting = ref(false)
const users = ref([])
const searchTerm = ref('')
const formError = ref('')
const editingUser = ref(null)
const userToDelete = ref(null)

// Form data
const form = ref({
  fullName: '',
  email: '',
  password: '',
  role: 'Employé',
  managerId: null
})

// Computed
const filteredUsers = computed(() => {
  if (!searchTerm.value) return users.value
  
  const term = searchTerm.value.toLowerCase()
  return users.value.filter(user => 
    user.fullName.toLowerCase().includes(term) ||
    user.email.toLowerCase().includes(term) ||
    user.role.toLowerCase().includes(term)
  )
})

const availableManagers = computed(() => {
  return users.value.filter(user => 
    user.id !== editingUser.value?.id // Exclure l'utilisateur en cours d'édition
  )
})

// Methods
const fetchUsers = async () => {
  try {
    loading.value = true
    const response = await apiService.getUsers()
    users.value = response.data || []
  } catch (error) {
    console.error('Erreur lors de la récupération des utilisateurs:', error)
  } finally {
    loading.value = false
  }
}

const showCreateModal = () => {
  editingUser.value = null
  resetForm()
  const modal = new Modal(document.getElementById('userModal'))
  modal.show()
}

const editUser = (user) => {
  editingUser.value = user
  form.value = {
    fullName: user.fullName,
    email: user.email,
    password: '',
    role: user.role,
    managerId: user.managerId
  }
  formError.value = ''
  const modal = new Modal(document.getElementById('userModal'))
  modal.show()
}

const resetForm = () => {
  form.value = {
    fullName: '',
    email: '',
    password: '',
    role: 'Employé',
    managerId: null
  }
  formError.value = ''
}

const saveUser = async () => {
  if (saving.value) return
  
  try {
    saving.value = true
    formError.value = ''
    
    const userData = { ...form.value }
    if (!userData.managerId) {
      userData.managerId = null
    }
    
    if (editingUser.value) {
      // Modification
      await apiService.updateUser(editingUser.value.id, userData)
    } else {
      // Création
      await apiService.createUser(userData)
    }
    
    // Fermer le modal et recharger la liste
    const modal = Modal.getInstance(document.getElementById('userModal'))
    modal.hide()
    await fetchUsers()
    
  } catch (error) {
    formError.value = error.response?.data?.message || 'Erreur lors de la sauvegarde'
  } finally {
    saving.value = false
  }
}

const confirmDelete = (user) => {
  userToDelete.value = user
  const modal = new Modal(document.getElementById('deleteModal'))
  modal.show()
}

const deleteUser = async () => {
  if (!userToDelete.value || deleting.value) return
  
  try {
    deleting.value = true
    await apiService.deleteUser(userToDelete.value.id)
    
    // Fermer le modal et recharger la liste
    const modal = Modal.getInstance(document.getElementById('deleteModal'))
    modal.hide()
    await fetchUsers()
    
  } catch (error) {
    console.error('Erreur lors de la suppression:', error)
  } finally {
    deleting.value = false
  }
}

onMounted(() => {
  fetchUsers()
})
</script>

<style scoped>
.table-responsive {
  border-radius: 0.5rem;
  overflow: hidden;
}

.btn-group .btn {
  margin: 0 1px;
}

.badge {
  font-size: 0.75em;
}

.modal-content {
  border: none;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.modal-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-bottom: none;
}

.modal-header .btn-close {
  filter: invert(1);
}

.card {
  border: none;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}
</style>
