public class TransactionCreateDto
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }  // opzionale, default = now
    public int AccountId { get; set; }
    public int? SubcategoryId { get; set; }
}
