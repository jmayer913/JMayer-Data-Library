using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Service;

#warning I think this will work for local and remote.

/// <summary>
/// The class represents the result for a service operation.
/// </summary>
public sealed class OperationResult
{
    /// <summary>
    /// The property gets the data object returned by the operation.
    /// </summary>
    public DataObject? DataObject { get; private init; }

    /// <summary>
    /// The property gets if the operation succeed or not.
    /// </summary>
    public bool IsSuccess => ProblemDetails is null && ValidationErrors.Count is 0;

    /// <summary>
    /// The property gets the user friendly details of a problem that occurred during the operation.
    /// </summary>
    public string? ProblemDetails { get; private init; }

    /// <summary>
    /// The property gets any validation errors determined before the operation.
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; private init; } = [];

    /// <summary>
    /// The constructor which accepts a data object.
    /// </summary>
    /// <param name="dataObject">The data object returned by the operation.</param>
    public OperationResult(DataObject dataObject)
        => DataObject = dataObject;

    /// <summary>
    /// The constructor which accepts the problem details.
    /// </summary>
    /// <param name="problemDetails">The user friendly details of a problem that occurred during the operation.</param>
    public OperationResult(string problemDetails)
        => ProblemDetails = problemDetails;

    /// <summary>
    /// The constructor which accepts the validation results.
    /// </summary>
    /// <param name="validationResults">The list to add into ValidationErrors.</param>
    public OperationResult(List<ValidationResult> validationResults)
    {
        foreach (var result in validationResults)
        {
            if (result.ErrorMessage is not null)
            {
                foreach (var memberName in result.MemberNames)
                {
                    if (ValidationErrors.ContainsKey(memberName))
                    {
                        ValidationErrors[memberName].Add(result.ErrorMessage);
                    }
                    else
                    {
                        ValidationErrors.Add(memberName, [result.ErrorMessage]);
                    }
                }
            }
        }
    }
}
