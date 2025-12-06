// ------------------------------------------------------
// <copyright file="IQuery.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Messaging;

/// <summary>
/// Represents a query in the messaging system that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQuery<TResponse> : IRequest;