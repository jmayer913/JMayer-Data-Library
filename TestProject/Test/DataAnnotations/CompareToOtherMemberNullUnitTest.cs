using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations.CompareToOtherMembers.Null;

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
        CompareToOtherMemberEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;

        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return success when the two properties are both null.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberEqual dataObject = new()
        {
            Property1 = null,
            Property2 = null,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies a failure when the depends on member has a different type than the other property.
    /// </summary>
    [Fact]
    public void VerifyFailureTypeMismatch()
    {
        CompareToOtherMemberTypeMismatch dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.TypeMismatchErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return failure when the two properties are not null.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberNotEqual dataObject = new()
        {
            Property1 = null,
            Property2 = null,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberNotEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation return success when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberNotEqual dataObject = new()
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
        CompareToOtherMemberGreaterThan dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();
        
        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanOrEqualFailure()
    {
        CompareToOtherMemberGreaterThanOrEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyLessThanFailure()
    {
        CompareToOtherMemberLessThan dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOtherMember data annotation returns failure when one of the two properties is null.
    /// </summary>
    [Fact]
    public void VerifyLessThanOrEqualFailure()
    {
        CompareToOtherMemberLessThanOrEqual dataObject = new()
        {
            Property1 = null,
            Property2 = int.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = int.MaxValue;
        dataObject.Property2 = null;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }
}
