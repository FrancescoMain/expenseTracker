using System.ComponentModel.DataAnnotations;

public class Account
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "bank"; // banca, contanti, prepagata...

    public decimal InitialBalance { get; set; }

    // FK
    public int UserId { get; set; }
    public User? User { get; set; }

    // Navigazione
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
