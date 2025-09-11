using System.ComponentModel.DataAnnotations;

namespace GestionCongeFrais.Models
{
    public class ExpenseType
    {
  public int Id { get; set; }

 [Required]
  public string Name { get; set; } = string.Empty;
 public string? Description { get; set; }
    }
}
//Utilité :
//Ajoutés / modifiés par l’admin
//Utilisés par l’employé quand il soumet une note de frais
