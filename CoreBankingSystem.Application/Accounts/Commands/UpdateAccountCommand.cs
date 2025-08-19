using AutoMapper;
using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Accounts.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Application.Accounts.Commands;

public record UpdateAccountCommand(
    string AccountNumber,
    string AccountType,
    decimal InitialBalance,
    bool Status
) : IRequest<AccountDto?>;

public class UpdateAccountCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateAccountCommand, AccountDto?>
{
    public async Task<AccountDto?> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber, cancellationToken);
        if (entity is null) return null;

        entity.AccountType = request.AccountType;
        entity.InitialBalance = request.InitialBalance;
        entity.Status = request.Status;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<AccountDto>(entity);
    }
}
