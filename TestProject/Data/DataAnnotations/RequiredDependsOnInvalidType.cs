using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the RequiredDependsOn attribute when the types are invalid.
/// </summary>
internal class RequiredDependsOnInvalidType : DataObject
{
    /// <summary>
    /// The property gets/sets the string value evaluated by the RequiredDependsOn attribute.
    /// </summary>
    [RequiredDependsOn(dependentMemberName: nameof(DependentOnMember), conditionValue: 0)]
    public int ConditionalRequireValue { get; set; }

    /// <summary>
    /// The property gets/sets the boolean value the ConditionalRequire property is dependent on.
    /// </summary>
    public int DependentOnMember { get; set; }
}
