using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record UpdateAccountCommand(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status,
    Guid ClientId
) : IRequest<AccountDto?>;

public class UpdateAccountCommandHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<UpdateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByNumberAsync(request.AccountNumber, cancellationToken);
        if (entity is null) throw new NotFoundException("Account", request.AccountNumber);

        entity.AccountType = request.AccountType;
        entity.InitialBalance = request.InitialBalance;
        entity.Status = request.Status;
        entity.ClientId = request.ClientId;

        await repository.UpdateAsync(entity, cancellationToken);

        return mapper.Map<AccountDto>(entity);
    }
}
