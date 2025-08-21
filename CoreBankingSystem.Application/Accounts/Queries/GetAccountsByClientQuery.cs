using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountsByClientQuery(Guid ClientId) : IRequest<List<AccountDto>>;

public class GetAccountsByClientQueryHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountsByClientQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsByClientQuery request, CancellationToken cancellationToken)
    {
        var accounts = await repository.GetByClientIdAsync(request.ClientId, cancellationToken);
        return mapper.Map<List<AccountDto>>(accounts);
    }
}
