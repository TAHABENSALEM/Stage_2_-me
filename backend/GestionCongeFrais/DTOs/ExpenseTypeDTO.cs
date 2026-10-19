using System.ComponentModel.DataAnnotations;

namespace GestionCongeFrais.DTOs
{
    public class ExpenseTypeDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
