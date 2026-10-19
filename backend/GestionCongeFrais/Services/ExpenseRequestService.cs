using GestionCongeFrais.Data;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCongeFrais.Services
{
    public class ExpenseRequestService : IExpenseRequestService
    {
        private readonly AppDbContext _context;

        public ExpenseRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseRequest> CreateAsync(int requesterId, ExpenseRequestCreateDTO dto)
        {
            var type = await _context.ExpenseTypes.FindAsync(dto.ExpenseTypeId)
                ?? throw new Exception("ExpenseType not found");

            if (dto.Amount <= 0) throw new ArgumentException("Amount must be > 0");

            var ex = new ExpenseRequest
            {
                RequesterId = requesterId,
                ExpenseTypeId = dto.ExpenseTypeId,
                Amount = dto.Amount,
                Description = dto.Description,
                Status = ExpenseStatus.Pending
            };

            _context.ExpenseRequests.Add(ex);
            await _context.SaveChangesAsync();
            return ex;
        }

        public async Task<ExpenseRequest?> GetByIdAsync(int id)
        {
            return await _context.ExpenseRequests
                .Include(e => e.Requester)
                .Include(e => e.ExpenseType)
                .Include(e => e.ReviewedBy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExpenseRequest>> GetForUserAsync(int userId)
        {
            return await _context.ExpenseRequests
                .Where(e => e.RequesterId == userId)
                .Include(e => e.ExpenseType)
                .Include(e => e.ReviewedBy)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExpenseRequest>> GetPendingForReviewerAsync(int reviewerId)
        {
            var subordinateIds = await _context.Users
                .Where(u => u.ManagerId == reviewerId)
                .Select(u => u.Id)
                .ToListAsync();

            return await _context.ExpenseRequests
                .Where(e => subordinateIds.Contains(e.RequesterId) && e.Status == ExpenseStatus.Pending)
                .Include(e => e.Requester)
                .Include(e => e.ExpenseType)
                .ToListAsync();
        }

        public async Task<bool> ReviewAsync(int reviewerId, int requestId, ExpenseRequestActionDTO action)
        {
            var req = await _context.ExpenseRequests.FindAsync(requestId);
            if (req == null) return false;
            if (req.Status != ExpenseStatus.Pending) return false;

            var reviewer = await _context.Users.FindAsync(reviewerId);
            if (reviewer == null) return false;

            if (reviewer.Role != "Admin")
            {
                var requester = await _context.Users.FindAsync(req.RequesterId);
                if (requester == null) return false;
                if (requester.ManagerId != reviewerId) return false;
            }

            req.Status = action.Approve ? ExpenseStatus.Approved : ExpenseStatus.Rejected;
            req.ReviewedById = reviewerId;
            req.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
