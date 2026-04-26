using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations.CompareToOtherMembers;

internal class CompareOtherMemberPassRegisteredMemberIfNull : DataObject
{
    [CompareToOtherMember(nameof(Property2), ComparisonOperation.Equal, passRegisteredMemberIfNull: true)]
    public string? Property1 { get; set; }

    public string? Property2 { get; set; }
}
