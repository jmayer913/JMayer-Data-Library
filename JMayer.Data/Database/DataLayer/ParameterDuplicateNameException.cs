namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents an exception caused when the passed in data objects (for create or update) 
/// had duplicate names and the data layer is enforcing name uniqueness.
/// </summary>
public class ParameterDuplicateNameException : Exception
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public ParameterDuplicateNameException() : base() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    public ParameterDuplicateNameException(string? message) : base(message) { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    /// <param name="innerException">The inner exception associated with this exception.</param>
    public ParameterDuplicateNameException(string? message, Exception? innerException) : base(message, innerException) { }
}
