using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;

namespace GestionCongeFrais.Services
{
    public interface ILeaveTypeService
    {
        Task<IEnumerable<LeaveType>> GetAllAsync();
        Task<LeaveType?> GetByIdAsync(int id);
        Task<LeaveType> CreateAsync(LeaveTypeDTO dto);
        Task<bool> UpdateAsync(int id, LeaveTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
