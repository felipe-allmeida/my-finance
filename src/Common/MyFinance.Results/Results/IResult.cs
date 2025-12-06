// ------------------------------------------------------
// <copyright file="IResult.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Represents a result that contains a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
public interface IResult<out TValue> : IResultBase
{
    /// <summary>
    /// Gets the value contained in the result.
    /// </summary>
    TValue? Value { get; }
}