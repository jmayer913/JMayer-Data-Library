using JMayer.Data.Data;
using JMayer.Data.Data.Query;

#warning I feel like there should be more to this. Maybe user editable can do export/import?

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The interface for interacting with remote data using CRUD operations.
/// </summary>
/// <typeparam name="T">An UserEditableDataObject which represents data on the remote server.</typeparam>
public interface IUserEditableDataLayer<T> : IStandardCRUDDataLayer<T> where T : UserEditableDataObject
{
    /// <summary>
    /// The method returns all the remote data objects as a list view.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects as a list view.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);
}
