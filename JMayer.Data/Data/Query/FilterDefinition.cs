using System.Linq.Expressions;
using System.Reflection;

namespace JMayer.Data.Data.Query;

/// <summary>
/// The class represents the filter definition to be used when querying data.
/// </summary>
public class FilterDefinition
{
    /// <summary>
    /// The constant for the contains operator.
    /// </summary>
    public const string ContainsOperator = "contains";

    /// <summary>
    /// The property gets/sets the public property/field to filter on.
    /// </summary>
    public string FilterOn { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the operator to use when filtering.
    /// </summary>
    public string Operator { get; set; } = ContainsOperator;

    /// <summary>
    /// The property gets/sets the value to filter against.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The method returns a contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToContainsExpression<T>()
    {
        MethodInfo? stringContainsMethodInfo = typeof(string).GetMethod("Contains", [typeof(string)]);
        MethodInfo? objectToStringMethodInfo = typeof(object).GetMethod("ToString");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var stringConversion = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var contains = Expression.Call(stringConversion, stringContainsMethodInfo, value);

        return Expression.Lambda<Func<T, bool>>(contains, parameter);
    }

    /// <summary>
    /// The method returns an expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>The expression to be used for filtering.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Expression<Func<T, bool>> ToExpression<T>()
    {
        return Operator.ToLower() switch
        {
            ContainsOperator => ToContainsExpression<T>(),
            _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
        };
    }
}
