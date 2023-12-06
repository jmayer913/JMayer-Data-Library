using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The class represents the arguments for the IDataLayer.Created event.
/// </summary>
public sealed class CreatedEventArgs : EventArgs
{
    /// <summary>
    /// The property gets the data objects which were created.
    /// </summary>
    public List<DataObject> DataObjects { get; private init; }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="dataObjects">The data objects which were created.</param>
    public CreatedEventArgs(List<DataObject> dataObjects) : base() => DataObjects = dataObjects;
}
