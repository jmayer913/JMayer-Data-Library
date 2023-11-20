using JMayer.Data.Data;
using System.Net;
using System.Net.Http.Json;

namespace JMayer.Data.HTTP
{
    /// <summary>
    /// The class for interacting with remote data via HTTP using CRUD operations.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data on the remote server.</typeparam>
    public class DataLayer<T> : IDataLayer<T> where T : DataObject
    {
        /// <summary>
        /// The property gets/sets the base address for the data layer.
        /// </summary>
        public Uri? BaseAddress
        {
            get => _httpClient.BaseAddress;
            set => _httpClient.BaseAddress = value;
        }

        /// <summary>
        /// The HTTP client used to interact with the remote server.
        /// </summary>
        protected readonly HttpClient _httpClient = new();

        /// <summary>
        /// The name of the type associated with the data layer.
        /// </summary>
        /// <remarks>
        /// The type name is used in the route for the API. It uses 
        /// the standard format, api/type name/action.
        /// </remarks>
        protected readonly string _typeName = nameof(T);

        /// <summary>
        /// The default constructor.
        /// </summary>
        public DataLayer() { }

        /// <summary>
        /// The constructor which takes the HTTP client.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to interact with the remote server.</param>
        public DataLayer(HttpClient httpClient) => _httpClient = httpClient;

        /// <summary>
        /// The method creates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The created data object.</returns>
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            int count = 0;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Count", cancellationToken);

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
        /// <returns>The created data object.</returns>
        public async Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? lastDataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{_typeName}", dataObject, cancellationToken);

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                lastDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
            }

            return new OperationResult(lastDataObject, httpResponseMessage.StatusCode);
        }

        /// <summary>
        /// The method deletes a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A Task object for the async.</returns>
        public async Task<OperationResult> DeleteAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);
            HttpResponseMessage httpResponseMessage = await _httpClient.DeleteAsync($"{_typeName}/{dataObject.Key}", cancellationToken);
            return new OperationResult(null, httpResponseMessage.StatusCode);
        }

        /// <summary>
        /// The method returns all the remote data objects.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        public async Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<T>? dataObjects = [];
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/All", cancellationToken);

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
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Single", cancellationToken);

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
            }

            return dataObject;
        }

        /// <summary>
        /// The method returns the remote data object based on a key.
        /// </summary>
        /// <param name="key">The key to filter for.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        public async Task<T?> GetSingleAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            T? dataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Single/{key}", cancellationToken);

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
        /// <returns>The updated data object.</returns>
        public async Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? lastDataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.PutAsJsonAsync($"{_typeName}", dataObject, cancellationToken);

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                lastDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>(cancellationToken);
            }

            return new OperationResult(lastDataObject, httpResponseMessage.StatusCode);
        }
    }
}
