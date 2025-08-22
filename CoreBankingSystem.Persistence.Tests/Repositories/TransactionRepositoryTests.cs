using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Persistence;
using CoreBankingSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Tests.Repositories;

public class TransactionRepositoryTests
{
    private static ApplicationDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Add_GetAll_GetById_GetByAccountNumber_Update_Remove_Works()
    {
        await using var db = NewDb();
        var client = new Client { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Name = "Jane" };
        db.Clients.Add(client);
        db.Accounts.Add(new Account { AccountNumber = "ACC-1", AccountType = "Checking", InitialBalance = 100m, Status = true, ClientId = client.Id });
        await db.SaveChangesAsync();

        var repo = new TransactionRepository(db);
        var tx = new Transaction
        {
            TransactionId = Guid.NewGuid(),
            AccountNumber = "ACC-1",
            Date = new DateTime(2025, 1, 1),
            TransactionType = "Deposit",
            Amount = 50m,
            Balance = 150m
        };
        await repo.AddAsync(tx);

        var all = await repo.GetAllAsync();
        Assert.Single(all);

        var byId = await repo.GetByIdAsync(tx.TransactionId);
        Assert.NotNull(byId);

        var byAcc = await repo.GetByAccountNumberAsync("ACC-1");
        Assert.Single(byAcc);

        tx.Amount = 60m;
        await repo.UpdateAsync(tx);
        var updated = await repo.GetByIdAsync(tx.TransactionId);
        Assert.Equal(60m, updated!.Amount);

        await repo.RemoveAsync(tx);
        var deleted = await repo.GetByIdAsync(tx.TransactionId);
        Assert.Null(deleted);
    }
}
