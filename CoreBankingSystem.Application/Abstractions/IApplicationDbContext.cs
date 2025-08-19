namespace CoreBankingSystem.Application.Abstractions;

using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    DbSet<Person> People { get; }
    DbSet<Client> Clients { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
