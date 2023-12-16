using JMayer.Data.Data;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The interface for interacting with a collection/table in a database using CRUD operations.
/// </summary>
/// <typeparam name="T">A UserEditableDataObject which represents the user editable data in the collection/table.</typeparam>
public interface IUserEditableDataLayer<T> : IStandardCRUDDataLayer<T> where T : UserEditableDataObject
{
    /// <summary>
    /// The method returns all the data objects for the table or collection as a list view.
    /// </summary>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>> GetAllListViewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects for the collection/table as a list view based on a where predicate.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>> wherePredicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects for the collection/table as a list view with an order.
    /// </summary>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns all the data objects for the collection/table as a list view based on a where predicate with an order.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of DataObjects.</returns>
    Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, object>> orderByPredicate, bool descending = false, CancellationToken cancellationToken = default);
}
