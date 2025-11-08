using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.Details;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class for interacting with remote data via HTTP using CRUD operations.
/// </summary>
/// <typeparam name="T">A DataObject which represents data on the remote server.</typeparam>
public class StandardCRUDDataLayer<T> : IStandardCRUDDataLayer<T> where T : DataObject
{
    /// <summary>
    /// The HTTP client used to interact with the remote server.
    /// </summary>
    protected HttpClient HttpClient { get; init; } = new();

    /// <summary>
    /// The name of the type associated with the data layer.
    /// </summary>
    /// <remarks>
    /// The type name is used in the route for the API. It uses 
    /// the standard format, api/typeName/action.
    /// </remarks>
    protected string TypeName { get; init; } = typeof(T).Name;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public StandardCRUDDataLayer() { }

    /// <summary>
    /// The constructor which takes the HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to interact with the remote server.</param>
    public StandardCRUDDataLayer(HttpClient httpClient) => HttpClient = httpClient;

    /// <inheritdoc/>
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        long count = 0;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Count", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            string content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
            _ = long.TryParse(content, out count);
        }

        return count;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            T? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, dataObject: returnedDataObject);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail, validationErrors: details?.Errors);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }

        return new OperationResult(httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async Task<OperationResult> DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"api/{TypeName}/{(!string.IsNullOrWhiteSpace(dataObject.StringID) ? dataObject.StringID : dataObject.Integer64ID)}", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.Conflict)
        {
            ConflictDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ConflictDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }

        return new OperationResult(httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    public async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<T>? dataObjects = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>(cancellationToken: cancellationToken);
        }

        return dataObjects;
    }

    /// <inheritdoc/>
    public async Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
    {
        List<ListView>? listView = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All/ListView", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            listView = await httpResponseMessage.Content.ReadFromJsonAsync<List<ListView>?>(cancellationToken: cancellationToken);
        }

        return listView;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<T>? pagedDataObjects = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Page?{queryDefinition.ToQueryString()}", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            pagedDataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<T>?>(cancellationToken: cancellationToken);
        }

        return pagedDataObjects;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<ListView>? pagedListView = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Page/ListView?{queryDefinition.ToQueryString()}", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            pagedListView = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<ListView>?>(cancellationToken: cancellationToken);
        }

        return pagedListView;
    }

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Single", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
        }

        return dataObject;
    }

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(id.ToString(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        T? dataObject = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Single/{id}", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
        }

        return dataObject;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            T? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, dataObject: returnedDataObject);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.Conflict)
        {
            ConflictDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ConflictDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail, validationErrors: details?.Errors);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            NotFoundDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<NotFoundDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return new OperationResult(httpResponseMessage.StatusCode, problemDetails: details?.Detail);
        }

        return new OperationResult(httpResponseMessage.StatusCode);
    }
}
