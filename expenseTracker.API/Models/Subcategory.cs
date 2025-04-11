using System.ComponentModel.DataAnnotations;

public class Subcategory
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
