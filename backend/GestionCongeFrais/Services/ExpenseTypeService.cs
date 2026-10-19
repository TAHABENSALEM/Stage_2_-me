using GestionCongeFrais.Data;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCongeFrais.Services
{
    public class ExpenseTypeService : IExpenseTypeService
    {
        private readonly AppDbContext _context;

        public ExpenseTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseType>> GetAllAsync()
        {
            return await _context.ExpenseTypes.ToListAsync();
        }

        public async Task<ExpenseType?> GetByIdAsync(int id)
        {
            return await _context.ExpenseTypes.FindAsync(id);
        }

        public async Task<ExpenseType> CreateAsync(ExpenseTypeDTO dto)
        {
            var expenseType = new ExpenseType
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.ExpenseTypes.Add(expenseType);
            await _context.SaveChangesAsync();
            return expenseType;
        }

        public async Task<bool> UpdateAsync(int id, ExpenseTypeDTO dto)
        {
            var expenseType = await _context.ExpenseTypes.FindAsync(id);
            if (expenseType == null) return false;

            expenseType.Name = dto.Name;
            expenseType.Description = dto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var expenseType = await _context.ExpenseTypes.FindAsync(id);
            if (expenseType == null) return false;

            _context.ExpenseTypes.Remove(expenseType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
