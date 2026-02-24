using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace TestProject.Data.DataAnnotations;

/// <summary>
/// The class is used to test the CompareToOtherMember attribute when the member is not found.
/// </summary>
internal class CompareToOtherMemberNotFound : DataObject
{
    [CompareToOtherMember("MemberNotFound", ComparisonOperation.Equal)]
    public bool Property1 { get; set; }

    public bool Property2 { get; set; }
}
