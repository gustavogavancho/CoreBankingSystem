namespace CoreBankingSystem.Domain.Entities;

public class Client : Person
{
    // Separate Client PK as requested
    public Guid ClientId { get; set; }

    public string Password { get; set; } = string.Empty;
    public bool Status { get; set; }
}
