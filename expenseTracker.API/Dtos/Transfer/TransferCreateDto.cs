public class TransferCreateDto
{
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
    public int FromAccountId { get; set; }
    public int? ToAccountId { get; set; }
    public int? SavingGoalId { get; set; }
    public string? Note { get; set; }
}
