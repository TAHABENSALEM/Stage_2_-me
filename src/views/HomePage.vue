<template>
  <div class="main-container d-flex align-items-center justify-content-center">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-8 col-lg-6">
          <div class="text-center mb-5">
            <h1 class="display-4 text-primary mb-3">
              <i class="fas fa-building me-3"></i>
              Gestion Congés & Frais
            </h1>
            <p class="lead text-muted">
              Système de gestion des demandes de congés et notes de frais
            </p>
          </div>
          
          <div class="row g-4">
            <!-- Espace Admin -->
            <div class="col-md-6">
              <div class="card h-100 border-primary">
                <div class="card-body text-center d-flex flex-column">
                  <div class="mb-4">
                    <i class="fas fa-user-shield fa-4x text-primary mb-3"></i>
                    <h4 class="card-title">Espace Administrateur</h4>
                    <p class="card-text text-muted">
                      Gestion des utilisateurs, types de congés et configuration système
                    </p>
                  </div>
                  <div class="mt-auto">
                    <router-link 
                      to="/login/admin" 
                      class="btn btn-primary btn-lg w-100"
                    >
                      <i class="fas fa-sign-in-alt me-2"></i>
                      Accéder
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Espace Employé -->
            <div class="col-md-6">
              <div class="card h-100 border-success">
                <div class="card-body text-center d-flex flex-column">
                  <div class="mb-4">
                    <i class="fas fa-users fa-4x text-success mb-3"></i>
                    <h4 class="card-title">Espace Employé</h4>
                    <p class="card-text text-muted">
                      Demandes de congés, notes de frais et suivi des approbations
                    </p>
                  </div>
                  <div class="mt-auto">
                    <router-link 
                      to="/login/employee" 
                      class="btn btn-success btn-lg w-100"
                    >
                      <i class="fas fa-sign-in-alt me-2"></i>
                      Accéder
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Informations supplémentaires -->
          <div class="row mt-5">
            <div class="col-12">
              <div class="card bg-light">
                <div class="card-body">
                  <h6 class="card-title text-center mb-3">
                    <i class="fas fa-info-circle me-2"></i>
                    Informations
                  </h6>
                  <div class="row text-center">
                    <div class="col-md-4 mb-2">
                      <small class="text-muted">
                        <i class="fas fa-calendar-alt me-1"></i>
                        Gestion des congés
                      </small>
                    </div>
                    <div class="col-md-4 mb-2">
                      <small class="text-muted">
                        <i class="fas fa-receipt me-1"></i>
                        Notes de frais
                      </small>
                    </div>
                    <div class="col-md-4 mb-2">
                      <small class="text-muted">
                        <i class="fas fa-sitemap me-1"></i>
                        Workflow d'approbation
                      </small>
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
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

onMounted(() => {
  // Si l'utilisateur est déjà connecté, le rediriger vers son dashboard
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
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.display-4 {
  font-weight: 700;
}

.btn-lg {
  padding: 0.75rem 1.5rem;
  font-weight: 600;
}

.border-primary {
  border-width: 2px !important;
}

.border-success {
  border-width: 2px !important;
}

.bg-light {
  background-color: rgba(255, 255, 255, 0.95) !important;
}
</style>
