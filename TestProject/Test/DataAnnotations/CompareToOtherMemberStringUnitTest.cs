using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the string data type.
/// </summary>
public class CompareToOtherMemberStringUnitTest
{
    /// <summary>
    /// The constant for a string.
    /// </summary>
    private const string AString = "A String";

    /// <summary>
    /// The constant for a different string.
    /// </summary>
    private const string DifferentString = "Different String";

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberStringEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = DifferentString;
        dataObject.Property2 = AString;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberStringEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = AString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = DifferentString;
        dataObject.Property2 = DifferentString;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberStringNotEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = AString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = DifferentString;
        dataObject.Property2 = DifferentString;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberStringEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties don't equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberStringNotEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = DifferentString;
        dataObject.Property2 = AString;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanNotSupported()
    {
        CompareToOtherMemberStringGreaterThan dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForStringErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanOrEqualNotSupported()
    {
        CompareToOtherMemberStringGreaterThanOrEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForStringErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanNotSupported()
    {
        CompareToOtherMemberStringLessThan dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForStringErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanOrEqualNotSupported()
    {
        CompareToOtherMemberStringLessThanOrEqual dataObject = new()
        {
            Property1 = AString,
            Property2 = DifferentString,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForStringErrorMessage, validationResults[0].ErrorMessage);
    }
}
