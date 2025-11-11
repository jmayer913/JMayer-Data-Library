namespace JMayer.Data.Service;

/// <summary>
/// The enumeration for the operation failures.
/// </summary>
public enum OperationFailureType
{
    /// <summary>
    /// A general error occurred for the operation.
    /// </summary>
    General = 0,
    
    /// <summary>
    /// There was a data conflict with the data object. This depends on the operation.
    /// For edit, it would be an old data object was submitted; conflicts from two or more users
    /// submitting the same data object. For delete, it would be a dependency issue; another data
    /// object depends on the data object that a user wants to delete.
    /// </summary>
    DataConflict,
    
    /// <summary>
    /// The data object was not found so it could not be edited or deleted.
    /// </summary>
    NotFound,

    /// <summary>
    /// The data object failed validation.
    /// </summary>
    Validation,
}
