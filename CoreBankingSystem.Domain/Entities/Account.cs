namespace CoreBankingSystem.Domain.Entities;

public class Account
{
    // Primary Key
    public string AccountNumber { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
    public bool Status { get; set; }

    // FK to Client (required)
    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    // Navigation
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
