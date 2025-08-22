using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Reports.Models;
using CoreBankingSystem.Application.Reports.Queries;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Domain.Entities;
using Moq;

namespace CoreBankingSystem.Application.Tests.Reports;

public class GetClientStatementReportQueryTests
{
    private readonly Mock<IClientRepository> _clientRepo = new();
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<ITransactionRepository> _txRepo = new();
    private readonly IMapper _mapper;

    public GetClientStatementReportQueryTests()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.AddProfile(new Application.Transactions.TransactionMappingProfile());
        });
        _mapper = cfg.CreateMapper();
    }

    [Fact]
    public async Task Handle_ComputesBalancesAndTotals_IncludesTransactions()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var client = new Client { Id = Guid.NewGuid(), ClientId = clientId, Name = "Test" };
        _clientRepo.Setup(r => r.GetByClientIdAsync(clientId)).ReturnsAsync(client);

        var account = new Account { AccountNumber = "ACC-1", AccountType = "Checking", InitialBalance = 100m, Status = true, ClientId = client.Id };
        _accountRepo.Setup(r => r.GetByClientIdAsync(clientId)).ReturnsAsync(new List<Account> { account });

        var start = new DateTime(2025, 1, 10);
        var end = new DateTime(2025, 1, 20);
        var txs = new List<Transaction>
        {
            new() { AccountNumber = account.AccountNumber, Date = new DateTime(2025,1,1), TransactionType = "Deposit", Amount = 50m, Balance = 150m }, // before range
            new() { AccountNumber = account.AccountNumber, Date = new DateTime(2025,1,12), TransactionType = "Deposit", Amount = 25m, Balance = 175m }, // in range +
            new() { AccountNumber = account.AccountNumber, Date = new DateTime(2025,1,15), TransactionType = "Withdrawal", Amount = -10m, Balance = 165m }, // in range -
            new() { AccountNumber = account.AccountNumber, Date = new DateTime(2025,1,25), TransactionType = "Deposit", Amount = 5m, Balance = 170m }, // after range
        };
        _txRepo.Setup(r => r.GetByAccountNumberAsync(account.AccountNumber)).ReturnsAsync(txs);

        var handler = new GetClientStatementReportQueryHandler(_clientRepo.Object, _accountRepo.Object, _txRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetClientStatementReportQuery(clientId, start, end), CancellationToken.None);

        // Assert
        Assert.Equal(client.Id, result.Id);
        Assert.Single(result.Accounts);
        var acc = result.Accounts[0];
        Assert.Equal("ACC-1", acc.AccountNumber);
        // starting = 100 + 50 = 150
        Assert.Equal(150m, acc.StartingBalance);
        // credits in range = 25
        Assert.Equal(25m, acc.TotalCredits);
        // debits in range = 10 (absolute)
        Assert.Equal(10m, acc.TotalDebits);
        // ending = starting + (25 - 10) = 165
        Assert.Equal(165m, acc.EndingBalance);
        // transactions list contains only in-range (12th and 15th)
        Assert.Equal(2, acc.Transactions.Count);
        Assert.All(acc.Transactions, t => Assert.InRange(t.Date, start, end));
    }
}
