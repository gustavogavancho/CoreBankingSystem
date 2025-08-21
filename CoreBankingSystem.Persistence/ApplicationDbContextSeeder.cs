using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        // Seed Clients
        if (!await db.Clients.AnyAsync(cancellationToken))
        {
            var client1 = new Client
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Name = "John Doe",
                Gender = "Male",
                Age = 35,
                Identification = "ID-1001",
                PhoneNumber = "+1-555-0100",
                Password = "P@ssw0rd1",
                Status = true
            };

            var client2 = new Client
            {
                Id = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Name = "Jane Smith",
                Gender = "Female",
                Age = 31,
                Identification = "ID-1002",
                PhoneNumber = "+1-555-0101",
                Password = "P@ssw0rd2",
                Status = true
            };

            db.Clients.AddRange(client1, client2);
        }

        await db.SaveChangesAsync(cancellationToken);

        // Seed Accounts linked to existing clients
        if (!await db.Accounts.AnyAsync(cancellationToken))
        {
            // pick two existing clients
            // IMPORTANT: Accounts.ClientId FK points to Clients.Id (Person primary key), not Client.ClientId (business id)
            var firstTwoClients = await db.Clients.Take(2).Select(c => c.Id).ToListAsync(cancellationToken);
            var clientA = firstTwoClients.ElementAtOrDefault(0);
            var clientB = firstTwoClients.ElementAtOrDefault(1);

            var acc1 = new Account
            {
                AccountNumber = "ACC-1001",
                AccountType = "Checking",
                InitialBalance = 1000m,
                Status = true,
                ClientId = clientA
            };

            var acc2 = new Account
            {
                AccountNumber = "ACC-2002",
                AccountType = "Savings",
                InitialBalance = 2500m,
                Status = true,
                ClientId = clientB
            };

            var acc3 = new Account
            {
                AccountNumber = "ACC-3003",
                AccountType = "Checking",
                InitialBalance = 500m,
                Status = true,
                ClientId = clientA
            };

            db.Accounts.AddRange(acc1, acc2, acc3);
        }

        // Seed Transactions
        if (!await db.Transactions.AnyAsync(cancellationToken))
        {
            var txs = new List<Transaction>
            {
                new() { TransactionId = Guid.NewGuid(), AccountNumber = "ACC-1001", Date = DateTime.UtcNow.AddDays(-5), TransactionType = "Deposit", Amount = 500m, Balance = 1500m },
                new() { TransactionId = Guid.NewGuid(), AccountNumber = "ACC-1001", Date = DateTime.UtcNow.AddDays(-3), TransactionType = "Withdrawal", Amount = -200m, Balance = 1300m },
                new() { TransactionId = Guid.NewGuid(), AccountNumber = "ACC-2002", Date = DateTime.UtcNow.AddDays(-4), TransactionType = "Deposit", Amount = 750m, Balance = 3250m },
                new() { TransactionId = Guid.NewGuid(), AccountNumber = "ACC-3003", Date = DateTime.UtcNow.AddDays(-2), TransactionType = "Deposit", Amount = 300m, Balance = 800m }
            };

            db.Transactions.AddRange(txs);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
