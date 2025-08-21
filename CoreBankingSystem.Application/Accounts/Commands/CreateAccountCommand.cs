using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Application.Common.Exceptions;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record CreateAccountCommand(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status,
    Guid ClientId
) : IRequest<AccountDto>;

public class CreateAccountCommandHandler(IAccountRepository repository, IClientRepository clientRepository, IMapper mapper)
    : IRequestHandler<CreateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // Validate duplicate account number
        var existing = await repository.GetByNumberAsync(request.AccountNumber);
        if (existing is not null)
        {
            throw new BadRequestException($"Account with number '{request.AccountNumber}' already exists.");
        }

        // Resolve provided ClientId to the correct FK (Clients.Id). Accept either Person PK (Id) or business ClientId.
        var client = await clientRepository.GetByIdAsync(request.ClientId)
                     ?? await clientRepository.GetByClientIdAsync(request.ClientId);
        if (client is null)
        {
            throw new BadRequestException($"Client not found for id '{request.ClientId}'. Use either the client's Id or ClientId.");
        }

        var entity = new Account
        {
            AccountNumber = request.AccountNumber,
            AccountType = request.AccountType,
            InitialBalance = request.InitialBalance,
            Status = request.Status,
            ClientId = client.Id // FK to Clients.Id (Person primary key)
        };

        await repository.AddAsync(entity);

        return mapper.Map<AccountDto>(entity);
    }
}
