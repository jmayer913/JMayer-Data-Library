using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.TimeOnly;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than or equal to property 2.
/// </summary>
internal class CompareToOtherMemberLessThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThanOrEqual)]
    public System.TimeOnly Property1 { get; set; }

    public System.TimeOnly Property2 { get; set; }
}
