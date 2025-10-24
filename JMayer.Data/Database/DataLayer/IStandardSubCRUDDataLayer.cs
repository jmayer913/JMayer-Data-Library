using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer;

/// <summary>
/// The interface for interacting with sub data objects (records) in a database collection/table using CRUD operations.
/// </summary>
/// <typeparam name="T">A SubDataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// This currently doesn't have anything defined specifically for a SubDataObject and it only exists for tigher coupling.
/// </remarks>
public interface IStandardSubCRUDDataLayer<T> : IStandardCRUDDataLayer<T> 
    where T : SubDataObject
{
}
