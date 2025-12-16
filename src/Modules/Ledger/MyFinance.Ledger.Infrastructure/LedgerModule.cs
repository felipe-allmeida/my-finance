using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyFinance.Common.Application.Messaging;
using MyFinance.Common.Domain;
using MyFinance.Common.Infrastructure.Extensions;
using MyFinance.Ledger.Infrastructure.Database;

namespace MyFinance.Ledger.Infrastructure;

public static class LedgerModule
{
    public static IServiceCollection AddLedgerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddEndpoints(MyFinance.Ledger.Application.AssemblyReference.Assembly);

        // Register Ledger module services here
        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LedgerContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "ledger"))
                .UseSnakeCaseNamingConvention()
                //.AddInterceptors(sp.GetRequiredService<Object>())
                );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableFrom(typeof(IDomainEventHandler)))
            .ToArray();

        foreach(var domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            var domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            //var closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);
            //services.Decorate(domainEventHandler, closedIdempotentHandler);

        }
    }
}
