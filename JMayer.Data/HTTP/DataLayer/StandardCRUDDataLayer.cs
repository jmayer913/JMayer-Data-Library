using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class for interacting with remote data via HTTP using CRUD operations.
/// </summary>
/// <typeparam name="T">A DataObject which represents data on the remote server.</typeparam>
public class StandardCRUDDataLayer<T> : IStandardCRUDDataLayer<T> where T : DataObject
{
    /// <summary>
    /// The json serializer options for the bad request content.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    /// <summary>
    /// The HTTP client used to interact with the remote server.
    /// </summary>
    protected readonly HttpClient HttpClient = new();

    /// <summary>
    /// The name of the type associated with the data layer.
    /// </summary>
    /// <remarks>
    /// The type name is used in the route for the API. It uses 
    /// the standard format, api/typeName/action.
    /// </remarks>
    protected readonly string TypeName = typeof(T).Name;

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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Count", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            string content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            _ = long.TryParse(content, out count);
        }

        return count;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        T? latestDataObject = null;
        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            latestDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }
        else if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            validationResult = await DeserializedBadRequestContent(httpResponseMessage, cancellationToken);
        }

        return new OperationResult(latestDataObject, validationResult, httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public async Task<OperationResult> DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"api/{TypeName}/{(!string.IsNullOrWhiteSpace(dataObject.StringID) ? dataObject.StringID : dataObject.Integer64ID)}", cancellationToken);
        return new OperationResult(null, null, httpResponseMessage.StatusCode);
    }

    /// <summary>
    /// The method deserializes the content of a bad request.
    /// </summary>
    /// <param name="httpResponseMessage">The HTTP response.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A ServerSideValidationResult object.</returns>
    /// <remarks>
    /// When a controller has the [ApiController] attribute, asp.net core will check the serialized [FromBody] object against the data
    /// annotations on the object and if validation fails, asp.net core will return a bad request with a ValidationProblemDetails
    /// object. Because asp.net core can return its own object, the HTTP datalyer needs to handle if a ServerSideValidationResult object 
    /// or a ValidationProblemDetails object is returned in the response.
    /// </remarks>
    protected async Task<ServerSideValidationResult?> DeserializedBadRequestContent(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        Exception? originalException = null;
        ServerSideValidationResult? result = null;
        string content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            result = JsonSerializer.Deserialize<ServerSideValidationResult>(content, _jsonSerializerOptions);
        }
        catch (Exception e)
        { 
            //Capture the original exception so it can be potentially be thrown later.
            originalException = e;
        }

        if (result == null)
        {
            try
            {
                ValidationProblemDetails? dataAnnotationValidation = JsonSerializer.Deserialize<ValidationProblemDetails>(content, _jsonSerializerOptions);

                if (dataAnnotationValidation != null)
                {
                    result = new(dataAnnotationValidation);
                }
            }
            catch { }
        }

        //Throw the original exception if the content failed to parse into a ServerSideValidationResult object or a ValidationProblemDetails object.
        if (result == null && originalException != null)
        {
            throw originalException;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<T>? dataObjects = [];
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/All", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>(cancellationToken);
        }

        return dataObjects;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        PagedList<T>? pagedDataObjects = new();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Page?{queryDefinition.ToQueryString()}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            pagedDataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<PagedList<T>?>(cancellationToken);
        }

        return pagedDataObjects;
    }

    /// <inheritdoc/>
    public async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Single", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
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
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Single/{id}", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }

        return dataObject;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        T? latestDataObject = null;
        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            latestDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
        }
        else if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            validationResult = await DeserializedBadRequestContent(httpResponseMessage, cancellationToken);
        }

        return new OperationResult(latestDataObject, validationResult, httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    public async Task<ServerSideValidationResult?> ValidationAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        ServerSideValidationResult? validationResult = null;
        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/{TypeName}/Validate", dataObject, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            validationResult = await httpResponseMessage.Content.ReadFromJsonAsync<ServerSideValidationResult?>(cancellationToken);
        }
        else if (!httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            validationResult = await DeserializedBadRequestContent(httpResponseMessage, cancellationToken);
        }

        return validationResult;
    }
}
