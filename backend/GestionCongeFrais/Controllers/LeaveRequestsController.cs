using Microsoft.AspNetCore.Mvc;
using GestionCongeFrais.Services;
using GestionCongeFrais.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GestionCongeFrais.Data;
using Microsoft.EntityFrameworkCore;
using GestionCongeFrais.Models;

namespace GestionCongeFrais.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _service;
        private readonly AppDbContext _context;

        public LeaveRequestsController(ILeaveRequestService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = await _service.GetForUserAsync(userId);
            return Ok(list);
        }

        // Lister les demandes à approuver (pour managers)
        [Authorize]
        [HttpGet("pending-for-me")]
        public async Task<IActionResult> GetPendingForMe()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = await _service.GetPendingForReviewerAsync(userId);
            return Ok(list);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var created = await _service.CreateAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var req = await _service.GetByIdAsync(id);
            if (req == null) return NotFound();
            return Ok(req);
        }

        [Authorize]
        [HttpPost("{id}/review")]
        public async Task<IActionResult> Review(int id, LeaveRequestActionDTO action)
        {
            var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _service.ReviewAsync(reviewerId, id, action);
            return success ? NoContent() : Forbid();
        }

        [Authorize]
        [HttpGet("pending/subordinates")]
        public async Task<IActionResult> GetPendingForReviewer()
        {
            var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = await _service.GetPendingForReviewerAsync(reviewerId);
            return Ok(list);
        }

        // Lister l'historique des décisions du manager pour ses subordonnés (demandes de congés)
        [Authorize]
        [HttpGet("my-decisions")]
        public async Task<IActionResult> GetMyDecisions()
        {
            var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Récupérer toutes les demandes que ce manager a révisées
            var decisions = await _context.LeaveRequests
                .Include(l => l.Requester)
                .Include(l => l.LeaveType)
                .Include(l => l.ReviewedBy)
                .Where(l => l.ReviewedById == reviewerId && 
                           (l.Status == LeaveStatus.Approved || l.Status == LeaveStatus.Rejected))
                .OrderByDescending(l => l.ReviewedAt)
                .ToListAsync();

            return Ok(decisions);
        }

        // Lister toutes les demandes (admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _context.LeaveRequests
                .Include(l => l.Requester)
                .Include(l => l.LeaveType)
                .ToListAsync();

            return Ok(leaves);
        }

        // Lister les congés approuvés pour le calendrier
        [Authorize]
        [HttpGet("calendar")]
        public async Task<IActionResult> GetApprovedForCalendar([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var approved = await _context.LeaveRequests
                .Include(l => l.Requester)
                .Include(l => l.LeaveType)
                .Where(l => l.Status == LeaveStatus.Approved &&
                           l.StartDate <= end && l.EndDate >= start)
                .ToListAsync();

            return Ok(approved);
        }
    }
}
