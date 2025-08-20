using CoreBankingSystem.Application.Abstractions.Repositories;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Repositories;

public class ClientRepository(ApplicationDbContext context) : IClientRepository
{
    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
    {
        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Clients.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Client?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        => await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);

    public async Task RemoveAsync(Client client, CancellationToken cancellationToken = default)
    {
        context.Clients.Remove(client);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        context.Clients.Update(client);
        await context.SaveChangesAsync(cancellationToken);
    }
}
