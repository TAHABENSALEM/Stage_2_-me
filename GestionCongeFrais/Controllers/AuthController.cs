using Microsoft.AspNetCore.Mvc;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using GestionCongeFrais.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestionCongeFrais.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestionCongeFrais.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _hasher;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, IPasswordHasher<User> hasher, JwtService jwtService)
        {
            _context = context;
            _hasher = hasher;
            _jwtService = jwtService;
        }

        // Dans AuthController.cs - Méthode Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Utilisateur non trouvé.");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Mot de passe incorrect.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role,
                MustChangePassword = !user.PasswordChanged
            });
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("Utilisateur non trouvé.");

            // Vérifier l'ancien mot de passe
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Ancien mot de passe incorrect.");

            // Hasher le nouveau mot de passe
            user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);
            user.PasswordChanged = true;

            await _context.SaveChangesAsync();

            return Ok("Mot de passe changé avec succès.");
        }
    }
}
