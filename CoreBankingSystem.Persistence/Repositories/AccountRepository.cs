using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class AccountRepository(ApplicationDbContext db) : IAccountRepository
{
    public async Task<List<Account>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Accounts.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Account?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
        => await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);

    // Interpret the input Guid as the business Client.ClientId, not the Person.Id
    public async Task<List<Account>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        => await db.Accounts
            .AsNoTracking()
            .Where(a => a.Client != null && a.Client.ClientId == clientId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        db.Accounts.Add(account);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        db.Accounts.Update(account);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Account account, CancellationToken cancellationToken = default)
    {
        db.Accounts.Remove(account);
        await db.SaveChangesAsync(cancellationToken);
    }
}
