using JMayer.Data.Data;

#warning I should add cancellation tokens to the interface.

namespace JMayer.Data.HTTP
{
    /// <summary>
    /// The interface for interacting with remote data using CRUD operations.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data on the remote server.</typeparam>
    public interface IDataLayer<T> where T : DataObject
    {
        /// <summary>
        /// The method returns the total count of data objects in a collection/table.
        /// </summary>
        /// <returns>A count.</returns>
        public Task<int> CountAsync();

        /// <summary>
        /// The method creates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <returns>The created data object.</returns>
        public Task<OperationResult> CreateAsync(T dataObject);

        /// <summary>
        /// The method deletes a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <returns>A Task object for the async.</returns>
        public Task<OperationResult> DeleteAsync(T dataObject);

        /// <summary>
        /// The method returns all the remote data objects.
        /// </summary>
        /// <returns>A list of DataObjects.</returns>
        public Task<List<T>?> GetAllAsync();

        /// <summary>
        /// The method returns the first remote data object.
        /// </summary>
        /// <returns>A DataObject.</returns>
        public Task<T?> GetSingleAsync();

        /// <summary>
        /// The method returns the remote data object based on a key.
        /// </summary>
        /// <param name="key">The key to filter for.</param>
        /// <returns>A DataObject.</returns>
        public Task<T?> GetSingleAsync(string key);

        /// <summary>
        /// The method updates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <returns>The updated data object.</returns>
        public Task<OperationResult> UpdateAsync(T dataObject);
    }
}
