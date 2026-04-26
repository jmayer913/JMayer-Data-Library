using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Decimal;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than property 2.
/// </summary>
internal class CompareToOtherMemberLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan)]
    public decimal Property1 { get; set; }

    public decimal Property2 { get; set; }
}
