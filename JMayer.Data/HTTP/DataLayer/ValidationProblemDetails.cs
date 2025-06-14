using System.Text.Json.Serialization;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class represents the validation problem details returned from the server.
/// </summary>
/// <remarks>
/// Microsoft.AspNetCore.App does not work with blazor web-assembly so we need
/// to have our own ValidationProblemDetails class.
/// </remarks>
internal sealed class ValidationProblemDetails
{
    /// <summary>
    /// The property gets/sets an explanation specific to this occurrence of the problem.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// The property gets/sets the errors.
    /// </summary>
    /// <remarks>
    /// Key is the field and Value is a list of error messages associated with the field.
    /// </remarks>
    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    /// <summary>
    /// The property gets/sets a URI reference that identifies the specific occurrence of the problem.
    /// </summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>
    /// The property gets/sets the HTTP status code.
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// The property gets/sets a summary of the problem type.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The property gets/sets the problem type.
    /// "about:blank".
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
