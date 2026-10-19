using GestionCongeFrais.Data;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCongeFrais.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;

        public LeaveRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest> CreateAsync(int requesterId, LeaveRequestCreateDTO dto)
        {
            if (dto.EndDate.Date < dto.StartDate.Date)
                throw new ArgumentException("EndDate must be >= StartDate");

            // Vérifier que le type de congé existe
            var type = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId)
                ?? throw new Exception("LeaveType not found");

            // Vérifier que la période ne contient pas de jours fériés
            var holidays = await _context.Holidays
                .Where(h => h.Date.Date >= dto.StartDate.Date && h.Date.Date <= dto.EndDate.Date)
                .ToListAsync();

            if (holidays.Any())
                throw new Exception("La période inclut des jours fériés. Modifiez la période.");

            // Vérifier chevauchement avec une autre demande approuvée
            bool overlap = await _context.LeaveRequests
                .AnyAsync(l => l.RequesterId == requesterId
                               && l.Status == LeaveStatus.Approved
                               && l.StartDate <= dto.EndDate.Date
                               && l.EndDate >= dto.StartDate.Date);

            if (overlap)
                throw new Exception("La période chevauche un autre congé déjà approuvé.");

            // Vérifier le solde : MaxDays par an et par type de congé (0 = illimité)
            if (type.MaxDays > 0)
            {
                int requestedDays = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;
                int year = dto.StartDate.Year;

                var approvedLeaves = await _context.LeaveRequests
                    .Where(l => l.RequesterId == requesterId
                                && l.LeaveTypeId == dto.LeaveTypeId
                                && l.Status == LeaveStatus.Approved
                                && l.StartDate.Year == year)
                    .ToListAsync();

                int usedDays = approvedLeaves.Sum(l => (l.EndDate.Date - l.StartDate.Date).Days + 1);

                if (usedDays + requestedDays > type.MaxDays)
                    throw new Exception($"Solde insuffisant pour « {type.Name} » : {requestedDays} jour(s) demandé(s), {usedDays}/{type.MaxDays} déjà utilisé(s) en {year}.");
            }

            // Création de la demande
            var leave = new LeaveRequest
            {
                RequesterId = requesterId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                Reason = dto.Reason,
                Status = LeaveStatus.Pending
            };

            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();
            return leave;
        }



        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(l => l.Requester)
                .Include(l => l.LeaveType)
                .Include(l => l.ReviewedBy)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetForUserAsync(int userId)
        {
            return await _context.LeaveRequests
                .Where(l => l.RequesterId == userId)
                .Include(l => l.LeaveType)
                .Include(l => l.ReviewedBy)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingForReviewerAsync(int reviewerId)
        {
            // obtenir IDs des subordonnés directs
            var subordinateIds = await _context.Users
                .Where(u => u.ManagerId == reviewerId)
                .Select(u => u.Id)
                .ToListAsync();

            return await _context.LeaveRequests
                .Where(l => subordinateIds.Contains(l.RequesterId) && l.Status == LeaveStatus.Pending)
                .Include(l => l.Requester)
                .Include(l => l.LeaveType)
                .ToListAsync();
        }

        public async Task<bool> ReviewAsync(int reviewerId, int requestId, LeaveRequestActionDTO action)
        {
            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) return false;
            if (req.Status != LeaveStatus.Pending) return false;

            var reviewer = await _context.Users.FindAsync(reviewerId);
            if (reviewer == null) return false;

            // Autorisé si Admin ou manager direct
            if (reviewer.Role != "Admin")
            {
                var requester = await _context.Users.FindAsync(req.RequesterId);
                if (requester == null) return false;
                if (requester.ManagerId != reviewerId) return false;
            }

            req.Status = action.Approve ? LeaveStatus.Approved : LeaveStatus.Rejected;
            req.ReviewedById = reviewerId;
            req.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelAsync(int requesterId, int requestId)
        {
            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) return false;

            // Vérifier que c'est bien l'utilisateur qui a fait la demande
            if (req.RequesterId != requesterId) return false;

            // Annulation possible uniquement si en attente
            if (req.Status != LeaveStatus.Pending) return false;

            req.Status = LeaveStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }
       

    }
}
