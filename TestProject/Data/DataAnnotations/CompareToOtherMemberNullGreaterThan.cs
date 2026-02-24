using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test a false is returned when one of the properties null for the CompareToOtherMember attribute.
/// </summary>
internal class CompareToOtherMemberNullGreaterThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThan, allowNullCheck: true)]
    public int? Property1 { get; set; }

    public int? Property2 { get; set; }
}
