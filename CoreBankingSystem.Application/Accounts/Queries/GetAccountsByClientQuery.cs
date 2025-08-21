using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;

namespace CoreBankingSystem.Application.Accounts.Queries;

// With no FK from Account to Client, this query will be removed to avoid confusion.
public record GetAccountsByClientQuery(Guid ClientId) : IRequest<List<AccountDto>>;

public class GetAccountsByClientQueryHandler(IAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountsByClientQuery, List<AccountDto>>
{
    public Task<List<AccountDto>> Handle(GetAccountsByClientQuery request, CancellationToken cancellationToken)
    {
        // Deprecated: return empty list to maintain compatibility if invoked
        return Task.FromResult(new List<AccountDto>());
    }
}
