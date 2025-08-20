using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class TransactionRepository(ApplicationDbContext db) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Transactions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await db.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.TransactionId == id, cancellationToken);

    public async Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
        => await db.Transactions.AsNoTracking().Where(t => t.AccountNumber == accountNumber).ToListAsync(cancellationToken);

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }
}
