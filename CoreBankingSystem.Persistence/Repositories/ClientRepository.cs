using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class ClientRepository(ApplicationDbContext context) : IClientRepository
{
    public async Task AddAsync(Client client)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync();
    }

    public async Task<List<Client>> GetAllAsync()
        => await context.Clients.AsNoTracking().ToListAsync();

    public async Task<Client?> GetByIdAsync(Guid id)
        => await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Client?> GetByClientIdAsync(Guid clientId)
        => await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == clientId);

    public async Task<Client?> GetByIdentificationAsync(string identification)
        => await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Identification == identification);

    public async Task RemoveAsync(Client client)
    {
        context.Clients.Remove(client);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        context.Clients.Update(client);
        await context.SaveChangesAsync();
    }
}
