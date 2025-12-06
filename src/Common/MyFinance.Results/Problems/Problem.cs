// ------------------------------------------------------
// <copyright file="Problem.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <inheritdoc/>
public class Problem : IProblem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Problem"/> class with the specified problem type, title, and detail.
    /// </summary>
    /// <param name="type">The type of the problem.</param>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    public Problem(ProblemType type, string title, string? detail)
    {
        this.Type = type;
        this.Title = title;
        this.Detail = detail;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Problem"/> class with the specified problem type, title, detail, and errors.
    /// </summary>
    /// <param name="type">The type of the problem.</param>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    /// <param name="errors">A dictionary of errors associated with the problem, where the key is the property name and the value is an array of error messages.</param>
    public Problem(ProblemType type, string title, string? detail, Dictionary<string, string[]> errors)
    {
        this.Type = type;
        this.Title = title;
        this.Detail = detail;
        this.Errors = errors;
    }

    /// <inheritdoc/>
    public ProblemType Type { get; private set; }

    /// <inheritdoc/>
    public string Title { get; private set; }

    /// <inheritdoc/>
    public string? Detail { get; private set; }

    /// <inheritdoc/>
    public Dictionary<string, string[]> Errors { get; private set; } = [];

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a general failure.
    /// </summary>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem NotFound(string detail)
    {
        return new Problem(ProblemType.NotFound, "NotFound", detail);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a general failure.
    /// </summary>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem NotFound(string title, string detail)
    {
        return new Problem(ProblemType.NotFound, title, detail);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a validation error.
    /// </summary>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    /// <param name="errors">A dictionary of errors associated with the problem, where the key is the property name and the value is an array of error messages.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem Validation(string title, string detail, Dictionary<string, string[]> errors)
    {
        return new Problem(ProblemType.Validation, title, detail, errors);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a conflict.
    /// </summary>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <param name="detail">Additional details about the problem, providing more context or information about the issue.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem Conflict(string title, string detail)
    {
        return new Problem(ProblemType.Conflict, title, detail);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing an unauthorized access attempt.
    /// </summary>
    /// <param name="code">The code representing the unauthorized access attempt, typically a status code.</param>
    /// <param name="title">The title of the problem, which is a brief description of the issue.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem Unauthorized(string code, string title)
    {
        return new Problem(ProblemType.Unauthorized, title, null);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a forbidden action.
    /// </summary>
    /// <param name="code">The code representing the forbidden access attempt, typically a status code.</param>
    /// <param name="message">The message providing more context or information about the forbidden action.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem Forbidden(string code, string message)
    {
        return new Problem(ProblemType.Forbidden, code, message);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Problem"/> class representing a general failure.
    /// </summary>
    /// <param name="code">The code representing the failure, typically a status code.</param>
    /// <param name="message">The message providing more context or information about the failure.</param>
    /// <returns>A new instance of the <see cref="Problem"/> class.</returns>
    public static Problem Failure(string code, string message)
    {
        return new Problem(ProblemType.Failure, code, message);
    }
}