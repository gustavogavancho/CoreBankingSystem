using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record CreateTransactionCommand(
    string AccountNumber,
    DateTime Date,
    string TransactionType,
    decimal Amount
) : IRequest<TransactionDto>;

public class CreateTransactionCommandHandler(ITransactionRepository repository, IAccountRepository accountRepository, IMapper mapper)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        // Ensure account exists
        var account = await accountRepository.GetByNumberAsync(request.AccountNumber);
        if (account is null)
        {
            throw new BadRequestException($"Account with number '{request.AccountNumber}' not found.");
        }

        // Get existing transactions for the account
        var transactions = await repository.GetByAccountNumberAsync(request.AccountNumber);

        // Enforce daily negative transaction limit (<= 1000 absolute per day)
        if (request.Amount < 0)
        {
            var sameDayNegatives = transactions
                .Where(t => t.Date.Date == request.Date.Date && t.Amount < 0)
                .Sum(t => Math.Abs(t.Amount));

            var projectedNegatives = sameDayNegatives + Math.Abs(request.Amount);
            if (projectedNegatives > 1000m)
            {
                throw new BadRequestException("Daily transaction limit of 1000 exceeded");
            }
        }

        // Compute resulting balance using InitialBalance + sum(existing amounts) + current amount
        var currentBalance = account.InitialBalance + transactions.Sum(t => t.Amount);
        var newBalance = currentBalance + request.Amount;

        // Balance cannot be zero or negative
        if (newBalance < 0m)
        {
            throw new BadRequestException("Insufficient balance");
        }

        var entity = new Transaction
        {
            TransactionId = Guid.NewGuid(),
            AccountNumber = request.AccountNumber,
            Date = request.Date,
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            Balance = newBalance
        };

        await repository.AddAsync(entity);

        return mapper.Map<TransactionDto>(entity);
    }
}
