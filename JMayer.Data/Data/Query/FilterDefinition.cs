using System.Linq.Expressions;
using System.Reflection;

#warning The expressions do not handle if the value being evaluated is null.

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
    /// The constant for the string.EndsWith() method name.
    /// </summary>
    public const string EndsWithMethodName = nameof(string.EndsWith);

    /// <summary>
    /// The constant for the ends with operator.
    /// </summary>
    public const string EndsWithOperator = "ends with";

    /// <summary>
    /// The constant for the equals operator.
    /// </summary>
    public const string EqualsOperator = "equals";

    /// <summary>
    /// The property gets/sets the public property/field to filter on.
    /// </summary>
    public string FilterOn { get; set; } = string.Empty;

    /// <summary>
    /// The constant for the is empty operator.
    /// </summary>
    public const string IsEmptyOperator = "is empty";

    /// <summary>
    /// The constant for the is not empty operator.
    /// </summary>
    public const string IsNotEmptyOperator = "is not empty";

    /// <summary>
    /// The constant for the not contains operator.
    /// </summary>
    public const string NotContainsOperator = "not contains";

    /// <summary>
    /// The constant for the not equals operator.
    /// </summary>
    public const string NotEqualsOperator = "not equals";

    /// <summary>
    /// The property gets/sets the operator to use when filtering.
    /// </summary>
    public string Operator { get; set; } = ContainsOperator;

    /// <summary>
    /// The constant for the string.StartsWith() method name.
    /// </summary>
    private const string StartsWithMethodName = nameof(string.StartsWith);

    /// <summary>
    /// The constant for the starts with operator.
    /// </summary>
    public const string StartsWithOperator = "starts with";

    /// <summary>
    /// The constant for the object.ToString() method name.
    /// </summary>
    private const string ToStringMethodName = nameof(ToString);

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
        MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(ContainsMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var containsCall = Expression.Call(toStringCall, stringContainsMethodInfo, value);

        return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
    }

    /// <summary>
    /// The method returns a ends with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A ends with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToEndsWithExpression<T>()
    {
        MethodInfo stringEndsWithMethodInfo = typeof(string).GetMethod(EndsWithMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {EndsWithMethodName} method was not found.");
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var endsWithCall = Expression.Call(toStringCall, stringEndsWithMethodInfo, value);

        return Expression.Lambda<Func<T, bool>>(endsWithCall, parameter);
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
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var equalsOperator = Expression.Equal(toStringCall, value);

        return Expression.Lambda<Func<T, bool>>(equalsOperator, parameter);
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
            EndsWithOperator => ToEndsWithExpression<T>(),
            EqualsOperator => ToEqualsExpression<T>(),
            NotContainsOperator => ToNotContainsExpression<T>(),
            NotEqualsOperator => ToNotEqualsExpression<T>(),
            StartsWithOperator => ToStartsWithExpression<T>(),
            _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
        };
    }

    /// <summary>
    /// The method returns a not contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A not contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToNotContainsExpression<T>()
    {
        MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(ContainsMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var containsCall = Expression.Call(toStringCall, stringContainsMethodInfo, value);
        var notOperator = Expression.Not(containsCall);

        return Expression.Lambda<Func<T, bool>>(notOperator, parameter);
    }

    /// <summary>
    /// The method returns a not equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A not equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToNotEqualsExpression<T>()
    {
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var equalsOperator = Expression.Equal(toStringCall, value);
        var notOperator = Expression.Not(equalsOperator);

        return Expression.Lambda<Func<T, bool>>(notOperator, parameter);
    }

    /// <summary>
    /// The method returns a starts with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A starts with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStartsWithExpression<T>()
    {
        MethodInfo stringStartsWithMethodInfo = typeof(string).GetMethod(StartsWithMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {StartsWithMethodName} method was not found.");
        MethodInfo objectToStringMethodInfo = typeof(object).GetMethod(ToStringMethodName) ?? throw new MissingMethodException($"The {ToStringMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var toStringCall = Expression.Call(Expression.Convert(property, typeof(object)), objectToStringMethodInfo);
        var value = Expression.Constant(Value, typeof(string));
        var startsWithCall = Expression.Call(toStringCall, stringStartsWithMethodInfo, value);

        return Expression.Lambda<Func<T, bool>>(startsWithCall, parameter);
    }
}
