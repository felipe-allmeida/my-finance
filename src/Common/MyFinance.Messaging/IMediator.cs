// ------------------------------------------------------
// <copyright file="IMediator.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

using MyFinance.Messaging;
using MyFinance.Results;

namespace MyFinance.Messaging;

/// <summary>
/// Defines a mediator for handling commands and queries in a CQRS architecture.
/// This interface provides a centralized point for dispatching commands and queries to their respective handlers.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a command to its corresponding handler and returns the result.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the command execution result.</returns>
    Task<Result> Send(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command to its corresponding handler and returns the result with a value.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response value.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the command execution result with a value.</returns>
    Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query to its corresponding handler and returns the result.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response value.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the query execution result.</returns>
    Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}