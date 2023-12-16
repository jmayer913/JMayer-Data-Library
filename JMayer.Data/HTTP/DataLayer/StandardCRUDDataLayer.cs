using JMayer.Data.Data;
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
    protected readonly HttpClient _httpClient = new();

    /// <summary>
    /// The name of the type associated with the data layer.
    /// </summary>
    /// <remarks>
    /// The type name is used in the route for the API. It uses 
    /// the standard format, api/typeName/action.
    /// </remarks>
    protected readonly string _typeName = typeof(T).Name;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public StandardCRUDDataLayer() { }

    /// <summary>
    /// The constructor which takes the HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to interact with the remote server.</param>
    public StandardCRUDDataLayer(HttpClient httpClient) => _httpClient = httpClient;

    /// <summary>
    /// The method creates a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The created data object.</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        int count = 0;
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/api/{_typeName}/Count", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            string content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            _ = int.TryParse(content, out count);
        }

        return count;
    }

    /// <summary>
    /// The method creates a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>The results of the create operation.</returns>
    public async Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        T? latestDataObject = null;
        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/api/{_typeName}", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            latestDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }
        else if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            validationResult = await httpResponseMessage.Content.ReadFromJsonAsync<ServerSideValidationResult>(cancellationToken);
        }

        return new OperationResult(latestDataObject, validationResult, httpResponseMessage.StatusCode);
    }

    /// <summary>
    /// The method deletes a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>The results of the delete operation.</returns>
    public async Task<OperationResult> DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        HttpResponseMessage httpResponseMessage = await _httpClient.DeleteAsync($"/api/{_typeName}/{(!string.IsNullOrWhiteSpace(dataObject.StringID) ? dataObject.StringID : dataObject.Integer64ID)}", cancellationToken);
        return new OperationResult(null, null, httpResponseMessage.StatusCode);
    }

    /// <summary>
    /// The method returns all the remote data objects.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<T>? dataObjects = [];
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/api/{_typeName}/All", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>(cancellationToken);
        }

        return dataObjects;
    }

    /// <summary>
    /// The method returns the first remote data object.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A DataObject.</returns>
    public async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/api/{_typeName}/Single", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }

        return dataObject;
    }

    /// <summary>
    /// The method returns the remote data object based on an ID.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentException">Thrown if the ID parameter is null or whitespace.</exception>
    /// <returns>A DataObject.</returns>
    public async Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        T? dataObject = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/api/{_typeName}/Single/{id}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }

        return dataObject;
    }

    /// <summary>
    /// The method updates a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>The results of the update operation.</returns>
    public async Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        T? latestDataObject = null;
        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.PutAsJsonAsync($"/api/{_typeName}", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            latestDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }
        else if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            validationResult = await httpResponseMessage.Content.ReadFromJsonAsync<ServerSideValidationResult>(cancellationToken);
        }

        return new OperationResult(latestDataObject, validationResult, httpResponseMessage.StatusCode);
    }

    /// <summary>
    /// The method validates the data object on the remote server.
    /// </summary>
    /// <param name="dataObject">The data object to validate.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    /// <returns>The results of the validation.</returns>
    public async Task<ServerSideValidationResult?> ValidationAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/api/{_typeName}/Validate", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            validationResult = await httpResponseMessage.Content.ReadFromJsonAsync<ServerSideValidationResult?>(cancellationToken);
        }

        return validationResult;
    }
}
