using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record DeleteTransactionCommand(Guid TransactionId) : IRequest<Unit>;

public class DeleteTransactionCommandHandler(ITransactionRepository repository)
    : IRequestHandler<DeleteTransactionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (entity is null) throw new NotFoundException("Transaction", request.TransactionId);

        await repository.RemoveAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
