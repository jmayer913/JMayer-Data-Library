using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers;

internal class CompareOtherMemberFailOtherMemberIfNull : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal)]
    public string? Property1 { get; set; }

    public string? Property2 { get; set; }
}
