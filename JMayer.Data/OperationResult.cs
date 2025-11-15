using JMayer.Data.Data;
using JMayer.Data.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace JMayer.Data;

#warning Success() and certain Failure() methods don't accept a status code and instead I hard code a HttpStatusCode. I wonder if all Success() and Failure() methods need to accept a code.
#warning This is meant to be a general framework so if someone wants to build the pieces for whatever database they can and that also means they can define whatever status codes they want.

/// <summary>
/// The class represents the result for an operation at the service or repository layer.
/// </summary>
public sealed class OperationResult<T> where T : DataObject
{
    /// <summary>
    /// The property gets the data objects returned by the operation.
    /// </summary>
    public List<T> DataObjects { get; private init; } = [];

    /// <summary>
    /// The property gets if the operation succeed or not.
    /// </summary>
    public bool IsSuccess => ProblemDetails is null && ValidationErrors.Count is 0;

    /// <summary>
    /// The property gets the user friendly error message that occurred during the operation.
    /// </summary>
    public string? ProblemDetails { get; private init; }

    /// <summary>
    /// The property gets the status code for the operation.
    /// </summary>
    /// <remarks>
    /// For the memory database and http repository, http status codes are used. This makes it were
    /// two different sets of codes need to be remembered.
    /// </remarks>
    public int StatusCode { get; private init; }

    /// <summary>
    /// The property gets any validation errors determined before the operation.
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; private init; } = [];

    /// <summary>
    /// The constructor which accepts a data object.
    /// </summary>
    /// <param name="statusCode">The status code for the operation.</param>
    /// <param name="dataObjects">The data object returned by the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    private OperationResult(int statusCode, List<T> dataObjects)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);
        StatusCode = statusCode;
        DataObjects = dataObjects;
    }

    /// <summary>
    /// The constructor which accepts the status code and optionally, a problem details.
    /// </summary>
    /// <param name="statusCode">The status code for the operation.</param>
    /// <param name="problemDetails">The user friendly error message that occurred during the operation.</param>
    private OperationResult(int statusCode, string? problemDetails)
    {
        ProblemDetails = problemDetails;
        StatusCode = statusCode;
    }

    /// <summary>
    /// The constructor which accepts the status code and the validation errors.
    /// </summary>
    /// <param name="statusCode">The status code for the operation.</param>
    /// <param name="validationErrors">Any validation errors determined before the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown if the validationErrors parameter is null.</exception>
    private OperationResult(int statusCode, Dictionary<string, List<string>> validationErrors)
    {
        ArgumentNullException.ThrowIfNull(validationErrors);
        StatusCode = statusCode;
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// The method returns a failed operation.
    /// </summary>
    /// <param name="statusCode">The status code for the operation.</param>
    /// <param name="problemDetails">The user friendly error message that occurred during the operation.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Failure(int statusCode, string? problemDetails = default)
        => new(statusCode, problemDetails);

    /// <summary>
    /// The method returns a failed operation.
    /// </summary>
    /// <param name="validationErrors">Any validation errors determined before the operation.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Failure(Dictionary<string, List<string>> validationErrors)
        => new((int)HttpStatusCode.BadRequest, validationErrors);

    /// <summary>
    /// The method returns a failed operation.
    /// </summary>
    /// <param name="validationResults">Any validation errors determined before the operation.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Failure(List<ValidationResult> validationResults)
        => new((int)HttpStatusCode.BadRequest, validationResults.ToErrorDictionary());

    /// <summary>
    /// The method returns a successful operation.
    /// </summary>
    /// <param name="dataObject">The data object for the operation.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult<T> Success(T dataObject) 
        => Success([dataObject]);

    /// <summary>
    /// The method returns a successful operation.
    /// </summary>
    /// <param name="dataObjects">The data objects for the operation.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult<T> Success(List<T> dataObjects)
        => new((int)HttpStatusCode.OK, dataObjects);
}
