using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Float;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two byte properties to be not equal.
/// </summary>
internal class CompareToOtherMemberNotEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.NotEqual)]
    public float Property1 { get; set; }

    public float Property2 { get; set; }
}
