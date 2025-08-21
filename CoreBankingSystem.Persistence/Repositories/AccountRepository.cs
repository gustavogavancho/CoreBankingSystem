using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class AccountRepository(ApplicationDbContext db) : IAccountRepository
{
    public async Task<List<Account>> GetAllAsync()
        => await db.Accounts.AsNoTracking().ToListAsync();

    public async Task<Account?> GetByNumberAsync(string accountNumber)
        => await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

    // Interpret the input Guid as the business Client.ClientId, not the Person.Id
    public async Task<List<Account>> GetByClientIdAsync(Guid clientId)
        => await db.Accounts
            .AsNoTracking()
            .Where(a => a.Client != null && a.Client.ClientId == clientId)
            .ToListAsync();

    public async Task AddAsync(Account account)
    {
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        db.Accounts.Update(account);
        await db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Account account)
    {
        db.Accounts.Remove(account);
        await db.SaveChangesAsync();
    }
}
