using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionsByAccountNumberQuery(string AccountNumber) : IRequest<List<TransactionDto>>;

public class GetTransactionsByAccountNumberQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetTransactionsByAccountNumberQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsByAccountNumberQuery request, CancellationToken cancellationToken)
    {
        var list = await context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountNumber == request.AccountNumber)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<TransactionDto>>(list);
    }
}
