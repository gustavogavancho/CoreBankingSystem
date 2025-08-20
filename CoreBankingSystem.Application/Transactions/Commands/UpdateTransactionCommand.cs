using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Transactions.Commands;

public record UpdateTransactionCommand(
    Guid TransactionId,
    string AccountNumber,
    DateTime Date,
    string TransactionType,
    decimal Amount,
    decimal Balance
) : IRequest<TransactionDto?>;

public class UpdateTransactionCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateTransactionCommand, TransactionDto?>
{
    public async Task<TransactionDto?> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == request.TransactionId, cancellationToken);
        if (entity is null) return null;

        entity.AccountNumber = request.AccountNumber;
        entity.Date = request.Date;
        entity.TransactionType = request.TransactionType;
        entity.Amount = request.Amount;
        entity.Balance = request.Balance;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<TransactionDto>(entity);
    }
}
