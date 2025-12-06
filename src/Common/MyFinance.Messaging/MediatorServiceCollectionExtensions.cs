// ------------------------------------------------------
// <copyright file="MediatorServiceCollectionExtensions.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using MyFinance.Messaging;

namespace MyFinance.Messaging;

/// <summary>
/// Provides extension methods for configuring mediator services in the dependency injection container.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the mediator services to the specified <see cref="IServiceCollection"/>.
    /// This includes registering the <see cref="IMediator"/> interface with its default implementation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        return services;
    }

    /// <summary>
    /// Adds the mediator services and automatically scans for command and query handlers using Scrutor.
    /// This method combines mediator registration with automatic handler discovery and registration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembliesContainingHandlers">The assemblies to scan for handlers. If not provided, scans from assemblies containing ICommand.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMediatorWithHandlers(this IServiceCollection services, params Type[] assembliesContainingHandlers)
    {
        // Register the mediator
        services.AddMediator();

        // Use Scrutor to scan for and register handlers
        var assemblyTypes = assembliesContainingHandlers.Length > 0
            ? assembliesContainingHandlers
            : [typeof(ICommand)];

        services.Scan(scan => scan
            .FromAssembliesOf(assemblyTypes)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssembliesOf(assemblyTypes)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssembliesOf(assemblyTypes)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Register pipeline behaviors
        services.Scan(scan => scan
            .FromAssembliesOf(assemblyTypes)
            .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}