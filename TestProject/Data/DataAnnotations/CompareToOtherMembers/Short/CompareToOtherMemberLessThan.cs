using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Short;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than property 2.
/// </summary>
internal class CompareToOtherMemberLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan)]
    public short Property1 { get; set; }

    public short Property2 { get; set; }
}
