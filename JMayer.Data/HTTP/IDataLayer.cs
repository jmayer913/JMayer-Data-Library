﻿using JMayer.Data.Data;

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
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A count.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method creates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The results of the create operation.</returns>
        Task<OperationResult> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method deletes a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The results of the delete operation.</returns>
        Task<OperationResult> DeleteAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the remote data objects.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>?> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns the first remote data object.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns the remote data object based on a key.
        /// </summary>
        /// <param name="key">The key to filter for.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method updates a remote data object.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The results of the update operation.</returns>
        Task<OperationResult> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method validates the data object on the remote server.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The results of the validation.</returns>
        Task<ServerSideValidationResult?> ValidationAsync(T dataObject, CancellationToken cancellationToken = default);
    }
}