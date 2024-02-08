using JMayer.Data.Data;
using System.Net;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class represents the result for a remote operation.
/// </summary>
public class OperationResult
{
    /// <summary>
    /// The property gets the data object returned by the operation.
    /// </summary>
    public DataObject? DataObject { get; private init; }

    /// <summary>
    /// The property gets if the operation was a success.
    /// </summary>
    public bool IsSuccessStatusCode { get => StatusCode == HttpStatusCode.OK; }

    /// <summary>
    /// The property gets the server side validation result.
    /// </summary>
    public ServerSideValidationResult? ServerSideValidationResult { get; private init; }

    /// <summary>
    /// The property gets the HTTP status code returned by the remote operation.
    /// </summary>
    public HttpStatusCode StatusCode { get; private init; }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObject">The data object returned by the operation.</param>
    /// <param name="serverSideValidationResult">The server side validation result returned by the operation.</param>
    /// <param name="httpStatusCode">The HTTP status code returned by the remote operation.</param>
    public OperationResult(DataObject? dataObject, ServerSideValidationResult? serverSideValidationResult, HttpStatusCode httpStatusCode)
    {
        DataObject = dataObject;
        ServerSideValidationResult = serverSideValidationResult;
        StatusCode = httpStatusCode;
    }
}
