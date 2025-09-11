using System.ComponentModel.DataAnnotations;

public class LeaveType
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int MaxDays { get; set; } = 0; // 0 = illimité
}