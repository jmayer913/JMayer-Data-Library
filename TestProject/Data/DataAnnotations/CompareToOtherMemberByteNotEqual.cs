using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when comparing two byte properties to be not equal.
/// </summary>
internal class CompareToOtherMemberByteNotEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.NotEqual)]
    public byte Property1 { get; set; }

    public byte Property2 { get; set; }
}
