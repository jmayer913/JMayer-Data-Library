using JMayer.Data.HTTP.DataLayer;
using TestProject.Data;

namespace TestProject.HTTP;

/// <summary>
/// The class manages CRUD interactions with a remote HTTP server for the simple data object.
/// </summary>
internal class SimpleDataLayer : StandardCRUDDataLayer<SimpleDataObject>
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public SimpleDataLayer() : base() { }

    /// <summary>
    /// The constructor which takes the HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to interact with the remote server.</param>
    public SimpleDataLayer(HttpClient httpClient) : base(httpClient) { }
}
