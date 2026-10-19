using GestionCongeFrais.Data;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace GestionCongeFrais.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<object>> GetAllAsync()
        {
            var users = await _context.Users.Include(u => u.Manager).ToListAsync();
            Console.WriteLine($"Retrieved {users.Count} users from database");
            
            return users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                u.ManagerId,
                u.PasswordChanged,
                Manager = u.Manager != null ? new
                {
                    u.Manager.Id,
                    u.Manager.FullName,
                    u.Manager.Email,
                    u.Manager.Role
                } : null
            });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Manager).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateAsync(UserDTO dto)
        {
            Console.WriteLine($"Creating user with email: {dto.Email}");
            
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email déjà utilisé");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Role = dto.Role,
                ManagerId = dto.ManagerId,
                PasswordChanged = false // Nouvel utilisateur, n'a pas encore changé son mot de passe
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"User created successfully with ID: {user.Id}");
            return user;
        }

        public async Task<bool> UpdateAsync(int id, UserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.ManagerId = dto.ManagerId;
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<object> GetOrganigramAsync()
        {
            var allUsers = await _context.Users
                .Include(u => u.Subordinates)
                .ToListAsync();

            // Trouver les racines (utilisateurs sans manager)
            var rootUsers = allUsers.Where(u => u.ManagerId == null).ToList();

            // Construire l'arbre hiérarchique
            var organigram = rootUsers.Select(u => BuildUserNode(u, allUsers)).ToList();

            return organigram;
        }

        private object BuildUserNode(User user, List<User> allUsers)
        {
            return new
            {
                id = user.Id,
                name = user.FullName,
                email = user.Email,
                role = user.Role,
                subordinates = allUsers
                    .Where(u => u.ManagerId == user.Id)
                    .Select(u => BuildUserNode(u, allUsers))
                    .ToList()
            };
        }
    }
}
