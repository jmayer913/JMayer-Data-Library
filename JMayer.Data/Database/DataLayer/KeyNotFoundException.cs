namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents an exception caused when a key is not found.
    /// </summary>
    public class KeyNotFoundException : Exception
    {
        /// <summary>
        /// The property gets the key associated with the missing record.
        /// </summary>
        public string? Key { get; private set; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public KeyNotFoundException() : base() { }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public KeyNotFoundException(string? key) 
            : base($"The {key} key was not found.")
        {
            Key = key;
        }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public KeyNotFoundException(string? key, string? message)
            : base(message) 
        { 
            Key = key;
        }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public KeyNotFoundException(string? key, string? message, Exception? innerException)
            : base(message, innerException)
        { 
            Key = key;
        }
    }
}
