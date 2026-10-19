using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestionCongeFrais.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore] // jamais renvoyé par l'API
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Employé";

        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public User? Manager { get; set; }

        public ICollection<User>? Subordinates { get; set; }
        public bool PasswordChanged { get; set; } = false;
    }
}
