using System.Reflection;

namespace JMayer.Data.DataAnnotations;

/// <summary>
/// The static class contains common contants and methods for member access.
/// </summary>
internal class DataAnnotationMemberHelper
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
}
