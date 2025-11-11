using JMayer.Data.Data;
using JMayer.Data.Data.Query;

namespace JMayer.Data.Repository;

/// <summary>
/// The interface is used to access the repository layer.
/// </summary>
/// <typeparam name="T">A DataObject which represents data stored in the repository.</typeparam>
/// <remarks>
/// This is meant to be universal meaning it'll work if your accessing a database or accessing a server with HTTP.
/// </remarks>
public interface IStandardCRUDRepository<T> where T : DataObject
{
    /// <summary>
    /// The method returns the total count of data objects in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A count.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method creates a data object in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The created data object.</returns>
    Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method creates multiple data objects in the repository.
    /// </summary>
    /// <param name="dataObjects">The data objects to create.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The created data object.</returns>
    Task<List<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method deletes a data object in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A Task for the async.</returns>
    Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method deletes multiple data objects in the repository.
    /// </summary>
    /// <param name="dataObjects">The data objects to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A Task for the async.</returns>
    Task DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if an issue occurred.</returns>
    Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects as a list view in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of list views or null if an issue occurred.</returns>
    Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of data objects in the repository.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if an issue occurred.</returns>
    Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of data objects as a list view in the repository.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A page of list views or null if an issue occurred.</returns>
    Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the first data object in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or an issue occurred.</returns>
    Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the data object based on an ID in the repository.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or an issue occurred.</returns>
    Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the data object based on an ID in the repository.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or an issue occurred.</returns>
    Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns if the name exists or not in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to check.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>If the name exists or not.</returns>
    Task<bool> NameExistsAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method updates a data object in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The updated data object.</returns>
    Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method updates multiple data objects in the repository.
    /// </summary>
    /// <param name="dataObjects">The data objects to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The latest data objects.</returns>
    Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default);
}
