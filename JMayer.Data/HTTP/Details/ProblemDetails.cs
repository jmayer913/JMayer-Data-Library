using System.Text.Json.Serialization;

namespace JMayer.Data.HTTP.Details;

/// <summary>
/// The class represnets the details when a problem is returned from the server.
/// </summary>
public class ProblemDetails
{
    /// <summary>
    /// The property gets/sets a human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-2)]
    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    /// <summary>
    /// The property gets the additional members for the details.
    /// </summary>
    [JsonExtensionData]
    [JsonPropertyName("extensions")]
    public Dictionary<string, object?> Extensions { get; init; } = [];

    /// <summary>
    /// The property gets/sets a URI reference that identifies the specific occurrence of the problem.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-1)]
    [JsonPropertyName("instance")]
    public string? Instance { get; init; }

    /// <summary>
    /// The property gets/sets the HTTP status code.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-3)]
    [JsonPropertyName("status")]
    public int? Status { get; init; }

    /// <summary>
    /// The property gets/sets a short, human-readable summary of the problem type.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-4)]
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// The property gets a URI reference [RFC3986] that identifies the problem type.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyOrder(-5)]
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ProblemDetails() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    /// <param name="status">The HTTP status code.</param>
    /// <param name="title">A short, human-readable summary of the problem type.</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem.</param>
    /// <param name="type">a URI reference [RFC3986] that identifies the problem type.</param>
    /// <param name="extensions">Additional members for the details.</param>
    public ProblemDetails(string? detail = null, int? status = null, string? title = null, string? instance = null, string? type = null, Dictionary<string, object?>? extensions = null)
    {
        Detail = detail;
        Extensions = extensions ?? [];
        Instance = instance;
        Status = status;
        Title = title;
        Type = type;
    }
}
