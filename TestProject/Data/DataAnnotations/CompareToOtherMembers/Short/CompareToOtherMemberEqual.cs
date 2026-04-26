using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Short;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two byte properties to be equal.
/// </summary>
internal class CompareToOtherMemberEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public short Property1 { get; set; }

    public short Property2 { get; set; }
}
