using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations.CompareToOtherMembers.Char;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the char data type.
/// </summary>
public class CompareToOtherMemberCharUnitTest
{
    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = char.MaxValue;
        dataObject.Property2 = char.MinValue;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = char.MaxValue;
        dataObject.Property2 = char.MaxValue;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberNotEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = char.MaxValue;
        dataObject.Property2 = char.MaxValue;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties don't equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberNotEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = char.MaxValue;
        dataObject.Property2 = char.MinValue;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanNotSupported()
    {
        CompareToOtherMemberGreaterThan dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForCharErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanOrEqualNotSupported()
    {
        CompareToOtherMemberGreaterThanOrEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForCharErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanNotSupported()
    {
        CompareToOtherMemberLessThan dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForCharErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanOrEqualNotSupported()
    {
        CompareToOtherMemberLessThanOrEqual dataObject = new()
        {
            Property1 = char.MinValue,
            Property2 = char.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForCharErrorMessage, validationResults[0].ErrorMessage);
    }
}
