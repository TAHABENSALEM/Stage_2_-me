using GestionCongeFrais.Data;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCongeFrais.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly AppDbContext _context;

        public LeaveTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveType>> GetAllAsync()
        {
            return await _context.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType?> GetByIdAsync(int id)
        {
            return await _context.LeaveTypes.FindAsync(id);
        }

        public async Task<LeaveType> CreateAsync(LeaveTypeDTO dto)
        {
            var leaveType = new LeaveType
            {
                Name = dto.Name,
                Description = dto.Description,
                MaxDays = dto.MaxDays
            };

            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();
            return leaveType;
        }

        public async Task<bool> UpdateAsync(int id, LeaveTypeDTO dto)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null) return false;

            leaveType.Name = dto.Name;
            leaveType.Description = dto.Description;
            leaveType.MaxDays = dto.MaxDays;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null) return false;

            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
