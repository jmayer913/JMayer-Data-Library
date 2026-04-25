using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations.CompareToOtherMembers;

namespace TestProject.Test.DataAnnotations;

#warning I need some sort of default error message when validation fails but it also needs to respect when a custom error message is set.
#warning The base has a reference to the default and custom error message but those are private and the public ErrorMessage property returns one of them.

/// <summary>
/// The class manages general tests for the CompareToOtherMemberAttribute object.
/// </summary>
public class CompareToOtherMemberUnitTest
{
    /// <summary>
    /// The method verifies an argument null exception is thrown when null is passed into the constructor for the conditionValue parameter.
    /// </summary>
    [Fact]
    public void VerifyConstructorThrowsArgumentNullExceptionForOtherMemberNameParameter() => Assert.Throws<ArgumentNullException>(() => new CompareToOtherMemberAttribute(null!, ComparisonOperation.Equal));

    /// <summary>
    /// The method verifies when the passIfOtherMemberIsNull parameter is false and the other member is null, it'll fail.
    /// </summary>
    [Fact]
    public void VerifyFailureIfOtherMemberIsNull()
    {
        CompareOtherMemberFailOtherMemberIfNull dataObject = new()
        {
            Property1 = string.Empty,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareOtherMemberFailOtherMemberIfNull.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareOtherMemberFailOtherMemberIfNull.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies when the passIfRegisteredMemberIsNull parameter is false and the registered member is null, it'll fail.
    /// </summary>
    [Fact]
    public void VerifyFailureIfRegisteredMemberIsNull()
    {
        CompareOtherMemberFailRegisteredMemberIfNull dataObject = new()
        {
            Property2 = string.Empty,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareOtherMemberFailRegisteredMemberIfNull.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareOtherMemberFailRegisteredMemberIfNull.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies a failure when the types are invalid.
    /// </summary>
    [Fact]
    public void VerifyFailureInvalidType()
    {
        CompareToOtherMemberInvalidType dataObject = new();
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
        CompareToOtherMemberNotFound dataObject = new();
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
        CompareToOtherMemberSameMember dataObject = new();
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        Assert.Equal(DataAnnotationMemberHelper.SameMemberErrorMessage, validationResults[0].ErrorMessage);
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
    /// The method verifies when the passIfOtherMemberIsNull parameter is true and the other member is null, it'll pass.
    /// </summary>
    [Fact]
    public void VerifyPassIfOtherMemberIsNull()
    {
        CompareOtherMemberPassOtherMemberIfNull dataObject = new()
        {
            Property1 = string.Empty,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies when the passIfRegisteredMemberIsNull parameter is true and the registered member is null, it'll pass.
    /// </summary>
    [Fact]
    public void VerifyPassIfRegisteredMemberIsNull()
    {
        CompareOtherMemberPassRegisteredMemberIfNull dataObject = new()
        {
            Property2 = string.Empty,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Empty(validationResults);
    }
}
