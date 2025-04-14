public class AccountResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "bank";
    public decimal InitialBalance { get; set; }

    public decimal CurrentBalance { get; set; }
}
