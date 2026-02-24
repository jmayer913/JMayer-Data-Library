using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

internal class CompareToOtherMemberStringGreaterThanOrEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.GreaterThanOrEqual)]
    public string? Property1 { get; set; }

    public string? Property2 { get; set; }
}
