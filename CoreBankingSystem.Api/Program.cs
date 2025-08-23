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

// CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                "http://localhost:65153",
                "http://127.0.0.1:65153",
                "http://localhost:4200",
                "http://127.0.0.1:4200"
            );
    });
});

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

// Serve Swagger in all environments to aid diagnostics
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable CORS only in development
    app.UseCors("DevCors");

    // Only force HTTPS locally (Docker compose uses HTTP between containers)
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Minimal endpoints for quick checks
app.MapGet("/", () => Results.Ok(new { status = "ok", service = "CoreBankingSystem.Api" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllers();

app.Run();
