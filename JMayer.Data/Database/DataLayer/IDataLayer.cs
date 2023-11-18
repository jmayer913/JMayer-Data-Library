using JMayer.Data.Data;
using System.Linq.Expressions;

#warning I should add cancellation tokens to the interface.

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
        /// <returns>A count.</returns>
        Task<int> CountAsync();

        /// <summary>
        /// The method returns a count of data objects in a collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A count.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> wherePredicate);

        /// <summary>
        /// The method creates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <returns>The created data object.</returns>
        Task<T> CreateAsync(T dataObject);

        /// <summary>
        /// The method deletes a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <returns>A Task object for the async.</returns>
        Task DeleteAsync(T dataObject);

        /// <summary>
        /// The method returns all the data objects for the table or collection.
        /// </summary>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate);

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate with an order.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderByPredicate, bool descending = false);

        /// <summary>
        /// The method returns the first data object in the collection/table.
        /// </summary>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync();

        /// <summary>
        /// The method returns a data object in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate);

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <returns>The latest data object.</returns>
        Task<T> UpdateAsync(T dataObject);
    }
}
