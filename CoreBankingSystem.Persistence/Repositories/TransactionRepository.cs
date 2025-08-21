using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class TransactionRepository(ApplicationDbContext db) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllAsync()
        => await db.Transactions.AsNoTracking().ToListAsync();

    public async Task<Transaction?> GetByIdAsync(Guid id)
        => await db.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.TransactionId == id);

    public async Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber)
        => await db.Transactions.AsNoTracking().Where(t => t.AccountNumber == accountNumber).ToListAsync();

    public async Task AddAsync(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Transaction transaction)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();
    }
}
