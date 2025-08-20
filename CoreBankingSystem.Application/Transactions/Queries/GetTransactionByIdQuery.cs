using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto?>;

public class GetTransactionByIdQueryHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await repository.GetByIdAsync(request.TransactionId, cancellationToken);
        return transaction is null ? null : mapper.Map<TransactionDto>(transaction);
    }
}
