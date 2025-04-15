public class SavingGoalCreateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public int AccountId { get; set; }
}