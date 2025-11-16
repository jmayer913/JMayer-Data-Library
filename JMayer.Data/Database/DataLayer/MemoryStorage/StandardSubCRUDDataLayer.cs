using JMayer.Data.Data;

namespace JMayer.Data.Database.DataLayer.MemoryStorage;

/// <summary>
/// The class manages CRUD interactions with a list memory storage for sub data objects.
/// </summary>
/// <typeparam name="T">A SubDataObject which represents data in the collection/table.</typeparam>
/// <remarks>
/// <para>
/// This currently doesn't have anything defined specifically for a SubDataObject and it only exists for tigher coupling.
/// </para>
/// <para>
/// Data objects will inherit from this class and the subclasses
/// will further build out the data it represents. For example, an
/// account data object will have properties related to the account.
/// </para>
/// <para>
/// This contains an integer or string ID and each is stored
/// indepedently. Because of this, it must be known beforehand which
/// will be used by the data object, data layer and UI. For example, mongodb
/// generates a unique ID hash which can be mapped to a string property so
/// the mongo data object, mongo data layer and UI need to use the StringID 
/// property.
/// </para>
/// </remarks>
public class StandardSubCRUDDataLayer<T> : StandardCRUDDataLayer<T>, IStandardSubCRUDDataLayer<T>
    where T : SubDataObject, new()
{
}
