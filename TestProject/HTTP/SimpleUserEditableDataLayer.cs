using JMayer.Data.HTTP.DataLayer;
using TestProject.Data;

namespace TestProject.HTTP;

/// <summary>
/// The class manages CRUD interactions with a remote HTTP server for the simple user editable data object.
/// </summary>
public class SimpleUserEditableDataLayer : UserEditableDataLayer<SimpleUserEditableDataObject>
{
    /// <inheritdoc/>
    public SimpleUserEditableDataLayer() : base() { }

    /// <inheritdoc/>
    public SimpleUserEditableDataLayer(HttpClient httpClient) : base(httpClient) { }
}
