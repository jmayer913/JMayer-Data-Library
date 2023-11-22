namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents an exception caused when the data being updated is detected to be old.
    /// </summary>
    public class UpdateConflictException : Exception
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public UpdateConflictException() : base() { }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public UpdateConflictException(string? message) : base(message) { }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public UpdateConflictException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
