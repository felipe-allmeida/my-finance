// ------------------------------------------------------
// <copyright file="ResultBase.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <inheritdoc/>
public abstract class ResultBase : IResultBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class with the specified result type and problem.
    /// </summary>
    /// <param name="type">The type of the result, indicating whether it is a success or failure.</param>
    /// <param name="problem">The problem associated with the result, if any. This will be non-null if the result is a failure.</param>
    protected ResultBase(ResultType type, Problem? problem)
    {
        this.Type = type;
        this.Problem = problem;
    }

    /// <inheritdoc/>
    public ResultType Type { get; protected set; }

    /// <inheritdoc/>
    public bool IsFailure => this.Type == ResultType.Failure;

    /// <inheritdoc/>
    public bool IsSuccess => !this.IsFailure;

    /// <inheritdoc/>
    public Problem? Problem { get; protected set; }
}

/// <inheritdoc/>
public abstract class ResultBase<TResult> : ResultBase
    where TResult : ResultBase<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase{TResult}"/> class with the specified result type and problem.
    /// </summary>
    /// <param name="type">The type of the result, indicating whether it is a success or failure.</param>
    /// <param name="problem">The problem associated with the result, if any. This will be non-null if the result is a failure.</param>
    protected ResultBase(ResultType type, Problem? problem)
        : base(type, problem)
    {
    }

    /// <summary>
    /// Implicitly converts the result to a boolean value indicating success or failure.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    public static implicit operator bool(ResultBase<TResult> result) => result.IsSuccess;
}