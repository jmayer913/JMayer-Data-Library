using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two nullable properties and both need to be null.
/// </summary>
internal class CompareToOtherMemberNullEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal, allowNullCheck: true)]
    public int? Property1 { get; set; }

    public int? Property2 { get; set; }
}
