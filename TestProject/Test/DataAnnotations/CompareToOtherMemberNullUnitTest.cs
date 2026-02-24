using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the null values.
/// </summary>
public class CompareToOtherMemberNullUnitTest
{
    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return failure when the two properties do not both equal null.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberNullEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return success when the two properties are both null.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberNullEqual dataObject = new()
        {
            Property1 = null,
            Property2 = null,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return failure when the two properties are not null.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberNullNotEqual dataObject = new()
        {
            Property1 = null,
            Property2 = null,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullNotEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return success when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberNullNotEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;

        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanFailure()
    {
        CompareToOtherMemberNullGreaterThan dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThan.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();
        
        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanOrEqualFailure()
    {
        CompareToOtherMemberNullGreaterThanOrEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyLessThanFailure()
    {
        CompareToOtherMemberNullLessThan dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThan.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyLessThanOrEqualFailure()
    {
        CompareToOtherMemberNullLessThanOrEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNullLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }
}
