using CoreBankingSystem.Application.Abstractions.Repositories;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record DeleteTransactionCommand(Guid TransactionId) : IRequest<bool>;

public class DeleteTransactionCommandHandler(ITransactionRepository repository)
    : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (entity is null) return false;

        await repository.RemoveAsync(entity, cancellationToken);

        return true;
    }
}
