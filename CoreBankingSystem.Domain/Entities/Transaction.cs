namespace CoreBankingSystem.Domain.Entities;

public class Transaction
{
    // Primary Key
    public Guid TransactionId { get; set; }

    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }

    // FK to Account
    public string AccountNumber { get; set; } = string.Empty;
    public Account? Account { get; set; }
}
