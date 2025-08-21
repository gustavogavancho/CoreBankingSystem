using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreBankingSystem.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity(PersonConfiguration());
        modelBuilder.Entity(ClientConfiguration());
        modelBuilder.Entity(AccountConfiguration());
        modelBuilder.Entity(TransactionConfiguration());
    }

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Person>> PersonConfiguration()
        => entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Identification).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Client>> ClientConfiguration()
        => entity =>
        {
            entity.ToTable("Clients");
            // Unique external identifier for clients
            entity.HasIndex(e => e.ClientId).IsUnique();
            // NOTE: Do not configure alternate keys on derived types (Client derives from Person)
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Account>> AccountConfiguration()
        => entity =>
        {
            entity.HasKey(e => e.AccountNumber);
            entity.Property(e => e.AccountNumber).HasMaxLength(30);
            entity.Property(e => e.AccountType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.InitialBalance).HasColumnType("decimal(18,2)");

            // Relationship: Account belongs to Client via Person primary key (Id)
            entity.HasOne(a => a.Client)
                  .WithMany(c => c.Accounts)
                  .HasForeignKey(a => a.ClientId)
                  // Use principal PK by default; do not set HasPrincipalKey to a derived-type alternate key
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Transactions)
                  .WithOne(t => t.Account!)
                  .HasForeignKey(t => t.AccountNumber)
                  .OnDelete(DeleteBehavior.Cascade);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Transaction>> TransactionConfiguration()
        => entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
        };
}
