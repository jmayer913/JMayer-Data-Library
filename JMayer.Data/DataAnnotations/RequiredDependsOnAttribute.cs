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
/// public class ObjectRequiringValidation
/// {
///     //A required validation check will be done if DependentBoolMember is true.
///     [RequiredDependsOn(dependentMemberName: nameof(DependentBoolMember), conditionValue: true)]
///     public string ConditionalRequiredProperty { get; set; }
///     
///     public bool DependentBoolMember { get; set; }
/// }
/// </code>
/// or
/// <code>
/// public class ObjectRequiringValidation
/// {
///     //A required validation check will be done if DependentEnumMember is YourEnum.RequiredValue.
///     [RequiredDependsOn(dependentMemberName: nameof(DependentEnumMember), conditionValue: YourEnum.RequiredValue)]
///     public string ConditionalRequiredProperty { get; set; }
///     
///     public YourEnum DependentEnumMember { get; set; }
/// }
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
            return new ValidationResult(DataAnnotationMemberHelper.SameMemberErrorMessage);
        }

        MemberInfo? dependentMemberInfo = validationContext.ObjectType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, Type.FilterName, _dependentMemberName).FirstOrDefault();

        if (dependentMemberInfo is null)
        {
            return new ValidationResult(DataAnnotationMemberHelper.MemberNotFoundErrorMessage);
        }

        object? dependentMemberValue = DataAnnotationMemberHelper.GetMemberValue(dependentMemberInfo, validationContext.ObjectInstance);

        if (dependentMemberValue is null)
        {
            return ValidationResult.Success;
        }

        if (dependentMemberValue.GetType() != _conditionValue.GetType())
        {
            return new ValidationResult(DataAnnotationMemberHelper.TypeMismatchErrorMessage);
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
        else
        {
            return new ValidationResult(DataAnnotationMemberHelper.InvalidTypeErrorMessage);
        }

        return ValidationResult.Success;
    }
}
