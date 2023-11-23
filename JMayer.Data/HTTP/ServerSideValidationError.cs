namespace JMayer.Data.HTTP
{
    /// <summary>
    /// The class represents the validation error for a property.
    /// </summary>
    public class ServerSideValidationError
    {
        /// <summary>
        /// The property gets/sets the error message.
        /// </summary>
        public string? ErrorMessage { get; private init; }

        /// <summary>
        /// The property gets/sets the name of the property which is in error.
        /// </summary>
        public string? PropertyName { get; private init; }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="propertyName">The name of the property which is in error.</param>
        public ServerSideValidationError(string? errorMessage, string? propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
