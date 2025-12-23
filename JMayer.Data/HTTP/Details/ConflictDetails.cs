using System.Net;

namespace JMayer.Data.HTTP.Details;

/// <summary>
/// The class represents the details when a conflict is returned from the server.
/// </summary>
public sealed class ConflictDetails : ProblemDetails
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public ConflictDetails() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    /// <param name="title">A short, human-readable summary of the problem type.</param>
    /// <param name="instance">A URI reference that identifies the specific occurrence of the problem.</param>
    /// <param name="type">a URI reference [RFC3986] that identifies the problem type.</param>
    /// <param name="extensions">Additional members for the details.</param>
    public ConflictDetails(string? detail = null, string? title = null, string? instance = null, string? type = null, Dictionary<string, object?>? extensions = null)
        : base(detail, (int)HttpStatusCode.Conflict, title, instance, type, extensions) { }
}
