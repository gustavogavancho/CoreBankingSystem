namespace CoreBankingSystem.Application.Transactions.Models;

public class TransactionDto
{
    public Guid TransactionId { get; set; }
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
}
