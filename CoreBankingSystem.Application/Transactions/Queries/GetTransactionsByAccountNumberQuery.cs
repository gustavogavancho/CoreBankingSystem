using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionsByAccountNumberQuery(string AccountNumber) : IRequest<List<TransactionDto>>;

public class GetTransactionsByAccountNumberQueryHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetTransactionsByAccountNumberQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsByAccountNumberQuery request, CancellationToken cancellationToken)
    {
        var list = await repository.GetByAccountNumberAsync(request.AccountNumber, cancellationToken);
        return mapper.Map<List<TransactionDto>>(list);
    }
}
