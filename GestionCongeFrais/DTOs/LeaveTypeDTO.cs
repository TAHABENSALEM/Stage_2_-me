using System.ComponentModel.DataAnnotations;

namespace GestionCongeFrais.DTOs
{
    public class LeaveTypeDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int MaxDays { get; set; } = 0; // 0 = illimité
    }
}
