using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations.CompareToOtherMembers.Decimal;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the decimal data type.
/// </summary>
public class CompareToOtherMemberDecimalUnitTest
{
    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberEqual dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = decimal.MinValue;
        dataObject.Property2 = decimal.MaxValue;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
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
            Property1 = decimal.MaxValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = decimal.MinValue;
        dataObject.Property2 = decimal.MinValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is less than property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanFailure()
    {
        CompareToOtherMemberGreaterThan dataObject = new()
        {
            Property1 = decimal.MinValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the prperty 1 is greater than property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanSuccess()
    {
        CompareToOtherMemberGreaterThan dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is less than property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanOrEqualFailure()
    {
        CompareToOtherMemberGreaterThanOrEqual dataObject = new()
        {
            Property1 = decimal.MinValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is greater than or equal property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanOrEqualSuccess()
    {
        CompareToOtherMemberGreaterThanOrEqual dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = decimal.MaxValue;
        dataObject.Property2 = decimal.MaxValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is greater than property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanFailure()
    {
        CompareToOtherMemberLessThan dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is less than property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanSuccess()
    {
        CompareToOtherMemberLessThan dataObject = new()
        {
            Property1 = decimal.MinValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is greater than property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanOrEqualFailure()
    {
        CompareToOtherMemberLessThanOrEqual dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is less than or equal property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanOrEqualSuccess()
    {
        CompareToOtherMemberLessThanOrEqual dataObject = new()
        {
            Property1 = decimal.MinValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = decimal.MaxValue;
        dataObject.Property2 = decimal.MaxValue;
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
            Property1 = decimal.MaxValue,
            Property2 = decimal.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = decimal.MinValue;
        dataObject.Property2 = decimal.MinValue;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberNotEqual dataObject = new()
        {
            Property1 = decimal.MaxValue,
            Property2 = decimal.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = decimal.MinValue;
        dataObject.Property2 = decimal.MaxValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }
}
