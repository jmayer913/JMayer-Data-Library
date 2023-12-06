using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents the arguments for the IDataLayer.Deleted event.
/// </summary>
public sealed class DeletedEventArgs : EventArgs
{
    /// <summary>
    /// The property gets the data objects which were deleted.
    /// </summary>
    public List<DataObject> DataObjects { get; private init; }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObjects">The data objects which were deleted.</param>
    public DeletedEventArgs(List<DataObject> dataObjects) => DataObjects = dataObjects;
}
