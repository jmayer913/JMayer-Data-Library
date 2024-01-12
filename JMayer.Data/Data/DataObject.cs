using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Data;

/// <summary>
/// The class represents a record in the database.
/// </summary>
/// <remarks>
/// Data objects will inherit from this class and the subclasses
/// will further build out the data it represents. For example, an
/// account data object will have properties related to the account.
/// 
/// This contains an integer or string ID and each is stored
/// indepedently. Because of this, it must be known beforehand which
/// will be used by the data object, data layer & UI. For example, mongodb
/// generates a unique ID hash which can be mapped to a string property so
/// the mongo data object, mongo data layer and UI need to use the StringID 
/// property.
/// </remarks>
public class DataObject
{
    /// <summary>
    /// The property gets/sets the ID for the data object as an 32-bit integer.
    /// </summary>
    /// <remarks>
    /// This wraps around the Integer64ID property.
    /// </remarks>
    public int Integer32ID
    {
        get => (int)Integer64ID;
        set => Integer64ID = value;
    }

    /// <summary>
    /// The property gets/sets the ID for the data object as an 64-bit integer.
    /// </summary>
    public virtual long Integer64ID { get; set; }

    /// <summary>
    /// The property gets/sets the ID for the data object as a string.
    /// </summary>
    public virtual string StringID { get; set; } = string.Empty;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public DataObject() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public DataObject(DataObject copy) => MapProperties(copy);

    /// <summary>
    /// The method maps a DataObject to this object.
    /// </summary>
    /// <param name="dataObject">The data object.</param>
    public virtual void MapProperties(DataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        Integer64ID = dataObject.Integer64ID;
        StringID = dataObject.StringID;
    }

    /// <summary>
    /// The method validates the data annotations on the data object.
    /// </summary>
    /// <param name="dataObject">The data object to validate.</param>
    /// <returns>The validation results.</returns>
    public List<ValidationResult> Validate()
    {
        List<ValidationResult> validationResults = [];
        _ = Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true);
        return validationResults;
    }
}
