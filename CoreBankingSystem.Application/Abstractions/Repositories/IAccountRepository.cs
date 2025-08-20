using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Account?> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<List<Account>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    Task RemoveAsync(Account account, CancellationToken cancellationToken = default);
}
