using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The class represents the arguments for the IDataLayer.Deleted event.
    /// </summary>
    /// <param name="dataObjects">The data objects which were deleted.</param>
    public sealed class DeletedEventArgs(List<DataObject> dataObjects) : EventArgs()
    {
        /// <summary>
        /// The property gets the data objects which were deleted.
        /// </summary>
        public List<DataObject> DataObjects { get; private init; } = dataObjects;
    }
}
