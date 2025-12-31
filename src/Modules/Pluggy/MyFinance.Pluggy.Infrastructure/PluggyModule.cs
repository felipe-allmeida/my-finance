using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Common.Domain;
using MyFinance.Common.Infrastructure.Extensions;
using MyFinance.Pluggy.Application.Services;
using MyFinance.Pluggy.Infrastructure.Database;
using MyFinance.Pluggy.Infrastructure.Services;
using Pluggy.SDK;

namespace MyFinance.Pluggy.Infrastructure;

public static class PluggyModule
{
    public static IServiceCollection AddPluggyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddPluggyClient(configuration);
        services.AddServices();

        services.AddEndpoints(MyFinance.Pluggy.Application.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PluggyContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "pluggy"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddPluggyClient(this IServiceCollection services, IConfiguration configuration)
    {
        var clientId = configuration["Pluggy:ClientId"];
        var clientSecret = configuration["Pluggy:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Pluggy credentials not configured");
        }

        services.AddSingleton<PluggyAPI>(sp =>
            new PluggyAPI(clientId, clientSecret));
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionSyncService, TransactionSyncService>();
        services.AddScoped<IPluggyConnectionService, PluggyConnectionService>();
    }
}
