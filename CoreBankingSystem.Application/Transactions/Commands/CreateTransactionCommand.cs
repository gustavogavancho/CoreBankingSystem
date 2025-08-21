using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Transactions.Models;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record CreateTransactionCommand(
    string AccountNumber,
    DateTime Date,
    string TransactionType,
    decimal Amount,
    decimal Balance
) : IRequest<TransactionDto>;

public class CreateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Transaction
        {
            TransactionId = Guid.NewGuid(),
            AccountNumber = request.AccountNumber,
            Date = request.Date,
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            Balance = request.Balance
        };

        await repository.AddAsync(entity);

        return mapper.Map<TransactionDto>(entity);
    }
}
