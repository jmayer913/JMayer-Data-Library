using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute will return a failure when one of the two properties is null.
/// </summary>
internal class CompareToOtherMemberNullLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan, allowNullCheck: true)]
    public int? Property1 { get; set; }

    public int? Property2 { get; set; }
}
