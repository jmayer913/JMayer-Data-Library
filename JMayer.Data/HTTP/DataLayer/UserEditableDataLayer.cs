using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class for interacting with remote data via HTTP using CRUD operations.
/// </summary>
/// <typeparam name="T">A UserEditableDataObject which represents data on the remote server.</typeparam>
public class UserEditableDataLayer<T> : StandardCRUDDataLayer<T>, IUserEditableDataLayer<T> 
    where T : UserEditableDataObject
{
    /// <inheritdoc/>
    public UserEditableDataLayer() : base() { }

    /// <inheritdoc/>
    public UserEditableDataLayer(HttpClient httpClient) : base(httpClient) { }

    /// <inheritdoc/>
    public async Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
    {
        List<ListView>? listView = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"/api/{TypeName}/All/ListView", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            listView = await httpResponseMessage.Content.ReadFromJsonAsync<List<ListView>?>(cancellationToken);
        }

        return listView;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<ListView>? pagedListView = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"/api/{TypeName}/Page/ListView?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            pagedListView = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<ListView>?>(cancellationToken);
        }

        return pagedListView;
    }
}
