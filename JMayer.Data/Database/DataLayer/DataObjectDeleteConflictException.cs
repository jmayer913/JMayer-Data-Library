namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents an exception caused when the data object being deleted has dependencies which prevent the deletion.
/// </summary>
public class DataObjectDeleteConflictException : Exception
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public DataObjectDeleteConflictException() : base() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    public DataObjectDeleteConflictException(string? message) : base(message) { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    /// <param name="innerException">The inner exception associated with this exception.</param>
    public DataObjectDeleteConflictException(string? message, Exception? innerException) : base(message, innerException) { }
}
