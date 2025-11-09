using JMayer.Data.Data;
using JMayer.Data.Data.Query;

namespace JMayer.Data.Repository;

/// <summary>
/// The interface used to access the repository layer; contains CRUD operations for local or remote data.
/// </summary>
/// <typeparam name="T">A DataObject which represents data stored in the repository.</typeparam>
public interface IStandardCRUDRepository<T> where T : DataObject
{
    /// <summary>
    /// The method returns the total count of data objects in a collection/table.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A count.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method creates a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The created data object.</returns>
    Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method deletes a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The deleted data object.</returns>
    Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the remote data objects.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if issue on the server side.</returns>
    Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the remote data objects as a list view.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of list views or null if issue on the server side.</returns>
    Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if issue on the server side.</returns>
    Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects as a list view.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A page of list views or null if issue on the server side.</returns>
    Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the first remote data object.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the remote data object based on an ID.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the remote data object based on an ID.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns if the name exists or not in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to check.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>If the name exists or not.</returns>
    Task<bool> NameExistsAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method updates a remote data object.
    /// </summary>
    /// <param name="dataObject">The data object to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The updated data object.</returns>
    Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);
}
