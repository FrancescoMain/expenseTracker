using System.ComponentModel.DataAnnotations;

public class Transfer
{
    public int Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int FromAccountId { get; set; }
    public Account? FromAccount { get; set; }

    public int? ToAccountId { get; set; } // 👈 deve essere nullable
    public Account? ToAccount { get; set; }

    public int? SavingGoalId { get; set; } // 👈 collegamento opzionale all'obiettivo
    public SavingGoal? SavingGoal { get; set; }

    public string? Note { get; set; }
}
