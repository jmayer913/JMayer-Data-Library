using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the RequiredDependsOn attribute when there is a type mismatch between the conditionValue and dependent on member.
/// </summary>
internal class RequiredDependsOnTypeMismatch : DataObject
{
    /// <summary>
    /// The property gets/sets the string value evaluated by the RequiredDependsOn attribute.
    /// </summary>
    [RequiredDependsOn(dependentMemberName: nameof(DependentOnMember), conditionValue: ConditionalEnum.Condition1)]
    public string ConditionalRequireValue { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the boolean value the ConditionalRequire property is dependent on.
    /// </summary>
    public bool DependentOnMember { get; set; }
}
