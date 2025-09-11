using GestionCongeFrais.Models;
using GestionCongeFrais.DTOs;
namespace GestionCongeFrais.Services
{
    public interface IUserService
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(UserDTO dto);
        Task<bool> UpdateAsync(int id, UserDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<object> GetOrganigramAsync();
    }
}
