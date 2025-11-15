using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.HTTP.Details;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Data.Repository.Http.WebApi;

#warning I need to figure out how delete multiple will work on the controller side.
#warning I'm not sure what to do about NameExistsAsync(). The controller doesn't have this and I'm not sure I want to expose a way to find data objects based on name.

#warning I should call the events.

public class StandardCRUDRepository<T> : IStandardCRUDRepository<T>
    where T : DataObject
{
    /// <inheritdoc/>
    public event EventHandler<CreatedEventArgs>? Created;

    /// <inheritdoc/>
    public event EventHandler<DeletedEventArgs>? Deleted;

    /// <summary>
    /// The property gets the HTTP client used to interact with the remote server.
    /// </summary>
    protected HttpClient HttpClient { get; init; } = new();

    /// <summary>
    /// The property gets the name of the type associated with the repository layer.
    /// </summary>
    /// <remarks>
    /// The type name is used in the route for the API. It uses 
    /// the standard format, api/TypeName.
    /// </remarks>
    protected string TypeName { get; init; } = typeof(T).Name;

    /// <summary>
    /// The constant for the unexpected data object message.
    /// </summary>
    private const string UnexpectedDataObjectMessage = "The server responded with an unexpected data object.";

    /// <inheritdoc/>
    public event EventHandler<UpdatedEventArgs>? Updated;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public StandardCRUDRepository() { }

    /// <summary>
    /// The constructor which takes the HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to interact with the remote server.</param>
    public StandardCRUDRepository(HttpClient httpClient) => HttpClient = httpClient;

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(CancellationToken cancellationToken = default)
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
    public virtual async Task<OperationResult<T>> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            T? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);

            if (returnedDataObject is not null)
            {
                return OperationResult<T>.Success(returnedDataObject);
            }
            else
            {
                return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, UnexpectedDataObjectMessage);
            }
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure(details?.Errors ?? []);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }

        return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    public virtual async Task<OperationResult<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        HttpResponseMessage httpResponseMessage = await HttpClient.PostAsJsonAsync($"api/{TypeName}", dataObjects, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            List<T>? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>(cancellationToken: cancellationToken);

            if (returnedDataObject is not null)
            {
                return OperationResult<T>.Success(returnedDataObject);
            }
            else
            {
                return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, UnexpectedDataObjectMessage);
            }
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure(details?.Errors ?? []);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }

        return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public virtual async Task<OperationResult<T>> DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync($"api/{TypeName}/{(!string.IsNullOrWhiteSpace(dataObject.StringID) ? dataObject.StringID : dataObject.Integer64ID)}", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            T? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);

            if (returnedDataObject is not null)
            {
                return OperationResult<T>.Success(returnedDataObject);
            }
            else
            {
                return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, UnexpectedDataObjectMessage);
            }
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.Conflict)
        {
            ConflictDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ConflictDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }

        return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not expected to be called.</exception>
    /// /// <remarks>
    /// The JMayer Web API Standard does not accept a remote source to check if a data object exists by name so the NotImplementedException 
    /// will be thrown. You can override this if you expand the JMayer Web API Standard to accept this type of request or you decide to use a
    /// different Web API backend.
    /// </remarks>
    public virtual Task<OperationResult<T>> DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
#warning The controller accepts an id and I would need to think about to make it accept multiple ids.
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public virtual async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
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
    public virtual async Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
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
    public virtual async Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
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
    public virtual async Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
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
    public virtual async Task<T?> GetSingleAsync(CancellationToken cancellationToken = default)
    {
        T? dataObject = default;
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync($"api/{TypeName}/Single", cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);
        }

        return dataObject;
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(id.ToString(), cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default)
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
    /// <exception cref="NotImplementedException">This method is not expected to be called.</exception>
    /// /// <remarks>
    /// The JMayer Web API Standard does not accept a remote source to check if a data object exists by name so the NotImplementedException 
    /// will be thrown. You can override this if you expand the JMayer Web API Standard to accept this type of request or you decide to use a
    /// different Web API backend.
    /// </remarks>
    public virtual Task<bool> NameExistsAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The method calls the Created event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnCreated(CreatedEventArgs e) => Created?.Invoke(this, e);

    /// <summary>
    /// The method calls the Deleted event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnDeleted(DeletedEventArgs e) => Deleted?.Invoke(this, e);

    /// <summary>
    /// The method calls the Updated event so any registered handler can react to the event.
    /// </summary>
    /// <param name="e">The arguments associated with the event.</param>
    protected virtual void OnUpdated(UpdatedEventArgs e) => Updated?.Invoke(this, e);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public virtual async Task<OperationResult<T>> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/{TypeName}", dataObject, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            T? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);

            if (returnedDataObject is not null)
            {
                return OperationResult<T>.Success(returnedDataObject);
            }
            else
            {
                return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, UnexpectedDataObjectMessage);
            }
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.Conflict)
        {
            ConflictDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ConflictDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure(details?.Errors ?? []);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            NotFoundDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<NotFoundDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }

        return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
    public virtual async Task<OperationResult<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObjects);

        HttpResponseMessage httpResponseMessage = await HttpClient.PutAsJsonAsync($"api/{TypeName}", dataObjects, cancellationToken: cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode is not HttpStatusCode.NoContent)
        {
            List<T>? returnedDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>(cancellationToken: cancellationToken);

            if (returnedDataObject is not null)
            {
                return OperationResult<T>.Success(returnedDataObject);
            }
            else
            {
                return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, UnexpectedDataObjectMessage);
            }
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.Conflict)
        {
            ConflictDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ConflictDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure(details?.Errors ?? []);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.NotFound)
        {
            NotFoundDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<NotFoundDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }
        else if (httpResponseMessage.IsSuccessStatusCode is false && httpResponseMessage.StatusCode is HttpStatusCode.InternalServerError)
        {
            ProblemDetails? details = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
            return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode, details?.Detail);
        }

        return OperationResult<T>.Failure((int)httpResponseMessage.StatusCode);
    }
}
