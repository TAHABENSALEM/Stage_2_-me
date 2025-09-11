using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionCongeFrais.Data;
using GestionCongeFrais.Models;
using GestionCongeFrais.DTOs;

namespace GestionCongeFrais.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly AppDbContext _context;

        public HolidayService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Holiday>> GetAllAsync()
        {
            return await _context.Holidays.ToListAsync();
        }

        public async Task<Holiday> GetByIdAsync(int id)
        {
            return await _context.Holidays.FindAsync(id);
        }

        public async Task<Holiday> CreateAsync(HolidayDTO dto)
        {
            var holiday = new Holiday
            {
                Date = dto.Date,
                Description = dto.Description
            };
            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();
            return holiday;
        }

        public async Task<Holiday> UpdateAsync(int id, HolidayDTO dto)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null) return null;

            holiday.Date = dto.Date;
            holiday.Description = dto.Description;
            await _context.SaveChangesAsync();

            return holiday;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null) return false;

            _context.Holidays.Remove(holiday);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}