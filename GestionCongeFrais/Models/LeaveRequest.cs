using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionCongeFrais.Models
{
    public enum LeaveStatus { Pending, Approved, Rejected, Cancelled }

    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public int RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public User? Requester { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }
        [ForeignKey("LeaveTypeId")]
        public LeaveType? LeaveType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        public int? ReviewedById { get; set; }
        [ForeignKey("ReviewedById")]
        public User? ReviewedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }
}
