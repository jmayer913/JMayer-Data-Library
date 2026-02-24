using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when the same member is used.
/// </summary>
internal class CompareToOtherMemberSameMember : DataObject
{
    [CompareToOtherMember(nameof(Property1), ComparisonOperation.Equal)]
    public bool Property1 { get; set; }

    public bool Property2 { get; set; }
}
