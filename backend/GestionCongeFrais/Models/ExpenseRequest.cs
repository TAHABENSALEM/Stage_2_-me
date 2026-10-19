using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCongeFrais.Models
{
    public enum ExpenseStatus { Pending, Approved, Rejected, Cancelled }

    public class ExpenseRequest
    {
        public int Id { get; set; }

        [Required]
        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public User? Requester { get; set; }

        [Required]
        public int ExpenseTypeId { get; set; }
        [ForeignKey("ExpenseTypeId")]
        public ExpenseType? ExpenseType { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        public int? ReviewedById { get; set; }
        [ForeignKey("ReviewedById")]
        public User? ReviewedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }
}
