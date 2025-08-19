using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Accounts.Queries;

public record GetAccountByNumberQuery(string AccountNumber) : IRequest<AccountDto?>;

public class GetAccountByNumberQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetAccountByNumberQuery, AccountDto?>
{
    public async Task<AccountDto?> Handle(GetAccountByNumberQuery request, CancellationToken cancellationToken)
    {
        var account = await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber, cancellationToken);
        return account is null ? null : mapper.Map<AccountDto>(account);
    }
}
