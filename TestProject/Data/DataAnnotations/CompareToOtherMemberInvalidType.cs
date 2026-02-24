using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when the type is invalid.
/// </summary>
internal class CompareToOtherMemberInvalidType : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public object Property1 { get; set; } = new();

    public object Property2 { get; set; } = new();
}
