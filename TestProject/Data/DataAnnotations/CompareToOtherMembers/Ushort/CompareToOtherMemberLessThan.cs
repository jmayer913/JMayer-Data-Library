using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Ushort;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than property 2.
/// </summary>
internal class CompareToOtherMemberLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan)]
    public ushort Property1 { get; set; }

    public ushort Property2 { get; set; }
}
