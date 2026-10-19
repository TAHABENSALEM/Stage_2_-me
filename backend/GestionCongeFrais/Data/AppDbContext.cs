using Microsoft.EntityFrameworkCore;
using GestionCongeFrais.Models;

namespace GestionCongeFrais.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tables principales
       
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<ExpenseRequest> ExpenseRequests { get; set; }
        // Tables liées aux employés (à venir)
        //public DbSet<LeaveRequest> LeaveRequests { get; set; }
        //public DbSet<ExpenseNote> ExpenseNotes { get; set; }


    }
}
