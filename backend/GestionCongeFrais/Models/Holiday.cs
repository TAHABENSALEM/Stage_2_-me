namespace GestionCongeFrais.Models
{
    public class Holiday
    {
        public int Id { get; set; }                 // Identifiant du jour férié
        public DateTime Date { get; set; }          // Date du jour férié (ex: 2025-01-01)
        public required string Description { get; set; }     // Description (ex: Nouvel an)
    }

}
//Utilité :
//Ajoutés par l’admin
//S’affichent dans le calendrier de l’employé
//Ignorés dans les demandes de congés