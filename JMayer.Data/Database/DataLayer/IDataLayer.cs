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
        /// A event for when a data object is created in the data layer.
        /// </summary>
        event EventHandler<CreatedEventArgs>? Created;

        /// <summary>
        /// A event for when a data object is deleted in the data layer.
        /// </summary>
        event EventHandler<DeletedEventArgs>? Deleted;

        /// <summary>
        /// A event for when a data object is updated in the data layer.
        /// </summary>
        event EventHandler<UpdatedEventArgs>? Updated;

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
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
        /// <returns>A count.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method creates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to create.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
        /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
        /// <returns>The created data object.</returns>
        Task<T> CreateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method creates multiple data objects in the collection/table.
        /// </summary>
        /// <param name="dataObjects">The data objects to create.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
        /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
        /// <returns>The created data object.</returns>
        Task<List<T>> CreateAsync(List<T> dataObjects, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method deletes a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to delete.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
        /// <returns>A Task object for the async.</returns>
        Task DeleteAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method deletes multiple data objects in the collection/table.
        /// </summary>
        /// <param name="dataObjects">The data objects to delete.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
        /// <returns>A Task object for the async.</returns>
        Task DeleteAsync(List<T> dataObjects, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method deletes multiple data objects in the collection/table.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use when deleting records.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
        /// <returns>A Task object for the async.</returns>
        Task DeleteAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns if data objects exists in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the collection/table with an order.
        /// </summary>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the orderByPredicate parameter is null.</exception>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns all the data objects for the collection/table based on a where predicate with an order.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
        /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate or orderByPredicate parameters are null.</exception>
        /// <returns>A list of DataObjects.</returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns the first data object in the collection/table.
        /// </summary>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The method returns a data object in the collection/table based on a where predicate.
        /// </summary>
        /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the wherePredicate parameter is null.</exception>
        /// <returns>A DataObject.</returns>
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method updates a data object in the table or collection.
        /// </summary>
        /// <param name="dataObject">The data object to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
        /// <exception cref="DataObjectValidationException">Thrown if the data object fails validation.</exception>
        /// <exception cref="IDNotFoundException">Thrown if the data object's ID is not found in the collection/table.</exception>
        /// <returns>The latest data object.</returns>
        Task<T> UpdateAsync(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method updates multiple data objects in the collection/table.
        /// </summary>
        /// <param name="dataObjects">The data objects to update.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObjects parameter is null.</exception>
        /// <exception cref="DataObjectValidationException">Thrown if any data object fails validation.</exception>
        /// <exception cref="IDNotFoundException">Thrown if any data objects' ID is not found in the collection/table.</exception>
        /// <returns>The latest data objects.</returns>
        Task<List<T>> UpdateAsync(List<T> dataObjects, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method validates a data object.
        /// </summary>
        /// <param name="dataObject">The data object to validate.</param>
        /// <param name="cancellationToken">A token used for task cancellations.</param>
        /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
        /// <returns>The validation result.</returns>
        Task<List<ValidationResult>> ValidateAsync(T dataObject, CancellationToken cancellationToken = default);
    }
}
