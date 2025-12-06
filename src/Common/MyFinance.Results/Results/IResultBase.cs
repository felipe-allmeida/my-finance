// ------------------------------------------------------
// <copyright file="IResultBase.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Represents a base interface for results that do not contain a value.
/// </summary>
public interface IResultBase
{
    /// <summary>
    /// Gets the type of the result, indicating whether it is a success or failure.
    /// </summary>
    ResultType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the result is successful (i.e., it does not contain any errors or problems).
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a failure (i.e., it contains errors or problems).
    /// </summary>
    bool IsFailure { get; }

    /// <summary>
    /// Gets the problem associated with the result, if any. This will be non-null if the result is a failure.
    /// </summary>
    public Problem? Problem { get; }
}