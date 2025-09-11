using GestionCongeFrais.Models;
using GestionCongeFrais.DTOs;

namespace GestionCongeFrais.Services
{
    public interface IExpenseRequestService
    {
        Task<IEnumerable<ExpenseRequest>> GetForUserAsync(int userId);
        Task<IEnumerable<ExpenseRequest>> GetPendingForReviewerAsync(int reviewerId);
        Task<ExpenseRequest?> GetByIdAsync(int id);
        Task<ExpenseRequest> CreateAsync(int requesterId, ExpenseRequestCreateDTO dto);
        Task<bool> ReviewAsync(int reviewerId, int requestId, ExpenseRequestActionDTO action);
    }
}
