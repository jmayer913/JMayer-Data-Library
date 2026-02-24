using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when there is a type mismatch.
/// </summary>
internal class CompareToOtherMemberTypeMismatch : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public bool Property1 { get; set; } = new();

    public int Property2 { get; set; } = new();
}
