using CoreBankingSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record DeleteTransactionCommand(Guid TransactionId) : IRequest<bool>;

public class DeleteTransactionCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == request.TransactionId, cancellationToken);
        if (entity is null) return false;

        context.Transactions.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
