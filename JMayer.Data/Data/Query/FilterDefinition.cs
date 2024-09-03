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
    /// The constant for the string.EndsWith() method name.
    /// </summary>
    public const string EndsWithMethodName = nameof(string.EndsWith);

    /// <summary>
    /// The constant for the numeric equals operator.
    /// </summary>
    public const string EqualsOperator = "=";

    /// <summary>
    /// The property gets/sets the public property/field to filter on.
    /// </summary>
    public string FilterOn { get; set; } = string.Empty;

    /// <summary>
    /// The constant for the numeric greater than operator.
    /// </summary>
    public const string GreaterThanOperator = ">";

    /// <summary>
    /// The constant for the numeric greater than or equals operator.
    /// </summary>
    public const string GreaterThanOrEqualsOperator = ">=";

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
    /// The constant for the numeric less than operator.
    /// </summary>
    public const string LessThanOperator = "<";

    /// <summary>
    /// The constant for the numeric less than or equals operator.
    /// </summary>
    public const string LessThanOrEqualsOperator = "<=";

    /// <summary>
    /// The constant for the numeric not equals operator.
    /// </summary>
    public const string NotEqualsOperator = "!=";

    /// <summary>
    /// The property gets/sets the operator to use when filtering.
    /// </summary>
    public string Operator { get; set; } = StringContainsOperator;

    /// <summary>
    /// The constant for the string.StartsWith() method name.
    /// </summary>
    private const string StartsWithMethodName = nameof(string.StartsWith);

    /// <summary>
    /// The constant for the string contains operator.
    /// </summary>
    public const string StringContainsOperator = "contains";

    /// <summary>
    /// The constant for the string ends with operator.
    /// </summary>
    public const string StringEndsWithOperator = "ends with";

    /// <summary>
    /// The constant for the string equals operator.
    /// </summary>
    public const string StringEqualsOperator = "equals";

    /// <summary>
    /// The constant for the string not contains operator.
    /// </summary>
    public const string StringNotContainsOperator = "not contains";

    /// <summary>
    /// The constant for the not equals operator.
    /// </summary>
    public const string StringNotEqualsOperator = "not equals";

    /// <summary>
    /// The constant for the string starts with operator.
    /// </summary>
    public const string StringStartsWithOperator = "starts with";

    /// <summary>
    /// The constant for the object.ToString() method name.
    /// </summary>
    private const string ToStringMethodName = nameof(ToString);

    /// <summary>
    /// The property gets/sets the value to filter against.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The method returns a default value for a type.
    /// </summary>
    /// <param name="propertyOrFieldType">The type for the property or field.</param>
    /// <returns>The value as a string.</returns>
    private static string GetDefaultValue(Type propertyOrFieldType)
    {
        if (propertyOrFieldType == typeof(bool))
        {
            return false.ToString();
        }
        else if (propertyOrFieldType == typeof(DateTime))
        {
            return DateTime.MinValue.ToString();
        }
        else if (propertyOrFieldType == typeof(TimeSpan))
        {
            return TimeSpan.MinValue.ToString();
        }
        else
        {
            return 0.ToString();
        }
    }

    /// <summary>
    /// The method returns the type for the property or field.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>The Type object.</returns>
    /// <exception cref="MissingMemberException">Throw if the property or field is not found.</exception>
    private Type GetPropertyOrFieldType<T>()
    {
        Type type = typeof(T);
        PropertyInfo? propertyInfo = type.GetProperty(FilterOn);

        if (propertyInfo != null)
        {
            return propertyInfo.PropertyType;
        }

        FieldInfo? fieldInfo = type.GetField(FilterOn);

        if (fieldInfo != null)
        {
            return fieldInfo.FieldType;
        }

        throw new MissingMemberException($"The {FilterOn} property or field was not in the {type.Name} type.");
    }

    /// <summary>
    /// The method returns a equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToEqualsExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var equalsOperator = Expression.Equal(property, value);

        return Expression.Lambda<Func<T, bool>>(equalsOperator, parameter);
    }

    /// <summary>
    /// The method returns an expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>The expression to be used for filtering.</returns>
    /// <exception cref="NotImplementedException">Thrown if the type or operator is not handled.</exception>
    public Expression<Func<T, bool>> ToExpression<T>()
    {
        Type propertyOrFieldType = GetPropertyOrFieldType<T>();

        if (propertyOrFieldType == typeof(string))
        {
            return Operator.ToLower() switch
            {
                StringContainsOperator => ToStringContainsExpression<T>(),
                StringEndsWithOperator => ToStringEndsWithExpression<T>(),
                StringEqualsOperator => ToStringEqualsExpression<T>(),
                IsEmptyOperator => ToStringIsEmptyExpression<T>(),
                IsNotEmptyOperator => ToStringIsNotEmptyExpression<T>(),
                StringNotContainsOperator => ToStringNotContainsExpression<T>(),
                StringNotEqualsOperator => ToStringNotEqualsExpression<T>(),
                StringStartsWithOperator => ToStringStartsWithExpression<T>(),
                _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
            };
        }
        else
        {
            return Operator.ToLower() switch
            {
                EqualsOperator => ToEqualsExpression<T>(propertyOrFieldType),
                GreaterThanOperator => ToGreaterThanExpression<T>(propertyOrFieldType),
                GreaterThanOrEqualsOperator => ToGreaterThanOrEqualsExpression<T>(propertyOrFieldType),
                IsEmptyOperator => ToIsEmptyExpression<T>(propertyOrFieldType),
                IsNotEmptyOperator => ToIsNotEmptyExpression<T>(propertyOrFieldType),
                LessThanOperator => ToLessThanExpression<T>(propertyOrFieldType),
                LessThanOrEqualsOperator => ToLessThanOrEqualsExpression<T>(propertyOrFieldType),
                NotEqualsOperator => ToNotEqualsExpression<T>(propertyOrFieldType),
                _ => throw new NotImplementedException($"The {Operator} operator is not handled.")
            };
        }
    }

    /// <summary>
    /// The method returns a greater than expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A greater than expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToGreaterThanExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var greaterThanOperator = Expression.GreaterThan(property, value);

        return Expression.Lambda<Func<T, bool>>(greaterThanOperator, parameter);
    }

    /// <summary>
    /// The method returns a greater than or equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A greater than or equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToGreaterThanOrEqualsExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var greaterThanOrEqualsOperator = Expression.GreaterThanOrEqual(property, value);

        return Expression.Lambda<Func<T, bool>>(greaterThanOrEqualsOperator, parameter);
    }

    /// <summary>
    /// The method returns an is emtpy expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>An is empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToIsEmptyExpression<T>(Type propertyOrFieldType)
    {
        //Use null as empty if the type is nullable.
        if (Nullable.GetUnderlyingType(propertyOrFieldType) != null)
        {
            var parameter = Expression.Parameter(typeof(T), "obj");
            var property = Expression.PropertyOrField(parameter, FilterOn);
            var value = Expression.Constant(null, typeof(object));
            var equalOperator = Expression.Equal(property, value);

            return Expression.Lambda<Func<T, bool>>(equalOperator, parameter);
        }
        else
        {
            //For non-nullable types, use a default value to represent emtpy.
            if (!string.IsNullOrEmpty(Value))
            {
                Value = GetDefaultValue(propertyOrFieldType);
            }

            var parameter = Expression.Parameter(typeof(T), "obj");
            var property = Expression.PropertyOrField(parameter, FilterOn);
            var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
            var equalOperator = Expression.Equal(property, value);

            return Expression.Lambda<Func<T, bool>>(equalOperator, parameter);
        }
    }

    /// <summary>
    /// The method returns an is not emtpy expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>An is not empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToIsNotEmptyExpression<T>(Type propertyOrFieldType)
    {
        //Use null as empty if the type is nullable.
        if (Nullable.GetUnderlyingType(propertyOrFieldType) != null)
        {
            var parameter = Expression.Parameter(typeof(T), "obj");
            var property = Expression.PropertyOrField(parameter, FilterOn);
            var value = Expression.Constant(null, typeof(object));
            var notEqualOperator = Expression.NotEqual(property, value);

            return Expression.Lambda<Func<T, bool>>(notEqualOperator, parameter);
        }
        else
        {
            //For non-nullable types, use a default value to represent emtpy.
            if (!string.IsNullOrEmpty(Value))
            {
                Value = GetDefaultValue(propertyOrFieldType);
            }

            var parameter = Expression.Parameter(typeof(T), "obj");
            var property = Expression.PropertyOrField(parameter, FilterOn);
            var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
            var notEqualOperator = Expression.NotEqual(property, value);

            return Expression.Lambda<Func<T, bool>>(notEqualOperator, parameter);
        }
    }

    /// <summary>
    /// The method returns a less than expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A less than expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToLessThanExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var lessThanOperator = Expression.LessThan(property, value);

        return Expression.Lambda<Func<T, bool>>(lessThanOperator, parameter);
    }

    /// <summary>
    /// The method returns a less than or equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A less than or equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToLessThanOrEqualsExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var lessThanOrEqualsOperator = Expression.LessThanOrEqual(property, value);

        return Expression.Lambda<Func<T, bool>>(lessThanOrEqualsOperator, parameter);
    }

    /// <summary>
    /// The method returns a equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <param name="propertyOrFieldType">The type for the property or field; needed to convert the string value.</param>
    /// <returns>A equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToNotEqualsExpression<T>(Type propertyOrFieldType)
    {
        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var value = Expression.Constant(Convert.ChangeType(Value, propertyOrFieldType), propertyOrFieldType);
        var notEqualsOperator = Expression.NotEqual(property, value);

        return Expression.Lambda<Func<T, bool>>(notEqualsOperator, parameter);
    }

    /// <summary>
    /// The method returns a string contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringContainsExpression<T>()
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
    /// The method returns a string ends with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string ends with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringEndsWithExpression<T>()
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
    /// The method returns a string equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringEqualsExpression<T>()
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
    /// The method returns a string is empty expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string is empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringIsEmptyExpression<T>()
    {
        MethodInfo stringIsNullOrEmptyMethodInfo = typeof(string).GetMethod(IsNullOrEmptyMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var isNullOrEmptyCall = Expression.Call(stringIsNullOrEmptyMethodInfo, property);
            
        return Expression.Lambda<Func<T, bool>>(isNullOrEmptyCall, parameter);
    }

    /// <summary>
    /// The method returns a string is not empty expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string is not empty expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringIsNotEmptyExpression<T>()
    {
        MethodInfo stringIsNullOrEmptyMethodInfo = typeof(string).GetMethod(IsNullOrEmptyMethodName, [typeof(string)]) ?? throw new MissingMethodException($"The {ContainsMethodName} method was not found.");

        var parameter = Expression.Parameter(typeof(T), "obj");
        var property = Expression.PropertyOrField(parameter, FilterOn);
        var isNullOrEmptyCall = Expression.Call(stringIsNullOrEmptyMethodInfo, property);
        var notOperator = Expression.Not(isNullOrEmptyCall);

        return Expression.Lambda<Func<T, bool>>(notOperator, parameter);
    }

    /// <summary>
    /// The method returns a string not contains expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string not contains expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringNotContainsExpression<T>()
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
    /// The method returns a string not equals expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string not equals expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringNotEqualsExpression<T>()
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
    /// The method returns a string starts with expression to be used for filtering.
    /// </summary>
    /// <typeparam name="T">Can be any object.</typeparam>
    /// <returns>A string starts with expression to be used for filtering.</returns>
    private Expression<Func<T, bool>> ToStringStartsWithExpression<T>()
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