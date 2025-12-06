// ------------------------------------------------------
// <copyright file="IProblem.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Interface representing a problem that can occur in the application.
/// </summary>
public interface IProblem
{
    /// <summary>
    /// Gets the type of the problem.
    /// </summary>
    ProblemType Type { get; }

    /// <summary>
    /// Gets the title of the problem, which is a brief description of the issue.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets additional details about the problem, providing more context or information about the issue.
    /// </summary>
    string? Detail { get; }

    /// <summary>
    /// Gets a dictionary of errors associated with the problem, where the key is the property name and the value is an array of error messages.
    /// </summary>
    Dictionary<string, string[]> Errors { get; }
}