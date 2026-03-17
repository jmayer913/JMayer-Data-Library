using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Enum;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when the type is invalid.
/// </summary>
internal class CompareToOtherMemberTypeMismatch : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public TestEnum Property1 { get; set; } = new();

    public ConditionalEnum Property2 { get; set; } = new();
}
