using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class represents the result of server side validation.
/// </summary>
public sealed class ServerSideValidationResult
{
    /// <summary>
    /// The property gets the errors in the result.
    /// </summary>
    public List<ServerSideValidationError> Errors { get; init; } = [];

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
    /// <exception cref="ArgumentNullException">Thrown if the validationResults parameter is null.</exception>
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

    /// <summary>
    /// The conversion constructor.
    /// </summary>
    /// <param name="validationProblemDetails">The data annotation results from a HTTP response.</param>
    /// <exception cref="ArgumentNullException">Thrown if the dataAnnotationValidation parameter is null.</exception>
    internal ServerSideValidationResult(ValidationProblemDetails validationProblemDetails)
    {
        ArgumentNullException.ThrowIfNull(validationProblemDetails);

        foreach (var error in validationProblemDetails.Errors)
        {
            foreach (var message in error.Value)
            {
                Errors.Add(new ServerSideValidationError()
                {
                    ErrorMessage = message,
                    PropertyName = error.Key,
                });
            }
        }
    }
}
