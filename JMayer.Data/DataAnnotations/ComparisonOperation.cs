namespace JMayer.Data.DataAnnotations;

/// <summary>
/// The enumeration for the comparison operations.
/// </summary>
public enum ComparisonOperation
{
    /// <summary>
    /// The CompareToOtherMember attribute will do an equal check on the member its registered too against
    /// another member.
    /// </summary>
    Equal = 0,

    /// <summary>
    /// The CompareToOtherMember attribute will do a not equal check on the member its registered too against
    /// another member.
    /// </summary>
    NotEqual,

    /// <summary>
    /// The CompareToOtherMember attribute will do a greater than check on the member its registered too against
    /// another member.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The CompareToOtherMember attribute will do a greater than or equal check on the member its registered too against
    /// another member.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// The CompareToOtherMember attribute will do a less than check on the member its registered too against
    /// another member.
    /// </summary>
    LessThan,

    /// <summary>
    /// The CompareToOtherMember attribute will do a less than or equal check on the member its registered too against
    /// another member.
    /// </summary>
    LessThanOrEqual,
}
