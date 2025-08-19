using CoreBankingSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record DeleteAccountCommand(string AccountNumber) : IRequest<bool>;

public class DeleteAccountCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber, cancellationToken);
        if (entity is null) return false;

        context.Accounts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
