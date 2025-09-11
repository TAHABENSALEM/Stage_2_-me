import { defineStore } from 'pinia'
import { apiService } from '@/services/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user') || 'null'),
    token: localStorage.getItem('token'),
    mustChangePassword: localStorage.getItem('mustChangePassword') === 'true'
  }),
  
  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.role === 'Admin',
    isEmployee: (state) => state.user?.role === 'Employé'
  },
  
  actions: {
    async login(credentials) {
      try {
        console.log('Tentative de connexion avec:', credentials)
        const response = await apiService.login(credentials)
        console.log('Réponse de connexion:', response.data)
        
        const { token, email, role, mustChangePassword } = response.data
        
        this.token = token
        this.user = { email, role }
        this.mustChangePassword = mustChangePassword
        
        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify({ email, role }))
        localStorage.setItem('mustChangePassword', mustChangePassword.toString())
        
        // L'intercepteur ajoutera automatiquement le token
        
        console.log('Connexion réussie, utilisateur:', this.user)
        return { success: true, mustChangePassword }
      } catch (error) {
        console.error('Erreur de connexion complète:', error)
        console.error('Réponse d\'erreur:', error.response)
        return {
          success: false,
          message: error.response?.data?.message || error.response?.data || 'Erreur de connexion'
        }
      }
    },
    
    async changePassword(oldPassword, newPassword) {
      try {
        await apiService.changePassword({
          oldPassword,
          newPassword
        })
        this.mustChangePassword = false
        return { success: true }
      } catch (error) {
        return {
          success: false,
          message: error.response?.data?.message || 'Erreur lors du changement de mot de passe'
        }
      }
    },
    
    logout() {
      this.user = null
      this.token = null
      this.mustChangePassword = false
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      localStorage.removeItem('mustChangePassword')
    },
    
    initializeAuth() {
      // L'intercepteur gérera automatiquement l'ajout du token
    }
  }
})
