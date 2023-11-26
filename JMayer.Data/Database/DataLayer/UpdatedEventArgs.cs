using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents the arguments for the IDataLayer.Updated event.
    /// </summary>
    /// <param name="dataObjects">The data objects which were updated.</param>
    public sealed class UpdatedEventArgs(List<DataObject> dataObjects) : EventArgs()
    {
        /// <summary>
        /// The property gets the data objects which were updated.
        /// </summary>
        public List<DataObject> DataObjects { get; private init; } = dataObjects;
    }
}
