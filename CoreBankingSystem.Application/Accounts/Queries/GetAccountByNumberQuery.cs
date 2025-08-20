using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountByNumberQuery(string AccountNumber) : IRequest<AccountDto?>;

public class GetAccountByNumberQueryHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountByNumberQuery, AccountDto?>
{
    public async Task<AccountDto?> Handle(GetAccountByNumberQuery request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByNumberAsync(request.AccountNumber, cancellationToken);
        return account is null ? null : mapper.Map<AccountDto>(account);
    }
}
