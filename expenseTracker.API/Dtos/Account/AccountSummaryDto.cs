public class AccountSummaryDto
{
    public List<AccountResponseDto> Accounts { get; set; } = new();
    public decimal TotalBalance { get; set; }
}
