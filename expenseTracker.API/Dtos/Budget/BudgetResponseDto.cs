public class BudgetResponseDto
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = "";
    public string? SubcategoryName { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public decimal Spent { get; set; }  // 🔥 somma spese collegate
}
