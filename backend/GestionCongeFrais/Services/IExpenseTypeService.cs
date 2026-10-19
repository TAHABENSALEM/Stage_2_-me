using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;

namespace GestionCongeFrais.Services
{
    public interface IExpenseTypeService
    {
        Task<List<ExpenseType>> GetAllAsync();
        Task<ExpenseType?> GetByIdAsync(int id);
        Task<ExpenseType> CreateAsync(ExpenseTypeDTO dto);
        Task<bool> UpdateAsync(int id, ExpenseTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
