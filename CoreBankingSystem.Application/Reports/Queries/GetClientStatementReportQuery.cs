using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Reports.Models;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Reports.Queries;

public record GetClientStatementReportQuery(Guid ClientId, DateTime StartDate, DateTime EndDate) : IRequest<ClientStatementReportDto>;

public class GetClientStatementReportQueryHandler(
    IClientRepository clientRepository,
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IMapper mapper
) : IRequestHandler<GetClientStatementReportQuery, ClientStatementReportDto>
{
    public async Task<ClientStatementReportDto> Handle(GetClientStatementReportQuery request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByClientIdAsync(request.ClientId)
                     ?? throw new InvalidOperationException($"Client with ClientId '{request.ClientId}' was not found");

        // Get accounts for this client
        var accounts = await accountRepository.GetByClientIdAsync(request.ClientId);

        var report = new ClientStatementReportDto
        {
            Id = client.Id,
            ClientId = client.ClientId,
            Name = client.Name,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
        };

        foreach (var account in accounts)
        {
            var allTransactions = await transactionRepository.GetByAccountNumberAsync(account.AccountNumber);

            // Transactions prior to start date determine starting balance
            var transactionsBefore = allTransactions.Where(t => t.Date.Date < request.StartDate.Date);
            var startingBalance = account.InitialBalance + transactionsBefore.Sum(t => t.Amount);

            // Transactions within range
            var inRange = allTransactions
                .Where(t => t.Date.Date >= request.StartDate.Date && t.Date.Date <= request.EndDate.Date)
                .ToList();

            var totalCredits = inRange.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var totalDebits = inRange.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            var endingBalance = startingBalance + inRange.Sum(t => t.Amount);

            report.Accounts.Add(new AccountStatementDto
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Status = account.Status,
                StartingBalance = startingBalance,
                TotalCredits = totalCredits,
                TotalDebits = totalDebits,
                EndingBalance = endingBalance,
                Transactions = mapper.Map<List<TransactionDto>>(inRange)
            });
        }

        return report;
    }
}
