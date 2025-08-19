using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionsQuery : IRequest<List<TransactionDto>>;

public class GetTransactionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await context.Transactions.AsNoTracking().ToListAsync(cancellationToken);
        return mapper.Map<List<TransactionDto>>(transactions);
    }
}
