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
        var createdAccounts = false;
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
            createdAccounts = true;
        }

        // Ensure accounts are persisted before we compute transaction balances
        if (createdAccounts)
        {
            await db.SaveChangesAsync(cancellationToken);
        }

        // Seed Transactions
        if (!await db.Transactions.AnyAsync(cancellationToken))
        {
            // Load accounts to get InitialBalance
            var accounts = await db.Accounts.AsNoTracking().ToListAsync(cancellationToken);
            var initialByAccount = accounts.ToDictionary(a => a.AccountNumber, a => a.InitialBalance);

            // Define seed transaction inputs without precomputed balance
            var inputs = new List<(string AccountNumber, DateTime Date, string Type, decimal Amount)>
            {
                ("ACC-1001", DateTime.UtcNow.AddDays(-5), "Deposit", 500m),
                ("ACC-1001", DateTime.UtcNow.AddDays(-3), "Withdrawal", -200m),
                ("ACC-2002", DateTime.UtcNow.AddDays(-4), "Deposit", 750m),
                ("ACC-3003", DateTime.UtcNow.AddDays(-2), "Deposit", 300m)
            };

            // Compute running balances per account
            var grouped = inputs
                .GroupBy(i => i.AccountNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Date).ToList());

            var txs = new List<Transaction>();
            foreach (var kvp in grouped)
            {
                var accountNumber = kvp.Key;
                if (!initialByAccount.TryGetValue(accountNumber, out var running))
                {
                    // Skip if account not found (should not happen in seeded data)
                    continue;
                }

                foreach (var i in kvp.Value)
                {
                    var newBalance = running + i.Amount;

                    // Avoid seeding a zero-balance transaction per business rule
                    if (newBalance == 0m)
                    {
                        // Nudge by 0.01 to keep data valid for demo purposes
                        newBalance += 0.01m;
                    }

                    txs.Add(new Transaction
                    {
                        TransactionId = Guid.NewGuid(),
                        AccountNumber = accountNumber,
                        Date = i.Date,
                        TransactionType = i.Type,
                        Amount = i.Amount,
                        Balance = newBalance
                    });

                    running = newBalance;
                }
            }

            db.Transactions.AddRange(txs);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
