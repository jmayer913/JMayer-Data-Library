using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JMayer.Data.DataAnnotations;

/// <summary>
/// The class does a value comparison with the member the attribute is registered too and another member on the 
/// same instance being validated. The following types are supported: bool, byte, sbyte, short, ushort, int, uint, long, 
/// ulong, float, double, decimal, char, string, DateTime, DateOffset, DateOnly, TimeOnly, Timespan and Enum. The following comparison
/// operations are suppored: ==, !=, &lt;, &lt;=, &gt;, and &gt;=; bool, char, string and Enum only support == and !=.
/// </summary>
/// <remarks>
/// To use, add the attribute to the public field or property that needs to be compared with another public field or
/// property in the same class. Set the otherMemberName to the name of the other public field or property which needs to be
/// compared against. Set the comparisonOperation to operation that needs to be preformed when the comparison is done. 
/// Set the allowNullCheck if you want both values being null to pass an Equal comparison or if you want one being null 
/// to pass a NotEqual comparison.
/// <code>
/// public class ObjectRequiringValidation
/// {
///     //IntMember will need to be less than OtherIntMember in order to pass validation.
///     [CompareToOtherMember(otherMemberName: nameof(OtherIntMember), comparisonOperation: ComparisonOperation.LessThan)]
///     public int IntMember { get; set; }
/// 
///     public int OtherIntMember { get; set; }
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class CompareToOtherMemberAttribute : ValidationAttribute
{
    /// <summary>
    /// Allows the CompareToOtherMember data annotation to return success when the comparison is equal and both members
    /// have a null value or the comparison is not equal and one of the members has a null value.
    /// </summary>
    private readonly bool _allowNullCheck;

    /// <summary>
    /// The operation used when the CompareToOtherMember data annotation does the comparison.
    /// </summary>
    private readonly ComparisonOperation _comparisonOperation;

    /// <summary>
    /// The name of the other member the CompareToOtherMember data annotation will compare against.
    /// </summary>
    private readonly string _otherMemberName;

    /// <summary>
    /// The constant for the invalid comparison for boolean values error message.
    /// </summary>
    internal const string InvalidComparisonForBoolErrorMessage = "Failed to validate because an equal and not equal comparison is only allowed for boolean values.";

    /// <summary>
    /// The constant for the invalid comparison for char values error message.
    /// </summary>
    internal const string InvalidComparisonForCharErrorMessage = "Failed to validate because an equal and not equal comparison is only allowed for character values.";

    /// <summary>
    /// The constant for the invalid comparison for Enum values error message.
    /// </summary>
    internal const string InvalidComparisonForEnumErrorMessage = "Failed to validate because an equal and not equal comparison is only allowed for Enum values.";

    /// <summary>
    /// The constant for the invalid comparison for string values error message.
    /// </summary>
    internal const string InvalidComparisonForStringErrorMessage = "Failed to validate because an equal and not equal comparison is only allowed for string values.";

    /// <summary>
    /// The constructor which takes the comparison info.
    /// </summary>
    /// <param name="otherMemberName">The name of the other member the CompareToOtherMember data annotation will compare against.</param>
    /// <param name="compareToOperation">The operation used when the CompareToOtherMember data annotation does the comparison.</param>
    /// <param name="allowNullCheck">
    /// Allows the CompareToOtherMember data annotation to return success when the comparison is equal and both members
    /// have a null value or the comparison is not equal and one of the members has a null value.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the otherMemberName parameter is null or an empty string.</exception>
    public CompareToOtherMemberAttribute(string otherMemberName, ComparisonOperation compareToOperation, bool allowNullCheck = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(otherMemberName);

        _allowNullCheck = allowNullCheck;
        _comparisonOperation = compareToOperation;
        _otherMemberName = otherMemberName;
    }

    /// <summary>
    /// The method returns if the character values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckCharacterComparison(char registeredMemberValue, char otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the bool values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckBoolValues(bool registeredMemberValue, bool otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the date values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckDateValues(DateTime registeredMemberValue, DateTime otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the date values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckDateValues(DateTimeOffset registeredMemberValue, DateTimeOffset otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the date values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckDateValues(DateOnly registeredMemberValue, DateOnly otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the decimal values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckDecimalValues(decimal registeredMemberValue, decimal otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the double values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckDoubleValues(double registeredMemberValue, double otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the enum values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckEnumValues(Enum registeredMemberValue, Enum otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue.Equals(otherMemberValue),
            ComparisonOperation.NotEqual => registeredMemberValue.Equals(otherMemberValue) is false,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the integer values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckIntegerValues(long registeredMemberValue, long otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method does a comparison for null values; equal needs both to be null and not equal needs
    /// one to be null.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckNullVaues(object? registeredMemberValue, object? otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue is null && otherMemberValue is null,
            ComparisonOperation.NotEqual => (registeredMemberValue is null && otherMemberValue is not null) || (registeredMemberValue is not null && otherMemberValue is null),
            _ => false,
        };
    }

    /// <summary>
    /// The method returns if the string values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckStringValues(string registeredMemberValue, string otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the time values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckTimeValues(TimeOnly registeredMemberValue, TimeOnly otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the time values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckTimeValues(TimeSpan registeredMemberValue, TimeSpan otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <summary>
    /// The method returns if the unsigned integer values pass the comparison.
    /// </summary>
    /// <param name="registeredMemberValue">The value of the member the attribute is registered too.</param>
    /// <param name="otherMemberValue">The value of the other member.</param>
    /// <returns>True means the comparison passed.</returns>
    private bool CheckUnsignedIntegerValues(ulong registeredMemberValue, ulong otherMemberValue)
    {
        return _comparisonOperation switch
        {
            ComparisonOperation.Equal => registeredMemberValue == otherMemberValue,
            ComparisonOperation.GreaterThan => registeredMemberValue > otherMemberValue,
            ComparisonOperation.GreaterThanOrEqual => registeredMemberValue >= otherMemberValue,
            ComparisonOperation.LessThan => registeredMemberValue < otherMemberValue,
            ComparisonOperation.LessThanOrEqual => registeredMemberValue <= otherMemberValue,
            ComparisonOperation.NotEqual => registeredMemberValue != otherMemberValue,
            _ => false
        };
    }

    /// <inheritdoc/>
    /// <remarks>Overridden to compare the member the attribute is registered too and an another member on the instance.</remarks>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (_otherMemberName == validationContext.MemberName)
        {
            return new ValidationResult(DataAnnotationMemberHelper.SameMemberErrorMessage);
        }

        MemberInfo? memberInfo = validationContext.ObjectType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, Type.FilterName, validationContext.MemberName).FirstOrDefault();

        if (memberInfo is null)
        {
            return new ValidationResult(DataAnnotationMemberHelper.MemberNotFoundErrorMessage);
        }

        MemberInfo? otherMemberInfo = validationContext.ObjectType.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance, Type.FilterName, _otherMemberName).FirstOrDefault();

        if (otherMemberInfo is null)
        {
            return new ValidationResult(DataAnnotationMemberHelper.MemberNotFoundErrorMessage);
        }
        
        if (DataAnnotationMemberHelper.IsSameType(memberInfo, otherMemberInfo) is false)
        {
            return new ValidationResult(DataAnnotationMemberHelper.TypeMismatchErrorMessage);
        }

        object? otherMemberValue = DataAnnotationMemberHelper.GetMemberValue(otherMemberInfo, validationContext.ObjectInstance);

        bool success = false;

        if (_allowNullCheck)
        {
            success = CheckNullVaues(value, otherMemberValue);
        }

        if (value is bool registeredMemberBoolValue && otherMemberValue is bool otherMemberBoolValue)
        {
            //Only equal and not equal are supported for this type.
            if (_comparisonOperation is not ComparisonOperation.Equal and not ComparisonOperation.NotEqual)
            {
                return new ValidationResult(InvalidComparisonForBoolErrorMessage);
            }

            success = CheckBoolValues(registeredMemberBoolValue, otherMemberBoolValue);
        }
        else if (value is sbyte or short or int or long && otherMemberValue is sbyte or short or int or long)
        {
            success = CheckIntegerValues(Convert.ToInt64(value), Convert.ToInt64(otherMemberValue));
        }
        else if (value is byte or ushort or uint or ulong && otherMemberValue is byte or ushort or uint or ulong)
        {
            success = CheckUnsignedIntegerValues(Convert.ToUInt64(value), Convert.ToUInt64(otherMemberValue));
        }
        else if (value is float or double && otherMemberValue is float or double)
        {
            success = CheckDoubleValues(Convert.ToDouble(value), Convert.ToDouble(otherMemberValue));
        }
        else if (value is decimal && otherMemberValue is decimal)
        {
            success = CheckDecimalValues(Convert.ToDecimal(value), Convert.ToDecimal(otherMemberValue));
        }
        else if (value is char registeredMemberdCharacterValue && otherMemberValue is char otherMemberdCharacterValue)
        {
            //Only equal and not equal are supported for this type.
            if (_comparisonOperation is not ComparisonOperation.Equal and not ComparisonOperation.NotEqual)
            {
                return new ValidationResult(InvalidComparisonForCharErrorMessage);
            }

            success = CheckCharacterComparison(registeredMemberdCharacterValue, otherMemberdCharacterValue);
        }
        else if (value is string registeredMemberdStringValue && otherMemberValue is string otherMemberdStringValue)
        {
            //Only equal and not equal are supported for this type.
            if (_comparisonOperation is not ComparisonOperation.Equal and not ComparisonOperation.NotEqual)
            {
                return new ValidationResult(InvalidComparisonForStringErrorMessage);
            }

            success = CheckStringValues(registeredMemberdStringValue, otherMemberdStringValue);
        }
        else if (value is DateTime registeredMemberDateValue && otherMemberValue is DateTime otherMemberDateValue)
        {
            success = CheckDateValues(registeredMemberDateValue, otherMemberDateValue);
        }
        else if (value is DateTimeOffset registeredMemberDateOffsetValue && otherMemberValue is DateTimeOffset otherMemberDateOffsetValue)
        {
            success = CheckDateValues(registeredMemberDateOffsetValue, otherMemberDateOffsetValue);
        }
        else if (value is DateOnly registeredMemberDateOnlyValue && otherMemberValue is DateOnly otherMemberDateOnlyValue)
        {
            success = CheckDateValues(registeredMemberDateOnlyValue, otherMemberDateOnlyValue);
        }
        else if (value is TimeOnly registeredMemberTimeOnlyValue && otherMemberValue is TimeOnly otherMemberTimeOnlyValue)
        {
            success = CheckTimeValues(registeredMemberTimeOnlyValue, otherMemberTimeOnlyValue);
        }
        else if (value is TimeSpan registeredMemberTimeValue && otherMemberValue is TimeSpan otherMemberTimeValue)
        {
            success = CheckTimeValues(registeredMemberTimeValue, otherMemberTimeValue);
        }
        else if (value is Enum registeredMemberEnumValue && otherMemberValue is Enum otherMemberEnumValue)
        {
            //Only equal and not equal are supported for this type.
            if (_comparisonOperation is not ComparisonOperation.Equal and not ComparisonOperation.NotEqual)
            {
                return new ValidationResult(InvalidComparisonForEnumErrorMessage);
            }

            success = CheckEnumValues(registeredMemberEnumValue, otherMemberEnumValue);
        }
        //At this point, the type must not be supported.
        else if (value is not null && otherMemberValue is not null)
        {
            return new ValidationResult(DataAnnotationMemberHelper.InvalidTypeErrorMessage);
        }

        if (success is false)
        {
            if (validationContext.MemberName is not null)
            {
                return new ValidationResult(ErrorMessage, [validationContext.MemberName, _otherMemberName]);
            }
            else
            {
                return new ValidationResult(ErrorMessage, [_otherMemberName]);
            }
        }

        return ValidationResult.Success;
    }
}
