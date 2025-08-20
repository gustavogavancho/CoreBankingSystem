using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using CoreBankingSystem.Domain.Entities;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record CreateAccountCommand(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status
) : IRequest<AccountDto>;

public class CreateAccountCommandHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<CreateAccountCommand, AccountDto>
{
    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = new Account
        {
            AccountNumber = request.AccountNumber,
            AccountType = request.AccountType,
            InitialBalance = request.InitialBalance,
            Status = request.Status
        };

        await repository.AddAsync(entity, cancellationToken);

        return mapper.Map<AccountDto>(entity);
    }
}
