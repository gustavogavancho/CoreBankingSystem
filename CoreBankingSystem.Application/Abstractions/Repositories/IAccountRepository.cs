using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAllAsync();
    Task<Account?> GetByNumberAsync(string accountNumber);
    Task<List<Account>> GetByClientIdAsync(Guid clientId);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task RemoveAsync(Account account);
}
