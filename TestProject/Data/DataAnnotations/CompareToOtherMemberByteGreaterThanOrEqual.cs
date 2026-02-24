using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be greater than or equal to property 2.
/// </summary>
internal class CompareToOtherMemberByteGreaterThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThanOrEqual)]
    public byte Property1 { get; set; }

    public byte Property2 { get; set; }
}
