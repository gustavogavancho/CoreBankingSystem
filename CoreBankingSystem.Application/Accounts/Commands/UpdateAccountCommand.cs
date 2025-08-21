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

public class UpdateAccountCommandHandler(IAccountRepository repository, IClientRepository clientRepository, IMapper mapper)
    : IRequestHandler<UpdateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByNumberAsync(request.AccountNumber);
        if (entity is null) throw new NotFoundException("Account", request.AccountNumber);

        // Resolve provided ClientId to the correct FK (Clients.Id). Accept either Person PK (Id) or business ClientId.
        var client = await clientRepository.GetByIdAsync(request.ClientId)
                     ?? await clientRepository.GetByClientIdAsync(request.ClientId);
        if (client is null)
        {
            throw new BadRequestException($"Client not found for id '{request.ClientId}'. Use either the client's Id or ClientId.");
        }

        entity.AccountType = request.AccountType;
        entity.InitialBalance = request.InitialBalance;
        entity.Status = request.Status;
        entity.ClientId = client.Id;

        await repository.UpdateAsync(entity);

        return mapper.Map<AccountDto>(entity);
    }
}
