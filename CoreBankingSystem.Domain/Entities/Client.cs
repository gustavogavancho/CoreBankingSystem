namespace CoreBankingSystem.Domain.Entities;

public class Client : Person
{
    // Separate Client PK as requested
    public Guid ClientId { get; set; }

    public string Password { get; set; } = string.Empty;
    public bool Status { get; set; }

    // Navigation: a client has many accounts
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
