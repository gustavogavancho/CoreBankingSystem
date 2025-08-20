using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task RemoveAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
