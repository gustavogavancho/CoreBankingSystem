using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record UpdateTransactionCommand(
    Guid TransactionId,
    string AccountNumber,
    DateTime Date,
    string TransactionType,
    decimal Amount
) : IRequest<TransactionDto?>;

public class UpdateTransactionCommandHandler(ITransactionRepository repository, IAccountRepository accountRepository, IMapper mapper)
    : IRequestHandler<UpdateTransactionCommand, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.TransactionId);
        if (entity is null) throw new NotFoundException("Transaction", request.TransactionId);

        // Ensure account exists
        var account = await accountRepository.GetByNumberAsync(request.AccountNumber);
        if (account is null)
        {
            throw new BadRequestException($"Account with number '{request.AccountNumber}' not found.");
        }

        // Get all transactions for the account excluding the one being updated
        var transactions = (await repository.GetByAccountNumberAsync(request.AccountNumber))
            .Where(t => t.TransactionId != request.TransactionId)
            .ToList();

        // Enforce daily negative transaction limit for the updated date
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

        // Recompute balance as InitialBalance + sum(other tx amounts) + this amount
        var baseBalance = account.InitialBalance + transactions.Sum(t => t.Amount);
        var newBalance = baseBalance + request.Amount;

        // Balance cannot be zero or negative
        if (newBalance < 0m)
        {
            throw new BadRequestException("Insufficient balance");
        }

        entity.AccountNumber = request.AccountNumber;
        entity.Date = request.Date;
        entity.TransactionType = request.TransactionType;
        entity.Amount = request.Amount;
        entity.Balance = newBalance;

        await repository.UpdateAsync(entity);

        return mapper.Map<TransactionDto>(entity);
    }
}
