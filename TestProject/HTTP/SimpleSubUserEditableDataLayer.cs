using JMayer.Data.HTTP.DataLayer;
using TestProject.Data;

namespace TestProject.HTTP;

/// <summary>
/// The class manages CRUD interactions with a remote HTTP server for the simple sub user editable data object.
/// </summary>
public class SimpleSubUserEditableDataLayer : SubUserEditableDataLayer<SimpleSubUserEditableDataObject>, ISubUserEditableDataLayer<SimpleSubUserEditableDataObject>
{
    /// <inheritdoc/>
    public SimpleSubUserEditableDataLayer() : base() { }

    /// <inheritdoc/>
    public SimpleSubUserEditableDataLayer(HttpClient httpClient) : base(httpClient) { }
}
