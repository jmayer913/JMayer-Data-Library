using System.Linq.Expressions;

namespace JMayer.Data.Data.Query;

/// <summary>
/// The class represents the sort definition to be used when querying data.
/// </summary>
public class SortDefinition
{
    /// <summary>
    /// The property gets/sets if sorting is ascending or descending.
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// The property gets/sets the public property/field to sort on.
    /// </summary>
    public string SortOn { get; set; } = string.Empty;

    /// <summary>
    /// The method returns an expression to be used by OrderBy or OrderByDescending.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>The expression which needs to be passed to OrderBy or OrderByDescending.</returns>
    public Expression<Func<T, object>> ToExpression<T>() where T : class
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, SortOn);
        var convertOperation = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(convertOperation, parameter);
    }
}
