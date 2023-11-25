namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents an exception caused when an ID is not found.
    /// </summary>
    public class IDNotFoundException : Exception
    {
        /// <summary>
        /// The property gets the ID associated with the missing record.
        /// </summary>
        public string? ID { get; private init; }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public IDNotFoundException() : base() { }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="id">The ID associated with the missing record.</param>
        public IDNotFoundException(string? id) 
            : base($"The {id} ID was not found.")
        {
            ID = id;
        }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="id">The ID associated with the missing record.</param>
        /// <param name="message">The message associated with the exception.</param>
        public IDNotFoundException(string? id, string? message)
            : base(message) 
        { 
            ID = id;
        }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="id">The ID associated with the missing record.</param>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public IDNotFoundException(string? id, string? message, Exception? innerException)
            : base(message, innerException)
        { 
            ID = id;
        }
    }
}
