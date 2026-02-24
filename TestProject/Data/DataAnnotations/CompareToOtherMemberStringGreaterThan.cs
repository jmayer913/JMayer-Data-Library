using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

internal class CompareToOtherMemberStringGreaterThan : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThan)]
    public string? Property1 { get; set; }

    public string? Property2 { get; set; }
}
