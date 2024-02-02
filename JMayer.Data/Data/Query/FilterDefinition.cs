using System.Linq.Expressions;
using System.Reflection;

#warning Mudblazor has multiple filter operators; I need to support all.

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
    /// The constant for the string.IsNullOrEmpty() method name.
    /// </summary>
    public const string IsNullOrEmptyMethodName = nameof(string.IsNullOrEmpty);

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
    /// The method returns the type for the property or field.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldName">The name of the property or field to search for.</param>
    /// <returns>The Type object.</returns>
    /// <exception cref="MissingMemberException">Throw if the property or field is not found.</exception>
    private static Type GetPropertyOrFieldType<T>(string propertyOrFieldName)
    {
        PropertyInfo? propertyInfo = typeof(T).GetProperty(propertyOrFieldName, BindingFlags.Public | BindingFlags.GetProperty);

        if (propertyInfo != null)
        {
            return propertyInfo.PropertyType;
        }

        FieldInfo? fieldInfo = typeof(T).GetField(propertyOrFieldName, BindingFlags.Public | BindingFlags.GetField);

        if (fieldInfo != null)
        {
            return fieldInfo.FieldType;
        }

        throw new MissingMemberException($"The {propertyOrFieldName} property or field was not in the {typeof(T).Name} type.");
    }

    /// <summary>
    /// The method returns a contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToContainsExpression<T>()
    {
        MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(ContainsMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var containsCall = Expression.Call(property, stringContainsMethodInfo, value);
        var andOperator = Expression.AndAlso(notNullCheck, containsCall);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
    }

    /// <summary>
    /// The method returns a ends with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A ends with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToEndsWithExpression<T>()
    {
        MethodInfo stringEndsWithMethodInfo = typeof(string).GetMethod(EndsWithMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {EndsWithMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var endsWithCall = Expression.Call(property, stringEndsWithMethodInfo, value);
        var andOperator = Expression.AndAlso(notNullCheck, endsWithCall);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
    }

    /// <summary>
    /// The method returns an equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>An equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToEqualsExpression<T>()
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var equalsOperator = Expression.Equal(property, value);
        var andOperator = Expression.AndAlso(notNullCheck, equalsOperator);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
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
            IsEmptyOperator => ToIsEmptyExpression<T>(),
            IsNotEmptyOperator => ToIsNotEmptyExpression<T>(),
            NotContainsOperator => ToNotContainsExpression<T>(),
            NotEqualsOperator => ToNotEqualsExpression<T>(),
            StartsWithOperator => ToStartsWithExpression<T>(),
            _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
        };
    }

    /// <summary>
    /// The method returns an is empty expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>An is empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToIsEmptyExpression<T>()
    {
        MethodInfo stringIsNullOrEmptyMethodInfo = typeof(string).GetMethod(IsNullOrEmptyMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var isNullOrEmptyCall = Expression.Call(stringIsNullOrEmptyMethodInfo, property);
            
        return Expression.Lambda<Func<T, bool>>(isNullOrEmptyCall, parameter);
    }

    /// <summary>
    /// The method returns an is not empty expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>An is not empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToIsNotEmptyExpression<T>()
    {
        MethodInfo stringIsNullOrEmptyMethodInfo = typeof(string).GetMethod(IsNullOrEmptyMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var isNullOrEmptyCall = Expression.Call(stringIsNullOrEmptyMethodInfo, property);
        var notOperator = Expression.Not(isNullOrEmptyCall);

        return Expression.Lambda<Func<T, bool>>(notOperator, parameter);
    }

    /// <summary>
    /// The method returns a not contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A not contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToNotContainsExpression<T>()
    {
        MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(ContainsMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var containsCall = Expression.Call(property, stringContainsMethodInfo, value);
        var notOperator = Expression.Not(containsCall);
        var andOperator = Expression.AndAlso(notNullCheck, notOperator);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
    }

    /// <summary>
    /// The method returns a not equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A not equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToNotEqualsExpression<T>()
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var equalsOperator = Expression.Equal(property, value);
        var notOperator = Expression.Not(equalsOperator);
        var andOperator = Expression.AndAlso(notNullCheck, notOperator);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
    }

    /// <summary>
    /// The method returns a starts with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A starts with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStartsWithExpression<T>()
    {
        MethodInfo stringStartsWithMethodInfo = typeof(string).GetMethod(StartsWithMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {StartsWithMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var value = Expression.Constant(Value, typeof(string));
        var startsWithCall = Expression.Call(property, stringStartsWithMethodInfo, value);
        var andOperator = Expression.AndAlso(notNullCheck, startsWithCall);

        return Expression.Lambda<Func<T, bool>>(andOperator, parameter);
    }
}
