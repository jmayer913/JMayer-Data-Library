using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two bool properties to be not equal.
/// </summary>
internal class CompareToOtherMemberBoolNotEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.NotEqual)]
    public bool Property1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Property2 { get; set; }
}
