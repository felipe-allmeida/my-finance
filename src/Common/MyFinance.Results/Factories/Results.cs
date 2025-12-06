// ------------------------------------------------------
// <copyright file="Results.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Factory class for creating instances of <see cref="Result"/> and handling common result scenarios.
/// </summary>
public partial class Result
{
    /// <summary>
    /// Creates a new instance of <see cref="Result"/> representing a successful operation without any errors or problems.
    /// </summary>
    /// <returns>A new instance of <see cref="Result"/> indicating success.</returns>
    public static Result Success() => new Result();

    /// <summary>
    /// Creates a new instance of <see cref="Result"/> representing a successful operation with a specified value.
    /// </summary>
    /// <param name="property">The name of the property that was successfully validated.</param>
    /// <param name="message">The success message associated with the property.</param>
    /// <returns>A new instance of <see cref="Result"/> indicating success with a validation message.</returns>
    public static Result ValidationFailed(string property, string message) => new Problem(ProblemType.Failure, "One or more validation errors ocurred.", "Please refer to the errors property for additional details", new Dictionary<string, string[]>
    {
        { property, new[] { message } },
    });

    /// <summary>
    /// Merges multiple <see cref="ResultBase"/> instances into a single <see cref="Result"/>.
    /// </summary>
    /// <param name="results">An array of <see cref="ResultBase"/> instances to merge.</param>
    /// <returns>A new <see cref="Result"/> instance that represents the merged results.</returns>
    public static Result Merge(params ResultBase[] results)
    {
        var problems = results.Where(x => x.IsFailure).Select(x => x.Problem).ToList();

        if (problems.Any())
        {
            var errors = problems.SelectMany(x => x!.Errors).ToDictionary();
            var problem = new Problem(ProblemType.Failure, "One or more errors ocurred.", "Please refer to the errors property for additional details", errors);
            return new Result(problem);
        }

        return new Result();
    }
}