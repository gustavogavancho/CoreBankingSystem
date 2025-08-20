using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionsQuery : IRequest<List<TransactionDto>>;

public class GetTransactionsQueryHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<List<TransactionDto>>(transactions);
    }
}
