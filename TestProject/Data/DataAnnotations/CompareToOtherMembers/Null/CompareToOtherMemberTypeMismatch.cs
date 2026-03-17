using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers.Null;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when the type is invalid.
/// </summary>
internal class CompareToOtherMemberTypeMismatch : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal, allowNullCheck: true)]
    public string? Property1 { get; set; }

    public System.DateTime? Property2 { get; set; }
}
