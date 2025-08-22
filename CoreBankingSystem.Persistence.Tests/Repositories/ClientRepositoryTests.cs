using CoreBankingSystem.Domain.Entities;
using CoreBankingSystem.Persistence;
using CoreBankingSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence.Tests.Repositories;

public class ClientRepositoryTests
{
    private static ApplicationDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Add_GetById_GetByClientId_Update_Remove_Works()
    {
        await using var db = NewDb();
        var repo = new ClientRepository(db);

        var client = new Client
        {
            Id = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "John",
            Gender = "Male",
            Age = 30,
            Identification = "ID-1",
            PhoneNumber = "+1",
            Password = "pwd",
            Status = true
        };

        await repo.AddAsync(client);

        var fromId = await repo.GetByIdAsync(client.Id);
        var fromClientId = await repo.GetByClientIdAsync(client.ClientId);
        Assert.NotNull(fromId);
        Assert.NotNull(fromClientId);
        Assert.Equal(client.Name, fromId!.Name);

        client.Name = "John Updated";
        await repo.UpdateAsync(client);
        var updated = await repo.GetByIdAsync(client.Id);
        Assert.Equal("John Updated", updated!.Name);

        await repo.RemoveAsync(client);
        var deleted = await repo.GetByIdAsync(client.Id);
        Assert.Null(deleted);
    }
}
