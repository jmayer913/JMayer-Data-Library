using System.Reflection;

namespace JMayer.Data.DataAnnotations;

/// <summary>
/// The static class contains common contants and methods for member access.
/// </summary>
internal static class DataAnnotationMemberHelper
{
    /// <summary>
    /// The constant for an invalid type error message.
    /// </summary>
    public const string InvalidTypeErrorMessage = "Failed to vaidate because the type is not supported.";

    /// <summary>
    /// The constant for the member not found error message.
    /// </summary>
    public const string MemberNotFoundErrorMessage = "Failed to validate because the member name was not found.";

    /// <summary>
    /// The constant for the same member error message.
    /// </summary>
    public const string SameMemberErrorMessage = "Failed to validate because the other member name is the same as the member name the attribute's registered too.";

    /// <summary>
    /// The constant for the type mismatch error message.
    /// </summary>
    public const string TypeMismatchErrorMessage = "Failed to validate because there is a type mismatch between the values being compared.";

    /// <summary>
    /// The method returns the type from the member info.
    /// </summary>
    /// <param name="memberInfo">The information on the member.</param>
    /// <returns>The type or null if member is not a field or property.</returns>
    public static Type? GetMemberType(MemberInfo memberInfo)
    {
        if (memberInfo is FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType;
        }
        else if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType;
        }

        return null;
    }

    /// <summary>
    /// The method returns the value stored a member.
    /// </summary>
    /// <param name="memberInfo">Meta data about the member.</param>
    /// <param name="objectInstance">The instance of the object being validated.</param>
    /// <returns>The value or null if not found.</returns>
    public static object? GetMemberValue(MemberInfo memberInfo, object objectInstance)
    {
        if (memberInfo is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(objectInstance);
        }
        else if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(objectInstance);
        }

        return null;
    }

    /// <summary>
    /// The method returns if the members are the same type or not.
    /// </summary>
    /// <param name="memberInfo">The information on the member in the validation context.</param>
    /// <param name="otherMemberInfo">The information on the other member.</param>
    /// <returns>True means the types are the same.</returns>
    public static bool IsSameType(MemberInfo memberInfo, MemberInfo otherMemberInfo)
        => GetMemberType(memberInfo) == GetMemberType(otherMemberInfo);
}
