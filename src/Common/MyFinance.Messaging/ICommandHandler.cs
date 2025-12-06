// ------------------------------------------------------
// <copyright file="ICommandHandler.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

using MyFinance.Messaging;
using MyFinance.Results;

namespace MyFinance.Messaging;

/// <summary>
/// Represents a command handler that processes commands of type <typeparamref name="TCommand"/> and returns a result.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command and returns a result.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token to observe while handling the command.</param>
    /// <returns>A task that represents the asynchronous operation, containing a result of type <see cref="Result"/>.</returns>
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a command handler that processes commands of type <typeparamref name="TCommand"/> and returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the command handler, which must be compatible with <see cref="Result{TValue}"/>.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles <typeparamref name="TCommand"/> and returns a result of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token to observe while handling the command.</param>
    /// <returns>A task that represents the asynchronous operation, containing a result of type <typeparamref name="TResponse"/>.</returns>
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}