using JMayer.Data.Data;
using System.Net;
using System.Net.Http.Json;

#warning I feel like there should be more to this. Maybe user editable can do export/import?

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class for interacting with remote data via HTTP using CRUD operations.
/// </summary>
/// <typeparam name="T">A UserEditableDataObject which represents data on the remote server.</typeparam>
public class UserEditableDataLayer<T> : StandardCRUDDataLayer<T>, IUserEditableDataLayer<T> where T : UserEditableDataObject
{
    /// <summary>
    /// The method returns all the remote data objects as a list view.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    public async Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default)
    {
        List<ListView>? listView = [];
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{_typeName}/All/ListView", cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode && httpResponseMessage.StatusCode != HttpStatusCode.NoContent)
        {
            listView = await httpResponseMessage.Content.ReadFromJsonAsync<List<ListView>?>(cancellationToken);
        }

        return listView;
    }
}
