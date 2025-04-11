using System.ComponentModel.DataAnnotations;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }

    public bool IsTransfer { get; set; } = false;
}
