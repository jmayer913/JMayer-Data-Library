using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Bool;

/// <summary>
/// The class is used to test the less than or equal comparison operator is not suppored for the CompareToOtherMember attribute.
/// </summary>
internal class CompareToOtherMemberLessThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThanOrEqual)]
    public bool Property1 { get; set; }

    public bool Property2 { get; set; }
}
