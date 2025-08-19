using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountsByClientQuery(Guid ClientId) : IRequest<List<AccountDto>>;

public class GetAccountsByClientQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAccountsByClientQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsByClientQuery request, CancellationToken cancellationToken)
    {
        // For now, accounts are not linked to clients with FK in domain. This query returns all accounts.
        var accounts = await context.Accounts.AsNoTracking().ToListAsync(cancellationToken);
        return mapper.Map<List<AccountDto>>(accounts);
    }
}
