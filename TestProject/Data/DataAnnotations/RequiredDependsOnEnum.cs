using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the RequiredDependsOn attribute when the condition is for a enum value.
/// </summary>
internal class RequiredDependsOnEnum : DataObject
{
    /// <summary>
    /// The property gets/sets the string value evaluated by the RequiredDependsOn attribute.
    /// </summary>
    [RequiredDependsOn(dependentMemberName: nameof(DependentOnMember), conditionValue: ConditionalEnum.Condition2)]
    public string ConditionalRequireValue { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the enum value the ConditionalRequire property is dependent on.
    /// </summary>
    public ConditionalEnum DependentOnMember { get; set; }
}
