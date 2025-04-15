public class SavingGoalResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; } // Calcolato da transfers
    public string AccountName { get; set; } = string.Empty;
}