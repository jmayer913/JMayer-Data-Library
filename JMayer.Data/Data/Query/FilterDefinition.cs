using System.Linq.Expressions;
using System.Reflection;

namespace JMayer.Data.Data.Query;

/// <summary>
/// The class represents the filter definition to be used when querying data.
/// </summary>
public class FilterDefinition
{
    /// <summary>
    /// The constant for the string.Contains() method name.
    /// </summary>
    private const string ContainsMethodName = nameof(string.Contains);

    /// <summary>
    /// The constant for the contains operator.
    /// </summary>
    public const string ContainsOperator = "contains";

    /// <summary>
    /// The constant for the equals operator.
    /// </summary>
    public const string EqualsOperator = "equals";

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
    /// The constant for the object.ToString() method name.
    /// </summary>
    private const string ToStringMethodName = nameof(ToString);

    /// <summary>
    /// The method returns a contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToContainsExpression<T>()
    {
        MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(ContainsMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var stringConversion = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var contains = Expression.Call(stringConversion, stringContainsMethodInfo, value);

        return Expression.Lambda<Func<T, bool>>(contains, parameter);
    }

    /// <summary>
    /// The method returns an equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>An equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToEqualsExpression<T>()
    {
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var stringConversion = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var equals = Expression.Equal(stringConversion, value);

        return Expression.Lambda<Func<T, bool>>(equals, parameter);
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
            EqualsOperator => ToEqualsExpression<T>(),
            _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
        };
    }
}
