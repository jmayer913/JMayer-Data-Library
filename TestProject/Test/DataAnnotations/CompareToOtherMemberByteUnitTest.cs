using System.ComponentModel.DataAnnotations;
using TestProject.Data.DataAnnotations;

namespace TestProject.Test.DataAnnotations;

/// <summary>
/// The class manages tests for the CompareToOtherMemberAttribute object for the byte data type.
/// </summary>
public class CompareToOtherMemberByteUnitTest
{
    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualFailure()
    {
        CompareToOtherMemberByteEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = byte.MinValue;
        dataObject.Property2 = byte.MaxValue;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyEqualSuccess()
    {
        CompareToOtherMemberByteEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = byte.MinValue;
        dataObject.Property2 = byte.MinValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is less than property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanFailure()
    {
        CompareToOtherMemberByteGreaterThan dataObject = new()
        {
            Property1 = byte.MinValue,
            Property2 = byte.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteGreaterThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteGreaterThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the prperty 1 is greater than property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanSuccess()
    {
        CompareToOtherMemberByteGreaterThan dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
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
        CompareToOtherMemberByteGreaterThanOrEqual dataObject = new()
        {
            Property1 = byte.MinValue,
            Property2 = byte.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteGreaterThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteGreaterThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is greater than or equal property 2.
    /// </summary>
    [Fact]
    public void VerifyGreaterThanOrEqualSuccess()
    {
        CompareToOtherMemberByteGreaterThanOrEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = byte.MaxValue;
        dataObject.Property2 = byte.MaxValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the property 1 is greater than property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanFailure()
    {
        CompareToOtherMemberByteLessThan dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteLessThan.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteLessThan.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is less than property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanSuccess()
    {
        CompareToOtherMemberByteLessThan dataObject = new()
        {
            Property1 = byte.MinValue,
            Property2 = byte.MaxValue,
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
        CompareToOtherMemberByteLessThanOrEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteLessThanOrEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteLessThanOrEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the property 1 is less than or equal property 2.
    /// </summary>
    [Fact]
    public void VerifyLessThanOrEqualSuccess()
    {
        CompareToOtherMemberByteLessThanOrEqual dataObject = new()
        {
            Property1 = byte.MinValue,
            Property2 = byte.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = byte.MaxValue;
        dataObject.Property2 = byte.MaxValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return failure when the two properties do not equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualFailure()
    {
        CompareToOtherMemberByteNotEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MaxValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteNotEqual.Property2), validationResults[0].MemberNames.Last());

        dataObject.Property1 = byte.MinValue;
        dataObject.Property2 = byte.MinValue;
        validationResults = dataObject.Validate();

        Assert.Single(validationResults);
        //Assert.Equal($"The {nameof(RequiredDependsOnTrue.ConditionalRequireValue)} field is required.", validationResults[0].ErrorMessage);
        Assert.Equal(2, validationResults[0].MemberNames.Count());
        Assert.Equal(nameof(CompareToOtherMemberByteNotEqual.Property1), validationResults[0].MemberNames.First());
        Assert.Equal(nameof(CompareToOtherMemberByteNotEqual.Property2), validationResults[0].MemberNames.Last());
    }

    /// <summary>
    /// The method verifies the CompareToOther data annotation return success when the two properties equal each other.
    /// </summary>
    [Fact]
    public void VerifyNotEqualSuccess()
    {
        CompareToOtherMemberByteNotEqual dataObject = new()
        {
            Property1 = byte.MaxValue,
            Property2 = byte.MinValue,
        };
        List<ValidationResult> validationResults = dataObject.Validate();
        Assert.Empty(validationResults);

        dataObject.Property1 = byte.MinValue;
        dataObject.Property2 = byte.MaxValue;
        validationResults = dataObject.Validate();
        Assert.Empty(validationResults);
    }
}
