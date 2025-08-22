using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Persistence;
using CoreBankingSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Tests.Repositories;

public class AccountRepositoryTests
{
    private static ApplicationDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Add_GetByNumber_GetByClientId_Update_Remove_Works()
    {
        await using var db = NewDb();
        var client = new Client { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Name = "Jane" };
        db.Clients.Add(client);
        await db.SaveChangesAsync();

        var repo = new AccountRepository(db);
        var account = new Account
        {
            AccountNumber = "ACC-123",
            AccountType = "Checking",
            InitialBalance = 100m,
            Status = true,
            ClientId = client.Id
        };
        await repo.AddAsync(account);

        var byNum = await repo.GetByNumberAsync("ACC-123");
        Assert.NotNull(byNum);

        var byClientId = await repo.GetByClientIdAsync(client.ClientId);
        Assert.Single(byClientId);

        account.Status = false;
        await repo.UpdateAsync(account);
        var updated = await repo.GetByNumberAsync("ACC-123");
        Assert.False(updated!.Status);

        await repo.RemoveAsync(account);
        var deleted = await repo.GetByNumberAsync("ACC-123");
        Assert.Null(deleted);
    }
}
