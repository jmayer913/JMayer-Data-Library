using JMayer.Data.Data;

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
}
