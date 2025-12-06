// ------------------------------------------------------
// <copyright file="ProblemType.cs" company="Pampa Devs">
// Copyright (c) Pampa Devs. All rights reserved.
// </copyright>
// ------------------------------------------------------

namespace MyFinance.Results;

/// <summary>
/// Enumeration representing different types of problems that can occur in the application.
/// </summary>
public enum ProblemType
{
    /// <summary>
    /// Represents a general failure or error that does not fit into other categories.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Represents a validation error, typically used when input data does not meet the required criteria.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Represents a resource that was not found, such as when a requested item does not exist in the database.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Represents a conflict, often used when there is a conflict in the state of the resource, such as trying to create a duplicate entry.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Represents an unauthorized access attempt, typically used when the user does not have the necessary permissions to perform an action.
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Represents a forbidden action, indicating that the user is authenticated but does not have permission to access the requested resource.
    /// </summary>
    Forbidden = 5,
}