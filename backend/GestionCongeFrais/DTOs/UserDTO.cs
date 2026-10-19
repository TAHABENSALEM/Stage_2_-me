namespace GestionCongeFrais.DTOs
{
    public class UserDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // mot de passe clair à hasher
        public string Role { get; set; } = "Employé";
        public int? ManagerId { get; set; }
    }
}
