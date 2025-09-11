using GestionCongeFrais.Models;
using GestionCongeFrais.DTOs;

namespace GestionCongeFrais.Services
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequest>> GetForUserAsync(int userId);
        Task<IEnumerable<LeaveRequest>> GetPendingForReviewerAsync(int reviewerId);
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<LeaveRequest> CreateAsync(int requesterId, LeaveRequestCreateDTO dto);
        Task<bool> ReviewAsync(int reviewerId, int requestId, LeaveRequestActionDTO action);
        Task<bool> CancelAsync(int requesterId, int requestId);

    }
}
