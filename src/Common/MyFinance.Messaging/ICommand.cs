// ------------------------------------------------------
// <copyright file="ICommand.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Messaging;

/// <summary>
/// Represents a command in the messaging system.
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Represents a command that returns a response of type <typeparamref name="TResponse"/> in the messaging system.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest;