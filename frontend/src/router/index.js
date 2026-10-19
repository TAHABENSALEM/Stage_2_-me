import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Import des composants
import HomePage from '@/views/HomePage.vue'
import LoginPage from '@/views/LoginPage.vue'

// Admin views
import AdminDashboard from '@/views/admin/AdminDashboard.vue'
import UserManagement from '@/views/admin/UserManagement.vue'
import LeaveTypesAdmin from '@/views/admin/LeaveTypesAdmin.vue'
import ExpenseTypeManagement from '@/views/admin/ExpenseTypeManagement.vue'
import HolidayManagement from '@/views/admin/HolidayManagement.vue'
import Organigram from '@/views/admin/OrganigrammeAdmin.vue'

// Employee views
import EmployeeDashboard from '@/views/employee/EmployeeDashboard.vue'
import LeaveRequestForm from '@/views/employee/LeaveRequestForm.vue'
import ExpenseRequestForm from '@/views/employee/ExpenseRequestForm.vue'
import RequestHistory from '@/views/employee/RequestHistory.vue'
import PendingApprovals from '@/views/employee/PendingApprovals.vue'
import MyDecisionsHistory from '@/views/employee/MyDecisionsHistory.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: HomePage
  },
  {
    path: '/login/:type',
    name: 'Login',
    component: LoginPage,
    props: true
  },
  // Routes Admin
  {
    path: '/admin',
    name: 'AdminDashboard',
    component: AdminDashboard,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  {
    path: '/admin/users',
    name: 'UserManagement',
    component: UserManagement,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  {
    path: '/admin/leave-types',
    name: 'LeaveTypeManagement',
    component: LeaveTypesAdmin,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  {
    path: '/admin/expense-types',
    name: 'ExpenseTypeManagement',
    component: ExpenseTypeManagement,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  {
    path: '/admin/holidays',
    name: 'HolidayManagement',
    component: HolidayManagement,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  {
    path: '/admin/organigram',
    name: 'Organigram',
    component: Organigram,
    meta: { requiresAuth: true, role: 'Admin' }
  },
  // Routes Employé
  {
    path: '/employee',
    name: 'EmployeeDashboard',
    component: EmployeeDashboard,
    meta: { requiresAuth: true, role: 'Employé' }
  },
  {
    path: '/employee/leave-request',
    name: 'LeaveRequestForm',
    component: LeaveRequestForm,
    meta: { requiresAuth: true, role: 'Employé' }
  },
  {
    path: '/employee/expense-request',
    name: 'ExpenseRequestForm',
    component: ExpenseRequestForm,
    meta: { requiresAuth: true, role: 'Employé' }
  },
  {
    path: '/employee/history',
    name: 'RequestHistory',
    component: RequestHistory,
    meta: { requiresAuth: true, role: 'Employé' }
  },
  {
    path: '/employee/approvals',
    name: 'PendingApprovals',
    component: PendingApprovals,
    meta: { requiresAuth: true, role: 'Employé' }
  },
  {
    path: '/employee/my-decisions',
    name: 'MyDecisionsHistory',
    component: MyDecisionsHistory,
    meta: { requiresAuth: true, role: 'Employé' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Guard pour vérifier l'authentification
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth) {
    if (!authStore.isAuthenticated) {
      next('/')
      return
    }
    
    if (to.meta.role && authStore.user?.role !== to.meta.role) {
      // Rediriger vers le dashboard approprié selon le rôle
      if (authStore.user?.role === 'Admin') {
        next('/admin')
      } else {
        next('/employee')
      }
      return
    }
  }
  
  next()
})

export default router
