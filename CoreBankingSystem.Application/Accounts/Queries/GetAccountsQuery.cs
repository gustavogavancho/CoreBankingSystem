using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountsQuery : IRequest<List<AccountDto>>;

public class GetAccountsQueryHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<List<AccountDto>>(accounts);
    }
}
