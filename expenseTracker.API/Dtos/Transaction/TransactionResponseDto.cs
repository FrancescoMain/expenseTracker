public class TransactionResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string? SubcategoryName { get; set; }

    public string? CategoryName { get; set; }
}
