namespace GestionCongeFrais.DTOs
{
    public class ExpenseRequestCreateDTO
    {
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
