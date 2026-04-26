using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Bool;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two bool properties to be equal.
/// </summary>
internal class CompareToOtherMemberEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public bool Property1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Property2 { get; set; }
}
