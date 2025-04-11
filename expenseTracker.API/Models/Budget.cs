public class Budget
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }

    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
