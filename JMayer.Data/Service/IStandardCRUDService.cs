using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using JMayer.Data.Repository;

namespace JMayer.Data.Service;

/// <summary>
/// The interface is used to access the service layer.
/// </summary>
/// <typeparam name="T">A DataObject which represents data for the service interactions.</typeparam>
/// <typeparam name="U">An IStandardCRUDRepository which the service uses to interact with data in the repository.</typeparam>
public interface IStandardCRUDService<T, U> 
    where T : DataObject
    where U : IStandardCRUDRepository<T>
{
    /// <summary>
    /// The property gets/sets if the service layer checks if the data object being updated is considered old.
    /// </summary>
    /// <remarks>
    /// When enabled, the service layer will compare the data object passed into UpdateAsync() with the data
    /// object already stored in the repository. The LastEditedOn property will be compared and if the
    /// timestamps are the same then that means no one else has edited the data object and the update will occur. 
    /// If the timestamps are not the same, another user has edited the data object and the service layer 
    /// returns an operation failure.
    /// <br/>
    /// <br/>
    /// The default functionality is to not check for data submission conflicts between users but when you want this 
    /// then in the constructor of your child class, set this property to true.
    /// </remarks>
    bool IsOldDataDetectionEnabled { get; init; }

    /// <summary>
    /// The property gets/sets if the service layer ensures the name is unique in the repository.
    /// </summary>
    /// <remarks>
    /// The default functionality is to not check for name uniqueness because DataObject.Name is an optional field but if you do use 
    /// the name and you want to ensure name uniqueness then in the constructor of your child class, set this property to true.
    /// </remarks>
    bool IsUniqueNameRequired { get; init; }

    /// <summary>
    /// The property gets/sets if the service layer does any validation checks.
    /// </summary>
    /// <remarks>
    /// If the repository represents remote data, you may want the remote source to handle valdation so this allows you to control that.
    /// <br/>
    /// <br/>
    /// The default functionality is to check validation on the data object for a create or update operation but if you don't want 
    /// this then in the constructor of your child class, set this property to true.
    /// </remarks>
    bool IsValidationEnabled { get; init; }

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
    /// <returns>The results of the create operation.</returns>
    Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method deletes a ata object in the repository
    /// </summary>
    /// <param name="id">The id to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The results of the delete operation.</returns>
    Task<OperationResult> DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method deletes a ata object in the repository
    /// </summary>
    /// <param name="id">The id to delete.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The results of the delete operation.</returns>
    Task<OperationResult> DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if issue on the server side.</returns>
    Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects as a list view in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of list views or null if issue on the server side.</returns>
    Task<List<ListView>?> GetAllListViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of data objects in the repository.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of data objects or null if issue on the server side.</returns>
    Task<PagedList<T>?> GetPageAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of data objects as a list view in the repository.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A page of list views or null if issue on the server side.</returns>
    Task<PagedList<ListView>?> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the first data object in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the data object based on an ID in the repository.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns the data object based on an ID in the repository.
    /// </summary>
    /// <param name="id">The ID to filter for.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A data object or null if not found or issue on the server side.</returns>
    Task<T?> GetSingleAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method updates a data object in the repository.
    /// </summary>
    /// <param name="dataObject">The data object to update.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>The results of the update operation.</returns>
    Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);
}
