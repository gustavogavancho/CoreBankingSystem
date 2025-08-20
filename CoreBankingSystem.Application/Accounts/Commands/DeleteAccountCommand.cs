using CoreBankingSystem.Application.Abstractions.Repositories;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record DeleteAccountCommand(string AccountNumber) : IRequest<bool>;

public class DeleteAccountCommandHandler(IAccountRepository repository)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByNumberAsync(request.AccountNumber, cancellationToken);
        if (entity is null) return false;

        await repository.RemoveAsync(entity, cancellationToken);

        return true;
    }
}
