using CoreBankingSystem.Application.Abstractions;
using CoreBankingSystem.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBankingSystem.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                "Server=(localdb)\\mssqllocaldb;Database=CoreBankingSystem;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Repositories registrations
        services.AddScoped<IClientRepository, Repositories.ClientRepository>();
        services.AddScoped<IAccountRepository, Repositories.AccountRepository>();
        services.AddScoped<ITransactionRepository, Repositories.TransactionRepository>();

        return services;
    }
}
