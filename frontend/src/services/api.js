import axios from 'axios'

const API_BASE_URL = 'http://localhost:5000/api'

// Configuration Axios
axios.defaults.baseURL = API_BASE_URL
axios.defaults.headers.common['Content-Type'] = 'application/json'

// Intercepteur pour ajouter automatiquement le token
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Intercepteur pour gérer les réponses et erreurs
axios.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    if (error.response?.status === 401) {
      // Token expiré ou invalide
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      localStorage.removeItem('mustChangePassword')
      window.location.href = '/login/admin'
    }
    return Promise.reject(error)
  }
)

// Intercepteur pour ajouter automatiquement le token
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Intercepteur pour gérer les erreurs de réponse
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expiré ou invalide
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      localStorage.removeItem('mustChangePassword')
      // Rediriger vers la page de login si nécessaire
      if (window.location.pathname !== '/login/admin' && window.location.pathname !== '/login/employee') {
        window.location.href = '/'
      }
    }
    return Promise.reject(error)
  }
)

// Service API générique
export const apiService = {
  // Auth
  login: (credentials) => axios.post('/auth/login', credentials),
  changePassword: (data) => axios.post('/auth/change-password', data),
  
  // Users (Admin)
  getUsers: () => axios.get('/users'),
  createUser: (userData) => axios.post('/users', userData),
  updateUser: (id, userData) => axios.put(`/users/${id}`, userData),
  deleteUser: (id) => axios.delete(`/users/${id}`),
  getOrganigram: () => axios.get('/users/organigram'),
  
  // Leave Types (Admin)
  getLeaveTypes: () => axios.get('/LeaveTypes'),
  createLeaveType: (data) => axios.post('/LeaveTypes', data),
  updateLeaveType: (id, data) => axios.put(`/LeaveTypes/${id}`, data),
  deleteLeaveType: (id) => axios.delete(`/LeaveTypes/${id}`),
  
  // Expense Types (Admin)
  getExpenseTypes: () => axios.get('/ExpenseTypes'),
  createExpenseType: (data) => axios.post('/ExpenseTypes', data),
  updateExpenseType: (id, data) => axios.put(`/ExpenseTypes/${id}`, data),
  deleteExpenseType: (id) => axios.delete(`/ExpenseTypes/${id}`),
  
  // Holidays (Admin)
  getHolidays: () => axios.get('/Holiday'),
  createHoliday: (data) => axios.post('/Holiday', data),
  updateHoliday: (id, data) => axios.put(`/Holiday/${id}`, data),
  deleteHoliday: (id) => axios.delete(`/Holiday/${id}`),
  
  // Leave Requests (Employee)
  getMyLeaveRequests: () => axios.get('/leaverequests/me'),
  createLeaveRequest: (data) => axios.post('/leaverequests', data),
  getPendingLeaveRequests: () => axios.get('/leaverequests/pending-for-me'),
  updateLeaveRequest: (id, action) => axios.post(`/leaverequests/${id}/review`, action),
  getMyLeaveDecisions: () => axios.get('/leaverequests/my-decisions'),
  
  // Expense Requests (Employee)
  getMyExpenseRequests: () => axios.get('/expenserequests/me'),
  createExpenseRequest: (data) => axios.post('/expenserequests', data),
  getPendingExpenseRequests: () => axios.get('/expenserequests/pending-for-me'),
  updateExpenseRequest: (id, action) => axios.post(`/expenserequests/${id}/review`, action),
  getMyExpenseDecisions: () => axios.get('/expenserequests/my-decisions')
}

// Exporter l'instance axios pour utilisation dans les stores
export { axios }

export default apiService
