using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JMayer.Data.DataAnnotations;

/// <summary>
/// The class does the required data annotation check if it matches a specific condition on another field or property in the same instance being validated. 
/// The condition can be a boolean or an enum value. 
/// </summary>
/// <remarks>
/// To use, add the attribute to the public field or property which is required on a condition. Set the dependentMemberName to the name of the public field or property
/// which has the condition that needs to be evaluated. Set the conditionValue to true, false or an enum value based on what condition needs to occur for the required
/// data annotation to be evaluated.
/// <code>
/// [RequiredDependsOn(dependentMemberName: nameof(DependentBoolMember), conditionValue: true)]
/// public string ConditionalRequiredProperty { get; set; }
/// 
/// public bool DependentBoolMember { get; set; }
/// </code>
/// or
/// <code>
/// [RequiredDependsOn(dependentMemberName: nameof(DependentEnumMember), conditionValue: YourEnum.RequiredValue)]
/// public string ConditionalRequiredProperty { get; set; }
/// 
/// public YourEnum DependentEnumMember { get; set; }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RequiredDependsOnAttribute : RequiredAttribute
{
    /// <summary>
    /// The value to run the Required data annotation.
    /// </summary>
    private readonly object _conditionValue;

    /// <summary>
    /// The name of the member the Required data annotation is dependent on.
    /// </summary>
    private readonly string _dependentMemberName;

    /// <summary>
    /// The constant for the same member error message.
    /// </summary>
    internal const string SameMemberErrorMessage = "Failed to validate because the dependent member name is the same as the attribute's member name.";

    /// <summary>
    /// The constant for the member not found error message.
    /// </summary>
    internal const string MemberNotFoundErrorMessage = "Failled to validate because the dependent member name was not found.";

    /// <summary>
    /// The constant for the type mismatch error message.
    /// </summary>
    internal const string TypeMismatchErrorMessage = "Failed to validate because the dependent member value type does not match the required value type.";

    /// <summary>
    /// The constructor which takes the dependency and condition.
    /// </summary>
    /// <param name="dependentMemberName">The name of the member the Required data annotation is dependent on.</param>
    /// <param name="conditionValue">The value to run the Required data annotation.</param>
    /// <exception cref="ArgumentException">Thrown when the dependentMemberName parameter is null or an empty string.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the conditionValue parameter is null.</exception>
    public RequiredDependsOnAttribute(string dependentMemberName, object conditionValue)
    {
        ArgumentNullException.ThrowIfNull(conditionValue);
        ArgumentException.ThrowIfNullOrEmpty(dependentMemberName);

        _conditionValue = conditionValue;
        _dependentMemberName = dependentMemberName;
    }

    /// <summary>
    /// The method returns the value stored by the dependent member.
    /// </summary>
    /// <param name="dependentMemberInfo">Meta data about the depedent member.</param>
    /// <param name="objectInstance">The instance of the object being validated.</param>
    /// <returns>The value or null if not found.</returns>
    private static object? GetDependentMemberValue(MemberInfo dependentMemberInfo, object objectInstance)
    {
        if (dependentMemberInfo is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(objectInstance);
        }
        else if (dependentMemberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(objectInstance);
        }

        return null;
    }

    /// <summary>
    /// The method returns if the dependent member matches the required condition.
    /// </summary>
    /// <param name="dependentMemberValue">The bool value of the dependent member.</param>
    /// <param name="conditionValue">The bool value we are expecting for the dependent member.</param>
    /// <returns>True means the Required data annotation check occurs.</returns>
    private static bool IsRequiredCondition(bool dependentMemberValue, bool conditionValue) => dependentMemberValue == conditionValue;

    /// <summary>
    /// The method returns if the dependent member matches the required condition.
    /// </summary>
    /// <param name="dependentMemberValue">The bool value of the dependent member.</param>
    /// <param name="conditionValue">The bool value we are expecting for the dependent member.</param>
    /// <returns>True means the Required data annotation check occurs.</returns>
    private static bool IsRequiredCondition(Enum dependentMemberValue, Enum conditionValue) => dependentMemberValue.Equals(conditionValue);

    /// <inheritdoc/>
    /// <remarks>Overridden to run the required data annotation if it matches the dependent condition. Success will be return when it doesn't match the dependent condition.</remarks>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (_dependentMemberName == validationContext.MemberName)
        {
            return new ValidationResult(SameMemberErrorMessage);
        }

        MemberInfo? dependentMemberInfo = validationContext.ObjectType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, Type.FilterName, _dependentMemberName).FirstOrDefault();

        if (dependentMemberInfo is null)
        {
            return new ValidationResult(MemberNotFoundErrorMessage);
        }

        object? dependentMemberValue = GetDependentMemberValue(dependentMemberInfo, validationContext.ObjectInstance);

        if (dependentMemberValue is null)
        {
            return ValidationResult.Success;
        }

        if (dependentMemberValue.GetType() != _conditionValue.GetType())
        {
            return new ValidationResult(TypeMismatchErrorMessage);
        }

        if (dependentMemberValue is bool dependentMemberBoolValue && _conditionValue is bool conditionBoolValue)
        {
            if (IsRequiredCondition(dependentMemberBoolValue, conditionBoolValue))
            {
                return base.IsValid(value, validationContext);
            }
        }
        else if (dependentMemberValue is Enum dependentMemberEnumValue && _conditionValue is Enum conditionEnumValue)
        {
            if (IsRequiredCondition(dependentMemberEnumValue, conditionEnumValue))
            {
                return base.IsValid(value, validationContext);
            }
        }

        return ValidationResult.Success;
    }
}
