using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Data;

#warning I need to test if the property is overridden and [JsonIgnore] is applied will it be respected.

/// <summary>
/// The class represents a record in the database.
/// </summary>
/// <remarks>
/// <para>
/// Data objects will inherit from this class and the subclasses
/// will further build out the data it represents. For example, an
/// account data object will have properties related to the account.
/// </para>
/// <para>
/// This contains an integer or string ID and each is stored
/// indepedently. Because of this, it must be known beforehand which
/// will be used by the data object, data layer and UI. For example, mongodb
/// generates a unique ID hash which can be mapped to a string property so
/// the mongo data object, mongo data layer and UI need to use the StringID 
/// property.
/// </para>
/// </remarks>
public class DataObject
{
    /// <summary>
    /// The property gets/sets when the data object was created.
    /// </summary>
    public virtual DateTime CreatedOn { get; set; }

    /// <summary>
    /// The property gets/sets the description of the data object.
    /// </summary>
    /// <remarks>
    /// This property is optional and can be ignored if your data doesn't require it. If you do use it and
    /// it requires validation rules, you can override the property and add the necessry data annotation
    /// attributes to it.
    /// </remarks>
    public virtual string? Description { get; set; }

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
    /// The property gets/sets the user who last edited the data object.
    /// </summary>
    /// <remarks>This property is optional and can be ignored if your data doesn't require it.</remarks>
    public virtual string? LastEditedBy { get; set; }

    /// <summary>
    /// The property gets/sets the user ID for who last edited the data object.
    /// </summary>
    /// <remarks>This property is optional and can be ignored if your data doesn't require it.</remarks>
    public virtual string? LastEditedByID { get; set; }

    /// <summary>
    /// The property gets/sets the last time the data object was edited.
    /// </summary>
    /// <remarks>This property is optional and can be ignored if your data doesn't require it.</remarks>
    public virtual DateTime? LastEditedOn { get; set; }

    /// <summary>
    /// The property gets/sets the name of the user editable data.
    /// </summary>
    /// <remarks>
    /// This property is optional and can be ignored if your data doesn't require it. If you do use it and
    /// it requires validation rules, you can override the property and add the necessry data annotation
    /// attributes to it.
    /// </remarks>
    public virtual string? Name { get; set; }

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
    /// <exception cref="ArgumentNullException">Thrown if the dataObject parameter is null.</exception>
    public virtual void MapProperties(DataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        CreatedOn = dataObject.CreatedOn;
        Description = dataObject.Description;
        Integer64ID = dataObject.Integer64ID;
        LastEditedBy = dataObject.LastEditedBy;
        LastEditedByID = dataObject.LastEditedByID;
        LastEditedOn = dataObject.LastEditedOn;
        Name = dataObject.Name;
        StringID = dataObject.StringID;
    }

    /// <summary>
    /// The method validates the data annotations on the data object.
    /// </summary>
    /// <returns>The validation results.</returns>
    public List<ValidationResult> Validate()
    {
        List<ValidationResult> validationResults = [];
        _ = Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true);
        return validationResults;
    }
}
