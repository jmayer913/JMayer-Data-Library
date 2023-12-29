namespace JMayer.Data.HTTP.DataLayer;

#warning I don't know if HTTP json deserialization works with an init property or not. TO DO: Test when implementing the stack.

/// <summary>
/// The class represents the validation error for a property.
/// </summary>
public sealed class ServerSideValidationError
{
    /// <summary>
    /// The property gets/sets the error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the name of the property which is in error.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ServerSideValidationError() { }
}
