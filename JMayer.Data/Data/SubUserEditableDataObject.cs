namespace JMayer.Data.Data;

/// <summary>
/// The class represents a user editable record in the database which another data object has ownership of.
/// </summary>
/// <remarks>
/// This should be inherited when the data object is owned by another type 
/// of data object. For example, an account has transactions so transactions 
/// would be owned or has a reference to the account.
/// 
/// This contains an integer or string owner ID and each is stored
/// indepedently. Because of this, it must be known beforehand which
/// will be used by the data object, data layer & UI. For example, mongodb
/// generates a unique ID hash which can be mapped to a string property so
/// the mongo data object, mongo data layer and UI need to use the StringID 
/// property.
/// </remarks>
public class SubUserEditableDataObject : UserEditableDataObject
{
    /// <summary>
    /// The property gets/sets the owner ID for this data object as an 32-bit integer.
    /// </summary>
    /// <remarks>
    /// This wraps around the OwnerInteger64ID property.
    /// </remarks>
    public int OwnerInteger32ID
    {
        get => (int)OwnerInteger64ID;
        set => OwnerInteger64ID = value;
    }

    /// <summary>
    /// The property gets/sets the owner ID for this data object as an 64-bit integer.
    /// </summary>
    public virtual long OwnerInteger64ID { get; set; }

    /// <summary>
    /// The property gets/sets the owner ID for this data object as a string.
    /// </summary>
    public virtual string OwnerStringID { get; set; } = string.Empty;

    /// <inheritdoc/>
    public SubUserEditableDataObject() : base() { }

    /// <inheritdoc/>
    public SubUserEditableDataObject(SubUserEditableDataObject copy) : base(copy) { }

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is SubUserEditableDataObject subUserEditableDataObject)
        {
            OwnerInteger64ID = subUserEditableDataObject.OwnerInteger64ID;
            OwnerStringID = subUserEditableDataObject.OwnerStringID;
        }
    }
}
