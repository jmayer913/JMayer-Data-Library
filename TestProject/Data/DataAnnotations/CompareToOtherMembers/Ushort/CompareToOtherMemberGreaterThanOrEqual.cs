using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Ushort;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be greater than or equal to property 2.
/// </summary>
internal class CompareToOtherMemberGreaterThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThanOrEqual)]
    public ushort Property1 { get; set; }

    public ushort Property2 { get; set; }
}
