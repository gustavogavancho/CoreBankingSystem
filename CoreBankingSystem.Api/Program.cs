using CoreBankingSystem.Application;
using CoreBankingSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using CoreBankingSystem.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // Avoid synchronous IO which can cause thread pool starvation/hangs
    options.SuppressAsyncSuffixInActionNames = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture DI
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

// Ensure database is created/migrated and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var hasMigrations = db.Database.GetMigrations().Any();
    if (hasMigrations)
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        // In dev, drop and recreate to match current model when no migrations exist
        if (app.Environment.IsDevelopment())
        {
            await db.Database.EnsureDeletedAsync();
        }
        await db.Database.EnsureCreatedAsync();
    }

    await ApplicationDbContextSeeder.SeedAsync(db);
}

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
