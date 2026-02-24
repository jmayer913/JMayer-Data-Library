using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

internal class CompareToOtherMemberStringNotEqual : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.NotEqual)]
    public string? Property1 { get; set; }

    public string? Property2 { get; set; }
}
