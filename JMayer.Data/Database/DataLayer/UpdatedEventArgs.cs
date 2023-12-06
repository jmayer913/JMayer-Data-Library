using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents the arguments for the IDataLayer.Updated event.
/// </summary>
public sealed class UpdatedEventArgs : EventArgs
{
    /// <summary>
    /// The property gets the data objects which were updated.
    /// </summary>
    public List<DataObject> DataObjects { get; private init; }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObjects">The data objects which were updated.</param>
    public UpdatedEventArgs(List<DataObject> dataObjects) : base() => DataObjects = dataObjects;
}
