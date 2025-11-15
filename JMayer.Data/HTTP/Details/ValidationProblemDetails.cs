using System.Net;
using System.Text.Json.Serialization;

namespace JMayer.Data.HTTP.Details;

/// <summary>
/// The class represents the details when a validation problem is returned from the server.
/// </summary>
/// <remarks>
/// Microsoft.AspNetCore.App does not work with blazor web-assembly so we need to have our own ValidationProblemDetails class.
/// </remarks>
public sealed class ValidationProblemDetails : ProblemDetails
{
    /// <summary>
    /// The property gets the validation errors.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>> Errors { get; init; } = [];

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ValidationProblemDetails() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    /// <param name="title">A short, human-readable summary of the problem type.</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem.</param>
    /// <param name="type">a URI reference [RFC3986] that identifies the problem type.</param>
    /// <param name="extensions">Additional members for the details.</param>
    /// <param name="errors">The validation errors.</param>
    public ValidationProblemDetails(string? detail = null, string? title = "One or more validation errors occurred.", string? instance = null, string? type = null, Dictionary<string, object?>? extensions = null, Dictionary<string, List<string>>? errors = null)
        : base(detail, (int)HttpStatusCode.BadRequest, title, instance, type, extensions) 
        => Errors = errors ?? [];
}
