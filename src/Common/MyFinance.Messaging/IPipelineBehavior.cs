// ------------------------------------------------------
// <copyright file="IPipelineBehavior.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Messaging;

/// <summary>
/// Represents a delegate for the next handler in the pipeline.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <returns>A task representing the asynchronous operation with the response.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Defines a pipeline behavior that can be executed before and after command/query handling.
/// Pipeline behaviors allow for cross-cutting concerns like validation, logging, caching, etc.
/// </summary>
/// <typeparam name="TRequest">The type of the request (command or query).</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Handles the request by performing additional processing before and/or after the core handler.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="next">The next behavior in the pipeline or the final handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}