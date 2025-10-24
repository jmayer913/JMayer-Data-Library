using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage for sub data objects.
/// </summary>
/// <typeparam name="T">A SubDataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// This currently doesn't have anything defined specifically for a SubDataObject and it only exists for tigher coupling.
/// <br/>
/// <br/>
/// This uses an 64-integer identity (auto-increments) ID so the SubDataObject.Integer64ID will be
/// used by this and any outside interactions with the data layer must use SubDataObject.Integer64ID. 
/// Also, the underlying data storage is a List so this shouldn't be used with very large datasets.
/// </remarks>
public class StandardSubCRUDDataLayer<T> : StandardCRUDDataLayer<T>, IStandardSubCRUDDataLayer<T>
    where T : SubDataObject, new()
{
}
