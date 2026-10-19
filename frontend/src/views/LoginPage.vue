<template>
  <div class="main-container d-flex align-items-center justify-content-center">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-4">
          <!-- Retour à l'accueil -->
          <div class="text-center mb-4">
            <router-link to="/" class="btn btn-outline-secondary">
              <i class="fas fa-arrow-left me-2"></i>
              Retour à l'accueil
            </router-link>
          </div>
          
          <div class="card">
            <div class="card-header text-center">
              <div class="mb-3">
                <i 
                  :class="userTypeIcon" 
                  class="fa-3x mb-3"
                  :style="{ color: userTypeColor }"
                ></i>
              </div>
              <h4 class="mb-0">{{ userTypeTitle }}</h4>
              <small class="text-muted">Authentification requise</small>
            </div>
            
            <div class="card-body">
              <!-- Message d'erreur -->
              <div v-if="errorMessage" class="alert alert-danger" role="alert">
                <i class="fas fa-exclamation-triangle me-2"></i>
                {{ errorMessage }}
              </div>
              
              <!-- Formulaire de connexion -->
              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label for="email" class="form-label">
                    <i class="fas fa-envelope me-2"></i>
                    Email
                  </label>
                  <input
                    type="email"
                    class="form-control"
                    id="email"
                    v-model="credentials.email"
                    required
                    :disabled="loading"
                    placeholder="votre.email@entreprise.com"
                  >
                </div>
                
                <div class="mb-4">
                  <label for="password" class="form-label">
                    <i class="fas fa-lock me-2"></i>
                    Mot de passe
                  </label>
                  <input
                    type="password"
                    class="form-control"
                    id="password"
                    v-model="credentials.password"
                    required
                    :disabled="loading"
                    placeholder="Votre mot de passe"
                  >
                </div>
                
                <button 
                  type="submit" 
                  class="btn w-100"
                  :class="userTypeButtonClass"
                  :disabled="loading"
                >
                  <span v-if="loading">
                    <i class="fas fa-spinner fa-spin me-2"></i>
                    Connexion...
                  </span>
                  <span v-else>
                    <i class="fas fa-sign-in-alt me-2"></i>
                    Se connecter
                  </span>
                </button>
              </form>
            </div>
          </div>
          
          <!-- Informations de connexion par défaut (dev) -->
          <div class="card mt-3 bg-light" v-if="type === 'admin'">
            <div class="card-body">
              <h6 class="card-title text-center">
                <i class="fas fa-info-circle me-2"></i>
                Compte Admin par défaut
              </h6>
              <div class="text-center">
                <small class="text-muted d-block">Email: admin@entreprise.com</small>
                <small class="text-muted d-block">Mot de passe: Admin123!</small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Modal changement de mot de passe -->
    <div class="modal fade" id="changePasswordModal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              <i class="fas fa-key me-2"></i>
              Changement de mot de passe requis
            </h5>
          </div>
          <div class="modal-body">
            <p class="text-muted mb-3">
              Vous devez changer votre mot de passe lors de votre première connexion.
            </p>
            
            <!-- Formulaire changement mot de passe -->
            <form @submit.prevent="handleChangePassword">
              <div class="mb-3">
                <label for="oldPassword" class="form-label">Ancien mot de passe</label>
                <input
                  type="password"
                  class="form-control"
                  id="oldPassword"
                  v-model="passwordChange.oldPassword"
                  required
                >
              </div>
              
              <div class="mb-3">
                <label for="newPassword" class="form-label">Nouveau mot de passe</label>
                <input
                  type="password"
                  class="form-control"
                  id="newPassword"
                  v-model="passwordChange.newPassword"
                  required
                  minlength="6"
                >
              </div>
              
              <div class="mb-3">
                <label for="confirmPassword" class="form-label">Confirmer le nouveau mot de passe</label>
                <input
                  type="password"
                  class="form-control"
                  id="confirmPassword"
                  v-model="passwordChange.confirmPassword"
                  required
                >
              </div>
              
              <div v-if="passwordError" class="alert alert-danger">
                {{ passwordError }}
              </div>
              
              <button type="submit" class="btn btn-primary w-100" :disabled="changingPassword">
                <span v-if="changingPassword">
                  <i class="fas fa-spinner fa-spin me-2"></i>
                  Changement...
                </span>
                <span v-else>
                  <i class="fas fa-save me-2"></i>
                  Changer le mot de passe
                </span>
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { Modal } from 'bootstrap'

const props = defineProps({
  type: {
    type: String,
    required: true,
    validator: value => ['admin', 'employee'].includes(value)
  }
})

const router = useRouter()
const authStore = useAuthStore()

// State
const loading = ref(false)
const errorMessage = ref('')
const credentials = ref({
  email: '',
  password: ''
})

// Changement de mot de passe
const changingPassword = ref(false)
const passwordError = ref('')
const passwordChange = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Computed
const userTypeTitle = computed(() => {
  return props.type === 'admin' ? 'Espace Administrateur' : 'Espace Employé'
})

const userTypeIcon = computed(() => {
  return props.type === 'admin' ? 'fas fa-user-shield' : 'fas fa-users'
})

const userTypeColor = computed(() => {
  return props.type === 'admin' ? '#0d6efd' : '#198754'
})

const userTypeButtonClass = computed(() => {
  return props.type === 'admin' ? 'btn-primary' : 'btn-success'
})

// Methods
const handleLogin = async () => {
  if (!credentials.value.email || !credentials.value.password) {
    errorMessage.value = 'Veuillez remplir tous les champs'
    return
  }
  
  loading.value = true
  errorMessage.value = ''
  
  try {
    const result = await authStore.login(credentials.value)
    
    if (result.success) {
      if (result.mustChangePassword) {
        // Afficher le modal de changement de mot de passe
        const modal = new Modal(document.getElementById('changePasswordModal'))
        modal.show()
      } else {
        // Rediriger selon le rôle
        if (authStore.isAdmin) {
          router.push('/admin')
        } else {
          router.push('/employee')
        }
      }
    } else {
      errorMessage.value = result.message
    }
  } catch (error) {
    errorMessage.value = 'Une erreur est survenue lors de la connexion'
  } finally {
    loading.value = false
  }
}

const handleChangePassword = async () => {
  if (passwordChange.value.newPassword !== passwordChange.value.confirmPassword) {
    passwordError.value = 'Les mots de passe ne correspondent pas'
    return
  }
  
  if (passwordChange.value.newPassword.length < 6) {
    passwordError.value = 'Le mot de passe doit contenir au moins 6 caractères'
    return
  }
  
  changingPassword.value = true
  passwordError.value = ''
  
  try {
    const result = await authStore.changePassword(
      passwordChange.value.oldPassword,
      passwordChange.value.newPassword
    )
    
    if (result.success) {
      // Fermer le modal et rediriger
      const modal = Modal.getInstance(document.getElementById('changePasswordModal'))
      modal.hide()
      
      if (authStore.isAdmin) {
        router.push('/admin')
      } else {
        router.push('/employee')
      }
    } else {
      passwordError.value = result.message
    }
  } catch (error) {
    passwordError.value = 'Erreur lors du changement de mot de passe'
  } finally {
    changingPassword.value = false
  }
}

onMounted(() => {
  // Si déjà connecté, rediriger
  if (authStore.isAuthenticated) {
    if (authStore.isAdmin) {
      router.push('/admin')
    } else {
      router.push('/employee')
    }
  }
})
</script>

<style scoped>
.main-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.card {
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
  border: none;
}

.card-header {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-bottom: 2px solid #dee2e6;
}

.form-control:focus {
  border-color: #86b7fe;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}

.btn {
  font-weight: 600;
  padding: 0.75rem 1.5rem;
}

.bg-light {
  background-color: rgba(248, 249, 250, 0.95) !important;
}
</style>
