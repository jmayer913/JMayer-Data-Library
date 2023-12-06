using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents an exception caused by a validation issue with the data object on create/update.
/// </summary>
public class DataObjectValidationException : Exception
{
    /// <summary>
    /// The property gets the data object that had a validation issue.
    /// </summary>
    public DataObject? DataObject { get; private init; }

    /// <summary>
    /// The property gets the validation results for the data object.
    /// </summary>
    public List<ValidationResult>? ValidationResults { get; private init; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public DataObjectValidationException() : base() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObject">The data object that had a validation issue.</param>
    /// <param name="validationResults">The validation results for the data object.</param>
    public DataObjectValidationException(DataObject? dataObject, List<ValidationResult>? validationResults)
        : base($"The data object failed validation.")
    {
        DataObject = dataObject;
        ValidationResults = validationResults;
    }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObject">The data object that had a validation issue.</param>
    /// <param name="validationResults">The validation results for the data object.</param>
    /// <param name="message">The message associated with the exception.</param>
    public DataObjectValidationException(DataObject? dataObject, List<ValidationResult>? validationResults, string? message)
        : base(message)
    {
        DataObject = dataObject;
        ValidationResults = validationResults;
    }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObject">The data object that had a validation issue.</param>
    /// <param name="validationResults">The validation results for the data object.</param>
    /// <param name="message">The message associated with the exception.</param>
    /// <param name="innerException">The inner exception associated with this exception.</param>
    public DataObjectValidationException(DataObject? dataObject, List<ValidationResult>? validationResults, string? message, Exception? innerException)
        : base(message, innerException)
    {
        DataObject = dataObject;
        ValidationResults = validationResults;
    }
}
