using JMayer.Data.Data;
using JMayer.Data.Data.Query;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The interface for interacting with remote sub data objects using CRUD operations.
/// </summary>
/// <typeparam name="T">A SubDataObject which represents data on the remote server.</typeparam>
public interface IStandardSubCRUDDataLayer<T> : IStandardCRUDDataLayer<T>
    where T : SubDataObject
{
    /// <summary>
    /// The method returns all the remote data objects based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<List<T>?> GetAllAsync(long ownerID, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the remote data objects based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<List<T>?> GetAllAsync(string ownerID, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the remote data objects as a list view based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<List<ListView>?> GetAllListViewAsync(long ownerID, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the remote data objects as a list view based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<List<ListView>?> GetAllListViewAsync(string ownerID, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<PagedList<T>?> GetPageAsync(long ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<PagedList<T>?> GetPageAsync(string ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects as a list view based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<PagedList<ListView>?> GetPageListViewAsync(long ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of remote data objects as a list view based on an owner.
    /// </summary>
    /// <param name="ownerID">The owner ID to search for.</param>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of sub data objects.</returns>
    Task<PagedList<ListView>?> GetPageListViewAsync(string ownerID, QueryDefinition queryDefinition, CancellationToken cancellationToken = default);
}
