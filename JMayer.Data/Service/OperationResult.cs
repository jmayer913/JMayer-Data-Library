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
    /// The property gets the user friendly error message that occurred during the operation.
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// The property gets if the operation succeed or not.
    /// </summary>
    public bool IsSuccess => ErrorMessage is null && ValidationErrors.Count is 0;

    /// <summary>
    /// The property gets the type of operation failure.
    /// </summary>
    /// <remarks>
    /// Even when there is no failure, this will default to General. I thought about
    /// having a None but that would mean I would need to make sure None was not passed 
    /// into Failure().
    /// </remarks>
    public OperationFailureType OperationFailureType { get; private init; }

    /// <summary>
    /// The property gets any validation errors determined before the operation.
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; private init; } = [];

    /// <summary>
    /// The constructor which accepts a data object.
    /// </summary>
    /// <param name="dataObject">The data object returned by the operation.</param>
    private OperationResult(DataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        DataObject = dataObject;
    }

    /// <summary>
    /// The constructor which accepts the problem details.
    /// </summary>
    /// <param name="errorMesage">The user friendly error message that occurred during the operation.</param>
    /// <param name="operationFailureType">The </param>
    private OperationResult(string errorMesage, OperationFailureType operationFailureType = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorMesage);
        ErrorMessage = errorMesage;
        OperationFailureType = operationFailureType;
    }

    /// <summary>
    /// The constructor which accepts the validation results.
    /// </summary>
    /// <param name="validationResults">The list to add into ValidationErrors.</param>
    private OperationResult(List<ValidationResult> validationResults)
    {
        ArgumentNullException.ThrowIfNull(validationResults);

        if (validationResults.Count > 0)
        {
            OperationFailureType = OperationFailureType.Validation;

            foreach (var result in validationResults)
            {
                if (result.ErrorMessage is not null)
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        if (ValidationErrors.TryGetValue(memberName, out List<string>? value))
                        {
                            value.Add(result.ErrorMessage);
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

    /// <summary>
    /// The method returns a failed operation.
    /// </summary>
    /// <param name="errorMessage">The user friendly error message that occurred during the operation.</param>
    /// <param name="operationFailureType">The type of operation failure.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult Failure(string errorMessage, OperationFailureType operationFailureType = default)
        => new(errorMessage, operationFailureType);

    /// <summary>
    /// The method returns a failed operation.
    /// </summary>
    /// <param name="validationResults">Any validation errors determined before the operation.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult Failure(List<ValidationResult> validationResults)
        => new(validationResults);

    /// <summary>
    /// The method returns a successful operation.
    /// </summary>
    /// <param name="dataObject">The data object for the operation.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult Success(DataObject dataObject) 
        => new(dataObject);
}
