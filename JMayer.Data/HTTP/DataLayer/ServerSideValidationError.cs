namespace JMayer.Data.HTTP.DataLayer;

/// <summary>
/// The class represents the validation error for a property.
/// </summary>
public sealed class ServerSideValidationError
{
    /// <summary>
    /// The property gets/sets the error message.
    /// </summary>
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// The property gets/sets the name of the property which is in error.
    /// </summary>
    public string PropertyName { get; init; } = string.Empty;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public ServerSideValidationError() { }
}
