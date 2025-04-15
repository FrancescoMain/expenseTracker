public class TransferResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string FromAccountName { get; set; } = "";
    public string? ToAccountName { get; set; } // null se saving goal
    public string? SavingGoalName { get; set; } // null se trasferimento classico
    public string? Note { get; set; }
}
