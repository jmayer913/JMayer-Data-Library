using JMayer.Data.Data;

#warning What separates this from the regular data layer?

namespace JMayer.Data.Database.DataLayer
{
    /// <summary>
    /// The interface for interacting with a collection/table in a database using CRUD operations.
    /// </summary>
    /// <typeparam name="T">A DataObject which represents data in the collection/table.</typeparam>
    public interface IConfigurationDataLayer<T> : IDataLayer<T> where T : DataObject
    {
    }
}
