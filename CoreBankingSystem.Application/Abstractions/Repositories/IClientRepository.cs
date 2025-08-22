using CoreBankingSystem.Domain.Entities;

namespace CoreBankingSystem.Application.Abstractions.Repositories;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(Guid id);
    Task<Client?> GetByClientIdAsync(Guid clientId);
    Task<Client?> GetByIdentificationAsync(string identification);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task RemoveAsync(Client client);
}
