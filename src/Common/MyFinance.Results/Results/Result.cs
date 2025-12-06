// ------------------------------------------------------
// <copyright file="Result.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <inheritdoc/>
public partial class Result : ResultBase<Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class representing a successful result with no errors or problems.
    /// </summary>
    public Result()
        : base(ResultType.Success, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with the specified result type.
    /// </summary>
    /// <param name="type">The type of the result, indicating whether it is a success or failure.</param>
    public Result(ResultType type)
        : base(type, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class with the specified value.
    /// </summary>
    /// <param name="error">The error associated with the result, if any. This will be non-null if the result is a failure.</param>
    public Result(Problem error)
        : base(ResultType.Failure, error)
    {
    }

    /// <summary>
    /// Implicitly converts a value of type <see cref="Result"/> to a boolean indicating success or failure.
    /// </summary>
    /// <param name="error">The result to convert.</param>
    public static implicit operator Result(Problem error) => new Result(error);
}

/// <inheritdoc/>
public class Result<TValue> : ResultBase<Result<TValue>>, IResult<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class representing a successful result with no errors or problems.
    /// </summary>
    public Result()
        : base(ResultType.Success, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified result type.
    /// </summary>
    /// <param name="value">The value associated with the result, if any. This will be non-null if the result is a success.</param>
    public Result(TValue value)
        : base(ResultType.Success, null)
    {
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified result type and value.
    /// </summary>
    /// <param name="type">The type of the result, indicating whether it is a success or failure.</param>
    /// <param name="value">The value associated with the result, if any. This will be non-null if the result is a success.</param>
    public Result(ResultType type, TValue value)
        : base(type, null)
    {
        this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the result, if any. This will be non-null if the result is a failure.</param>
    public Result(Problem error)
        : base(ResultType.Failure, error)
    {
        this.Value = default;
    }

    /// <summary>
    /// Gets the value associated with the result, if any. This will be non-null if the result is a success.
    /// </summary>
    public TValue? Value { get; private set; }

    /// <summary>
    /// Implicitly converts a value of type <see cref="Result{TValue}"/> to a boolean indicating success or failure.
    /// </summary>
    /// <param name="value">The result to convert.</param>
    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);

    /// <summary>
    /// Implicitly converts a value of type <see cref="Problem"/> to a <see cref="Result{TValue}"/> indicating a failure.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<TValue>(Problem error) => new Result<TValue>(error);
}