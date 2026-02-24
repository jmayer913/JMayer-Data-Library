using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the bool data type.
/// </summary>
public class CompareToOtherMemberBoolUnitTest
{
    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two bool properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberBoolEqual dataObject = new()
        {
            Property1 = true,
            Property2 = false,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = false;
        dataObject.Property2 = true;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two bool properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberBoolEqual dataObject = new()
        {
            Property1 = true,
            Property2 = true,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = false;
        dataObject.Property2 = false;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two bool properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberBoolNotEqual dataObject = new()
        {
            Property1 = true,
            Property2 = true,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = false;
        dataObject.Property2 = false;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two bool properties don't equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberBoolNotEqual dataObject = new()
        {
            Property1 = true,
            Property2 = false,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = false;
        dataObject.Property2 = true;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanNotSupported()
    {
        CompareToOtherMemberBoolGreaterThan dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForBoolErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is greater than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureGreaterThanOrEqualNotSupported()
    {
        CompareToOtherMemberBoolGreaterThanOrEqual dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForBoolErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanNotSupported()
    {
        CompareToOtherMemberBoolLessThan dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForBoolErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the the comparison operation is less than or equal.
    /// </summary>
    [Fact]
    public void VerifyFailureLessThanOrEqualNotSupported()
    {
        CompareToOtherMemberBoolLessThanOrEqual dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(CompareToOtherMemberAttribute.InvalidComparisonForBoolErrorMessage, validationResults[0].ErrorMessage);
    }
}
