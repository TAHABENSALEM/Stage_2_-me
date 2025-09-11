using GestionCongeFrais.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

using GestionCongeFrais.DTOs;
namespace GestionCongeFrais.Services
{
    public interface IHolidayService
    {
        Task<List<Holiday>> GetAllAsync();
        Task<Holiday> GetByIdAsync(int id);
        Task<Holiday> CreateAsync(HolidayDTO dto);
        Task<Holiday> UpdateAsync(int id, HolidayDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}