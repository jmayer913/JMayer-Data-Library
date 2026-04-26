using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Bool;

/// <summary>
/// The class is used to test the less than comparison operator is not suppored for the CompareToOtherMember attribute.
/// </summary>
internal class CompareToOtherMemberLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan)]
    public bool Property1 { get; set; }

    public bool Property2 { get; set; }
}
