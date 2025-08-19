using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto?>;

public class GetTransactionByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.TransactionId == request.TransactionId, cancellationToken);
        return transaction is null ? null : mapper.Map<TransactionDto>(transaction);
    }
}
