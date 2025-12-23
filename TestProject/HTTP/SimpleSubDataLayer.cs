using JMayer.Data.HTTP.DataLayer;
using TestProject.Data;

namespace TestProject.HTTP;

/// <summary>
/// The class manages CRUD interactions with a remote HTTP server for the simple sub data object.
/// </summary>
public class SimpleSubDataLayer : StandardSubCRUDDataLayer<SimpleSubDataObject>, IStandardSubCRUDDataLayer<SimpleSubDataObject>
{
    /// <inheritdoc/>
    public SimpleSubDataLayer() : base() { }

    /// <inheritdoc/>
    public SimpleSubDataLayer(HttpClient httpClient) : base(httpClient) { }
}
