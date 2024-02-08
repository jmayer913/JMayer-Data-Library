using JMayer.Data.HTTP.DataLayer;
using TestProject.Data;

namespace TestProject.HTTP;

/// <summary>
/// The class manages CRUD interactions with a remote HTTP server for the simple data object.
/// </summary>
internal class SimpleDataLayer : StandardCRUDDataLayer<SimpleDataObject>
{
    /// <inheritdoc/>
    public SimpleDataLayer() : base() { }

    /// <inheritdoc/>
    public SimpleDataLayer(HttpClient httpClient) : base(httpClient) { }
}
