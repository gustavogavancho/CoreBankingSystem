using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task RemoveAsync(Client client, CancellationToken cancellationToken = default);
}
