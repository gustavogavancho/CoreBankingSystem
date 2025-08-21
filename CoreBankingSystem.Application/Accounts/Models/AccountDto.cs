namespace CoreBankingSystem.Application.Accounts.Models;

public class AccountDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
    public bool Status { get; set; }
    public Guid ClientId { get; set; }
}
