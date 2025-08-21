using AutoMapper;
using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Application.Common.Exceptions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;

namespace CoreBankingSystem.Application.Transactions.Queries;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto>;

public class GetTransactionByIdQueryHandler(ITransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    public async Task<TransactionDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await repository.GetByIdAsync(request.TransactionId);
        if (transaction is null) throw new NotFoundException(nameof(TransactionDto), request.TransactionId);
        return mapper.Map<TransactionDto>(transaction);
    }
}
