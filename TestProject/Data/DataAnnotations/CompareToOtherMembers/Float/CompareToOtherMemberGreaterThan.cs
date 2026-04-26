using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Float;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be greater than property 2.
/// </summary>
internal class CompareToOtherMemberGreaterThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThan)]
    public float Property1 { get; set; }

    public float Property2 { get; set; }
}
