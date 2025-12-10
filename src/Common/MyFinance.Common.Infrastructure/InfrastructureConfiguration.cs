using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyFinance.Common.Application.Caching;
using MyFinance.Common.Infrastructure.Authentication;
using MyFinance.Common.Infrastructure.Caching;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace MyFinance.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string databaseConnectionString,
        string redisConnectionString)
    {
        services.AddAuthenticationInternal();
        // services.AddAuthorizationInternal();


        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {       
            services.AddDistributedMemoryCache();
        }
        services.TryAddSingleton<ICacheService, CacheService>();


        services.AddMassTransit(configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services
           .AddOpenTelemetry()
           .ConfigureResource(resource => resource.AddService(serviceName))
           .WithTracing(tracing =>
           {
               tracing
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation()
                   .AddRedisInstrumentation()
                   .AddNpgsql()
                   .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

               tracing.AddOtlpExporter();
           });



        return services;
    }
}
