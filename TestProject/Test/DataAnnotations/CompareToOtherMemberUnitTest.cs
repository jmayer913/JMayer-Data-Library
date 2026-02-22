using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

#warning I need some sort of default error message when validation fails but it also needs to respect when a custom error message is set.
#warning The base has a reference to the default and custom error message but those are private and the public ErrorMessage property returns one of them.

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object.
/// </summary>
public class CompareToOtherMemberUnitTest
{
    /// <summary>
    /// The method verifies an argument null exception is thrown when null is passed into the constructor for the conditionValue parameter.
    /// </summary>
    [Fact]
    public void VerifyConstructorThrowsArgumentNullExceptionForOtherMemberNameParameter() => Assert.Throws<ArgumentNullException>(() => new CompareToOtherMemberAttribute(null, ComparisonOperation.Equal));

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two bool properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyCompareToOtherBoolMemberEqualFailure()
    {
        CompareToOtherMemberBoolEqual dataObject = new()
        {
            BoolProperty1 = true,
            BoolProperty2 = false,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.BoolProperty1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberBoolEqual.BoolProperty2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two bool properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyCompareToOtherBoolMemberEqualSuccess()
    {
        CompareToOtherMemberBoolEqual dataObject = new()
        {
            BoolProperty1 = true,
            BoolProperty2 = true,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }
}
