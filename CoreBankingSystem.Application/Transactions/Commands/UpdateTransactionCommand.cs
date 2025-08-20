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
    decimal Amount,
    decimal Balance
) : IRequest<TransactionDto?>;

public class UpdateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<UpdateTransactionCommand, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (entity is null) throw new NotFoundException("Transaction", request.TransactionId);

        entity.AccountNumber = request.AccountNumber;
        entity.Date = request.Date;
        entity.TransactionType = request.TransactionType;
        entity.Amount = request.Amount;
        entity.Balance = request.Balance;

        await repository.UpdateAsync(entity, cancellationToken);

        return mapper.Map<TransactionDto>(entity);
    }
}
