using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

#warning I'm only testing properties. I should also test fields.
#warning I need to decide if the errors on evaluation should return the member name or not.

/// <summary>
/// The class manages tests for the RequiredDependsOnAttribute object.
/// </summary>
public class RequiredDependsOnUnitTest
{
    /// <summary>
    /// The method verifies an argument null exception is thrown when null is passed into the constructor for the conditionValue parameter.
    /// </summary>
    [Fact]
    public void VerifyConstructorThrowsArgumentNullExceptionForConditionValueParameter() => Assert.Throws<ArgumentNullException>(() => new RequiredDependsOnAttribute("A Member", null));

    /// <summary>
    /// The method verifies an argument exception is thrown when an empty string is passed into the constructor for the dependentMemberName parameter.
    /// </summary>
    [Fact]
    public void VerifyConstructorThrowsArgumentExceptionForDependentMemberNameParameter() => Assert.Throws<ArgumentException>(() => new RequiredDependsOnAttribute(string.Empty, true));

    /// <summary>
    /// The method verifies an argument null exception is thrown when null is passed into the constructor for the dependentMemberName parameter.
    /// </summary>
    [Fact]
    public void VerifyConstructorThrowsArgumentNullExceptionForDependentMemberNameParameter() => Assert.Throws<ArgumentNullException>(() => new RequiredDependsOnAttribute(null, true));

    /// <summary>
    /// The method verifies a failure when the types are invalid.
    /// </summary>
    [Fact]
    public void VerifyFailureInvalidType()
    {
        RequiredDependsOnInvalidType dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.InvalidTypeErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the depends on member is not found.
    /// </summary>
    [Fact]
    public void VerifyFailureMemberNameNotFound()
    {
        RequiredDependsOnMemberNotFound dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.MemberNotFoundErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the depends on member is the same member of the evaluating member.
    /// </summary>
    [Fact]
    public void VerifyFailureSameMember()
    {
        RequiredDependsOnSameMemeber dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.SameMemberErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies a failure when the depends on member has a different type than the condition value.
    /// </summary>
    [Fact]
    public void VerifyFailureTypeMismatch()
    {
        RequiredDependsOnTypeMismatch dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.TypeMismatchErrorMessage, validationResults[0].ErrorMessage);
    }

    /// <summary>
    /// The method verifies the Required data annotation is evaluated when depends on condition is set to the required enum value.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnEnumConditionEvaluated(string conditionalRequireValue)
    {
        RequiredDependsOnEnum dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = ConditionalEnum.Condition2,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        if (conditionalRequireValue.Length > 0)
        {
            Assert.Empty(validationResults);
        }
        else
        {
            Assert.Single(validationResults);
            Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
            Assert.Single(validationResults[0].MemberNames);
            Assert.Equal(nameof(RequiredDependsOnTrue.ConditionalRequireValue), validationResults[0].MemberNames.First());
        }
    }

    /// <summary>
    /// The method verifies the Required data annotation is ignored.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnEnumConditionIgnored(string conditionalRequireValue)
    {
        RequiredDependsOnEnum dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = ConditionalEnum.Condition1,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the Required data annotation is evaluated when depends on condition is true.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnFalseConditionEvaluated(string conditionalRequireValue)
    {
        RequiredDependsOnFalse dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = false,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        if (conditionalRequireValue.Length > 0)
        {
            Assert.Empty(validationResults);
        }
        else
        {
            Assert.Single(validationResults);
            Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
            Assert.Single(validationResults[0].MemberNames);
            Assert.Equal(nameof(RequiredDependsOnTrue.ConditionalRequireValue), validationResults[0].MemberNames.First());
        }
    }

    /// <summary>
    /// The method verifies the Required data annotation is ignored.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnFalseConditionIgnored(string conditionalRequireValue)
    {
        RequiredDependsOnFalse dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = true,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the Required data annotation is evaluated when depends on condition is true.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnTrueConditionEvaluated(string conditionalRequireValue)
    {
        RequiredDependsOnTrue dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = true,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        if (conditionalRequireValue.Length > 0)
        {
            Assert.Empty(validationResults);
        }
        else
        {
            Assert.Single(validationResults);
            Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
            Assert.Single(validationResults[0].MemberNames);
            Assert.Equal(nameof(RequiredDependsOnTrue.ConditionalRequireValue), validationResults[0].MemberNames.First());
        }
    }

    /// <summary>
    /// The method verifies the Required data annotation is ignored.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("Text")]
    public void VerifyRequiredDependsOnTrueConditionIgnored(string conditionalRequireValue)
    {
        RequiredDependsOnTrue dataObject = new()
        {
            ConditionalRequireValue = conditionalRequireValue,
            DependentOnMember = false,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }
}
