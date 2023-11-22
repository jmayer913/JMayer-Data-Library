using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The interface for interacting with a collection/table in a database using CRUD operations.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
    public interface IDataLayer<T> where T : DataObject
    {
        /// <summary>
        /// The method returns the total count of data objects in a collection/table.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A count.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns a count of data objects in a collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A count.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method creates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The created data object.</returns>
        Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method deletes a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A Task object for the async.</returns>
        Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns if the key exists in the collection/table.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>True means the key exists; false means it does not.</returns>
        Task<bool> ExistAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns if data objects exists in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>True means the data objects exists based on the expression; false means none do.</returns>
        Task<bool> ExistAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the table or collection.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the collection/table with an order.
        /// </summary>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate with an order.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns the first data object in the collection/table.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns the first data object in the collection/table based on a key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns a data object in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The latest data object.</returns>
        Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method validates a data object.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>The validation result.</returns>
        Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default);
    }
}
