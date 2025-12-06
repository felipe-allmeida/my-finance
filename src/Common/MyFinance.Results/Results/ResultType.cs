// ------------------------------------------------------
// <copyright file="ResultType.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Enumeration representing the type of result.
/// </summary>
public enum ResultType
{
    /// <summary>
    /// Indicates a successful result with no errors or problems.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Indicates a failure result, which may contain errors or problems that occurred during processing.
    /// </summary>
    Failure = 1,
}