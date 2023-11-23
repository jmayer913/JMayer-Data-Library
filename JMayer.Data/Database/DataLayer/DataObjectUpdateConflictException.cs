namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents an exception caused when the data being updated is detected to be old.
    /// </summary>
    public class DataObjectUpdateConflictException : Exception
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public DataObjectUpdateConflictException() : base() { }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public DataObjectUpdateConflictException(string? message) : base(message) { }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public DataObjectUpdateConflictException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
