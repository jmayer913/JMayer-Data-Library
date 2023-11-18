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
        /// The method creates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <returns>The created data object.</returns>
        public async Task<int> CountAsync()
        {
            int count = 0;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Count");

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                _ = int.TryParse(content, out count);
            }

            return count;
        }

        /// <summary>
        /// The method creates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <returns>The created data object.</returns>
        public async Task<OperationResult> CreateAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? lastDataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{_typeName}", dataObject);

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                lastDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>();
            }

            return new OperationResult(lastDataObject, httpResponseMessage.StatusCode);
        }

        /// <summary>
        /// The method deletes a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <returns>A Task object for the async.</returns>
        public async Task<OperationResult> DeleteAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);
            HttpResponseMessage httpResponseMessage = await _httpClient.DeleteAsync($"{_typeName}/{dataObject.Key}");
            return new OperationResult(null, httpResponseMessage.StatusCode);
        }

        /// <summary>
        /// The method returns all the remote data objects.
        /// </summary>
        /// <returns>A list of DataObjects.</returns>
        public async Task<List<T>?> GetAllAsync()
        {
            List<T>? dataObjects = [];
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/All");

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                dataObjects = await httpResponseMessage.Content.ReadFromJsonAsync<List<T>?>();
            }

            return dataObjects;
        }

        /// <summary>
        /// The method returns the first remote data object.
        /// </summary>
        /// <returns>A DataObject.</returns>
        public async Task<T?> GetSingleAsync()
        {
            T? dataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Single");

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>();
            }

            return dataObject;
        }

        /// <summary>
        /// The method returns the remote data object based on a key.
        /// </summary>
        /// <param name="key">The key to filter for.</param>
        /// <returns>A DataObject.</returns>
        public async Task<T?> GetSingleAsync(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            T? dataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/Single/{key}");

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                dataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>();
            }

            return dataObject;
        }

        /// <summary>
        /// The method updates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <returns>The updated data object.</returns>
        public async Task<OperationResult> UpdateAsync(T dataObject)
        {
            ArgumentNullException.ThrowIfNull(dataObject);

            T? lastDataObject = null;
            HttpResponseMessage httpResponseMessage = await _httpClient.PutAsJsonAsync($"{_typeName}", dataObject);

            if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
            {
                lastDataObject = await httpResponseMessage.Content.ReadFromJsonAsync<T?>();
            }

            return new OperationResult(lastDataObject, httpResponseMessage.StatusCode);
        }
    }
}
