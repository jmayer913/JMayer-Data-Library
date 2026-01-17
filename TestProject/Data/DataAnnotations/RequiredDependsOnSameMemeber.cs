using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the RequiredDependsOn attribute when the same member is used.
/// </summary>
internal class RequiredDependsOnSameMemeber : DataObject
{
    /// <summary>
    /// The property gets/sets the string value evaluated by the RequiredDependsOn attribute.
    /// </summary>
    [RequiredDependsOn(dependentMemberName: nameof(ConditionalRequireValue), conditionValue: true)]
    public string ConditionalRequireValue { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the boolean value the ConditionalRequire property is dependent on.
    /// </summary>
    public bool DependentOnMember { get; set; }
}
