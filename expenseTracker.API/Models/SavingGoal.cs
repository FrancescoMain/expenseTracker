using System.ComponentModel.DataAnnotations;

public class SavingGoal
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
