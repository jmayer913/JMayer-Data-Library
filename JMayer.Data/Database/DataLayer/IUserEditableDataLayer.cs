using JMayer.Data.Data;
using JMayer.Data.Data.Query;
using System.Linq.Expressions;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The interface for interacting with a collection/table in a database using CRUD operations.
/// </summary>
/// <typeparam name="T">A UserEditableDataObject which represents the user editable data in the collection/table.</typeparam>
public interface IUserEditableDataLayer<T> : IStandardCRUDDataLayer<T> where T : UserEditableDataObject
{
    /// <summary>
    /// The property gets/sets if milliseconds, microseconds and nanoseconds are ignored when comparing the LastEditedOn 
    /// for old data object detection.
    /// </summary>
    /// <remarks>
    /// Syncfusion converts all DateTime properties in an object to UTC and the millseconds, microseconds and nanoseconds
    /// are stipped from the DateTime. Because of this, the data layer would always detect old data and throw a DataObjectUpdateConflictException 
    /// so this property was introduced when the comparison needs to be less precise.
    /// 
    /// This property is ignored if the IsOldDataObjectDetectionEnabled set to false.
    /// </remarks>
    bool IsLessPreciseTimestampComparisonEnabled { get; set; }

    /// <summary>
    /// The property gets/sets if the data layer checks if the data object being updated is considered old.
    /// </summary>
    /// <remarks>
    /// When enabled, the data layer will compare the data object passed into UpdateAsync() with the data
    /// object already stored in the collection/table. The LastEditedOn property will be compared and if the
    /// timestamps are the same then no one else has edited the data object and the update will occur. If the 
    /// timestamps are not the same, another user has edited the data object and the data layer throws a 
    /// DataObjectUpdateConflictException.
    /// </remarks>
    bool IsOldDataObjectDetectionEnabled { get; set; }

    /// <summary>
    /// The method returns all the data objects for the collection/table as a list view based on a where predicate with an order.
    /// </summary>
    /// <param name="wherePredicate">The where predicate to use against the collection/table.</param>
    /// <param name="orderByPredicate">The order predicate to use against the collection/table.</param>
    /// <param name="descending">False means the data is ordered ascending; true means the data is ordered descending.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of ListView objects.</returns>
    Task<List<ListView>> GetAllListViewAsync(Expression<Func<T, bool>>? wherePredicate = default, Expression<Func<T, object>>? orderByPredicate = default, bool descending = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// The method returns a page of data objects for the collection/table as a list view.
    /// </summary>
    /// <param name="queryDefinition">Defines how the data should be queried; includes filtering, paging and sorting.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <exception cref="ArgumentNullException">Thrown if the queryDefinition parameter is null.</exception>
    /// <returns>A list of ListView objects.</returns>
    Task<PagedList<ListView>> GetPageListViewAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default);
}
