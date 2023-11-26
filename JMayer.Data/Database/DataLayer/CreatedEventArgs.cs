using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents the arguments for the IDataLayer.Created event.
    /// </summary>
    /// <param name="dataObjects">The data objects which were created.</param>
    public sealed class CreatedEventArgs(List<DataObject> dataObjects) : EventArgs()
    {
        /// <summary>
        /// The property gets the data objects which were created.
        /// </summary>
        public List<DataObject> DataObjects { get; private init; } = dataObjects;
    }
}
