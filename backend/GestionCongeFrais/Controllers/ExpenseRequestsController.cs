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
    public class ExpenseRequestsController : ControllerBase
    {
        private readonly IExpenseRequestService _service;
        private readonly AppDbContext _context;

        public ExpenseRequestsController(IExpenseRequestService service, AppDbContext context)
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
        public async Task<IActionResult> Create(ExpenseRequestCreateDTO dto)
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
        public async Task<IActionResult> Review(int id, ExpenseRequestActionDTO action)
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

        // Lister l'historique des décisions du manager pour ses subordonnés (notes de frais)
        [Authorize]
        [HttpGet("my-decisions")]
        public async Task<IActionResult> GetMyDecisions()
        {
            var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Récupérer toutes les notes de frais que ce manager a révisées
            var decisions = await _context.ExpenseRequests
                .Include(e => e.Requester)
                .Include(e => e.ExpenseType)
                .Include(e => e.ReviewedBy)
                .Where(e => e.ReviewedById == reviewerId && 
                           (e.Status == ExpenseStatus.Approved || e.Status == ExpenseStatus.Rejected))
                .OrderByDescending(e => e.ReviewedAt)
                .ToListAsync();

            return Ok(decisions);
        }
    }
}
