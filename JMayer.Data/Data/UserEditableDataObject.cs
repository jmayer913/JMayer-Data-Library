using System.ComponentModel.DataAnnotations;

namespace JMayer.Data.Data;

/// <summary>
/// The class represents user editable data in the database.
/// </summary>
public class UserEditableDataObject : DataObject
{
    /// <summary>
    /// The property gets/sets when the user editable data was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// The property gets/sets the description of the user editable data.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The property gets/sets who last edited the user editable data.
    /// </summary>
    public string? LastEditedBy { get; set; }

    /// <summary>
    /// The property gets/sets the ID for who last edited the user editable data.
    /// </summary>
    public string? LastEditedByID { get; set; }

    /// <summary>
    /// The property gets/sets the last time the user editable data was edited.
    /// </summary>
    public DateTime? LastEditedOn { get; set; }

    /// <summary>
    /// The property gets/sets the name of the user editable data.
    /// </summary>
    [Required]
    public string? Name { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public UserEditableDataObject() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public UserEditableDataObject(UserEditableDataObject copy) => MapProperties(copy);

    /// <summary>
    /// The method maps a UserEditableDataObject to this object.
    /// </summary>
    /// <param name="dataObject">The data object.</param>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is UserEditableDataObject configurationDataObject)
        {
            CreatedOn = configurationDataObject.CreatedOn;
            Description = configurationDataObject.Description;
            LastEditedBy = configurationDataObject.LastEditedBy;
            LastEditedByID = configurationDataObject.LastEditedByID;
            LastEditedOn = configurationDataObject.LastEditedOn;
            Name = configurationDataObject.Name;
        }
    }
}
