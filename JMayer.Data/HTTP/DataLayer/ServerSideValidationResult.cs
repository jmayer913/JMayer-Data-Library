﻿namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class represents the result of server side validation.
/// </summary>
public sealed class ServerSideValidationResult
{
    /// <summary>
    /// The errors in the result.
    /// </summary>
    private readonly List<ServerSideValidationError> _errors = [];

    /// <summary>
    /// The property gets if no errors was produced by the validation.
    /// </summary>
    public bool IsSuccess
    {
        get => _errors.Count == 0;
    }

    /// <summary>
    /// The property gets the errors.
    /// </summary>
    public ServerSideValidationError[] Errors
    {
        get => _errors.ToArray();
    }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ServerSideValidationResult() { }

    /// <summary>
    /// The method adds errors 
    /// </summary>
    /// <param name="error"></param>
    internal void AddError(ServerSideValidationError error)
    {
        _errors.Add(error);
    }
}
