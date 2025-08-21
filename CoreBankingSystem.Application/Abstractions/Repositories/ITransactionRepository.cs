using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<List<Transaction>> GetByAccountNumberAsync(string accountNumber);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task RemoveAsync(Transaction transaction);
}
