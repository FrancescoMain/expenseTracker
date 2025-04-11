public class RecurringExpense
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal ExpectedAmount { get; set; }

    public string Frequency { get; set; } = "monthly"; // "weekly", "monthly", "yearly", etc.

    public DateTime NextDueDate { get; set; }

    public decimal? ActualAmount { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int? AccountId { get; set; }
    public Account? Account { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }
}
