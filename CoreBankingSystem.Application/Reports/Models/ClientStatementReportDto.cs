using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Transactions.Models;

namespace CoreBankingSystem.Application.Reports.Models;

public class ClientStatementReportDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<AccountStatementDto> Accounts { get; set; } = new();
}

public class AccountStatementDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public bool Status { get; set; }

    public decimal StartingBalance { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal EndingBalance { get; set; }

    // Transactions within the requested date range
    public List<TransactionDto> Transactions { get; set; } = new();
}
