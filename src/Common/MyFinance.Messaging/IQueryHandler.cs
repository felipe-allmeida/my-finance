// ------------------------------------------------------
// <copyright file="IQueryHandler.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

using MyFinance.Results;

namespace MyFinance.Messaging;

/// <summary>
/// Represents a query handler that processes queries of type <typeparamref name="TQuery"/> and returns a result.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle, which must implement <see cref="IQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the query handler, which must be compatible with <see cref="Result{TValue}"/>.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handles the specified query and returns a result of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">The cancellation token to observe while handling the query.</param>
    /// <returns>A task that represents the asynchronous operation, containing a result of type <typeparamref name="TResponse"/>.</returns>
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}