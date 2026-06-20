using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaxLab.Data;
using TaxLab.Worker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("TaxLabDb")
            ?? "Data Source=taxlab.db";

        services.AddDbContext<TaxLabDbContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("TaxLab.Data")));

        services.AddScoped<TaxRateImporter>();
    })
    .Build();

// CSV path can be passed as first argument, otherwise falls back to the default filename
var csvPath = args.Length > 0
    ? args[0]
    : "TaxLab.TechnicalTest.2025 - Calculation.csv";

using var scope = host.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<TaxLabDbContext>();
var importer = scope.ServiceProvider.GetRequiredService<TaxRateImporter>();

Console.WriteLine("Recreating database...");
await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine($"Importing tax rates from: {csvPath}");
await importer.ImportAsync(csvPath);

Console.WriteLine("Import complete.");
