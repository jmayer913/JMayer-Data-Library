using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the greater than comparison operator is not suppored for the CompareToOtherMember attribute.
/// </summary>
public class CompareToOtherMemberBoolGreaterThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThan)]
    public bool Property1 { get; set; }

    public bool Property2 { get; set; }
}
