using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Sbyte;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than or equal to property 2.
/// </summary>
internal class CompareToOtherMemberLessThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThanOrEqual)]
    public sbyte Property1 { get; set; }

    public sbyte Property2 { get; set; }
}
