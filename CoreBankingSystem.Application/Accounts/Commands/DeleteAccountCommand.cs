using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record DeleteAccountCommand(string AccountNumber) : IRequest<Unit>;

public class DeleteAccountCommandHandler(IAccountRepository repository)
    : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByNumberAsync(request.AccountNumber, cancellationToken);
        if (entity is null) throw new NotFoundException("Account", request.AccountNumber);

        await repository.RemoveAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
