public class BudgetCreateDto
{
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }  // opzionale
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
