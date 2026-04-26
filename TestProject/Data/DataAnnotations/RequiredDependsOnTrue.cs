using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the RequiredDependsOn attribute when the condition is true.
/// </summary>
internal class RequiredDependsOnTrue : DataObject
{
    /// <summary>
    /// The property gets/sets the string value evaluated by the RequiredDependsOn attribute.
    /// </summary>
    [RequiredDependsOn(dependentMemberName: nameof(DependentOnMember), conditionValue: true)]
    public string ConditionalRequireValue { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the boolean value the ConditionalRequire property is dependent on.
    /// </summary>
    public bool DependentOnMember { get; set; }
}
