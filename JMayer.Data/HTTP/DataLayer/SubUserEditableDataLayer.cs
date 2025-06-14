using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class for interacting with remote sub user editable data objects via HTTP using CRUD operations.
/// </summary>
/// <typeparam name="T">A SubUserEditableDataObject which represents data on the remote server.</typeparam>
public class SubUserEditableDataLayer<T> : UserEditableDataLayer<T>, ISubUserEditableDataLayer<T>
    where T : SubUserEditableDataObject
{
    /// <inheritdoc/>
    public SubUserEditableDataLayer() : base() { }

    /// <inheritdoc/>
    public SubUserEditableDataLayer(HttpClient httpClient) : base(httpClient) { }

    /// <inheritdoc/>
    public async Task<List<T>?> GetAllAsync(long ownerID, CancellationToken cancellationToken = default)
    {
        return await GetAllAsync(ownerID.ToString(), cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if the ownerID parameter is null or empty.</exception>
    public async Task<List<T>?> GetAllAsync(string ownerID, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerID);
        List<T>? dataObjects = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All/{ownerID}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>>(cancellationToken);
        }
        
        return dataObjects;
    }

    /// <inheritdoc/>
    public async Task<List<ListView>?> GetAllListViewAsync(long ownerID, CancellationToken cancellationToken = default)
    {
        return await GetAllListViewAsync(ownerID.ToString(), cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if the ownerID parameter is null or empty.</exception>
    public async Task<List<ListView>?> GetAllListViewAsync(string ownerID, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerID);
        List<ListView>? dataObjects = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All/ListView/{ownerID}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<ListView>>(cancellationToken);
        }
        
        return dataObjects;
    }

    /// <inheritdoc/>
    public async Task<PagedList<T>?> GetPageAsync(long ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        return await GetPageAsync(ownerID.ToString(), queryDefinition, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if the ownerID parameter is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<T>?> GetPageAsync(string ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerID);
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<T>? dataObjects = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Page/{ownerID}?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<T>>(cancellationToken);
        }

        return dataObjects;
    }

    /// <inheritdoc/>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(long ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        return await GetPageListViewAsync(ownerID.ToString(), queryDefinition, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Thrown if the ownerID parameter is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(string ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerID);
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<ListView>? dataObjects = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Page/ListView/{ownerID}?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<ListView>>(cancellationToken);
        }

        return dataObjects;
    }
}
