using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.HTTP.DataLayer;

#warning I don't know if HTTP json deserialization works with an init property or not. TO DO: Test when implementing the stack.

/// <summary>
/// The class represents the result of server side validation.
/// </summary>
public sealed class ServerSideValidationResult
{
    /// <summary>
    /// The property gets the errors in the result.
    /// </summary>
    public List<ServerSideValidationError> Errors { get; set; } = [];

    /// <summary>
    /// The property gets if no errors was produced by the validation.
    /// </summary>
    public bool IsSuccess
    {
        get => Errors.Count == 0;
    }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ServerSideValidationResult() { }

    /// <summary>
    /// The conversion constructor.
    /// </summary>
    /// <param name="validationResults">The data annotation results to convert into this object.</param>
    public ServerSideValidationResult(List<ValidationResult> validationResults)
    {
        ArgumentNullException.ThrowIfNull(validationResults);

        foreach (ValidationResult validationResult in validationResults)
        {
            Errors.Add(new ServerSideValidationError()
            {
                ErrorMessage = validationResult.ErrorMessage ?? string.Empty,
                PropertyName = validationResult.MemberNames.First(),
            });
        }
    }
}
