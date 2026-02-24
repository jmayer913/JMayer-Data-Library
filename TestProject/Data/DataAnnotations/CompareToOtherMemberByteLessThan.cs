using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing property 1 to be less than property 2.
/// </summary>
internal class CompareToOtherMemberByteLessThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.LessThan)]
    public byte Property1 { get; set; }

    public byte Property2 { get; set; }
}
