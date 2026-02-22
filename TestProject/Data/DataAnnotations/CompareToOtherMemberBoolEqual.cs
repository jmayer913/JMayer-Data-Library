using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two bool properties to be equal.
/// </summary>
internal class CompareToOtherMemberBoolEqual : DataObject
{
    /// <summary>
    /// The property gets/sets 
    /// </summary>
    [CompareToOtherMember(nameof(BoolProperty2), ComparisonOperation.Equal)]
    public bool BoolProperty1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool BoolProperty2 { get; set; }
}
